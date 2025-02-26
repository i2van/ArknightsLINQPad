<Query Kind="Program">
  <Namespace>static System.DateTime</Namespace>
  <Namespace>static System.Drawing.ColorTranslator</Namespace>
  <Namespace>static System.TimeZoneInfo</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
</Query>

// Ongoing Arknights Global event status tracker.

#LINQPad checked+

#nullable enable

#load "./lib/Context.linq"
#load "./lib/Extensions.linq"
#load "./lib/Images.linq"
#load "./lib/Parsable.linq"
#load "./lib/WikiHyperlinq.linq"

#define DUMP_MISSING_IMAGES

/*
#define DUMP_CONFIG
#define CONFIG_INTERPOLATE_COLORS
//*/

void Main()
{
	// TODO: Arknights events can be found at https://arknights.wiki.gg/wiki/Event
	var config = new
	{
		Title          = new WikiHyperlinq("Event#Ongoing", "Arknights Event Sanity Status"),
		// TODO: Specify your level max sanity.
		SanityPerPrime = 135,
		// TODO: Specify event's in-game date and time end.
		EventEndDate   = new DateOnly(Year.Now, Month.Feb, Day.OfMonth(14)),
		EventEndTime   = new TimeOnly(Hour.OfDay(3), Minute.OfHour(59)),
		// TODO: Specify in-game UTC offset.
		UtcOffset      = FromHours(-7),
		// TODO: Copy and paste current event data from EventData.Value below. Remove item(s) when done.
		Event          = new Event
		{
			[new("A_Kazdelian_Rescue#Furnace_Node", "A Kazdelian Rescue", "Slag")] = new("""
			// A Kazdelian Rescue
			350		Holiday HD66
			10	6	Information Fragment
			100	2	Module Data Block
			30	8	Data Supplement Instrument
			10	40	Data Supplement Stick
			200		Bipolar Nanoflake
			65		Chip Catalyst
			60	2	Grindstone Pentahydrate
			20	4	Semi-Synthetic Solvent
			15	4	Loxic Kohl
			5	10	LMD
			5	5	Strategic Battle Record
			5	5	Tactical Battle Record
			6	5	Skill Summary - 3
			6	5	Skill Summary - 2
			4	5	Pure Gold
			6	5	Oriron
			30	5	Recruitment Permit
			4	10	Furniture Part
			""")
		}
	};

	var eventEndTimeLocal = ConvertTime(
		config.EventEndDate.ToDateTime(config.EventEndTime),
		CreateCustomTimeZone("Arknights", config.UtcOffset, null, null),
		Local);

#if DUMP_MISSING_IMAGES
	{
		HashSet<string> existingImagesKeys = new(ImageData.Value.Keys);
		var missingImages = EventData.Value.Keys
			.SelectMany(static v => EventData.Value[v])
			.Concat(config.Event.Items)
			.WhereNot(static v => string.IsNullOrWhiteSpace(v.Name))
			.Where(v => existingImagesKeys.Add(v.Name))
			.OrderBy(static v => v.Name)
			.ToArray();

		if(missingImages.Any())
		{
			missingImages.Dump("Config - Images Missing");
		}
	}
#endif

	Context = new();

	var timeLeft = eventEndTimeLocal - Now;

	var currentEvent   = config.Event;
	var eventHyperlinq = currentEvent.WikiHyperlinq;

	var title = eventHyperlinq.Text;
	var uri   = eventHyperlinq.Uri;

	var (baseUri, stockName) = SplitUri(uri);
	var resourceUri = eventHyperlinq.ProcessMetadata(metadata => $"{baseUri.Replace("/Rerun", Empty)}#{metadata}") ?? uri;

	stockName = (stockName ?? title).SpaceUnderscores();

	using var _ = new DisposableAction(() => title.AsHyperlink(baseUri));

	if(NoTimeLeft(timeLeft))
	{
		HorizontalRun(false, new WikiHyperlinq("Event"), " is completed.").Dump(title);
		return;
	}

	var price = currentEvent.Items.Sum(static v => v.Total);

	if(price <= 0)
	{
		HorizontalRun(false, "No more ", new Hyperlinq(resourceUri, "sanity"), " to ", new Hyperlinq(uri, "spend"), ".").Dump(title);
		return;
	}

	var totalPrice = EventData.Value[eventHyperlinq].Sum(static v => v.Total);

	var timeRequired  = FromDays(CalcPrimesToRecover(price));
	var timeToRecover = (timeRequired - timeLeft).FloorSeconds();

	var primesToRecover = Ceiling(CalcPrimesToRecover(Abs(timeToRecover.TotalDays), config.SanityPerPrime)).ToString(DumpContext.CultureInfo);
	var noRecoverNeeded = NoTimeLeft(timeToRecover);

	var timeRequiredFormatted  = Format(timeRequired);
	var timeLeftFormatted      = Format(timeLeft);
	var timeToRecoverFormatted = Format(timeToRecover);

	var dateStyle = new[]
	{
		timeRequiredFormatted,
		timeLeftFormatted,
		timeToRecoverFormatted
	}
	.Select(static v => v.Length)
	.Distinct()
	.Count() == 1
		? Empty
		: DumpContext.Document.Styles.FloatRight;

	var sanity = new Sanity(resourceUri, price, totalPrice - price, config.SanityPerPrime, ColorData.Value, $"Event ends {ToDump(eventEndTimeLocal)}");

	var okColor = ColorData.Value.First();
	var warningColor = ColorData.Value.Last();

	var report = new
	{
		SanityLeft      = sanity,
		TimeRequired    = Format(timeRequiredFormatted, styles: dateStyle),
		TimeLeft        = Format(timeLeftFormatted, noRecoverNeeded ? okColor : warningColor, dateStyle),
		TimeSaved       = Format(timeToRecoverFormatted, okColor, dateStyle),
		PrimesSaved     = Format(primesToRecover, okColor),
		TimeToRecover   = Format(timeToRecoverFormatted, warningColor, dateStyle),
		PrimesToRecover = Format(primesToRecover, warningColor),
		SanityPerPrime  = Format(config.SanityPerPrime.ToString(DumpContext.CultureInfo))
	};

	HorizontalRun(false,
		VerticalRun(
#if DUMP_CONFIG
			Merge(
				config,
				new
				{
					EventEndTimeLocal = eventEndTimeLocal,
					EventData         = EventData.Value,
					ImageUriMapData   = ImageUriMapData.Value,
					ImageData         = ImageData.Value,
					ColorData         = ColorData.Value
				}
			),
#endif
			Merge(
				new
				{
					report.SanityLeft,
					report.TimeRequired,
					report.TimeLeft
				},
				noRecoverNeeded
					? new
					{
						report.TimeSaved,
						report.PrimesSaved
					}
					: new
					{
						report.TimeToRecover,
						report.PrimesToRecover
					}
			),
			new
			{
				Stock = new Hyperlinq(uri, stockName),
				Left = currentEvent.Items
			}
		),
		Context.Containers.Image
	).Dump(title);

	new DumpContainer(){ Style = $"height: {DumpContext.Document.MarginBottom}" }.Dump();
}

static double CalcPrimesToRecover(double value, double sanityPerPrime = default)
{
	const double DailySanity = 240;

	return sanityPerPrime == default
		? value / DailySanity
		: value * DailySanity / sanityPerPrime;
}

static bool NoTimeLeft(TimeSpan timeSpan) =>
	timeSpan.Ticks <= 0;

static string Format(TimeSpan timeSpan, bool withSeconds = true) =>
	timeSpan.ToString((timeSpan.Days != 0 ? "d'd '" : Empty) + "hh':'mm" + (withSeconds ? "':'ss" : Empty), DumpContext.CultureInfo);

static object Format(object value, Color color = default, params string[] styles) =>
	WithStyle(
		value,
		Join(";", styles.Append($"color:{(color == default ? "inherit" : ToHtml(color))}")));

static (string Uri, string? Fragment) SplitUri(string uri)
{
	var anchorIndex = uri.IndexOf("#");
	return
	(
		uri[..(anchorIndex > 0 ? anchorIndex : uri.Length)],
		anchorIndex > 0 ? (uri[(anchorIndex + 1)..uri.Length]) : null
	);
}

static object ToDump(object input) =>
	input switch
	{
		DateOnly dateOnly =>
			dateOnly.ToString("D", DumpContext.CultureInfo),
		TimeOnly timeOnly =>
			timeOnly.ToString("t", DumpContext.CultureInfo),
		DateTime dateTime =>
			dateTime.ToString("f", DumpContext.CultureInfo),
		Color color =>
			WithStyle($"{DumpContext.Glyphs.Circle} {ToHtml(color):x}", $"color:{ToHtml(color)}"),
		Sanity sanity =>
			HorizontalRun(false,
				new SanityHyperlink(sanity),
				new DumpContainer($" {DumpContext.Glyphs.Circle} ") { Style = $"color: {ToHtml(sanity.Color)}" },
				new Span($"{sanity.PercentLeft.ToString(DumpContext.CultureInfo)}%").SetTitle(sanity.Title)),
		KeyValuePair<string, LazyImage> itemImage =>
			new
			{
				Key = new WikiHyperlinq(itemImage.Key.UnderscoreSpaces(), itemImage.Key),
				itemImage.Value
			},
		Item item =>
			new
			{
				item.Price,
				item.Count,
				Name     = IsNullOrEmpty(item.Name) ? (object)Empty : ItemImage.GetHyperlink(item.Name),
				item.Total,
				Required = Format(Format(FromDays(CalcPrimesToRecover(item.Total)), false), styles: DumpContext.Document.Styles.FloatRight)
			},
		_ => input
	};

abstract class TimePart<T> where T : TimePart<T>
{
	private readonly int _value;

	protected TimePart(string name, int value, int minValue, int maxValue) =>
		_value = value >= minValue && value <= maxValue
			? value
			: throw new($"{name} should be between {minValue} and {maxValue}. Got {value}");

	public static implicit operator int(TimePart<T> timePart) =>
		timePart._value;
}

sealed class Year : TimePart<Year>
{
	public const int Now = 2025;

	private Year(int year)
		: base(nameof(Year), year, Now, Now+1)
	{
	}

	public static Year Of(int year) =>
		new(year);
}

sealed class Month : TimePart<Month>
{
	private const int First = 1;
	private const int Last  = First+11;

	private Month(int month)
		: base(nameof(Month), month, First, Last)
	{
	}

	public static readonly Month Jan = new(First);
	public static readonly Month Feb = new(First+1);
	public static readonly Month Mar = new(First+2);
	public static readonly Month Apr = new(First+3);
	public static readonly Month May = new(First+4);
	public static readonly Month Jun = new(First+5);
	public static readonly Month Jul = new(First+6);
	public static readonly Month Aug = new(First+7);
	public static readonly Month Sep = new(First+8);
	public static readonly Month Oct = new(First+9);
	public static readonly Month Nov = new(First+10);
	public static readonly Month Dec = new(Last);
}

sealed class Day : TimePart<Day>
{
	private Day(int day)
		: base(nameof(Day), day, 1, 31)
	{
	}

	public static Day OfMonth(int day) =>
		new(day);
}

sealed class Hour : TimePart<Hour>
{
	private Hour(int hour)
		: base(nameof(Hour), hour, 0, 23)
	{
	}

	public static Hour OfDay(int hour) =>
		new(hour);
}

sealed class Minute : TimePart<Minute>
{
	private Minute(int minute)
		: base(nameof(Minute), minute, 0, 59)
	{
	}

	public static Minute OfHour(int minute) =>
		new(minute);
}

sealed record Sanity(string Uri, int Left, int Spent, int SanityPerPrime, IReadOnlyCollection<Color> Colors, string Title)
{
	public int PercentLeft
	{
		get
		{
			if(Left > Total)
			{
				throw new($"Data inconsistency detected: sanity left is greater than total sanity. Original event can be found in the {nameof(EventData)} below.");
			}

			var percent = Floor(100.0 * Left / Total);
			return percent == 0 ? 1 : (int)percent;
		}
	}

	public int Total { get; } = Left + Spent;

	public int PercentSpent =>
		100 - PercentLeft;

	public Color Color =>
		Colors.Last().InterpolateTo(PercentLeft / 100, Colors.First());
}

sealed record Item(int Price, int Count, string Name)
{
	public int Total { get; } = Price * Count;
}

sealed class Items : Parsable<Item>
{
	private const string Price = nameof(Price);
	private const string Count = nameof(Count);
	private const string Name  = nameof(Name);

	protected override string Regex { get; } = $@"^(?<{Price}>[+-]?\d+)(\s+(?<{Count}>[+]?\d+))?(\s+(?<{Name}>.+))?$";
	protected override string ErrorMessage { get; } = $"Expected: '[+-]price [[+]count] [name]'";

	public Items(string items) :
		base(items)
	{
		var duplicates = this
			.GroupBy()
			.Where(   static g => g.Count() > 1)
			.Select(  static g => (Item: g.Key, Times: g.Count()))
			.WhereNot(static v => IsNullOrEmpty(v.Item.Name))
			.ToArray();

		if(duplicates.Any())
		{
			throw new($"Duplicates found: {Join(", ", duplicates.Select(static d => $"{d.Times} for {d.Item}"))}");
		}
	}

	protected override Item Create(Match match) =>
		new
		(
			GetNumber(match, Price, FallbackAction<int>(Throw)),
			GetNumber(match, Count, 1),
			GetString(match, Name)
		);
}

struct Event
{
	public WikiHyperlinq WikiHyperlinq { get; private set; }
	public Items Items { get; private set; }

	public Items this[WikiHyperlinq wikiHyperlinq]
	{
		get => Items;
		set => (WikiHyperlinq, Items) = (wikiHyperlinq, value);
	}
}

sealed class SanityHyperlink : Hyperlink
{
	public SanityHyperlink(Sanity sanity)
		: base(sanity.Left.ToString(DumpContext.CultureInfo), sanity.Uri) =>
		this.SetTitle($"{(
			sanity.Spent > 0
				? $"{sanity.Spent} 🡒 {sanity.Total} {DumpContext.Glyphs.Circle} {sanity.PercentSpent}% sanity spent"
				: "No sanity spent yet"
			)}. Sanity per prime: {sanity.SanityPerPrime}");
}

static class ColorData
{
	public static readonly ReadOnlyCollection<Color> Value = new(new[]
	{
		0x209020,
		0xFF0000
	}
	.Select(Color.FromArgb)
#if CONFIG_INTERPOLATE_COLORS
	.Aggregate(new[] { new List<Color>() }, static (r, c) => { r.First().Add(c); return r; })
	.Select(static c => Enumerable.Range(0, 101).Select(p => c.Last().InterpolateTo(p / 100.0, c.First())))
	.SelectMany()
#endif
	.ToArray());
}

static class EventData
{
	public static readonly ReadOnlyDictionary<WikiHyperlinq, Items> Value = new(new Dictionary<WikiHyperlinq, Items>
	{
		/*
			One or two whitespace/tab(s) separated number(s) per line in any order: Price and/or Count and optional item name.
			EventStockParser.linq script parses event stock.
			Key is (EventURI#EventStock, EventName, EventCurrency)
		*/

		[new("A_Kazdelian_Rescue#Furnace_Node", "A Kazdelian Rescue", "Slag")] = new("""
		// A Kazdelian Rescue
		350		Holiday HD66
		10	6	Information Fragment
		100	2	Module Data Block
		30	8	Data Supplement Instrument
		10	40	Data Supplement Stick
		200		Bipolar Nanoflake
		65		Chip Catalyst
		60	2	Grindstone Pentahydrate
		20	4	Semi-Synthetic Solvent
		15	4	Loxic Kohl
		5	10	LMD
		5	5	Strategic Battle Record
		5	5	Tactical Battle Record
		6	5	Skill Summary - 3
		6	5	Skill Summary - 2
		4	5	Pure Gold
		6	5	Oriron
		30	5	Recruitment Permit
		4	10	Furniture Part
		"""),

		[new("Adventure_That_Cannot_Wait_for_the_Sun#Grand_Bazaar", "Adventure That Cannot Wait for the Sun", "Flowing_Glimmerdust")] = new("""
		// Adventure That Cannot Wait for the Sun
		200		Papyrus's Token
		240		Papyrus's Token
		280		Papyrus's Token
		320		Papyrus's Token
		360		Papyrus's Token
		30		Tuye's Token
		40		Tuye's Token
		50		Tuye's Token
		60		Tuye's Token
		70		Tuye's Token
		500		Bitter Herbs
		150	3	Headhunting Permit
		75	3	Module Data Block
		100	5	Crystalline Electronic Unit
		25	10	Orirock Concentration
		30	10	Polymerized Gel
		35	10	Refined Solvent
		35	10	Grindstone Pentahydrate
		60		Carved Wood Decoration
		60		'Go No Further'
		80		Lunar Desk
		80		Historical Stone Vase
		110		Wall of Life
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		8	15	Sugar Pack
		10	15	Aketon
		10	15	Manganese Ore
		12	15	Compound Cutting Fluid
		7	120	LMD
		5	30	Strategic Battle Record
		3	50	Tactical Battle Record
		1	150	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		2	40	Orirock Cube
		3	30	Sugar
		3	30	Polyester
		3	25	Oriron
		3	25	Polyketon
		4	20	Device
		6	5	Medic Chip
		2	200	Furniture Part
		"""),

		[new("Babel_(event)#Motorized_Logistical_Camp", "Babel", "Raw_Sugar_Lumps")] = new("""
		// Babel
		200		Odda's Token
		240		Odda's Token
		280		Odda's Token
		320		Odda's Token
		360		Odda's Token
		500		Newsgirl
		150	3	Headhunting Permit
		75	3	Module Data Block
		100	5	D32 Steel
		35	10	Keton Colloid
		35	10	RMA70-24
		40	10	Crystalline Circuit
		45	10	Cyclicene Prefab
		60		Witchcraft Workbench
		60		Welded Storage Box
		80		Oddities Showcase
		80		Load-bearing Pillar
		90		Ice-Cold Patchwork Wallpaper
		90		Never Even Flooring
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		8	15	Sugar Pack
		10	15	Manganese Ore
		10	15	Incandescent Alloy
		12	15	Transmuted Salt
		7	120	LMD
		5	30	Strategic Battle Record
		3	50	Tactical Battle Record
		1	150	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		2	40	Orirock Cube
		3	30	Sugar
		3	30	Polyester
		3	25	Oriron
		3	25	Polyketon
		4	20	Device
		6	5	Guard Chip
		2	200	Furniture Part
		"""),

		[new("Come_Catastrophes_or_Wakes_of_Vultures#Shack_by_the_Tower", "Come Catastrophes or Wakes of Vultures", "Shoddy_Fuel")] = new("""
		// Come Catastrophes or Wakes of Vultures
		200		Coldshot's Token
		240		Coldshot's Token
		280		Coldshot's Token
		320		Coldshot's Token
		360		Coldshot's Token
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	Bipolar Nanoflake
		25	10	Orirock Concentration
		40	10	Oriron Block
		35	10	Grindstone Pentahydrate
		25		Reinforced Work Chair
		25		Explosion-proof Fluorescent Lamp
		30		Shock-proof Pillar
		50		'Advance Patchwork'
		50		Assembly Workbench
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		8	15	Loxic Kohl
		12	10	Semi-Synthetic Solvent
		7	100	LMD
		5	25	Strategic Battle Record
		3	50	Tactical Battle Record
		1	120	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		2	25	Orirock Cube
		3	25	Oriron
		4	25	Device
		6	5	Sniper Chip
		2	200	Furniture Part
		"""),

		[new("Here_A_People_Sows#Shennong_Market", "Here A People Sows", "Sky_Pole")] = new("""
		// Here A People Sows
		200		Wanqing's Token
		240		Wanqing's Token
		280		Wanqing's Token
		320		Wanqing's Token
		360		Wanqing's Token
		70		Bibeak's Token
		500		Night Watcher
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	Polymerization Preparation
		25	10	Orirock Concentration
		35	10	Manganese Trihydrate
		35	10	RMA70-24
		35	10	Cutting Fluid Solution
		100		'The Archives'
		90		'Prize for Excellence'
		70		Responsive Floorlamp
		35		Small Dougong Lamp
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		8	15	Polyester Pack
		10	15	Manganese Ore
		12	15	Coagulating Gel
		10	15	Incandescent Alloy
		7	120	LMD
		5	30	Strategic Battle Record
		3	50	Tactical Battle Record
		1	150	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		2	40	Orirock Cube
		3	30	Sugar
		3	30	Polyester
		3	25	Oriron
		3	25	Polyketon
		4	20	Device
		6	5	Vanguard Chip
		2	200	Furniture Part
		"""),

		[new("Operation_Lucent_Arrowhead#Galería_Boutique", "Operation Lucent Arrowhead", "Galería_Stamp_Card")] = new("""
		// Operation Lucent Arrowhead
		200		Fuze's Token
		240		Fuze's Token
		280		Fuze's Token
		320		Fuze's Token
		360		Fuze's Token
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	Bipolar Nanoflake
		30	10	White Horse Kohl
		35	10	RMA70-24
		40	10	Solidified Fiber Board
		30		Galería Lighting
		30 		'Awards Ceremony'
		40		Secret Exhibit 'Do Not Touch'
		40		'Light and Heat'
		50		Galería Carpet
		50		'Off the Rails'
		50		'Help Yourself'
		100		Galería Lamp
		100		Galería Plant
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		7	15	Orirock Cluster
		10	10	Crystalline Component
		7	100	LMD
		5	25	Strategic Battle Record
		3	50	Tactical Battle Record
		1	100	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		2	25	Orirock Cube
		3	25	Sugar
		3	25	Oriron
		6	5	Guard Chip
		2	200	Furniture Part
		"""),

		[new("Path_of_Life#Materianomicon_Moderatrix", "Path of Life", @"""Emergency_Supplies""_Blueprint_Chip")] = new("""
		// Path of Life
		200		Underflow's Token
		240		Underflow's Token
		280		Underflow's Token
		320		Underflow's Token
		360		Underflow's Token
		500		Invisible Dirge
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	Polymerization Preparation
		35	10	Manganese Trihydrate
		35	10	RMA70-24
		35	10	Incandescent Alloy Block
		90		Stable Surgical Gimbal
		70		Port Panoramic Display
		70		Specialized Load-Bearing Floor
		60		Automatic File Sorter
		40		'Gladiia's Reflection'
		40		'Skadi's Refreshment Point'
		40		'Ulpianus's Curtain'
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		8	15	Polyester Pack
		12	10	Grindstone
		7	100	LMD
		5	25	Strategic Battle Record
		3	50	Tactical Battle Record
		1	120	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		3	25	Sugar
		3	25	Oriron
		4	25	Device
		6	5	Guard Chip
		2	200	Furniture Part
		"""),

		[new("The_Rides_to_Lake_Silberneherze#Onboard_Services", "The Rides to Lake Silberneherze", "Burdenbeast_Blind_Box")] = new("""
		// The Rides to Lake Silberneherze
		200		Harold's Token
		240		Harold's Token
		280		Harold's Token
		320		Harold's Token
		360		Harold's Token
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	D32 Steel
		35	10	Keton Colloid
		35	10	Manganese Trihydrate
		35	10	RMA70-24
		30		Window to the Snowy Mountains
		45		Shipping Counter
		60		Small Mailbox
		60		'Kjeragandr's Bones'
		90		Small Post Office Floor
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		10	15	Aketon
		12	10	Transmuted Salt
		7	100	LMD
		5	25	Strategic Battle Record
		3	50	Tactical Battle Record
		1	120	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		3	25	Oriron
		3	25	Polyketon
		4	25	Device
		6	5	Medic Chip
		2	200	Furniture Part
		"""),

		[new("Zwillingstürme_im_Herbst#Herbstmondeskonzert", "Zwillingstürme im Herbst", "Die_Klänge_von_den_Erinnerungen")] = new("""
		// Zwillingstürme im Herbst
		150	3	Headhunting Permit
		75	3	Module Data Block
		100	5	Polymerization Preparation
		100	5	Nucleic Crystal Sinter
		25	10	Orirock Concentration
		40	10	Oriron Block
		35	10	Grindstone Pentahydrate
		30	10	Polymerized Gel
		35	10	Cutting Fluid Solution
		30	10	Transmuted Salt Agglomerate
		110		Pipe Organ Literature Stand
		90		Library Stairs
		90		'Scenery of Knowledge'
		90		Knowledge-Seeking Hall Flooring
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		8	15	Polyester Pack
		10	15	Manganese Ore
		12	15	Coagulating Gel
		10	15	Crystalline Component
		7	120	LMD
		5	30	Strategic Battle Record
		3	50	Tactical Battle Record
		1	150	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		2	40	Orirock Cube
		3	30	Sugar
		3	30	Polyester
		3	25	Oriron
		3	25	Polyketon
		4	20	Device
		6	5	Guard Chip
		2	200	Furniture Part
		""")
	});
}
