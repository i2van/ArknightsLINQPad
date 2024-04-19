<Query Kind="Program">
  <Namespace>ImageControl = LINQPad.Controls.Image</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>static LINQPad.Util</Namespace>
  <Namespace>static System.Environment</Namespace>
  <Namespace>static System.Globalization.CultureInfo</Namespace>
  <Namespace>static System.Globalization.NumberStyles</Namespace>
  <Namespace>static System.String</Namespace>
  <Namespace>static System.Text.RegularExpressions.Regex</Namespace>
  <Namespace>static System.Text.RegularExpressions.RegexOptions</Namespace>
  <Namespace>static System.TimeSpan</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
</Query>

// Arknights operators modules missions.

#nullable enable

/*
#define DUMP_OPERATORS
#define DUMP_ALL_OPERATOR_MODULES
//*/

static DumpContext Context = null!;

void Main()
{
	// TODO: Add operator(s) with (optionally) X or Y modules.
	var operators = new Operators("""
		// name <tab> type (X, Y, D or empty for all)
	""")
#if DUMP_OPERATORS
		.OrderBy(static op => op.Name)
		.Dump("Operators")
#endif
	;

	var operatorModules = DB.OperatorModules
#if !DUMP_ALL_OPERATOR_MODULES
		.OnlyFor(operators)
#endif
		.OrderBy(static v => v.Operator)
		.GroupBy(static v => v.Stage)
		.Select( static v => new
		{
			Stage     = v.Key,
			Names     = v.JoinStrings(static v => v.Operator),
			Operators = v.Select(Pass),
			Stars     = v.JoinStrings(static v => v.Stars),
			MaxStars  = v.Max(static v => v.Stars),
			Total     = v.Count()
		})
		.OrderByDescending(static v => v.Total)
		.ThenByDescending( static v => v.MaxStars)
		.ThenBy(static v => v.Names)
		.Select((v, i) => HighlightIf(v.Total > 1, new
		{
			ID          = i + 1,
			Stage       = new WikiHyperlinq(v.Stage, "Information"),
			Operators   = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Operator))),
			v.Stars,
			Module      = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Operator, v.Module, v.Module))),
			v.Total,
			Paradox     = VerticalRun(v.Operators.Select(static v => v.Paradox ? new Hyperlinq($"https://www.youtube.com/results?search_query={Uri.EscapeDataString(v.Operator)}+paradox+simulation", "YouTube") : (object)Empty)),
			E2Materials = VerticalRun(v.Operators.Select(static v => GetMaterials(v.Operator, v.E2Materials)))
		}))
		.ToArray();

	const string title = "Joined Operators Modules";

	Context = new();

	HorizontalRun(false,
		(operatorModules.Any() ? (object)operatorModules : "No operators modules found."),
		Context.Containers.Image
	).Dump(title);

	title.AsHyperlink(new WikiHyperlinq("Operator_Module", "List").Uri);

	new DumpContainer(){ Style = $"height: {DumpContext.Document.MarginBottom}" }.Dump();

	static T Pass<T>(T t) => t;
}

static object GetMaterials(string op, string materials)
{
	const StringSplitOptions StringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

	const char materialSeparator = '｜';

	return HorizontalRun(false,
		materials
			.Split(materialSeparator, StringSplitOptions)
			.SelectMany(GetMaterialLinks)
			.InsertAfterEach(3, materialSeparator)
	);

	IEnumerable<object> GetMaterialLinks(string materialCount)
	{
		const char countSeparator = '❂';

		var material = materialCount.Split(countSeparator, StringSplitOptions);

		yield return GetImageHyperlink(material.First());
		yield return countSeparator;
		yield return new WikiHyperlinq(op, "Promotion", material.Last());
	}
}

static Hyperlink GetImageHyperlink(string name)
{
	var itemHyperlinq = new WikiHyperlinq(name);

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
}

abstract class Parsable<T> : IEnumerable<T>
{
	private const string Comment = "//";

	private readonly Lazy<T[]> _parsed;

	public Parsable(string str) =>
		_parsed = new(() => Parse(str));

	public IEnumerator<T> GetEnumerator() =>
		_parsed.Value.AsEnumerable().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		_parsed.Value.GetEnumerator();

	protected abstract string ErrorMessage { get; }
	protected abstract string Regex { get; }

	protected abstract T Create(Match match);

	private T ParseAndCreate(string str)
	{
		var regex = new Regex(Regex, ExplicitCapture, FromSeconds(1));

		var match = regex.Match(str);
		if(match.Success)
		{
			return Create(match);
		}

		Throw(str, ErrorMessage);
		return default;
	}

	private T[] Parse(string values) =>
		values
			.Replace(Comment, NewLine + Comment)
			.Split(NewLine.ToCharArray())
			.Select(static v => v.Trim())
			.Where( static v => !IsNullOrEmpty(v))
			.Where( static v => !v.StartsWith(Comment))
			.Select(ParseAndCreate)
			.ToArray();

	protected static string GetString(Match match, string key) =>
		match.Groups[key].Value.Trim();

	protected static int GetNumber(Match match, string key, int fallbackValue = default) =>
		int.TryParse(GetString(match, key), Integer, InvariantCulture, out var value)
			? value
			: fallbackValue;

	[DoesNotReturn]
	protected static void Throw(string str, string message) =>
		throw new($"Invalid data: '{str}'. {message}");
}

internal record OperatorModule(string Operator, int Stars, string Module, string Stage, string E2Materials, bool Paradox);

class OperatorModules : Parsable<OperatorModule>
{
	private const string Operator    = nameof(Operator);
	private const string Stars       = nameof(Stars);
	private const string Stage       = nameof(Stage);
	private const string Module      = nameof(Module);
	private const string Paradox     = nameof(Paradox);
	private const string E2Materials = nameof(E2Materials);

	protected override string Regex { get; } = $@"^(?<{Operator}>[^\t]+)\t+(?<{Stars}>[1-6])\t+(?<{Stage}>[^\t]+)\t+(?<{Module}>[^\t]+)\t+(?<{E2Materials}>[^\t]+)(\t+(?<{Paradox}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = "Use OperatorModulesParser.linq script to update.";

	protected override OperatorModule Create(Match match) =>
		new
		(
			GetString(match, Operator),
			GetNumber(match, Stars),
			GetString(match, Stage),
			GetString(match, Module),
			GetString(match, E2Materials),
			!string.IsNullOrWhiteSpace(GetString(match, Paradox))
		);

	public OperatorModules(string operatorModules)
		: base(operatorModules)
	{
	}
}

internal enum ModuleType
{
	Unknown,
	All,
	X,
	Y,
	D
}

internal record Operator(string Name, ModuleType ModuleType);

class Operators : Parsable<Operator>
{
	private const string Comment = "//";

	private const string Operator = nameof(Operator);
	private const string Module   = nameof(Module);

	protected override string Regex { get; } = $@"^(?<{Operator}>[^\t]+)(\t+(?<{Module}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = "Format: name <tab> type (X, Y, D or empty for all)";

	protected override Operator Create(Match match)
	{
		var moduleType = GetString(match, Module).ToLowerInvariant() switch
		{
			"x" => ModuleType.X,
			"y" => ModuleType.Y,
			"d" => ModuleType.D,
			""  => ModuleType.All,
			_   => ModuleType.Unknown
		};

		if(moduleType == ModuleType.Unknown)
		{
			Throw(match.Value, ErrorMessage);
		}

		return new
		(
			GetString(match, Operator),
			moduleType
		);
	}

	public Operators(string operators)
		: base(operators)
	{
	}
}

class WikiHyperlinq : Hyperlinq
{
	public WikiHyperlinq(string uri, string? fragment = null, string? text = null)
		: base($"{DumpContext.Url.Wiki}/{uri.Replace(' ', '_')}{GetFragment(fragment)}", text ?? uri)
	{
	}

	private static string GetFragment(string? fragment) =>
		$"{(IsNullOrWhiteSpace(fragment) ? Empty : "#")}{fragment}";
}

class ItemImage
{
	public string Name { get; }
	public LazyImage Image { get; }

	public ItemImage(string name, string pathUri, string? fileName = null, int? height = null) =>
		(Name, Image) = (name, new($"{DumpContext.Url.Wiki}/images/{pathUri}/{(fileName ?? name).UnderscoreSpaces()}.png", height));
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

	public static string UnderscoreSpaces(this string str) =>
		str.Trim(Quotes).Replace("\" ", " ").Replace("\' ", " ").Replace(' ', '_');
}

static class CollectionExtensions
{
	public static string JoinStrings<T, TV>(this IEnumerable<T> items, Func<T, TV> selector) =>
		 Join(NewLine, items.Select(selector));

	public static IEnumerable<T> InsertAfterEach<T>(this IEnumerable<T> items, int afterEach, T insert)
	{
		var index = 0;

		foreach(var item in items)
		{
			if((++index % (afterEach+1)) == 0)
			{
				yield return insert;
			}

			yield return item;
		}
	}

	public static IEnumerable<OperatorModule> OnlyFor(this IEnumerable<OperatorModule> operatorModules, IEnumerable<Operator> operators)
	{
		var stringComparer   = StringComparer.OrdinalIgnoreCase;
		var stringComparison = StringComparison.OrdinalIgnoreCase;

		var operatorsModuleTypes = operators
			.ToLookup(static op => op.Name, static op => op.ModuleType, stringComparer)
			.ToDictionary(static g => g.Key, static g => g, stringComparer);

		return operatorModules.Where(operatorModule =>
				operatorsModuleTypes.TryGetValue(operatorModule.Operator, out var moduleTypes) &&
				moduleTypes.Any(moduleType => HasModule(moduleType, operatorModule.Module)));

		bool HasModule(ModuleType moduleType, string module) =>
			moduleType switch
			{
				ModuleType.All or ModuleType.Unknown => true,
				ModuleType.X when module.EndsWith("X", stringComparison) => true,
				ModuleType.Y when module.EndsWith("Y", stringComparison) => true,
				ModuleType.D when module.EndsWith("Δ", stringComparison) => true,
				_ => false
			};
	}
}

static class HtmlExtensions
{
	private const string H1ToA = $"{nameof(HtmlExtensions)}_{nameof(H1ToA)}";

	static HtmlExtensions() =>
		HtmlHead.AddScript($$"""
			function {{H1ToA}}(text, uri){
				const elem = [].find.call(document.getElementsByTagName('h1'), elem => elem.innerHTML === text);
				if(elem){
					elem.innerHTML = `<a href="${uri}" class="headingpresenter reference">${elem.innerHTML}</a>`;
				}
			}
			""");

	public static void AsHyperlink(this string text, string uri) =>
		InvokeScript(false, H1ToA, text, uri);
}

class DumpContext
{
	public static class Url
	{
		public const string Wiki = "https://arknights.wiki.gg";
	}

	public static class ImageHeight
	{
		public const int Item         = -96; // <= 0 for as is size.
		public const int NotAvailable = 480 / 2;
	}

	public static class Document
	{
		public const string MarginBottom = "6.5rem";
	}

	public class DumpContainers
	{
		public readonly DumpContainer Image = new();
	}

	public readonly DumpContainers Containers = new();
}

static class DB
{
	public static readonly ReadOnlyDictionary<string, LazyImage> Images = new(new ItemImage[]
	{
		new("Aggregate Cyclicene", "2/2f"),
		new("Aketon", "d/d3"),
		new("Bipolar Nanoflake", "b/b6"),
		new("Coagulating Gel", "b/bd"),
		new("Compound Cutting Fluid", "2/2c"),
		new("Crystalline Circuit", "4/46"),
		new("Crystalline Component", "2/20"),
		new("Crystalline Electronic Unit", "d/d0"),
		new("Cutting Fluid Solution", "9/93"),
		new("Cyclicene Prefab", "e/ea"),
		new("D32 Steel", "6/68"),
		new("Device", "a/a4"),
		new("Fuscous Fiber", "d/d6"),
		new("Grindstone Pentahydrate", "a/a5"),
		new("Grindstone", "6/69"),
		new("Incandescent Alloy Block", "a/a3"),
		new("Incandescent Alloy", "f/f4"),
		new("Integrated Device", "f/fb"),
		new("Keton Colloid", "a/ae"),
		new("Loxic Kohl", "4/4a"),
		new("Manganese Ore", "6/6a"),
		new("Manganese Trihydrate", "c/cd"),
		new("Module Data Block", "6/66"),
		new("Nucleic Crystal Sinter", "7/7e"),
		new("Optimized Device", "f/fd"),
		new("Orirock Cluster", "4/4d"),
		new("Orirock Concentration", "d/d4"),
		new("Orirock Cube", "d/d8"),
		new("Oriron Block", "5/52"),
		new("Oriron Cluster", "8/87"),
		new("Oriron", "4/44"),
		new("Polyester Lump", "e/ed"),
		new("Polyester Pack", "f/f3"),
		new("Polyester", "3/3a"),
		new("Polyketon", "9/96"),
		new("Polymerization Preparation", "9/9c"),
		new("Polymerized Gel", "6/66"),
		new("Recruitment Permit", "3/3b"),
		new("Refined Solvent", "9/9f"),
		new("RMA70-12", "1/10"),
		new("RMA70-24", "f/f1"),
		new("Semi-Synthetic Solvent", "5/58"),
		new("Solidified Fiber Board", "e/e2"),
		new("Sugar Lump", "d/d2"),
		new("Sugar Pack", "2/23"),
		new("Sugar", "a/a9"),
		new("Transmuted Salt Agglomerate", "b/bd"),
		new("Transmuted Salt", "b/bd"),
		new("White Horse Kohl", "4/4a")
	}.ToDictionary(static v => v.Name, static v => v.Image));

	// TODO: Run OperatorModulesParser.linq and paste from clipboard to the below.
	public static readonly OperatorModules OperatorModules = new("""
		Aak	6	GEE-X	2-8	D32 Steel❂4｜Polymerized Gel❂7	Paradox
		Absinthe	5	CCR-X	3-4	Orirock Concentration❂10｜Incandescent Alloy❂10
		Aciddrop	4	ARC-X	2-8	RMA70-12❂10｜Integrated Device❂8	Paradox
		Akafuyu	5	MUS-X	3-8	Polymerized Gel❂8｜Aketon❂15	Paradox
		Almond	5	HOK-X	BI-2	Oriron Block❂6｜Manganese Ore❂13	Paradox
		Ambriel	4	DEA-X	4-7	Oriron Cluster❂16｜RMA70-12❂6
		Amiya	5	CCR-Y	3-8	Orirock Concentration❂10｜Loxic Kohl❂10
		Andreana	5	DEA-X	3-2	Grindstone Pentahydrate❂8｜RMA70-12❂8
		Angelina	6	DEC-X	2-7	Bipolar Nanoflake❂4｜Sugar Lump❂5	Paradox
		Angelina	6	DEC-Y	4-4	Bipolar Nanoflake❂4｜Sugar Lump❂5	Paradox
		April	5	MAR-Y	5-3	Polymerized Gel❂9｜RMA70-12❂9	Paradox
		Archetto	6	MAR-X	SV-4	Polymerization Preparation❂4｜Orirock Concentration❂8	Paradox
		Archetto	6	MAR-Y	2-9	Polymerization Preparation❂4｜Orirock Concentration❂8	Paradox
		Aroma	5	BLA-X	GT-HX-3	RMA70-24❂7｜Loxic Kohl❂21
		Ascalon	6	AMB-X	11-6	D32 Steel❂4｜Keton Colloid❂6
		Ash	6	MAR-X	SV-8	D32 Steel❂4｜Polymerized Gel❂6
		Ash	6	MAR-Y	1-12	D32 Steel❂4｜Polymerized Gel❂6
		Ashlock	5	FOR-X	7-9	Polymerized Gel❂8｜Compound Cutting Fluid❂13
		Astgenne	5	CHA-Y	4-3	RMA70-24❂7｜Coagulating Gel❂10	Paradox
		Aurora	5	HES-X	3-8	White Horse Kohl❂9｜Orirock Cluster❂19	Paradox
		Bagpipe	6	CHG-X	4-5	Polymerization Preparation❂4｜Orirock Concentration❂9	Paradox
		Bagpipe	6	CHG-Y	9-5	Polymerization Preparation❂4｜Orirock Concentration❂9	Paradox
		Bassline	5	GUA-Y	LE-4	Cyclicene Prefab❂7｜Manganese Ore❂15
		Beanstalk	4	TAC-Y	S3-7	Coagulating Gel❂13｜Manganese Ore❂10	Paradox
		Beehunter	4	FGT-Y	7-4	Sugar Pack❂19｜RMA70-12❂7	Paradox
		Beeswax	5	PLX-X	DH-6	Optimized Device❂5｜Loxic Kohl❂18	Paradox
		Bena	5	PUM-Y	2-3	Oriron Block❂6｜Loxic Kohl❂15	Paradox
		Bibeak	5	SWO-X	3-7	Manganese Trihydrate❂8｜RMA70-12❂8	Paradox
		Bison	5	PRO-X	3-7	Grindstone Pentahydrate❂7｜RMA70-12❂11	Paradox
		Blacknight	5	TAC-X	3-1	Incandescent Alloy Block❂8｜Loxic Kohl❂15	Paradox
		Blaze	6	CEN-X	11-13	D32 Steel❂4｜Optimized Device❂6	Paradox
		Blemishine	6	GUA-X	MN-4	D32 Steel❂4｜RMA70-24❂7	Paradox
		Blemishine	6	GUA-Y	MN-2	D32 Steel❂4｜RMA70-24❂7	Paradox
		Blitz	5	SPT-X	3-4	Manganese Trihydrate❂8｜RMA70-12❂8
		Blue Poison	5	MAR-Y	3-4	Manganese Trihydrate❂8｜Integrated Device❂8	Paradox
		Breeze	5	RIN-X	4-1	Optimized Device❂5｜Loxic Kohl❂18	Paradox
		Broca	5	CEN-X	7-8	Sugar Lump❂7｜Grindstone❂13	Paradox
		Bryophyta	5	INS-X	9-3	Refined Solvent❂7｜Aketon❂15
		Bubble	4	PRO-X	4-6	Coagulating Gel❂16｜Loxic Kohl❂8
		Carnelian	6	PLX-X	7-3	D32 Steel❂4｜RMA70-24❂7	Paradox
		Carnelian	6	PLX-Y	4-6	D32 Steel❂4｜RMA70-24❂7	Paradox
		Cement	5	HES-X	IW-EX-6	Manganese Trihydrate❂7｜Grindstone❂13
		Ceobe	6	CCR-X	RI-6	Bipolar Nanoflake❂4｜Incandescent Alloy Block❂5	Paradox
		Ceobe	6	CCR-Y	11-7	Bipolar Nanoflake❂4｜Incandescent Alloy Block❂5	Paradox
		Ceylon	5	WAH-X	OF-3	Oriron Block❂7｜Aketon❂10	Paradox
		Ch'en	6	SWO-X	5-8	Polymerization Preparation❂4｜White Horse Kohl❂6	Paradox
		Ch'en	6	SWO-Y	5-9	Polymerization Preparation❂4｜White Horse Kohl❂6	Paradox
		Chiave	5	SOL-Y	1-1	Manganese Trihydrate❂7｜Grindstone❂13	Paradox
		Chongyue	6	FGT-X	WB-7	Polymerization Preparation❂4｜Incandescent Alloy❂5
		Click	4	FUN-Y	11-7	Manganese Ore❂15｜Oriron Cluster❂9	Paradox
		Cliffheart	5	HOK-X	MN-5	Oriron Block❂6｜Manganese Ore❂13	Paradox
		Conviction	4	DRE-X	2-4	Integrated Device❂11｜Coagulating Gel❂9
		Corroserum	5	BLA-X	BI-6	Optimized Device❂5｜RMA70-12❂10	Paradox
		Courier	4	SOL-X	S2-2	Integrated Device❂11｜Aketon❂10
		Croissant	5	PRO-Y	2-8	RMA70-24❂8｜Integrated Device❂8	Paradox
		Cuora	4	PRO-X	2-10	Grindstone❂14｜Loxic Kohl❂10	Paradox
		Cutter	4	SWO-Y	2-7	Coagulating Gel❂12｜Manganese Ore❂11	Paradox
		Dagda	5	FGT-Y	11-5	Incandescent Alloy Block❂7｜Orirock Cluster❂21	Paradox
		Deepcolor	4	SUM-Y	2-4	Orirock Cluster❂20｜Manganese Ore❂9	Paradox
		Degenbrecher	6	SWO-X	BI-6	Nucleic Crystal Sinter❂3｜Grindstone Pentahydrate❂7
		Delphine	5	MSC-Y	WD-6	Cyclicene Prefab❂8｜Semi-Synthetic Solvent❂10
		Dobermann	4	INS-Y	1-12	Manganese Ore❂15｜Loxic Kohl❂11
		Doc	5	INS-Y	2-5	Polymerized Gel❂8｜Integrated Device❂11
		Dorothy	6	TRP-Y	4-5	Polymerization Preparation❂4｜Cutting Fluid Solution❂8	Paradox
		Dusk	6	SPC-X	1-6	Crystalline Electronic Unit❂4｜Manganese Trihydrate❂6	Paradox
		Dusk	6	SPC-Y	4-3	Crystalline Electronic Unit❂4｜Manganese Trihydrate❂6	Paradox
		Earthspirit	4	DEC-Y	DM-2	Sugar Pack❂20｜Grindstone❂7	Paradox
		Ebenholz	6	MSC-X	3-7	D32 Steel❂4｜Refined Solvent❂7	Paradox
		Ebenholz	6	MSC-Y	3-6	D32 Steel❂4｜Refined Solvent❂7	Paradox
		Ebenholz	6	MSC-Δ	11-12	D32 Steel❂4｜Refined Solvent❂7	Paradox
		Elysium	5	BEA-X	S3-6	Incandescent Alloy Block❂7｜Aketon❂16	Paradox
		Enforcer	5	PUS-X	GA-EX-1	Polymerized Gel❂8｜Compound Cutting Fluid❂12	Paradox
		Erato	5	SIE-X	S5-9	Optimized Device❂5｜RMA70-12❂11	Paradox
		Estelle	4	CEN-Y	7-4	RMA70-12❂12｜Grindstone❂8	Paradox
		Ethan	4	AMB-X	TW-3	Sugar Pack❂17｜Orirock Cluster❂14	Paradox
		Eunectes	6	HES-X	RI-EX-4	Bipolar Nanoflake❂4｜Polymerized Gel❂7
		Eunectes	6	HES-Y	RI-7	Bipolar Nanoflake❂4｜Polymerized Gel❂7
		Executor the Ex Foedere	6	REA-X	7-14	Nucleic Crystal Sinter❂3｜Polymerized Gel❂7	Paradox
		Exusiai	6	MAR-X	4-10	Polymerization Preparation❂4｜Sugar Lump❂5	Paradox
		Eyjafjalla	6	CCR-X	OF-5	Polymerization Preparation❂4｜Optimized Device❂5	Paradox
		Fartooth	6	DEA-X	S2-2	D32 Steel❂4｜Cutting Fluid Solution❂7
		Fartooth	6	DEA-Y	2-5	D32 Steel❂4｜Cutting Fluid Solution❂7
		FEater	5	PUS-Y	WR-7	Grindstone Pentahydrate❂8｜Polyester Pack❂15	Paradox
		Fiammetta	6	ART-X	GA-4	Crystalline Electronic Unit❂3｜Grindstone Pentahydrate❂6	Paradox
		Fiammetta	6	ART-Y	S5-9	Crystalline Electronic Unit❂3｜Grindstone Pentahydrate❂6	Paradox
		Firewatch	5	DEA-Y	2-10	Polyester Lump❂7｜Loxic Kohl❂15	Paradox
		Firewhistle	5	FOR-X	S4-6	Grindstone Pentahydrate❂7｜Coagulating Gel❂13	Paradox
		Flamebringer	5	DRE-Y	S4-3	White Horse Kohl❂9｜Manganese Ore❂13
		Flametail	6	SOL-X	NL-5	Bipolar Nanoflake❂4｜Orirock Concentration❂9
		Flametail	6	SOL-Y	NL-3	Bipolar Nanoflake❂4｜Orirock Concentration❂9
		Flint	5	FGT-Y	RI-9	Orirock Concentration❂8｜Grindstone❂15	Paradox
		Folinic	5	PHY-Y	5-2	Keton Colloid❂8｜Integrated Device❂8
		Franka	5	DRE-X	9-3	Oriron Block❂6｜Sugar Pack❂18	Paradox
		Frost	5	TRP-Y	4-4	Grindstone Pentahydrate❂8｜Orirock Cluster❂17
		Fuze	5	CEN-Y	3-1	Incandescent Alloy Block❂8｜Sugar Pack❂16
		Gavial	4	PHY-Y	S4-5	Integrated Device❂13｜Oriron Cluster❂6
		Gavial the Invincible	6	CEN-X	9-2	Bipolar Nanoflake❂4｜Polymerized Gel❂6	Paradox
		Gitano	4	SPC-X	1-10	Sugar Pack❂17｜Orirock Cluster❂14	Paradox
		Gladiia	6	HOK-X	SV-EX-5	Crystalline Electronic Unit❂4｜Polymerized Gel❂6	Paradox
		Glaucus	5	DEC-X	5-7	Keton Colloid❂7｜Integrated Device❂10	Paradox
		Gnosis	6	UMD-X	BI-7	Crystalline Electronic Unit❂3｜Incandescent Alloy Block❂7
		Goldenglow	6	FUN-X	R8-8	Bipolar Nanoflake❂4｜Manganese Trihydrate❂5
		Grain Buds	5	DEC-X	9-11	Crystalline Circuit❂7｜Compound Cutting Fluid❂11
		Grani	5	CHG-X	GT-6	RMA70-24❂7｜Oriron Cluster❂13	Paradox
		Gravel	4	EXE-X	4-8	Polyester Pack❂18｜Orirock Cluster❂13	Paradox
		GreyThroat	5	MAR-Y	2-10	Oriron Block❂7｜Integrated Device❂9
		Greyy	4	SPC-Y	2-3	Manganese Ore❂15｜Aketon❂9	Paradox
		Greyy the Lightningbearer	5	BOM-X	1-3	Grindstone Pentahydrate❂7｜Incandescent Alloy❂16	Paradox
		Gummy	4	GUA-X	4-2	RMA70-12❂13｜Manganese Ore❂7	Paradox
		Harmonie	5	MSC-X	6-5	RMA70-24❂6｜Oriron Cluster❂15
		Haze	4	CCR-X	S2-4	Orirock Cluster❂19｜RMA70-12❂8	Paradox
		Heavyrain	5	PRO-Y	2-7	Orirock Concentration❂9｜Oriron Cluster❂14	Paradox
		Hellagur	6	MUS-X	5-10	Bipolar Nanoflake❂4｜Polyester Lump❂7	Paradox
		Hellagur	6	MUS-Y	7-10	Bipolar Nanoflake❂4｜Polyester Lump❂7	Paradox
		Hibiscus the Purifier	5	INC-X	LE-4	White Horse Kohl❂9｜Orirock Cluster❂18	Paradox
		Highmore	5	REA-X	SN-2	Orirock Concentration❂9｜Semi-Synthetic Solvent❂13
		Ho'olheyak	6	CCR-X	OF-7	D32 Steel❂4｜Transmuted Salt Agglomerate❂7
		Ho'olheyak	6	CCR-Y	4-3	D32 Steel❂4｜Transmuted Salt Agglomerate❂7
		Horn	6	FOR-X	7-15	D32 Steel❂4｜Oriron Block❂7	Paradox
		Hoshiguma	6	PRO-X	6-4	Polymerization Preparation❂4｜Grindstone Pentahydrate❂5
		Hoshiguma	6	PRO-Y	3-1	Polymerization Preparation❂4｜Grindstone Pentahydrate❂5
		Humus	4	REA-X	7-8	RMA70-12❂11｜Sugar Pack❂12	Paradox
		Hung	5	GUA-Y	S2-4	Incandescent Alloy Block❂7｜Aketon❂15	Paradox
		Iana	5	PUM-X	3-7	Transmuted Salt Agglomerate❂7｜Aggregate Cyclicene❂12
		Ifrit	6	BLA-X	1-5	D32 Steel❂4｜Polyester Lump❂7	Paradox
		Indigo	4	MSC-X	SV-5	Oriron Cluster❂14｜RMA70-12❂7	Paradox
		Indra	5	FGT-Y	11-5	Keton Colloid❂7｜Polyester Pack❂16	Paradox
		Insider	5	MAR-X	3-4	Polymerized Gel❂9｜Sugar Pack❂15
		Irene	6	SWO-Y	SV-4	Bipolar Nanoflake❂4｜RMA70-24❂7
		Iris	5	MSC-X	2-2	Oriron Block❂6｜Integrated Device❂11	Paradox
		Istina	5	DEC-Y	2-3	Optimized Device❂5｜RMA70-12❂9	Paradox
		Jackie	4	FGT-X	CB-4	Orirock Cluster❂19｜Loxic Kohl❂12	Paradox
		Jaye	4	MER-X	S3-2	Grindstone❂14｜Aketon❂8	Paradox
		Jessica	4	MAR-Y	SV-4	Loxic Kohl❂20｜Oriron Cluster❂7	Paradox
		Jessica the Liberated	6	SPT-X	3-8	Crystalline Electronic Unit❂4｜Optimized Device❂4
		Jieyun	5	ART-Y	S2-2	Keton Colloid❂7｜RMA70-12❂11
		Kafka	5	EXE-Y	2-7	Polymerized Gel❂8｜Oriron Cluster❂15	Paradox
		Kal'tsit	6	PHY-X	5-10	Crystalline Electronic Unit❂4｜Optimized Device❂4
		Kal'tsit	6	PHY-Y	5-10	Crystalline Electronic Unit❂4｜Optimized Device❂4
		Kazemaru	5	PUM-X	3-1	Orirock Concentration❂9｜Semi-Synthetic Solvent❂13	Paradox
		Kirara	5	AMB-X	3-1	Incandescent Alloy Block❂7｜Integrated Device❂11	Paradox
		Kirin R Yato	6	EXE-X	3-7	Nucleic Crystal Sinter❂3｜Keton Colloid❂6
		Kjera	5	FUN-Y	BI-7	Grindstone Pentahydrate❂8｜Incandescent Alloy❂13
		Kroos the Keen Glint	5	MAR-X	3-8	Crystalline Circuit❂7｜Oriron Cluster❂10
		La Pluma	5	REA-X	11-6	Keton Colloid❂7｜Manganese Ore❂13	Paradox
		Lava the Purgatory	5	SPC-X	WR-4	White Horse Kohl❂8｜Grindstone❂13
		Lee	6	MER-X	3-1	Polymerization Preparation❂4｜White Horse Kohl❂9	Paradox
		Lee	6	MER-Y	IW-EX-1	Polymerization Preparation❂4｜White Horse Kohl❂9	Paradox
		Leizi	5	CHA-X	S3-6	RMA70-24❂7｜Coagulating Gel❂13	Paradox
		Leonhardt	5	SPC-X	3-5	Keton Colloid❂7｜Loxic Kohl❂15	Paradox
		Lessing	6	DRE-X	7-11	Crystalline Electronic Unit❂4｜Cyclicene Prefab❂5
		Lin	6	PLX-X	11-6	D32 Steel❂4｜Cutting Fluid Solution❂8
		Ling	6	SUM-Y	3-4	D32 Steel❂4｜Crystalline Circuit❂5	Paradox
		Liskarm	5	SPT-X	4-6	Grindstone Pentahydrate❂7｜Aketon❂15	Paradox
		Lumen	6	WAH-X	6-9	Crystalline Electronic Unit❂4｜Optimized Device❂5	Paradox
		Lumen	6	WAH-Y	OF-5	Crystalline Electronic Unit❂4｜Optimized Device❂5	Paradox
		Lunacub	5	DEA-Y	2-5	Refined Solvent❂8｜Aketon❂13
		Lutonada	4	UNY-X	3-1	Semi-Synthetic Solvent❂14｜Integrated Device❂7
		Magallan	6	SUM-X	2-5	Polymerization Preparation❂4｜Manganese Trihydrate❂6	Paradox
		Magallan	6	SUM-Y	2-2	Polymerization Preparation❂4｜Manganese Trihydrate❂6	Paradox
		Manticore	5	AMB-X	1-9	Manganese Trihydrate❂8｜Sugar Pack❂12	Paradox
		Matoimaru	4	DRE-Y	S2-10	Aketon❂16｜Sugar Pack❂10
		Matterhorn	4	PRO-Y	3-6	Manganese Ore❂14｜Integrated Device❂7	Paradox
		May	4	MAR-X	4-4	Oriron Cluster❂14｜Polyester Pack❂12	Paradox
		Mayer	5	SUM-X	2-9	Oriron Block❂6｜RMA70-12❂11	Paradox
		Melanite	5	ARC-Y	S3-5	Refined Solvent❂8｜Loxic Kohl❂15
		Meteor	4	MAR-X	2-7	Oriron Cluster❂14｜Polyester Pack❂12	Paradox
		Meteorite	5	ART-Y	2-4	RMA70-24❂7｜Manganese Ore❂14	Paradox
		Minimalist	5	FUN-Y	IC-8	Crystalline Circuit❂6｜Oriron Cluster❂12	Paradox
		Mint	5	PLX-X	BI-2	Incandescent Alloy Block❂9｜Orirock Cluster❂14	Paradox
		Mizuki	6	AMB-X	4-3	Polymerization Preparation❂4｜Crystalline Circuit❂6	Paradox
		Mizuki	6	AMB-Y	4-5	Polymerization Preparation❂4｜Crystalline Circuit❂6	Paradox
		Morgan	5	DRE-X	4-5	Cutting Fluid Solution❂8｜Transmuted Salt❂11	Paradox
		Mostima	6	SPC-Y	CB-5	Bipolar Nanoflake❂4｜Grindstone Pentahydrate❂7	Paradox
		Mountain	6	FGT-Y	MB-EX-3	Crystalline Electronic Unit❂4｜Polymerized Gel❂8	Paradox
		Mr. Nothing	5	MER-X	3-7	Optimized Device❂6｜Manganese Ore❂10	Paradox
		Mudrock	6	UNY-X	11-7	Crystalline Electronic Unit❂4｜Incandescent Alloy Block❂5
		Muelsyse	6	TAC-X	3-4	Bipolar Nanoflake❂4｜Grindstone Pentahydrate❂4	Paradox
		Myrrh	4	PHY-X	6-11	Aketon❂14｜Polyester Pack❂12	Paradox
		Myrtle	4	BEA-X	6-3	Grindstone❂12｜Integrated Device❂8	Paradox
		Nearl	5	GUA-X	1-12	White Horse Kohl❂9｜Polyester Pack❂16
		Nearl the Radiant Knight	6	DRE-X	MN-8	Polymerization Preparation❂4｜Polymerized Gel❂8	Paradox
		Nearl the Radiant Knight	6	DRE-Y	NL-10	Polymerization Preparation❂4｜Polymerized Gel❂8	Paradox
		Nian	6	PRO-X	WR-9	Polymerization Preparation❂4｜Incandescent Alloy Block❂7	Paradox
		Nian	6	PRO-Y	S6-2	Polymerization Preparation❂4｜Incandescent Alloy Block❂7	Paradox
		Nightingale	6	RIN-X	3-6	D32 Steel❂4｜Keton Colloid❂6	Paradox
		Nightmare	5	CCR-Y	S4-1	Sugar Lump❂7｜Manganese Ore❂14
		Nine-Colored Deer	5	BLS-X	IW-3	Manganese Trihydrate❂7｜Crystalline Component❂14
		Pallas	6	INS-X	3-6	Crystalline Electronic Unit❂4｜White Horse Kohl❂6
		Pallas	6	INS-Y	4-3	Crystalline Electronic Unit❂4｜White Horse Kohl❂6
		Passenger	6	CHA-X	2-2	Bipolar Nanoflake❂4｜Oriron Block❂5
		Passenger	6	CHA-Y	5-10	Bipolar Nanoflake❂4｜Oriron Block❂5
		Penance	6	UNY-X	CB-4	D32 Steel❂4｜White Horse Kohl❂8
		Perfumer	4	RIN-Y	3-4	Loxic Kohl❂19｜Aketon❂8	Paradox
		Phantom	6	EXE-X	3-6	Polymerization Preparation❂4｜Polymerized Gel❂9	Paradox
		Phantom	6	EXE-Y	DM-5	Polymerization Preparation❂4｜Polymerized Gel❂9	Paradox
		Platinum	5	MAR-X	5-7	Grindstone Pentahydrate❂8｜Loxic Kohl❂15	Paradox
		Podenco	4	DEC-X	4-6	Incandescent Alloy❂15｜Grindstone❂5	Paradox
		Poncirus	5	SOL-X	OF-5	Incandescent Alloy Block❂8｜Orirock Cluster❂18
		Pozëmka	6	ARC-Y	2-10	Crystalline Electronic Unit❂3｜Orirock Concentration❂9	Paradox
		Pramanix	5	UMD-X	2-3	Keton Colloid❂7｜Grindstone❂11	Paradox
		Projekt Red	5	EXE-Y	S4-5	Manganese Trihydrate❂7｜Oriron Cluster❂14
		Provence	5	ARC-X	S5-1	Sugar Lump❂9｜Integrated Device❂7	Paradox
		Proviso	5	DEC-X	4-7	Oriron Block❂7｜Manganese Ore❂12	Paradox
		Ptilopsis	5	RIN-X	4-9	Orirock Concentration❂9｜Grindstone❂10	Paradox
		Pudding	4	CHA-Y	2-10	RMA70-12❂11｜Cutting Fluid Solution❂3	Paradox
		Purestream	4	WAH-Y	3-7	Integrated Device❂11｜Coagulating Gel❂9	Paradox
		Qanipalaat	5	CCR-Y	2-3	Oriron Block❂7｜Manganese Ore❂12	Paradox
		Quercus	5	BLS-X	9-2	Oriron Block❂6｜Manganese Ore❂13	Paradox
		Rathalos S Noir Corne	5	MUS-X	3-8	Transmuted Salt Agglomerate❂8｜Crystalline Component❂11
		Reed	5	CHG-Y	9-5	Orirock Concentration❂9｜Manganese Ore❂12	Paradox
		Reed the Flame Shadow	6	INC-X	11-6	Nucleic Crystal Sinter❂3｜Orirock Concentration❂9
		Roberta	4	CRA-X	3-8	Semi-Synthetic Solvent❂14｜Integrated Device❂7	Paradox
		Robin	5	TRP-Y	BI-EX-2	Incandescent Alloy Block❂8｜Aketon❂11
		Rockrock	5	FUN-X	RI-EX-4	Cutting Fluid Solution❂7｜Aketon❂14	Paradox
		Rope	4	HOK-X	3-3	Oriron Cluster❂15｜Sugar Pack❂11	Paradox
		Rosa	6	SIE-X	7-9	Bipolar Nanoflake❂4｜Optimized Device❂6
		Rosmontis	6	BOM-X	4-1	D32 Steel❂4｜Keton Colloid❂5
		Saga	6	SOL-X	WR-3	Bipolar Nanoflake❂4｜Incandescent Alloy Block❂6
		Saga	6	SOL-Y	WR-1	Bipolar Nanoflake❂4｜Incandescent Alloy Block❂6
		Saileach	6	BEA-X	9-2	Crystalline Electronic Unit❂4｜Refined Solvent❂6	Paradox
		Santalla	5	SPC-Y	BI-2	White Horse Kohl❂8｜Polyester Pack❂17
		Saria	6	GUA-X	MB-3	Bipolar Nanoflake❂4｜Manganese Trihydrate❂5
		Saria	6	GUA-Y	4-6	Bipolar Nanoflake❂4｜Manganese Trihydrate❂5
		Savage	5	CEN-X	4-5	Orirock Concentration❂9｜Sugar Pack❂18	Paradox
		Scavenger	4	SOL-X	1-3	Loxic Kohl❂20｜Integrated Device❂6	Paradox
		Scene	5	SUM-X	3-1	White Horse Kohl❂9｜Manganese Ore❂12	Paradox
		Schwarz	6	ARC-X	OF-7	D32 Steel❂4｜Oriron Block❂5	Paradox
		Schwarz	6	ARC-Y	2-1	D32 Steel❂4｜Oriron Block❂5	Paradox
		Sesa	5	ART-X	S3-6	Grindstone Pentahydrate❂8｜Orirock Cluster❂18	Paradox
		Shamare	5	UMD-X	6-5	Orirock Concentration❂8｜Incandescent Alloy❂17	Paradox
		Shaw	4	PUS-Y	2-8	Integrated Device❂12｜Polyester Pack❂11	Paradox
		Shining	6	PHY-X	NL-5	Bipolar Nanoflake❂4｜Oriron Block❂5
		Shining	6	PHY-Y	5-8	Bipolar Nanoflake❂4｜Oriron Block❂5
		Shirayuki	4	ART-Y	S2-11	Aketon❂15｜Oriron Cluster❂9	Paradox
		Shu	6	GUA-X	11-11	Crystalline Electronic Unit❂3｜Cutting Fluid Solution❂6
		Siege	6	SOL-X	1-7	Bipolar Nanoflake❂4｜Orirock Concentration❂6
		Siege	6	SOL-Y	11-2	Bipolar Nanoflake❂4｜Orirock Concentration❂6
		Silence	5	PHY-Y	4-8	Keton Colloid❂7｜Orirock Cluster❂18	Paradox
		Silence the Paradigmatic	6	BLS-X	6-3	Crystalline Electronic Unit❂3｜RMA70-24❂6
		Skadi	6	DRE-X	GT-4	D32 Steel❂4｜Orirock Concentration❂9	Paradox
		Skadi	6	DRE-Y	GT-6	D32 Steel❂4｜Orirock Concentration❂9	Paradox
		Skyfire	5	SPC-Y	S3-5	Polyester Lump❂7｜Grindstone❂13	Paradox
		Snowsant	5	HOK-X	4-6	Polymerized Gel❂8｜Oriron Cluster❂15	Paradox
		Specter	5	CEN-X	SN-EX-1	White Horse Kohl❂8｜Aketon❂15	Paradox
		Specter the Unchained	6	PUM-X	SV-EX-1	Polymerization Preparation❂4｜Keton Colloid❂6
		Specter the Unchained	6	PUM-Y	S4-1	Polymerization Preparation❂4｜Keton Colloid❂6
		Spuria	5	GEE-X	DM-2	Crystalline Circuit❂7｜Compound Cutting Fluid❂9
		Stainless	6	CRA-X	11-6	Polymerization Preparation❂4｜Refined Solvent❂6
		Sussurro	4	PHY-X	4-7	RMA70-12❂10｜Loxic Kohl❂13	Paradox
		Suzuran	6	DEC-X	TW-7	D32 Steel❂4｜Grindstone Pentahydrate❂8
		Suzuran	6	DEC-Y	DM-EX-1	D32 Steel❂4｜Grindstone Pentahydrate❂8
		Swire	5	INS-X	S2-3	Sugar Lump❂7｜Polyester Pack❂17	Paradox
		Swire the Elegant Wit	6	MER-X	5-2	D32 Steel❂4｜White Horse Kohl❂6
		Tachanka	5	SWO-X	S2-3	RMA70-24❂7｜Coagulating Gel❂12
		Texas	5	SOL-Y	CB-3	Polyester Lump❂8｜Orirock Cluster❂16	Paradox
		Texas the Omertosa	6	EXE-Y	CB-8	Bipolar Nanoflake❂4｜Oriron Block❂7
		Toddifons	5	SIE-X	4-6	Crystalline Circuit❂10｜Incandescent Alloy❂10	Paradox
		Tomimi	5	CCR-Y	RI-2	RMA70-24❂8｜Orirock Cluster❂14	Paradox
		Totter	4	SIE-X	S3-5	Orirock Cluster❂24｜Coagulating Gel❂6	Paradox
		Tsukinogi	5	BLS-X	S3-6	White Horse Kohl❂8｜Grindstone❂12	Paradox
		Tuye	5	PHY-X	4-6	Keton Colloid❂7｜Loxic Kohl❂15	Paradox
		Typhon	6	SIE-X	S2-1	Polymerization Preparation❂4｜Refined Solvent❂7
		Utage	4	MUS-X	1-12	Aketon❂14｜Orirock Cluster❂14	Paradox
		Vendela	5	INC-X	IC-2	Keton Colloid❂5｜Grindstone❂13
		Verdant	4	PUM-Y	9-3	Oriron Cluster❂15｜Fuscous Fiber❂9
		Vermeil	4	MAR-X	3-1	Polyester Pack❂18｜Sugar Pack❂12	Paradox
		Vigil	6	TAC-X	3-8	Crystalline Electronic Unit❂3｜Optimized Device❂4
		Vigna	4	CHG-Y	3-3	Oriron Cluster❂16｜Orirock Cluster❂11	Paradox
		Vulcan	5	UNY-X	S4-4	Orirock Concentration❂8｜Aketon❂15	Paradox
		W	6	ART-X	DM-3	Bipolar Nanoflake❂4｜Keton Colloid❂7
		W	6	ART-Y	DM-7	Bipolar Nanoflake❂4｜Keton Colloid❂7
		Waai Fu	5	EXE-X	4-7	RMA70-24❂7｜Orirock Cluster❂16	Paradox
		Wanqing	5	BEA-X	9-13	Cutting Fluid Solution❂9｜Coagulating Gel❂9
		Warfarin	5	PHY-X	2-10	Optimized Device❂5｜Sugar Pack❂17	Paradox
		Weedy	6	PUS-X	BI-5	D32 Steel❂4｜Manganese Trihydrate❂6	Paradox
		Weedy	6	PUS-Y	IC-6	D32 Steel❂4｜Manganese Trihydrate❂6	Paradox
		Whislash	5	INS-X	S3-6	Keton Colloid❂7｜Coagulating Gel❂12	Paradox
		Whisperain	5	WAH-X	2-5	Orirock Concentration❂9｜Crystalline Component❂13	Paradox
		Wild Mane	5	CHG-X	MN-2	Cutting Fluid Solution❂8｜Aketon❂11	Paradox
		Windflit	5	CRA-X	11-9	Keton Colloid❂7｜Grindstone❂11	Paradox
		Zima	5	SOL-X	1-5	Sugar Lump❂7｜RMA70-12❂11	Paradox
		Zuo Le	6	MUS-X	WB-5	Polymerization Preparation❂4｜White Horse Kohl❂8
	""");
}
