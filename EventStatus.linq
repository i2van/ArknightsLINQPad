<Query Kind="Program">
  <Namespace>static System.DateTime</Namespace>
  <Namespace>static System.Drawing.ColorTranslator</Namespace>
  <Namespace>static System.TimeZoneInfo</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
</Query>

// Ongoing Arknights Global event status tracker.

#nullable enable

#load "./lib/Context.linq"
#load "./lib/Extensions.linq"
#load "./lib/Images.linq"
#load "./lib/Parsable.linq"
#load "./lib/WikiHyperlinq.linq"

//#define DUMP_MISSING_IMAGES

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
		EventEndDate   = new DateOnly(Year.Now, Month.Sep, Day.OfMonth(19)),
		EventEndTime   = new TimeOnly(Hour.OfDay(3), Minute.OfHour(59)),
		// TODO: Specify in-game UTC offset.
		UtcOffset      = FromHours(-7),
		// TODO: Copy and paste current event data from EventData.Value below. Remove item(s) when done.
		Event          = new Event
		{
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
			30		Galería Lightning
			30 		'Awards Ceremony'
			40		Secret Exibit 'Do Not Touch'
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
			.SelectMany(static v => EventData.Value[v].Select(static k => k))
			.Concat(config.Event.Items)
			.Where(static v => !string.IsNullOrWhiteSpace(v.Name))
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

class Year : TimePart<Year>
{
	public const int Now = 2024;

	private Year(int year)
		: base(nameof(Year), year, Now, Now+1)
	{
	}

	public static Year Of(int year) =>
		new(year);
}

class Month : TimePart<Month>
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

class Day : TimePart<Day>
{
	private Day(int day)
		: base(nameof(Day), day, 1, 31)
	{
	}

	public static Day OfMonth(int day) =>
		new(day);
}

class Hour : TimePart<Hour>
{
	private Hour(int hour)
		: base(nameof(Hour), hour, 0, 23)
	{
	}

	public static Hour OfDay(int hour) =>
		new(hour);
}

class Minute : TimePart<Minute>
{
	private Minute(int minute)
		: base(nameof(Minute), minute, 0, 59)
	{
	}

	public static Minute OfHour(int minute) =>
		new(minute);
}

record Sanity(string Uri, int Left, int Spent, int SanityPerPrime, IReadOnlyCollection<Color> Colors, string Title)
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

record Item(int Price, int Count, string Name)
{
	public int Total { get; } = Price * Count;
}

class Items : Parsable<Item>
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
			.GroupBy(static v => v)
			.Where(  static g => g.Count() > 1)
			.Select( static g => (Item: g.Key, Times: g.Count()))
			.Where(  static v => !IsNullOrEmpty(v.Item.Name))
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

class SanityHyperlink : Hyperlink
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
	.SelectMany(static r => r)
#endif
	.ToArray());
}

static class EventData
{
	public static readonly ReadOnlyDictionary<WikiHyperlinq, Items> Value = new(new Dictionary<WikiHyperlinq, Items>
	{
		// One or two whitespace/tab(s) separated number(s) per line in any order: Price and/or Count and optional item name.
		// EventStockParser.linq script parses event stock.
		// Key is (EventURI#EventStock, EventName, EventCurrency)
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

		[new("Hortus_de_Escapismo#Convivia_Cenarum_Localia", "Hortus de Escapismo", "Holy_Statue_Fragment")] = new("""
		// Hortus de Escapismo
		200		Insider's Token
		240		Insider's Token
		280		Insider's Token
		320		Insider's Token
		360		Insider's Token
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	Crystalline Electronic Unit
		25	10	Orirock Concentration
		35	10	Manganese Trihydrate
		35	10	RMA70-24
		40		'Cultivating Hope'
		40		'Record of Suffering'
		40		'Audible Redemption'
		40		'Purge of Flaws'
		60		'Witness of Friendship'
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		8	15	Sugar Pack
		8	10	Loxic Kohl
		7	100	LMD
		5	25	Strategic Battle Record
		3	50	Tactical Battle Record
		1	120	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		2	25	Orirock Cube
		3	25	Oriron
		3	25	Polyketon
		6	5	Sniper Chip
		2	200	Furniture Part
		"""),

		[new("Lone_Trail#Special_Case_Contact,_C.U.D.O.D.", "Lone Trail", "Flight_Data_Recorder_Chip")] = new("""
		// Lone Trail
		250		Silence the Paradigmatic's Token
		300		Silence the Paradigmatic's Token
		350		Silence the Paradigmatic's Token
		400		Silence the Paradigmatic's Token
		450		Silence the Paradigmatic's Token
		150	3	Headhunting Permit
		75	3	Module Data Block
		100	5	Bipolar Nanoflake
		35	10	Keton Colloid
		30	10	White Horse Kohl
		35	10	Refined Solvent
		30	10	Transmuted Salt Agglomerate
		60		'Collection'
		60		'Indications'
		80		'Implantation'
		100		'Fowlbeast Accommodations'
		100		'Treeshade Lineations'
		150		'Elevations'
		200		'Nostalgic Habitations'
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		7	15	Orirock Cluster
		15	15	Integrated Device
		10	15	RMA70-12
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
		6	5	Supporter Chip
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
		30		Galería Lightning
		30 		'Awards Ceremony'
		40		Secret Exibit 'Do Not Touch'
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

		[new("Operation_Originium_Dust/Rerun#Commissary", "Operation Originium Dust Rerun", "Rhodes_Island_Resource_Allocation_Certificate")] = new("""
		// Operation Originium Dust Rerun
		200		Tachanka's Token
		240		Tachanka's Token
		280		Tachanka's Token
		320		Tachanka's Token
		360		Tachanka's Token
		150	3	Headhunting Permit
		100		D32 Steel
		45	3	Oriron Block
		40	3	Grindstone Pentahydrate
		35	3	White Horse Kohl
		55		Signal Disruptor
		45		Reinforcement Debris
		45		Projectile Interception System
		45		Wall-mounted Newspaper Rack
		30		Surveillance Camera
		30		Throwable Discharge Device
		25	2	Exhibition Room Ceiling Light (Bright)
		25	2	Exhibition Room Ceiling Light (Dim)
		15	5	RMA70-12
		12	5	Coagulating Gel
		7	20	LMD
		5	5	Strategic Battle Record
		3	10	Tactical Battle Record
		1	20	Frontline Battle Record
		4	10	Skill Summary - 3
		2	20	Skill Summary - 2
		4	8	Device
		3	8	Oriron
		2	8	Orirock Cube
		6	5	Guard Chip
		"""),

		[new(@"So_Long,_Adele#""White_Volcano""", "So Long, Adele", "Fluffy_Critter_Wool")] = new("""
		// So Long, Adele
		200		Bryophyta's Token
		240		Bryophyta's Token
		280		Bryophyta's Token
		320		Bryophyta's Token
		360		Bryophyta's Token
		150	3	Headhunting Permit
		75	3	Module Data Block
		100	5	Bipolar Nanoflake
		25	10	Orirock Concentration
		30	10	Transmuted Salt Agglomerate
		35	10	Manganese Trihydrate
		40	10	Crystalline Circuit
		70		Mischievously Spliced Table
		70		Stumpy Little Fridge
		75		Yellowy Gauze Curtain
		100		Toasty Shop Lamp
		45	2	Dizzy Spinning Chair
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		8	15	Polyester Pack
		12	15	Coagulating Gel
		12	15	Compound Cutting Fluid
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

		[new("What_the_Firelight_Casts/Rerun#River_Valley_Caravan", "What the Firelight Casts Rerun", "Manuscripts_of_Ballads")] = new("""
		// What the Firelight Casts Rerun
		150	3	Headhunting Permit
		100		Bipolar Nanoflake
		35	3	Keton Colloid
		30	3	Polymerized Gel
		35	3	Incandescent Alloy Block
		8	5	Polyester Pack
		10	5	Manganese Ore
		7	20	LMD
		5	5	Strategic Battle Record
		3	10	Tactical Battle Record
		1	20	Frontline Battle Record
		4	10	Skill Summary - 3
		2	20	Skill Summary - 2
		2	8	Orirock Cube
		3	8	Sugar
		4	8	Device
		6	5	Vanguard Chip
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
