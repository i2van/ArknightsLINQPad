<Query Kind="Program">
  <Namespace>ImageControl = LINQPad.Controls.Image</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>LINQPad.Controls.Core</Namespace>
  <Namespace>static LINQPad.Util</Namespace>
  <Namespace>static System.Convert</Namespace>
  <Namespace>static System.DateTime</Namespace>
  <Namespace>static System.Drawing.ColorTranslator</Namespace>
  <Namespace>static System.Environment</Namespace>
  <Namespace>static System.Globalization.NumberStyles</Namespace>
  <Namespace>static System.Math</Namespace>
  <Namespace>static System.String</Namespace>
  <Namespace>static System.Text.RegularExpressions.Regex</Namespace>
  <Namespace>static System.Text.RegularExpressions.RegexOptions</Namespace>
  <Namespace>static System.TimeSpan</Namespace>
  <Namespace>static System.TimeZoneInfo</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
</Query>

// Ongoing Arknights event status tracker.

#define DUMP_MISSING_IMAGES

/*
#define DUMP_CONFIG
#define CONFIG_INTERPOLATE_COLORS
//*/

#nullable enable

void Main()
{
	// TODO: Arknights events can be found at https://arknights.wiki.gg/wiki/Event
	var config = new
	{
		Title          = new WikiHyperlinq("Event#Ongoing", "Arknights Event Sanity Status"),
		// TODO: Specify your level max sanity.
		SanityPerPrime = 135,
		// TODO: Specify event's in-game date and time end.
		EventEndDate   = new DateOnly(Year.Now, Month.May, Day.OfMonth(21)),
		EventEndTime   = new TimeOnly(Hour.OfDay(3), Minute.OfHour(59)),
		// TODO: Specify in-game UTC offset.
		UtcOffset      = FromHours(-7),
		// TODO: Copy and paste current event data from DB.Events below. Remove item(s) when done.
		Event          = new Event
		{
			[new("Zwillingstürme_im_Herbst#Herbstmondeskonzert", "Zwillingstürme im Herbst", "Tune_Memories")] = new("""
			// Zwillingstürme im Herbst
			150	3	Headhunting Permit
			75	3	Module Data Block
			100	5	Polymerization Preparation
			100	5	Nucleic Crystal Sinter
			40	10	Oriron Block
			35	10	Grindstone Pentahydrate
			35	10	Cutting Fluid Solution
			30	10	Polymerized Gel
			30	10	Transmuted Salt Agglomerate
			25	10	Orirock Concentration
			//110		管风琴书卷台
			//90		图书馆阶梯
			//90		"知识之景"
			//90		求知厅堂地板
			15	10	Data Supplement Instrument
			5	60	Data Supplement Stick
			12	15	Coagulating Gel
			10	15	Manganese Ore
			10	15	Crystalline Component
			8	15	Polyester Pack
			7	120	LMD
			5	30	Strategic Battle Record
			3	50	Tactical Battle Record
			1	150	Frontline Battle Record
			4	25	Skill Summary - 3
			2	50	Skill Summary - 2
			4	20	Device
			3	25	Oriron
			3	30	Polyester
			3	25	Polyketon
			3	30	Sugar
			2	40	Orirock Cube
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
		HashSet<string> existingImagesKeys = new(DB.Images.Keys);
		var missingImages = DB.Events.Keys
			.SelectMany(static v => DB.Events[v].Select(static k => k))
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
		"Event is completed.".Dump(title);
		return;
	}

	var price = currentEvent.Items.Sum(static v => v.Total);

	if(price <= 0)
	{
		HorizontalRun("No more ", new Hyperlinq(resourceUri, "sanity"), " to ", new Hyperlinq(uri, "spend"), ".").Dump(title);
		return;
	}

	var totalPrice = DB.Events[eventHyperlinq].Sum(static v => v.Total);

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

	var sanity = new Sanity(resourceUri, price, totalPrice - price, config.SanityPerPrime, DB.Colors, $"Event ends {ToDump(eventEndTimeLocal)}");

	var okColor = DB.Colors.First();
	var warningColor = DB.Colors.Last();

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

	HorizontalRun(
		VerticalRun(
#if DUMP_CONFIG
			Merge(
				config,
				new
				{
					EventEndTimeLocal = eventEndTimeLocal,
					DB.Events,
					DB.ImageUriMap,
					DB.Images,
					DB.Colors
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

static object HorizontalRun(params object[] objs) =>
	Util.HorizontalRun(false, objs);

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
			HorizontalRun(
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
				Name = IsNullOrEmpty(item.Name) ? (object)Empty : GetImageHyperlink(item.Name),
				item.Total,
				Required = Format(Format(FromDays(CalcPrimesToRecover(item.Total)), false), styles: DumpContext.Document.Styles.FloatRight)
			},
		_ => input
	};

static (string Uri, string? Fragment) SplitUri(string uri)
{
	var anchorIndex = uri.IndexOf("#");
	return
	(
		uri[..(anchorIndex > 0 ? anchorIndex : uri.Length)],
		anchorIndex > 0 ? (uri[(anchorIndex + 1)..uri.Length]) : null
	);
}

static object GetImageHyperlink(string name)
{
	var itemHyperlinq = GetItemHyperlinq(name);

	if(!DB.Images.TryGetValue(name, out var imageData))
	{
		imageData = LazyImage.NotAvailable;
	}

	var hyperlink = new Hyperlink(itemHyperlinq.Text, itemHyperlinq.Uri);
	var htmlElement = hyperlink.HtmlElement;

	htmlElement.AddEventListener("mouseenter", ShowImageOnMouseEnterHandler);
	htmlElement.AddEventListener("mouseout",   static delegate { Context.Containers.Image.ClearContent(); });

	return hyperlink;

	void ShowImageOnMouseEnterHandler(object? elem, EventArgs e)
	{
		var top = htmlElement.InvokeScript(true, "eval", "targetElement.getBoundingClientRect().top - document.body.getBoundingClientRect().top");

		Context.Containers.Image.Style   = $"position: absolute; top: {top}px; z-index: 2";
		Context.Containers.Image.Content = imageData;
	}

	static Hyperlinq GetItemHyperlinq(string itemName)
	{
		var (uri, text) = GetUriName();

		return new WikiHyperlinq(uri, text);

		(string Uri, string Text) GetUriName()
		{
			const string token = "'s_Token";

			var itemUri = itemName.UnderscoreSpaces();

			return DB.ImageUriMap.TryGetValue(itemName, out var mappedUri)
					? (mappedUri, itemName)
					: itemUri.Contains(token)
						? ($"Operator_Token/5-star#{itemUri[..(itemUri.Length - token.Length)]}", itemName)
						: (itemUri, itemName);
		}
	}
}

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

class Items : IEnumerable<Item>
{
	private const string Comment = "//";

	private const string Price = nameof(Price);
	private const string Count = nameof(Count);
	private const string Name  = nameof(Name);

	private static readonly Regex ItemRegex = new($@"^(?<{Price}>[+-]?\d+)(\s+(?<{Count}>[+]?\d+))?(\s+(?<{Name}>.+))?$", ExplicitCapture, FromMilliseconds(100));

	private readonly Lazy<Item[]> _items;

	public Items(string items) =>
		_items = new(() => Parse(items));

	public IEnumerator<Item> GetEnumerator() =>
		_items.Value.AsEnumerable().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		_items.Value.GetEnumerator();

	private static Item[] Parse(string values)
	{
		var items = values
			.Replace(Comment, NewLine + Comment)
			.Split(NewLine.ToCharArray())
			.Select(static v => v.Trim())
			.Where( static v => !IsNullOrEmpty(v))
			.Where( static v => !v.StartsWith(Comment))
			.Select(CreateItem)
			.ToArray();

		var duplicates = items
			.GroupBy(static v => v)
			.Where(  static g => g.Count() > 1)
			.Select( static g => (Item: g.Key, Times: g.Count()))
			.Where(  static v => !IsNullOrEmpty(v.Item.Name))
			.ToArray();

		if(duplicates.Any())
		{
			throw new($"Duplicates found: {Join(", ", duplicates.Select(static d => $"{d.Times} for {d.Item}"))}");
		}

		return items;

		static Item CreateItem(string str)
		{
			var match = ItemRegex.Match(str);
			if(!match.Success)
			{
				Throw();
			}

			return new
			(
				GetNumber(Price, FallbackAction<int>(Throw)),
				GetNumber(Count, FallbackValue(1)),
				GetString(Name)
			);

			string GetString(string key) =>
				match.Groups[key].Value;

			int GetNumber(string key, Func<int> fallbackValue) =>
				int.TryParse(GetString(key), Integer, DumpContext.CultureInfo, out var value)
					? value
					: fallbackValue();

			[DoesNotReturn]
			void Throw() =>
				throw new($"Invalid data: '{str}'. Expected: '[+-]price [[+]count] [name]'");

			static Func<T> FallbackAction<T>(Action func) =>
				delegate { func(); return default!; };

			static Func<T> FallbackValue<T>(T v) =>
				() => v;
		}
	}
}

class WikiHyperlinq : Hyperlinq
{
	private const string WikiUri = $"{DumpContext.Url.Wiki}/wiki/";

	private readonly string? _metadata;

	public WikiHyperlinq(string uri, string text, string? metadata = null)
		: base(WikiUri + uri, text) =>
		_metadata = metadata;

	public string? ProcessMetadata(Func<string, string> process) =>
		_metadata is null ? null : process(_metadata!);
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

class ItemImage
{
	public string Name { get; }
	public LazyImage Image { get; }

	public ItemImage(string name, string pathUri, string? fileName = null, int? height = null) =>
		(Name, Image) = (name, new($"{DumpContext.Url.Wiki}/images/{pathUri}/{(fileName ?? name).UnderscoreSpaces()}.png", height));
}

class SkinImage : ItemImage
{
	public SkinImage(string name, string pathUri, string fileName)
		: base(name, pathUri, fileName, DumpContext.ImageHeight.Skin)
	{
	}
}

class LazyImage
{
	public static readonly LazyImage NotAvailable = new("https://upload.wikimedia.org/wikipedia/commons/a/ac/No_image_available.svg", DumpContext.ImageHeight.NotAvailable);

	private readonly Lazy<ImageControl> _value;

	private ImageControl Control => _value.Value;

	public LazyImage(string imageUri, int? height = null)
	{
		_value = new(CreateControl);

		ImageControl CreateControl()
		{
			var control = new ImageControl(new Uri(imageUri));

			if((height ??= DumpContext.ImageHeight.Item) > 0)
			{
				control.Height = height.Value;
			}

			return control;
		}
	}

	public object ToDump() =>
		Control;
}

static class Extensions
{
	private static readonly char[] Quotes = "\"'".ToCharArray();

	private static readonly TimeSpan AlmostSecond = TimeSpan
#if NET7
		.FromMicroseconds(999_999);
#else
		.FromMilliseconds(999);
#endif

	public static string UnderscoreSpaces(this string str) =>
		str.Trim(Quotes).Replace("\" ", " ").Replace("\' ", " ").Replace(' ', '_');

	public static string SpaceUnderscores(this string str) =>
		str.Replace('_', ' ');

	public static TimeSpan FloorSeconds(this TimeSpan timeSpan) =>
		timeSpan < Zero
			? timeSpan
			: timeSpan + AlmostSecond;

	public static Color InterpolateTo(this Color fromColor, double ratio, Color toColor)
	{
		var alt = 1.0 - ratio;

		var (xFrom, yFrom, zFrom) = (fromColor.R, fromColor.G, fromColor.B);
		var (xTo, yTo, zTo) = (toColor.R, toColor.G, toColor.B);

		var (magnitudeFrom, magnitudeTo) = (CalcMagnitude(xFrom, yFrom, zFrom), CalcMagnitude(xTo, yTo, zTo));
		var (x, y, z) = (CalcActual(xFrom, xTo), CalcActual(yFrom, yTo), CalcActual(zFrom, zTo));

		var magnitude = CalcActual(magnitudeFrom, magnitudeTo);
		var scale = magnitude / CalcMagnitude(x, y, z);

		return Color.FromArgb(ToColor(x), ToColor(y), ToColor(z));

		static Double CalcMagnitude(double x, double y, double z) =>
			Sqrt(x * x + y * y + z * z);

		double CalcActual(double from, double to) =>
			ratio * from + alt * to;

		int ToColor(double value) =>
			Max(0, Min(0xFF, (int)Round(value * scale)));
	}
}

static class ControlsExtensions
{
	private static T SetAttribute<T>(this T control, string attribute, [CallerArgumentExpression(nameof(attribute))] string attributeName = "")
		where T : Control
	{
		control.HtmlElement[attributeName] = attribute;
		return control;
	}

	public static T SetTitle<T>(this T control, string title) where T : Control =>
		control.SetAttribute(title);
}

static class HtmlExtensions
{
	private const string H1ToA = $"{nameof(HtmlExtensions)}_{nameof(H1ToA)}";

	static HtmlExtensions() =>
		HtmlHead.AddScript($$"""
			function {{H1ToA}}(text, uri){
				const elem = [].find.call(document.getElementsByTagName("h1"), elem => elem.innerHTML === text);
				if(elem){
					elem.innerHTML = `<a href="${uri}" class="headingpresenter reference">${elem.innerHTML}</a>`;
				}
			}
			""");

	public static void AsHyperlink(this string text, string uri) =>
		InvokeScript(false, H1ToA, text, uri);
}

ref struct DisposableAction
{
	private readonly Action _action;

	public DisposableAction(Action action) =>
		_action = action;

	public void Dispose() =>
		_action?.Invoke();
}

static DumpContext Context = null!;

class DumpContext
{
	public static readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;

	public static class Url
	{
		public const string Wiki = "https://arknights.wiki.gg";
	}

	public static class ImageHeight
	{
		public const int Item         = -96; // <= 0 for as is size.
		public const int Skin         = 480;
		public const int NotAvailable = Skin / 2;
	}

	public static class Document
	{
		public const string MarginBottom = "6.5rem";

		public static class Styles
		{
			public const string FloatRight = "float: right";
		}
	}

	public static class Glyphs
	{
		public const char Circle = '⬤';
	}

	public class DumpContainers
	{
		public readonly DumpContainer Image = new();
	}

	public readonly DumpContainers Containers = new();
}

static class DB
{
	// REGEX:
	// ^.+/([a-z0-9]/[a-z0-9]{2})/(.+?)\.png.*$
	// new\("$2", "$1"\),
	public static readonly ReadOnlyDictionary<string, LazyImage> Images = new(new ItemImage[]
	{
		new SkinImage("Major Field", "4/46", "Earthspirit_Skin_1"),
		new("'Advance Patchwork'", "d/d1"),
		new("'Audible Redemption'", "9/9d"),
		new("'Cohesion'", "f/fe"),
		new("'Collection'", "c/c1", "Collection_(furniture)"),
		new("'Concealed Edge'", "4/4a"),
		new("'Cultivating Hope'", "9/9f"),
		new("'Duel With Oneself'", "9/9a"),
		new("'Elevations'", "4/41"),
		new("'Fowlbeast Accommodations'", "4/4f"),
		new("'Implantation'", "2/22"),
		new("'Indications'", "0/02"),
		new("'Nostalgic Habitations'", "b/b1"),
		new("'Purge of Flaws'", "4/49"),
		new("'Record of Suffering'", "d/d0"),
		new("'Simplicity'", "e/ec"),
		new("'Treading Sand'", "f/fd"),
		new("'Treeshade Lineations'", "7/79"),
		new("'Truck Passage'", "e/e7"),
		new("'Witness of Friendship'", "f/fa"),
		new("Aketon", "d/d3"),
		new("Assembly Workbench", "2/2e"),
		new("Bipolar Nanoflake", "b/b6"),
		new("Bryophyta's Token", "e/eb"),
		new("Caster Chip", "9/94"),
		new("Chip Catalyst", "3/32"),
		new("Coagulating Gel", "b/bd"),
		new("Coldshot's Token", "9/9a"),
		new("Compound Cutting Fluid", "2/2c"),
		new("Cozy Plaid Rug", "e/eb"),
		new("Crystalline Circuit", "4/46"),
		new("Crystalline Component", "2/20"),
		new("Crystalline Electronic Unit", "d/d0"),
		new("Cured Fiberboard", "6/67"),
		new("Cutting Fluid Solution", "9/93"),
		new("D32 Steel", "6/68"),
		new("Data Supplement Instrument", "6/6d"),
		new("Data Supplement Stick", "f/f8"),
		new("Defender Chip", "2/24"),
		new("Device", "a/a4"),
		new("Dizzy Spinning Chair", "6/63"),
		new("Exhibition Room Ceiling Light (Bright)", "d/de"),
		new("Exhibition Room Ceiling Light (Dim)", "0/0b"),
		new("Explosion-proof Fluorescent Lamp", "e/e0"),
		new("Frontline Battle Record", "f/f7"),
		new("Furniture Part", "0/0d"),
		new("Fuze's Token", "b/ba"),
		new("Grindstone Pentahydrate", "a/a5"),
		new("Grindstone", "6/69"),
		new("Guard Chip", "6/63"),
		new("Harold's Token", "8/8b"),
		new("Headhunting Permit", "0/0b"),
		new("Incandescent Alloy Block", "a/a3"),
		new("Incandescent Alloy", "f/f4"),
		new("Information Fragment", "1/1d"),
		new("Insider's Token", "b/bc"),
		new("Integrated Device", "f/fb"),
		new("Jieyun's Token", "0/03"),
		new("Keton Colloid", "a/ae"),
		new("LMD", "e/e7"),
		new("Loxic Kohl", "4/4a"),
		new("Manganese Ore", "6/6a"),
		new("Manganese Trihydrate", "c/cd"),
		new("Medic Chip", "e/eb"),
		new("Mischievously Spliced Table", "9/9a"),
		new("Module Data Block", "6/66"),
		new("Nucleic Crystal Sinter", "7/7e"),
		new("Optimized Device", "f/fd"),
		new("Orirock Cluster", "4/4d"),
		new("Orirock Concentration", "d/d4"),
		new("Orirock Cube", "d/d8"),
		new("Oriron Block", "5/52"),
		new("Oriron Cluster", "8/87"),
		new("Oriron", "4/44"),
		new("Patching Planks", "8/8c"),
		new("Polyester Pack", "f/f3"),
		new("Polyester", "3/3a"),
		new("Polyketon", "9/96"),
		new("Polymerization Preparation", "9/9c"),
		new("Polymerized Gel", "6/66"),
		new("Projectile Interception System", "a/a0"),
		new("Public Bookshelf", "5/54"),
		new("Pure Gold", "0/0e"),
		new("Pure White Stone-Tiled Table", "1/10"),
		new("Puzzle's Token", "6/68"),
		new("Rattan Pendant Lamp", "7/74"),
		new("Recruitment Permit", "3/3b"),
		new("Refined Solvent", "9/9f"),
		new("Reinforced Work Chair", "a/ae"),
		new("Reinforcement Debris", "7/7e"),
		new("RMA70-12", "1/10"),
		new("RMA70-24", "f/f1"),
		new("Semi-Synthetic Solvent", "5/58"),
		new("Shock-proof Pillar", "2/22"),
		new("Signal Disruptor", "3/33"),
		new("Silence the Paradigmatic's Token", "2/20"),
		new("Skill Summary - 2", "b/b0", "Skill_Summary_Volume_2"),
		new("Skill Summary - 3", "a/a7", "Skill_Summary_Volume_3"),
		new("Sniper Chip", "c/cd"),
		new("Specialist Chip", "e/e2"),
		new("Strategic Battle Record", "5/55"),
		new("Stumpy Little Fridge", "f/f1"),
		new("Sugar Pack", "2/23"),
		new("Sugar", "a/a9"),
		new("Supporter Chip", "d/d7"),
		new("Surveillance Camera", "6/65"),
		new("Tachanka's Token", "a/a9"),
		new("Tactical Battle Record", "a/a7"),
		new("Throwable Discharge Device", "7/7a"),
		new("Toasty Shop Lamp", "6/6c"),
		new("Transmuted Salt Agglomerate", "b/bd"),
		new("Transmuted Salt", "b/bd"),
		new("Used Folding Chair", "c/c1"),
		new("Vanguard Chip", "d/df"),
		new("Wall-mounted Newspaper Rack", "9/91"),
		new("Wanqing's Token", "f/fc"),
		new("White Horse Kohl", "4/4a"),
		new("Wooden Coat Hanger", "7/7e"),
		new("Yellowy Gauze Curtain", "9/9d")
	}.ToDictionary(static v => v.Name, static v => v.Image));

	public static readonly ReadOnlyCollection<Color> Colors = new(new[]
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
	.ToList());

	public static readonly ReadOnlyDictionary<string, string> ImageUriMap = new(new Dictionary<string, string>
	{
		// Furniture.
		["'Collection'"] = "Collection_(furniture)",
		// Skins.
		["Major Field"]  = "Earthspirit/Gallery#Major_Field",
		// 6-star tokens.
		["Silence the Paradigmatic's Token"] = "Operator_Token/6-star#Silence_the_Paradigmatic"
	});

	public static readonly ReadOnlyDictionary<WikiHyperlinq, Items> Events = new(new Dictionary<WikiHyperlinq, Items>
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

		[new("Here_a_People_Sows#Shennong_Market", "Here a People Sows", "Tianzhuang")] = new("""
		// Here a People Sows
		200		Wanqing's Token
		240		Wanqing's Token
		280		Wanqing's Token
		320		Wanqing's Token
		360		Wanqing's Token
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	Polymerization Preparation
		35	10	Manganese Trihydrate
		35	10	RMA70-24
		35	10	Cutting Fluid Solution
		25	10	Orirock Concentration
		//100		"藏经阁"
		//90		"优秀作品"
		//70		感应式立灯
		//35		小型斗拱灯
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		12	15	Coagulating Gel
		10	15	Manganese Ore
		10	15	Incandescent Alloy
		8	15	Polyester Pack
		7	120	LMD
		5	30	Strategic Battle Record
		3	50	Tactical Battle Record
		1	120	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		4	20	Device
		3	25	Oriron
		3	25	Polyketon
		3	30	Polyester
		3	30	Sugar
		2	40	Orirock Cube
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

		[new("Il_Siracusano/Rerun#Dipartimento_di_Sicurezza_Alimentare", "Il Siracusano Rerun", "Permesso_di_Importazione")] = new("""
		// Il Siracusano Rerun
		150	3	Headhunting Permit
		100		Bipolar Nanoflake
		100		Crystalline Electronic Unit
		35	3	Keton Colloid
		35	3	Orirock Concentration
		35	3	Manganese Trihydrate
		35	3	Incandescent Alloy Block
		35	3	Polymerized Gel
		35	3	Refined Solvent
		110		'Truck Passage'
		90		Patching Planks
		90		Wooden Coat Hanger
		85		Public Bookshelf
		15	5	RMA70-12
		8	5	Sugar Pack
		7	20	LMD
		5	5	Strategic Battle Record
		3	10	Tactical Battle Record
		1	20	Frontline Battle Record
		4	10	Skill Summary - 3
		2	20	Skill Summary - 2
		3	8	Oriron
		3	8	Polyketon
		2	8	Orirock Cube
		6	5	Vanguard Chip
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

		[new("Operation_Lucent_Arrowhead#Galeria_Souvenir_Shop", "Operation Lucent Arrowhead", "Galeria_Stamp_Card")] = new("""
		// Operation Lucent Arrowhead
		200		Fuze's Token
		240		Fuze's Token
		280		Fuze's Token
		320		Fuze's Token
		360		Fuze's Token
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	Bipolar Nanoflake
		35	10	Manganese Trihydrate
		40	10	Cured Fiberboard
		35	10	RMA70-24
		30	10	White Horse Kohl
		//100		艺术馆吊灯
		//100		艺术馆盆栽
		//50		"脱轨"
		//50		"请自由使用"
		//40		隐藏展品"禁止触摸"
		//40		"光和热"
		//30		艺术馆照明系统
		//30		"颁奖典礼"
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		10	15	Crystalline Component
		7	15	Orirock Cluster
		7	100	LMD
		5	25	Strategic Battle Record
		3	50	Tactical Battle Record
		1	100	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		3	25	Oriron
		3	25	Sugar
		2	25	Orirock Cube
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
		//90		小邮局地板
		//60		小邮筒
		//60		"耶拉冈德之骨"
		//45		寄件柜台
		//30		"雪山之窗"
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		12	10	Transmuted Salt
		10	15	Aketon
		7	100	LMD
		5	25	Strategic Battle Record
		3	50	Tactical Battle Record
		1	120	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		4	25	Device
		3	25	Orirock Cube
		3	25	Polyketon
		6	5	Medic Chip
		2	200	Furniture Part
		"""),

		[new("To_the_Grinning_Valley#Sandbeast's_Cave", "To the Grinning Valley", "Spicy_Bottletree_Sap")] = new("""
		// To the Grinning Valley
		350		Major Field
		10	7	Information Fragment
		100		Module Data Block
		30	8	Data Supplement Instrument
		10	40	Data Supplement Stick
		200		Crystalline Electronic Unit
		65		Chip Catalyst
		50	2	Polymerized Gel
		20	4	Aketon
		20	4	Semi-Synthetic Solvent
		5	10	LMD
		5	5	Strategic Battle Record
		5	5	Tactical Battle Record
		5	5	Skill Summary - 3
		5	5	Skill Summary - 2
		4	5	Pure Gold
		6	5	Oriron
		30	5	Recruitment Permit
		4	10	Furniture Part
		"""),

		[new("What_the_Firelight_Casts/Rerun#River_Valley_Caravan", "What the Firelight Casts Rerun", "Manuscripts_of_Ballads")] = new("""
		// What the Firelight Casts Rerun
		200		Puzzle's Token
		240		Puzzle's Token
		280		Puzzle's Token
		320		Puzzle's Token
		360		Puzzle's Token
		150	3	Headhunting Permit
		100		Bipolar Nanoflake
		35	3	Keton Colloid
		35	3	Incandescent Alloy Block
		30	3	Polymerized Gel
		150		Cozy Plaid Rug
		100		Pure White Stone-Tiled Table
		60		Rattan Pendant Lamp
		25	4	Used Folding Chair
		8	5	Polyester Pack
		10	5	Manganese Ore
		7	20	LMD
		5	5	Strategic Battle Record
		3	10	Tactical Battle Record
		1	20	Frontline Battle Record
		4	10	Skill Summary - 3
		2	20	Skill Summary - 2
		4	8	Device
		3	8	Sugar
		2	8	Orirock Cube
		6	5	Vanguard Chip
		"""),

		[new("Where_Vernal_Winds_Will_Never_Blow#The_Swordsmith", "Where Vernal Winds Will Never Blow", "Freshly-Brewed_Liedaozi")] = new("""
		// Where Vernal Winds Will Never Blow
		200		Jieyun's Token
		240		Jieyun's Token
		280		Jieyun's Token
		320		Jieyun's Token
		360		Jieyun's Token
		150	3	Headhunting Permit
		75	2	Module Data Block
		100	5	D32 Steel
		25	10	Orirock Concentration
		35	10	Grindstone Pentahydrate
		50	10	Optimized Device
		150		'Concealed Edge'
		100		'Duel With Oneself'
		80		'Treading Sand'
		60		'Cohesion'
		40		'Simplicity'
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		15	15	RMA70-12
		10	10	Oriron Cluster
		7	100	LMD
		5	25	Strategic Battle Record
		3	50	Tactical Battle Record
		1	120	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		2	25	Orirock Cube
		3	25	Sugar
		3	25	Polyketon
		6	5	Sniper Chip
		2	200	Furniture Part
		"""),

		[new("Zwillingstürme_im_Herbst#Herbstmondeskonzert", "Zwillingstürme im Herbst", "Tune_Memories")] = new("""
		// Zwillingstürme im Herbst
		150	3	Headhunting Permit
		75	3	Module Data Block
		100	5	Polymerization Preparation
		100	5	Nucleic Crystal Sinter
		40	10	Oriron Block
		35	10	Grindstone Pentahydrate
		35	10	Cutting Fluid Solution
		30	10	Polymerized Gel
		30	10	Transmuted Salt Agglomerate
		25	10	Orirock Concentration
		//110		管风琴书卷台
		//90		图书馆阶梯
		//90		"知识之景"
		//90		求知厅堂地板
		15	10	Data Supplement Instrument
		5	60	Data Supplement Stick
		12	15	Coagulating Gel
		10	15	Manganese Ore
		10	15	Crystalline Component
		8	15	Polyester Pack
		7	120	LMD
		5	30	Strategic Battle Record
		3	50	Tactical Battle Record
		1	150	Frontline Battle Record
		4	25	Skill Summary - 3
		2	50	Skill Summary - 2
		4	20	Device
		3	25	Oriron
		3	30	Polyester
		3	25	Polyketon
		3	30	Sugar
		2	40	Orirock Cube
		6	5	Guard Chip
		2	200	Furniture Part
		""")
	});
}
