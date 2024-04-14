<Query Kind="Program">
  <Namespace>static LINQPad.Util</Namespace>
  <Namespace>static System.Environment</Namespace>
  <Namespace>static System.Globalization.CultureInfo</Namespace>
  <Namespace>static System.Globalization.NumberStyles</Namespace>
  <Namespace>static System.String</Namespace>
  <Namespace>static System.Text.RegularExpressions.Regex</Namespace>
  <Namespace>static System.Text.RegularExpressions.RegexOptions</Namespace>
  <Namespace>static System.TimeSpan</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
</Query>

// Arknights operators modules missions.

#nullable enable

/*
#define DUMP_OPERATORS
#define DUMP_ALL_OPERATOR_MODULES
//*/

void Main()
{
	// TODO: Add operator(s) with (optionally) X or Y modules.
	var operators = new Operators("""
		// name <tab> type (X or Y or empty for both)
	""")
#if DUMP_OPERATORS
	.Dump("Operators")
#endif
	;

	// TODO: Run OperatorModulesParser.linq and paste below.
	var operatorModules = new OperatorModules("""
		Aak	6	GEE-X	2-8	Paradox
		Absinthe	5	CCR-X	3-4
		Aciddrop	4	ARC-X	2-8	Paradox
		Akafuyu	5	MUS-X	3-8	Paradox
		Almond	5	HOK-X	BI-2
		Ambriel	4	DEA-X	4-7
		Amiya	5	CCR-Y	3-8
		Andreana	5	DEA-X	3-2
		Angelina	6	DEC-X	2-7	Paradox
		Angelina	6	DEC-Y	4-4	Paradox
		April	5	MAR-Y	5-3	Paradox
		Archetto	6	MAR-X	SV-4	Paradox
		Archetto	6	MAR-Y	2-9	Paradox
		Aroma	5	BLA-X	GT-HX-3
		Ascalon	6	AMB-X	11-6
		Ash	6	MAR-X	SV-8
		Ash	6	MAR-Y	1-12
		Ashlock	5	FOR-X	7-9
		Astgenne	5	CHA-Y	4-3	Paradox
		Aurora	5	HES-X	3-8	Paradox
		Bagpipe	6	CHG-X	4-5	Paradox
		Bagpipe	6	CHG-Y	9-5	Paradox
		Bassline	5	GUA-Y	LE-4
		Beanstalk	4	TAC-Y	S3-7	Paradox
		Beehunter	4	FGT-Y	7-4	Paradox
		Beeswax	5	PLX-X	DH-6	Paradox
		Bena	5	PUM-Y	2-3	Paradox
		Bibeak	5	SWO-X	3-7	Paradox
		Bison	5	PRO-X	3-7	Paradox
		Blacknight	5	TAC-X	3-1	Paradox
		Blaze	6	CEN-X	11-13	Paradox
		Blemishine	6	GUA-X	MN-4	Paradox
		Blemishine	6	GUA-Y	MN-2	Paradox
		Blitz	5	SPT-X	3-4
		Blue Poison	5	MAR-Y	3-4	Paradox
		Breeze	5	RIN-X	4-1	Paradox
		Broca	5	CEN-X	7-8	Paradox
		Bryophyta	5	INS-X	9-3
		Bubble	4	PRO-X	4-6
		Carnelian	6	PLX-X	7-3	Paradox
		Carnelian	6	PLX-Y	4-6	Paradox
		Cement	5	HES-X	IW-EX-6
		Ceobe	6	CCR-X	RI-6	Paradox
		Ceobe	6	CCR-Y	11-7	Paradox
		Ceylon	5	WAH-X	OF-3	Paradox
		Ch'en	6	SWO-X	5-8	Paradox
		Ch'en	6	SWO-Y	5-9	Paradox
		Chiave	5	SOL-Y	1-1	Paradox
		Chongyue	6	FGT-X	WB-7
		Click	4	FUN-Y	11-7	Paradox
		Cliffheart	5	HOK-X	MN-5	Paradox
		Conviction	4	DRE-X	2-4
		Corroserum	5	BLA-X	BI-6	Paradox
		Courier	4	SOL-X	S2-2
		Croissant	5	PRO-Y	2-8	Paradox
		Cuora	4	PRO-X	2-10	Paradox
		Cutter	4	SWO-Y	2-7	Paradox
		Dagda	5	FGT-Y	11-5	Paradox
		Deepcolor	4	SUM-Y	2-4	Paradox
		Degenbrecher	6	SWO-X	BI-6
		Delphine	5	MSC-Y	WD-6
		Dobermann	4	INS-Y	1-12
		Doc	5	INS-Y	2-5
		Dorothy	6	TRP-Y	4-5	Paradox
		Dusk	6	SPC-X	1-6	Paradox
		Dusk	6	SPC-Y	4-3	Paradox
		Earthspirit	4	DEC-Y	DM-2	Paradox
		Ebenholz	6	MSC-X	3-7	Paradox
		Ebenholz	6	MSC-Y	3-6	Paradox
		Ebenholz	6	MSC-Δ	11-12	Paradox
		Elysium	5	BEA-X	S3-6	Paradox
		Enforcer	5	PUS-X	GA-EX-1	Paradox
		Erato	5	SIE-X	S5-9	Paradox
		Estelle	4	CEN-Y	7-4	Paradox
		Ethan	4	AMB-X	TW-3	Paradox
		Eunectes	6	HES-X	RI-EX-4
		Eunectes	6	HES-Y	RI-7
		Executor the Ex Foedere	6	REA-X	7-14	Paradox
		Exusiai	6	MAR-X	4-10	Paradox
		Eyjafjalla	6	CCR-X	OF-5	Paradox
		Fartooth	6	DEA-X	S2-2
		Fartooth	6	DEA-Y	2-5
		FEater	5	PUS-Y	WR-7	Paradox
		Fiammetta	6	ART-X	GA-4	Paradox
		Fiammetta	6	ART-Y	S5-9	Paradox
		Firewatch	5	DEA-Y	2-10	Paradox
		Firewhistle	5	FOR-X	S4-6	Paradox
		Flamebringer	5	DRE-Y	S4-3
		Flametail	6	SOL-X	NL-5
		Flametail	6	SOL-Y	NL-3
		Flint	5	FGT-Y	RI-9	Paradox
		Folinic	5	PHY-Y	5-2
		Franka	5	DRE-X	9-3	Paradox
		Frost	5	TRP-Y	4-4
		Fuze	5	CEN-Y	3-1
		Gavial	4	PHY-Y	S4-5
		Gavial the Invincible	6	CEN-X	9-2	Paradox
		Gitano	4	SPC-X	1-10	Paradox
		Gladiia	6	HOK-X	SV-EX-5	Paradox
		Glaucus	5	DEC-X	5-7	Paradox
		Gnosis	6	UMD-X	BI-7
		Goldenglow	6	FUN-X	R8-8
		Grain Buds	5	DEC-X	9-11
		Grani	5	CHG-X	GT-6	Paradox
		Gravel	4	EXE-X	4-8	Paradox
		GreyThroat	5	MAR-Y	2-10
		Greyy	4	SPC-Y	2-3	Paradox
		Greyy the Lightningbearer	5	BOM-X	1-3	Paradox
		Gummy	4	GUA-X	4-2	Paradox
		Harmonie	5	MSC-X	6-5
		Haze	4	CCR-X	S2-4	Paradox
		Heavyrain	5	PRO-Y	2-7	Paradox
		Hellagur	6	MUS-X	5-10	Paradox
		Hellagur	6	MUS-Y	7-10	Paradox
		Hibiscus the Purifier	5	INC-X	LE-4	Paradox
		Highmore	5	REA-X	SN-2
		Ho'olheyak	6	CCR-X	OF-7
		Ho'olheyak	6	CCR-Y	4-3
		Horn	6	FOR-X	7-15	Paradox
		Hoshiguma	6	PRO-X	6-4
		Hoshiguma	6	PRO-Y	3-1
		Humus	4	REA-X	7-8	Paradox
		Hung	5	GUA-Y	S2-4	Paradox
		Iana	5	PUM-X	3-7
		Ifrit	6	BLA-X	1-5	Paradox
		Indigo	4	MSC-X	SV-5	Paradox
		Indra	5	FGT-Y	11-5	Paradox
		Insider	5	MAR-X	3-4
		Irene	6	SWO-Y	SV-4
		Iris	5	MSC-X	2-2	Paradox
		Istina	5	DEC-Y	2-3	Paradox
		Jackie	4	FGT-X	CB-4	Paradox
		Jaye	4	MER-X	S3-2	Paradox
		Jessica	4	MAR-Y	SV-4	Paradox
		Jessica the Liberated	6	SPT-X	3-8
		Jieyun	5	ART-Y	S2-2
		Kafka	5	EXE-Y	2-7	Paradox
		Kal'tsit	6	PHY-X	5-10
		Kal'tsit	6	PHY-Y	5-10
		Kazemaru	5	PUM-X	3-1	Paradox
		Kirara	5	AMB-X	3-1	Paradox
		Kirin R Yato	6	EXE-X	3-7
		Kjera	5	FUN-Y	BI-7
		Kroos the Keen Glint	5	MAR-X	3-8
		La Pluma	5	REA-X	11-6	Paradox
		Lava the Purgatory	5	SPC-X	WR-4
		Lee	6	MER-X	3-1	Paradox
		Lee	6	MER-Y	IW-EX-1	Paradox
		Leizi	5	CHA-X	S3-6	Paradox
		Leonhardt	5	SPC-X	3-5	Paradox
		Lessing	6	DRE-X	7-11
		Lin	6	PLX-X	11-6
		Ling	6	SUM-Y	3-4	Paradox
		Liskarm	5	SPT-X	4-6	Paradox
		Lumen	6	WAH-X	6-9	Paradox
		Lumen	6	WAH-Y	OF-5	Paradox
		Lunacub	5	DEA-Y	2-5
		Lutonada	4	UNY-X	3-1
		Magallan	6	SUM-X	2-5	Paradox
		Magallan	6	SUM-Y	2-2	Paradox
		Manticore	5	AMB-X	1-9	Paradox
		Matoimaru	4	DRE-Y	S2-10
		Matterhorn	4	PRO-Y	3-6	Paradox
		May	4	MAR-X	4-4	Paradox
		Mayer	5	SUM-X	2-9	Paradox
		Melanite	5	ARC-Y	S3-5
		Meteor	4	MAR-X	2-7	Paradox
		Meteorite	5	ART-Y	2-4	Paradox
		Minimalist	5	FUN-Y	IC-8	Paradox
		Mint	5	PLX-X	BI-2	Paradox
		Mizuki	6	AMB-X	4-3	Paradox
		Mizuki	6	AMB-Y	4-5	Paradox
		Morgan	5	DRE-X	4-5	Paradox
		Mostima	6	SPC-Y	CB-5	Paradox
		Mountain	6	FGT-Y	MB-EX-3	Paradox
		Mr. Nothing	5	MER-X	3-7	Paradox
		Mudrock	6	UNY-X	11-7
		Muelsyse	6	TAC-X	3-4	Paradox
		Myrrh	4	PHY-X	6-11	Paradox
		Myrtle	4	BEA-X	6-3	Paradox
		Nearl	5	GUA-X	1-12
		Nearl the Radiant Knight	6	DRE-X	MN-8	Paradox
		Nearl the Radiant Knight	6	DRE-Y	NL-10	Paradox
		Nian	6	PRO-X	WR-9	Paradox
		Nian	6	PRO-Y	S6-2	Paradox
		Nightingale	6	RIN-X	3-6	Paradox
		Nightmare	5	CCR-Y	S4-1
		Nine-Colored Deer	5	BLS-X	IW-3
		Pallas	6	INS-X	3-6
		Pallas	6	INS-Y	4-3
		Passenger	6	CHA-X	2-2
		Passenger	6	CHA-Y	5-10
		Penance	6	UNY-X	CB-4
		Perfumer	4	RIN-Y	3-4	Paradox
		Phantom	6	EXE-X	3-6	Paradox
		Phantom	6	EXE-Y	DM-5	Paradox
		Platinum	5	MAR-X	5-7	Paradox
		Podenco	4	DEC-X	4-6	Paradox
		Poncirus	5	SOL-X	OF-5
		Pozëmka	6	ARC-Y	2-10	Paradox
		Pramanix	5	UMD-X	2-3	Paradox
		Projekt Red	5	EXE-Y	S4-5
		Provence	5	ARC-X	S5-1	Paradox
		Proviso	5	DEC-X	4-7	Paradox
		Ptilopsis	5	RIN-X	4-9	Paradox
		Pudding	4	CHA-Y	2-10	Paradox
		Purestream	4	WAH-Y	3-7	Paradox
		Qanipalaat	5	CCR-Y	2-3	Paradox
		Quercus	5	BLS-X	9-2	Paradox
		Rathalos S Noir Corne	5	MUS-X	3-8
		Reed	5	CHG-Y	9-5
		Reed the Flame Shadow	6	INC-X	11-6
		Roberta	4	CRA-X	3-8	Paradox
		Robin	5	TRP-Y	BI-EX-2
		Rockrock	5	FUN-X	RI-EX-4	Paradox
		Rope	4	HOK-X	3-3	Paradox
		Rosa	6	SIE-X	7-9
		Rosmontis	6	BOM-X	4-1
		Saga	6	SOL-X	WR-3
		Saga	6	SOL-Y	WR-1
		Saileach	6	BEA-X	9-2	Paradox
		Santalla	5	SPC-Y	BI-2
		Saria	6	GUA-X	MB-3
		Saria	6	GUA-Y	4-6
		Savage	5	CEN-X	4-5	Paradox
		Scavenger	4	SOL-X	1-3	Paradox
		Scene	5	SUM-X	3-1	Paradox
		Schwarz	6	ARC-X	OF-7	Paradox
		Schwarz	6	ARC-Y	2-1	Paradox
		Sesa	5	ART-X	S3-6	Paradox
		Shamare	5	UMD-X	6-5	Paradox
		Shaw	4	PUS-Y	2-8	Paradox
		Shining	6	PHY-X	NL-5
		Shining	6	PHY-Y	5-8
		Shirayuki	4	ART-Y	S2-11	Paradox
		Shu	6	GUA-X	11-11
		Siege	6	SOL-X	1-7
		Siege	6	SOL-Y	11-2
		Silence	5	PHY-Y	4-8	Paradox
		Silence the Paradigmatic	6	BLS-X	6-3
		Skadi	6	DRE-X	GT-4	Paradox
		Skadi	6	DRE-Y	GT-6	Paradox
		Skyfire	5	SPC-Y	S3-5	Paradox
		Snowsant	5	HOK-X	4-6	Paradox
		Specter	5	CEN-X	SN-EX-1	Paradox
		Specter the Unchained	6	PUM-X	SV-EX-1
		Specter the Unchained	6	PUM-Y	S4-1
		Spuria	5	GEE-X	DM-2
		Stainless	6	CRA-X	11-6
		Sussurro	4	PHY-X	4-7	Paradox
		Suzuran	6	DEC-X	TW-7
		Suzuran	6	DEC-Y	DM-EX-1
		Swire	5	INS-X	S2-3	Paradox
		Swire the Elegant Wit	6	MER-X	5-2
		Tachanka	5	SWO-X	S2-3
		Texas	5	SOL-Y	CB-3	Paradox
		Texas the Omertosa	6	EXE-Y	CB-8
		Toddifons	5	SIE-X	4-6	Paradox
		Tomimi	5	CCR-Y	RI-2	Paradox
		Totter	4	SIE-X	S3-5	Paradox
		Tsukinogi	5	BLS-X	S3-6	Paradox
		Tuye	5	PHY-X	4-6	Paradox
		Typhon	6	SIE-X	S2-1
		Utage	4	MUS-X	1-12	Paradox
		Vendela	5	INC-X	IC-2
		Verdant	4	PUM-Y	9-3
		Vermeil	4	MAR-X	3-1	Paradox
		Vigil	6	TAC-X	3-8
		Vigna	4	CHG-Y	3-3	Paradox
		Vulcan	5	UNY-X	S4-4	Paradox
		W	6	ART-X	DM-3
		W	6	ART-Y	DM-7
		Waai Fu	5	EXE-X	4-7	Paradox
		Wanqing	5	BEA-X	9-13
		Warfarin	5	PHY-X	2-10	Paradox
		Weedy	6	PUS-X	BI-5	Paradox
		Weedy	6	PUS-Y	IC-6	Paradox
		Whislash	5	INS-X	S3-6	Paradox
		Whisperain	5	WAH-X	2-5	Paradox
		Wild Mane	5	CHG-X	MN-2	Paradox
		Windflit	5	CRA-X	11-9	Paradox
		Zima	5	SOL-X	1-5	Paradox
		Zuo Le	6	MUS-X	WB-5
		""")
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
		.Select(static (v, i) => HighlightIf(v.Total > 1, new
		{
			ID        = i + 1,
			Stage     = new WikiHyperlinq(v.Stage, "Information"),
			Operators = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Operator))),
			v.Stars,
			Module    = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Operator, v.Module, v.Module))),
			v.Total,
			Paradox   = VerticalRun(v.Operators.Select(static v => v.Paradox ? new Hyperlinq($"https://www.youtube.com/results?search_query={Uri.EscapeDataString(v.Operator)}+paradox+simulation", "YouTube") : (object)Empty))
		}))
		.ToArray();

	const string title = "Joined Operators Modules";

	(operatorModules.Any() ? (object)operatorModules : "No operators modules found.").Dump(title);

	title.AsHyperlink(new WikiHyperlinq("Operator_Module", "List").Uri);

	static T Pass<T>(T t) => t;
}

static class Extensions
{
	public static string JoinStrings<T, TV>(this IEnumerable<T> operatorModules, Func<T, TV> selector) =>
		 Join(NewLine, operatorModules.Select(selector));

	public static IEnumerable<OperatorModule> OnlyFor(this IEnumerable<OperatorModule> operatorModules, IEnumerable<Operator> operators)
	{
		var stringComparer   = StringComparer.OrdinalIgnoreCase;
		var stringComparison = StringComparison.OrdinalIgnoreCase;

		var operatorsToModuleType = operators.ToDictionary(static v => v.Name, static v => v.ModuleType, stringComparer);

		return operatorModules.Where(v => operatorsToModuleType.TryGetValue(v.Operator, out var moduleType) && HasModule(moduleType, v.Module));

		bool HasModule(ModuleType moduleType, string module) =>
			moduleType switch
			{
				ModuleType.X when module.EndsWith("X", stringComparison) => true,
				ModuleType.Y when module.EndsWith("Y", stringComparison) => true,
				ModuleType.Both or ModuleType.Unknown => true,
				_ => false
			};
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
		match.Groups[key].Value;

	protected static int GetNumber(Match match, string key, int fallbackValue = default) =>
		int.TryParse(GetString(match, key), Integer, InvariantCulture, out var value)
			? value
			: fallbackValue;

	[DoesNotReturn]
	protected static void Throw(string str, string message) =>
		throw new($"Invalid data: '{str}'. {message}");
}

internal record OperatorModule(string Operator, int Stars, string Module, string Stage, bool Paradox);

class OperatorModules : Parsable<OperatorModule>
{
	private const string Operator = nameof(Operator);
	private const string Stars    = nameof(Stars);
	private const string Stage    = nameof(Stage);
	private const string Module   = nameof(Module);
	private const string Paradox  = nameof(Paradox);

	protected override string Regex { get; } = $@"^(?<{Operator}>[^\t]+)\t+(?<{Stars}>[1-6])\t+(?<{Stage}>[^\t]+)\t+(?<{Module}>[^\t]+)(\t+(?<{Paradox}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = "Use OperatorModulesParser.linq script to update.";

	protected override OperatorModule Create(Match match) =>
		new
		(
			GetString(match, Operator),
			GetNumber(match, Stars),
			GetString(match, Stage),
			GetString(match, Module),
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
	Both,
	X,
	Y
}

internal record Operator(string Name, ModuleType ModuleType);

class Operators : Parsable<Operator>
{
	private const string Comment = "//";

	private const string Operator = nameof(Operator);
	private const string Module   = nameof(Module);

	protected override string Regex { get; } = $@"^(?<{Operator}>[^\t]+)(\t+(?<{Module}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = "Format: name <tab> type (X or Y or empty for both)";

	protected override Operator Create(Match match)
	{
		var moduleType = GetString(match, Module).ToLowerInvariant() switch
		{
			"x" => ModuleType.X,
			"y" => ModuleType.Y,
			""  => ModuleType.Both,
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
		: base($"https://arknights.wiki.gg/wiki/{uri.Replace(' ', '_')}{GetFragment(fragment)}", text ?? uri)
	{
	}

	private static string GetFragment(string? fragment) =>
		$"{(IsNullOrWhiteSpace(fragment) ? Empty : "#")}{fragment}";
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
