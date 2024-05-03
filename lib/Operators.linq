<Query Kind="Statements" />

#nullable enable

#load "./Parsable.linq"

record Operator(string Name, string Class, int Stars, string Module, string Stage, string E2Materials, bool Paradox);

class Operators : Parsable<Operator>
{
	private const string Name        = nameof(Name);
	private const string Class       = nameof(Class);
	private const string Stars       = nameof(Stars);
	private const string Stage       = nameof(Stage);
	private const string Module      = nameof(Module);
	private const string Paradox     = nameof(Paradox);
	private const string E2Materials = nameof(E2Materials);

	protected override string Regex { get; } = $@"^(?<{Name}>[^\t]+)\t+(?<{Class}>[^\t]+)\t+(?<{Stars}>[1-6])\t+(?<{Stage}>[^\t]+)\t+(?<{Module}>[^\t]+)\t+(?<{E2Materials}>[^\t]+)(\t+(?<{Paradox}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = "Use OperatorModulesParser.linq script to update.";

	protected override Operator Create(Match match) =>
		new
		(
			GetString(match, Name),
			GetString(match, Class),
			GetNumber(match, Stars),
			GetString(match, Stage),
			GetString(match, Module),
			GetString(match, E2Materials),
			!string.IsNullOrWhiteSpace(GetString(match, Paradox))
		);

	public Operators(string operatorModules)
		: base(operatorModules)
	{
	}
}

enum ModuleType
{
	Unknown,
	All,
	X,
	Y,
	D
}

record OperatorModule(string Name, ModuleType ModuleType);

class OperatorModules : Parsable<OperatorModule>
{
	private const string Name   = nameof(Name);
	private const string Module = nameof(Module);

	protected override string Regex { get; } = $@"^(?<{Name}>[^\t]+)(\t+(?<{Module}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = "Format: name <tab> type (X, Y, D or empty for all)";

	protected override OperatorModule Create(Match match)
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
			GetString(match, Name),
			moduleType
		);
	}

	public OperatorModules(string operators)
		: base(operators)
	{
	}
}

static partial class OperatorData
{
	public const char MaterialSeparator = '｜';
	public const char CountSeparator    = '❂';

	// TODO: Run OperatorModulesParser.linq and paste from clipboard to the property below.
	public static readonly Operators Value = new("""
		Aak	Specialist	6	GEE-X	2-8	D32 Steel❂4｜Polymerized Gel❂7	Paradox
		Absinthe	Caster	5	CCR-X	3-4	Orirock Concentration❂10｜Incandescent Alloy❂10
		Aciddrop	Sniper	4	ARC-X	2-8	RMA70-12❂10｜Integrated Device❂8	Paradox
		Akafuyu	Guard	5	MUS-X	3-8	Polymerized Gel❂8｜Aketon❂15	Paradox
		Almond	Specialist	5	HOK-X	BI-2	Oriron Block❂6｜Manganese Ore❂13	Paradox
		Ambriel	Sniper	4	DEA-X	4-7	Oriron Cluster❂16｜RMA70-12❂6
		Amiya	Caster	5	CCR-Y	3-8	Orirock Concentration❂10｜Loxic Kohl❂10
		Andreana	Sniper	5	DEA-X	3-2	Grindstone Pentahydrate❂8｜RMA70-12❂8
		Angelina	Supporter	6	DEC-X	2-7	Bipolar Nanoflake❂4｜Sugar Lump❂5	Paradox
		Angelina	Supporter	6	DEC-Y	4-4	Bipolar Nanoflake❂4｜Sugar Lump❂5	Paradox
		April	Sniper	5	MAR-Y	5-3	Polymerized Gel❂9｜RMA70-12❂9	Paradox
		Archetto	Sniper	6	MAR-X	SV-4	Polymerization Preparation❂4｜Orirock Concentration❂8	Paradox
		Archetto	Sniper	6	MAR-Y	2-9	Polymerization Preparation❂4｜Orirock Concentration❂8	Paradox
		Aroma	Caster	5	BLA-X	GT-HX-3	RMA70-24❂7｜Loxic Kohl❂21
		Ascalon	Specialist	6	AMB-X	11-6	D32 Steel❂4｜Keton Colloid❂6
		Ash	Sniper	6	MAR-X	SV-8	D32 Steel❂4｜Polymerized Gel❂6
		Ash	Sniper	6	MAR-Y	1-12	D32 Steel❂4｜Polymerized Gel❂6
		Ashlock	Defender	5	FOR-X	7-9	Polymerized Gel❂8｜Compound Cutting Fluid❂13
		Astgenne	Caster	5	CHA-Y	4-3	RMA70-24❂7｜Coagulating Gel❂10	Paradox
		Aurora	Defender	5	HES-X	3-8	White Horse Kohl❂9｜Orirock Cluster❂19	Paradox
		Bagpipe	Vanguard	6	CHG-X	4-5	Polymerization Preparation❂4｜Orirock Concentration❂9	Paradox
		Bagpipe	Vanguard	6	CHG-Y	9-5	Polymerization Preparation❂4｜Orirock Concentration❂9	Paradox
		Bassline	Defender	5	GUA-Y	LE-4	Cyclicene Prefab❂7｜Manganese Ore❂15
		Beanstalk	Vanguard	4	TAC-Y	S3-7	Coagulating Gel❂13｜Manganese Ore❂10	Paradox
		Beehunter	Guard	4	FGT-Y	7-4	Sugar Pack❂19｜RMA70-12❂7	Paradox
		Beeswax	Caster	5	PLX-X	DH-6	Optimized Device❂5｜Loxic Kohl❂18	Paradox
		Bena	Specialist	5	PUM-Y	2-3	Oriron Block❂6｜Loxic Kohl❂15	Paradox
		Bibeak	Guard	5	SWO-X	3-7	Manganese Trihydrate❂8｜RMA70-12❂8	Paradox
		Bison	Defender	5	PRO-X	3-7	Grindstone Pentahydrate❂7｜RMA70-12❂11	Paradox
		Blacknight	Vanguard	5	TAC-X	3-1	Incandescent Alloy Block❂8｜Loxic Kohl❂15	Paradox
		Blaze	Guard	6	CEN-X	11-13	D32 Steel❂4｜Optimized Device❂6	Paradox
		Blemishine	Defender	6	GUA-X	MN-4	D32 Steel❂4｜RMA70-24❂7	Paradox
		Blemishine	Defender	6	GUA-Y	MN-2	D32 Steel❂4｜RMA70-24❂7	Paradox
		Blitz	Defender	5	SPT-X	3-4	Manganese Trihydrate❂8｜RMA70-12❂8
		Blue Poison	Sniper	5	MAR-Y	3-4	Manganese Trihydrate❂8｜Integrated Device❂8	Paradox
		Breeze	Medic	5	RIN-X	4-1	Optimized Device❂5｜Loxic Kohl❂18	Paradox
		Broca	Guard	5	CEN-X	7-8	Sugar Lump❂7｜Grindstone❂13	Paradox
		Bryophyta	Guard	5	INS-X	9-3	Refined Solvent❂7｜Aketon❂15
		Bubble	Defender	4	PRO-X	4-6	Coagulating Gel❂16｜Loxic Kohl❂8
		Carnelian	Caster	6	PLX-X	7-3	D32 Steel❂4｜RMA70-24❂7	Paradox
		Carnelian	Caster	6	PLX-Y	4-6	D32 Steel❂4｜RMA70-24❂7	Paradox
		Cement	Defender	5	HES-X	IW-EX-6	Manganese Trihydrate❂7｜Grindstone❂13
		Ceobe	Caster	6	CCR-X	RI-6	Bipolar Nanoflake❂4｜Incandescent Alloy Block❂5	Paradox
		Ceobe	Caster	6	CCR-Y	11-7	Bipolar Nanoflake❂4｜Incandescent Alloy Block❂5	Paradox
		Ceylon	Medic	5	WAH-X	OF-3	Oriron Block❂7｜Aketon❂10	Paradox
		Ch'en	Guard	6	SWO-X	5-8	Polymerization Preparation❂4｜White Horse Kohl❂6	Paradox
		Ch'en	Guard	6	SWO-Y	5-9	Polymerization Preparation❂4｜White Horse Kohl❂6	Paradox
		Chiave	Vanguard	5	SOL-Y	1-1	Manganese Trihydrate❂7｜Grindstone❂13	Paradox
		Chongyue	Guard	6	FGT-X	WB-7	Polymerization Preparation❂4｜Incandescent Alloy❂5
		Click	Caster	4	FUN-Y	11-7	Manganese Ore❂15｜Oriron Cluster❂9	Paradox
		Cliffheart	Specialist	5	HOK-X	MN-5	Oriron Block❂6｜Manganese Ore❂13	Paradox
		Conviction	Guard	4	DRE-X	2-4	Integrated Device❂11｜Coagulating Gel❂9
		Corroserum	Caster	5	BLA-X	BI-6	Optimized Device❂5｜RMA70-12❂10	Paradox
		Courier	Vanguard	4	SOL-X	S2-2	Integrated Device❂11｜Aketon❂10
		Croissant	Defender	5	PRO-Y	2-8	RMA70-24❂8｜Integrated Device❂8	Paradox
		Cuora	Defender	4	PRO-X	2-10	Grindstone❂14｜Loxic Kohl❂10	Paradox
		Cutter	Guard	4	SWO-Y	2-7	Coagulating Gel❂12｜Manganese Ore❂11	Paradox
		Dagda	Guard	5	FGT-Y	11-5	Incandescent Alloy Block❂7｜Orirock Cluster❂21	Paradox
		Deepcolor	Supporter	4	SUM-Y	2-4	Orirock Cluster❂20｜Manganese Ore❂9	Paradox
		Degenbrecher	Guard	6	SWO-X	BI-6	Nucleic Crystal Sinter❂3｜Grindstone Pentahydrate❂7
		Delphine	Caster	5	MSC-Y	WD-6	Cyclicene Prefab❂8｜Semi-Synthetic Solvent❂10
		Dobermann	Guard	4	INS-Y	1-12	Manganese Ore❂15｜Loxic Kohl❂11
		Doc	Guard	5	INS-Y	2-5	Polymerized Gel❂8｜Integrated Device❂11
		Dorothy	Specialist	6	TRP-Y	4-5	Polymerization Preparation❂4｜Cutting Fluid Solution❂8	Paradox
		Dusk	Caster	6	SPC-X	1-6	Crystalline Electronic Unit❂4｜Manganese Trihydrate❂6	Paradox
		Dusk	Caster	6	SPC-Y	4-3	Crystalline Electronic Unit❂4｜Manganese Trihydrate❂6	Paradox
		Earthspirit	Supporter	4	DEC-Y	DM-2	Sugar Pack❂20｜Grindstone❂7	Paradox
		Ebenholz	Caster	6	MSC-X	3-7	D32 Steel❂4｜Refined Solvent❂7	Paradox
		Ebenholz	Caster	6	MSC-Y	3-6	D32 Steel❂4｜Refined Solvent❂7	Paradox
		Ebenholz	Caster	6	MSC-Δ	11-12	D32 Steel❂4｜Refined Solvent❂7	Paradox
		Elysium	Vanguard	5	BEA-X	S3-6	Incandescent Alloy Block❂7｜Aketon❂16	Paradox
		Enforcer	Specialist	5	PUS-X	GA-EX-1	Polymerized Gel❂8｜Compound Cutting Fluid❂12	Paradox
		Erato	Sniper	5	SIE-X	S5-9	Optimized Device❂5｜RMA70-12❂11	Paradox
		Estelle	Guard	4	CEN-Y	7-4	RMA70-12❂12｜Grindstone❂8	Paradox
		Ethan	Specialist	4	AMB-X	TW-3	Sugar Pack❂17｜Orirock Cluster❂14	Paradox
		Eunectes	Defender	6	HES-X	RI-EX-4	Bipolar Nanoflake❂4｜Polymerized Gel❂7
		Eunectes	Defender	6	HES-Y	RI-7	Bipolar Nanoflake❂4｜Polymerized Gel❂7
		Executor the Ex Foedere	Guard	6	REA-X	7-14	Nucleic Crystal Sinter❂3｜Polymerized Gel❂7	Paradox
		Exusiai	Sniper	6	MAR-X	4-10	Polymerization Preparation❂4｜Sugar Lump❂5	Paradox
		Eyjafjalla	Caster	6	CCR-X	OF-5	Polymerization Preparation❂4｜Optimized Device❂5	Paradox
		Fartooth	Sniper	6	DEA-X	S2-2	D32 Steel❂4｜Cutting Fluid Solution❂7
		Fartooth	Sniper	6	DEA-Y	2-5	D32 Steel❂4｜Cutting Fluid Solution❂7
		FEater	Specialist	5	PUS-Y	WR-7	Grindstone Pentahydrate❂8｜Polyester Pack❂15	Paradox
		Fiammetta	Sniper	6	ART-X	GA-4	Crystalline Electronic Unit❂3｜Grindstone Pentahydrate❂6	Paradox
		Fiammetta	Sniper	6	ART-Y	S5-9	Crystalline Electronic Unit❂3｜Grindstone Pentahydrate❂6	Paradox
		Firewatch	Sniper	5	DEA-Y	2-10	Polyester Lump❂7｜Loxic Kohl❂15	Paradox
		Firewhistle	Defender	5	FOR-X	S4-6	Grindstone Pentahydrate❂7｜Coagulating Gel❂13	Paradox
		Flamebringer	Guard	5	DRE-Y	S4-3	White Horse Kohl❂9｜Manganese Ore❂13
		Flametail	Vanguard	6	SOL-X	NL-5	Bipolar Nanoflake❂4｜Orirock Concentration❂9
		Flametail	Vanguard	6	SOL-Y	NL-3	Bipolar Nanoflake❂4｜Orirock Concentration❂9
		Flint	Guard	5	FGT-Y	RI-9	Orirock Concentration❂8｜Grindstone❂15	Paradox
		Folinic	Medic	5	PHY-Y	5-2	Keton Colloid❂8｜Integrated Device❂8
		Franka	Guard	5	DRE-X	9-3	Oriron Block❂6｜Sugar Pack❂18	Paradox
		Frost	Specialist	5	TRP-Y	4-4	Grindstone Pentahydrate❂8｜Orirock Cluster❂17
		Fuze	Guard	5	CEN-Y	3-1	Incandescent Alloy Block❂8｜Sugar Pack❂16
		Gavial	Medic	4	PHY-Y	S4-5	Integrated Device❂13｜Oriron Cluster❂6
		Gavial the Invincible	Guard	6	CEN-X	9-2	Bipolar Nanoflake❂4｜Polymerized Gel❂6	Paradox
		Gitano	Caster	4	SPC-X	1-10	Sugar Pack❂17｜Orirock Cluster❂14	Paradox
		Gladiia	Specialist	6	HOK-X	SV-EX-5	Crystalline Electronic Unit❂4｜Polymerized Gel❂6	Paradox
		Glaucus	Supporter	5	DEC-X	5-7	Keton Colloid❂7｜Integrated Device❂10	Paradox
		Gnosis	Supporter	6	UMD-X	BI-7	Crystalline Electronic Unit❂3｜Incandescent Alloy Block❂7
		Goldenglow	Caster	6	FUN-X	R8-8	Bipolar Nanoflake❂4｜Manganese Trihydrate❂5
		Grain Buds	Supporter	5	DEC-X	9-11	Crystalline Circuit❂7｜Compound Cutting Fluid❂11
		Grani	Vanguard	5	CHG-X	GT-6	RMA70-24❂7｜Oriron Cluster❂13	Paradox
		Gravel	Specialist	4	EXE-X	4-8	Polyester Pack❂18｜Orirock Cluster❂13	Paradox
		GreyThroat	Sniper	5	MAR-Y	2-10	Oriron Block❂7｜Integrated Device❂9
		Greyy	Caster	4	SPC-Y	2-3	Manganese Ore❂15｜Aketon❂9	Paradox
		Greyy the Lightningbearer	Sniper	5	BOM-X	1-3	Grindstone Pentahydrate❂7｜Incandescent Alloy❂16	Paradox
		Gummy	Defender	4	GUA-X	4-2	RMA70-12❂13｜Manganese Ore❂7	Paradox
		Harmonie	Caster	5	MSC-X	6-5	RMA70-24❂6｜Oriron Cluster❂15
		Haze	Caster	4	CCR-X	S2-4	Orirock Cluster❂19｜RMA70-12❂8	Paradox
		Heavyrain	Defender	5	PRO-Y	2-7	Orirock Concentration❂9｜Oriron Cluster❂14	Paradox
		Hellagur	Guard	6	MUS-X	5-10	Bipolar Nanoflake❂4｜Polyester Lump❂7	Paradox
		Hellagur	Guard	6	MUS-Y	7-10	Bipolar Nanoflake❂4｜Polyester Lump❂7	Paradox
		Hibiscus the Purifier	Medic	5	INC-X	LE-4	White Horse Kohl❂9｜Orirock Cluster❂18	Paradox
		Highmore	Guard	5	REA-X	SN-2	Orirock Concentration❂9｜Semi-Synthetic Solvent❂13
		Ho'olheyak	Caster	6	CCR-X	OF-7	D32 Steel❂4｜Transmuted Salt Agglomerate❂7
		Ho'olheyak	Caster	6	CCR-Y	4-3	D32 Steel❂4｜Transmuted Salt Agglomerate❂7
		Horn	Defender	6	FOR-X	7-15	D32 Steel❂4｜Oriron Block❂7	Paradox
		Hoshiguma	Defender	6	PRO-X	6-4	Polymerization Preparation❂4｜Grindstone Pentahydrate❂5
		Hoshiguma	Defender	6	PRO-Y	3-1	Polymerization Preparation❂4｜Grindstone Pentahydrate❂5
		Humus	Guard	4	REA-X	7-8	RMA70-12❂11｜Sugar Pack❂12	Paradox
		Hung	Defender	5	GUA-Y	S2-4	Incandescent Alloy Block❂7｜Aketon❂15	Paradox
		Iana	Specialist	5	PUM-X	3-7	Transmuted Salt Agglomerate❂7｜Aggregate Cyclicene❂12
		Ifrit	Caster	6	BLA-X	1-5	D32 Steel❂4｜Polyester Lump❂7	Paradox
		Indigo	Caster	4	MSC-X	SV-5	Oriron Cluster❂14｜RMA70-12❂7	Paradox
		Indra	Guard	5	FGT-Y	11-5	Keton Colloid❂7｜Polyester Pack❂16	Paradox
		Insider	Sniper	5	MAR-X	3-4	Polymerized Gel❂9｜Sugar Pack❂15
		Irene	Guard	6	SWO-Y	SV-4	Bipolar Nanoflake❂4｜RMA70-24❂7
		Iris	Caster	5	MSC-X	2-2	Oriron Block❂6｜Integrated Device❂11	Paradox
		Istina	Supporter	5	DEC-Y	2-3	Optimized Device❂5｜RMA70-12❂9	Paradox
		Jackie	Guard	4	FGT-X	CB-4	Orirock Cluster❂19｜Loxic Kohl❂12	Paradox
		Jaye	Specialist	4	MER-X	S3-2	Grindstone❂14｜Aketon❂8	Paradox
		Jessica	Sniper	4	MAR-Y	SV-4	Loxic Kohl❂20｜Oriron Cluster❂7	Paradox
		Jessica the Liberated	Defender	6	SPT-X	3-8	Crystalline Electronic Unit❂4｜Optimized Device❂4
		Jieyun	Sniper	5	ART-Y	S2-2	Keton Colloid❂7｜RMA70-12❂11
		Kafka	Specialist	5	EXE-Y	2-7	Polymerized Gel❂8｜Oriron Cluster❂15	Paradox
		Kal'tsit	Medic	6	PHY-X	5-10	Crystalline Electronic Unit❂4｜Optimized Device❂4
		Kal'tsit	Medic	6	PHY-Y	5-10	Crystalline Electronic Unit❂4｜Optimized Device❂4
		Kazemaru	Specialist	5	PUM-X	3-1	Orirock Concentration❂9｜Semi-Synthetic Solvent❂13	Paradox
		Kirara	Specialist	5	AMB-X	3-1	Incandescent Alloy Block❂7｜Integrated Device❂11	Paradox
		Kirin R Yato	Specialist	6	EXE-X	3-7	Nucleic Crystal Sinter❂3｜Keton Colloid❂6
		Kjera	Caster	5	FUN-Y	BI-7	Grindstone Pentahydrate❂8｜Incandescent Alloy❂13
		Kroos the Keen Glint	Sniper	5	MAR-X	3-8	Crystalline Circuit❂7｜Oriron Cluster❂10
		La Pluma	Guard	5	REA-X	11-6	Keton Colloid❂7｜Manganese Ore❂13	Paradox
		Lava the Purgatory	Caster	5	SPC-X	WR-4	White Horse Kohl❂8｜Grindstone❂13
		Lee	Specialist	6	MER-X	3-1	Polymerization Preparation❂4｜White Horse Kohl❂9	Paradox
		Lee	Specialist	6	MER-Y	IW-EX-1	Polymerization Preparation❂4｜White Horse Kohl❂9	Paradox
		Leizi	Caster	5	CHA-X	S3-6	RMA70-24❂7｜Coagulating Gel❂13	Paradox
		Leonhardt	Caster	5	SPC-X	3-5	Keton Colloid❂7｜Loxic Kohl❂15	Paradox
		Lessing	Guard	6	DRE-X	7-11	Crystalline Electronic Unit❂4｜Cyclicene Prefab❂5
		Lin	Caster	6	PLX-X	11-6	D32 Steel❂4｜Cutting Fluid Solution❂8
		Ling	Supporter	6	SUM-Y	3-4	D32 Steel❂4｜Crystalline Circuit❂5	Paradox
		Liskarm	Defender	5	SPT-X	4-6	Grindstone Pentahydrate❂7｜Aketon❂15	Paradox
		Lumen	Medic	6	WAH-X	6-9	Crystalline Electronic Unit❂4｜Optimized Device❂5	Paradox
		Lumen	Medic	6	WAH-Y	OF-5	Crystalline Electronic Unit❂4｜Optimized Device❂5	Paradox
		Lunacub	Sniper	5	DEA-Y	2-5	Refined Solvent❂8｜Aketon❂13
		Lutonada	Defender	4	UNY-X	3-1	Semi-Synthetic Solvent❂14｜Integrated Device❂7
		Magallan	Supporter	6	SUM-X	2-5	Polymerization Preparation❂4｜Manganese Trihydrate❂6	Paradox
		Magallan	Supporter	6	SUM-Y	2-2	Polymerization Preparation❂4｜Manganese Trihydrate❂6	Paradox
		Manticore	Specialist	5	AMB-X	1-9	Manganese Trihydrate❂8｜Sugar Pack❂12	Paradox
		Matoimaru	Guard	4	DRE-Y	S2-10	Aketon❂16｜Sugar Pack❂10
		Matterhorn	Defender	4	PRO-Y	3-6	Manganese Ore❂14｜Integrated Device❂7	Paradox
		May	Sniper	4	MAR-X	4-4	Oriron Cluster❂14｜Polyester Pack❂12	Paradox
		Mayer	Supporter	5	SUM-X	2-9	Oriron Block❂6｜RMA70-12❂11	Paradox
		Melanite	Sniper	5	ARC-Y	S3-5	Refined Solvent❂8｜Loxic Kohl❂15
		Meteor	Sniper	4	MAR-X	2-7	Oriron Cluster❂14｜Polyester Pack❂12	Paradox
		Meteorite	Sniper	5	ART-Y	2-4	RMA70-24❂7｜Manganese Ore❂14	Paradox
		Minimalist	Caster	5	FUN-Y	IC-8	Crystalline Circuit❂6｜Oriron Cluster❂12	Paradox
		Mint	Caster	5	PLX-X	BI-2	Incandescent Alloy Block❂9｜Orirock Cluster❂14	Paradox
		Mizuki	Specialist	6	AMB-X	4-3	Polymerization Preparation❂4｜Crystalline Circuit❂6	Paradox
		Mizuki	Specialist	6	AMB-Y	4-5	Polymerization Preparation❂4｜Crystalline Circuit❂6	Paradox
		Morgan	Guard	5	DRE-X	4-5	Cutting Fluid Solution❂8｜Transmuted Salt❂11	Paradox
		Mostima	Caster	6	SPC-Y	CB-5	Bipolar Nanoflake❂4｜Grindstone Pentahydrate❂7	Paradox
		Mountain	Guard	6	FGT-Y	MB-EX-3	Crystalline Electronic Unit❂4｜Polymerized Gel❂8	Paradox
		Mr. Nothing	Specialist	5	MER-X	3-7	Optimized Device❂6｜Manganese Ore❂10	Paradox
		Mudrock	Defender	6	UNY-X	11-7	Crystalline Electronic Unit❂4｜Incandescent Alloy Block❂5
		Muelsyse	Vanguard	6	TAC-X	3-4	Bipolar Nanoflake❂4｜Grindstone Pentahydrate❂4	Paradox
		Myrrh	Medic	4	PHY-X	6-11	Aketon❂14｜Polyester Pack❂12	Paradox
		Myrtle	Vanguard	4	BEA-X	6-3	Grindstone❂12｜Integrated Device❂8	Paradox
		Nearl	Defender	5	GUA-X	1-12	White Horse Kohl❂9｜Polyester Pack❂16
		Nearl the Radiant Knight	Guard	6	DRE-X	MN-8	Polymerization Preparation❂4｜Polymerized Gel❂8	Paradox
		Nearl the Radiant Knight	Guard	6	DRE-Y	NL-10	Polymerization Preparation❂4｜Polymerized Gel❂8	Paradox
		Nian	Defender	6	PRO-X	WR-9	Polymerization Preparation❂4｜Incandescent Alloy Block❂7	Paradox
		Nian	Defender	6	PRO-Y	S6-2	Polymerization Preparation❂4｜Incandescent Alloy Block❂7	Paradox
		Nightingale	Medic	6	RIN-X	3-6	D32 Steel❂4｜Keton Colloid❂6	Paradox
		Nightmare	Caster	5	CCR-Y	S4-1	Sugar Lump❂7｜Manganese Ore❂14
		Nine-Colored Deer	Supporter	5	BLS-X	IW-3	Manganese Trihydrate❂7｜Crystalline Component❂14
		Pallas	Guard	6	INS-X	3-6	Crystalline Electronic Unit❂4｜White Horse Kohl❂6
		Pallas	Guard	6	INS-Y	4-3	Crystalline Electronic Unit❂4｜White Horse Kohl❂6
		Passenger	Caster	6	CHA-X	2-2	Bipolar Nanoflake❂4｜Oriron Block❂5
		Passenger	Caster	6	CHA-Y	5-10	Bipolar Nanoflake❂4｜Oriron Block❂5
		Penance	Defender	6	UNY-X	CB-4	D32 Steel❂4｜White Horse Kohl❂8
		Perfumer	Medic	4	RIN-Y	3-4	Loxic Kohl❂19｜Aketon❂8	Paradox
		Phantom	Specialist	6	EXE-X	3-6	Polymerization Preparation❂4｜Polymerized Gel❂9	Paradox
		Phantom	Specialist	6	EXE-Y	DM-5	Polymerization Preparation❂4｜Polymerized Gel❂9	Paradox
		Platinum	Sniper	5	MAR-X	5-7	Grindstone Pentahydrate❂8｜Loxic Kohl❂15	Paradox
		Podenco	Supporter	4	DEC-X	4-6	Incandescent Alloy❂15｜Grindstone❂5	Paradox
		Poncirus	Vanguard	5	SOL-X	OF-5	Incandescent Alloy Block❂8｜Orirock Cluster❂18
		Pozëmka	Sniper	6	ARC-Y	2-10	Crystalline Electronic Unit❂3｜Orirock Concentration❂9	Paradox
		Pramanix	Supporter	5	UMD-X	2-3	Keton Colloid❂7｜Grindstone❂11	Paradox
		Projekt Red	Specialist	5	EXE-Y	S4-5	Manganese Trihydrate❂7｜Oriron Cluster❂14
		Provence	Sniper	5	ARC-X	S5-1	Sugar Lump❂9｜Integrated Device❂7	Paradox
		Proviso	Supporter	5	DEC-X	4-7	Oriron Block❂7｜Manganese Ore❂12	Paradox
		Ptilopsis	Medic	5	RIN-X	4-9	Orirock Concentration❂9｜Grindstone❂10	Paradox
		Pudding	Caster	4	CHA-Y	2-10	RMA70-12❂11｜Cutting Fluid Solution❂3	Paradox
		Purestream	Medic	4	WAH-Y	3-7	Integrated Device❂11｜Coagulating Gel❂9	Paradox
		Qanipalaat	Caster	5	CCR-Y	2-3	Oriron Block❂7｜Manganese Ore❂12	Paradox
		Quercus	Supporter	5	BLS-X	9-2	Oriron Block❂6｜Manganese Ore❂13	Paradox
		Rathalos S Noir Corne	Guard	5	MUS-X	3-8	Transmuted Salt Agglomerate❂8｜Crystalline Component❂11
		Reed	Vanguard	5	CHG-Y	9-5	Orirock Concentration❂9｜Manganese Ore❂12	Paradox
		Reed the Flame Shadow	Medic	6	INC-X	11-6	Nucleic Crystal Sinter❂3｜Orirock Concentration❂9
		Roberta	Supporter	4	CRA-X	3-8	Semi-Synthetic Solvent❂14｜Integrated Device❂7	Paradox
		Robin	Specialist	5	TRP-Y	BI-EX-2	Incandescent Alloy Block❂8｜Aketon❂11
		Rockrock	Caster	5	FUN-X	RI-EX-4	Cutting Fluid Solution❂7｜Aketon❂14	Paradox
		Rope	Specialist	4	HOK-X	3-3	Oriron Cluster❂15｜Sugar Pack❂11	Paradox
		Rosa	Sniper	6	SIE-X	7-9	Bipolar Nanoflake❂4｜Optimized Device❂6	Paradox
		Rosmontis	Sniper	6	BOM-X	4-1	D32 Steel❂4｜Keton Colloid❂5
		Saga	Vanguard	6	SOL-X	WR-3	Bipolar Nanoflake❂4｜Incandescent Alloy Block❂6
		Saga	Vanguard	6	SOL-Y	WR-1	Bipolar Nanoflake❂4｜Incandescent Alloy Block❂6
		Saileach	Vanguard	6	BEA-X	9-2	Crystalline Electronic Unit❂4｜Refined Solvent❂6	Paradox
		Santalla	Caster	5	SPC-Y	BI-2	White Horse Kohl❂8｜Polyester Pack❂17	Paradox
		Saria	Defender	6	GUA-X	MB-3	Bipolar Nanoflake❂4｜Manganese Trihydrate❂5
		Saria	Defender	6	GUA-Y	4-6	Bipolar Nanoflake❂4｜Manganese Trihydrate❂5
		Savage	Guard	5	CEN-X	4-5	Orirock Concentration❂9｜Sugar Pack❂18	Paradox
		Scavenger	Vanguard	4	SOL-X	1-3	Loxic Kohl❂20｜Integrated Device❂6	Paradox
		Scene	Supporter	5	SUM-X	3-1	White Horse Kohl❂9｜Manganese Ore❂12	Paradox
		Schwarz	Sniper	6	ARC-X	OF-7	D32 Steel❂4｜Oriron Block❂5	Paradox
		Schwarz	Sniper	6	ARC-Y	2-1	D32 Steel❂4｜Oriron Block❂5	Paradox
		Sesa	Sniper	5	ART-X	S3-6	Grindstone Pentahydrate❂8｜Orirock Cluster❂18	Paradox
		Shamare	Supporter	5	UMD-X	6-5	Orirock Concentration❂8｜Incandescent Alloy❂17	Paradox
		Shaw	Specialist	4	PUS-Y	2-8	Integrated Device❂12｜Polyester Pack❂11	Paradox
		Shining	Medic	6	PHY-X	NL-5	Bipolar Nanoflake❂4｜Oriron Block❂5
		Shining	Medic	6	PHY-Y	5-8	Bipolar Nanoflake❂4｜Oriron Block❂5
		Shirayuki	Sniper	4	ART-Y	S2-11	Aketon❂15｜Oriron Cluster❂9	Paradox
		Shu	Defender	6	GUA-X	11-11	Crystalline Electronic Unit❂3｜Cutting Fluid Solution❂6
		Siege	Vanguard	6	SOL-X	1-7	Bipolar Nanoflake❂4｜Orirock Concentration❂6
		Siege	Vanguard	6	SOL-Y	11-2	Bipolar Nanoflake❂4｜Orirock Concentration❂6
		Silence	Medic	5	PHY-Y	4-8	Keton Colloid❂7｜Orirock Cluster❂18	Paradox
		Silence the Paradigmatic	Supporter	6	BLS-X	6-3	Crystalline Electronic Unit❂3｜RMA70-24❂6
		Skadi	Guard	6	DRE-X	GT-4	D32 Steel❂4｜Orirock Concentration❂9	Paradox
		Skadi	Guard	6	DRE-Y	GT-6	D32 Steel❂4｜Orirock Concentration❂9	Paradox
		Skyfire	Caster	5	SPC-Y	S3-5	Polyester Lump❂7｜Grindstone❂13	Paradox
		Snowsant	Specialist	5	HOK-X	4-6	Polymerized Gel❂8｜Oriron Cluster❂15	Paradox
		Specter	Guard	5	CEN-X	SN-EX-1	White Horse Kohl❂8｜Aketon❂15	Paradox
		Specter the Unchained	Specialist	6	PUM-X	SV-EX-1	Polymerization Preparation❂4｜Keton Colloid❂6
		Specter the Unchained	Specialist	6	PUM-Y	S4-1	Polymerization Preparation❂4｜Keton Colloid❂6
		Spuria	Specialist	5	GEE-X	DM-2	Crystalline Circuit❂7｜Compound Cutting Fluid❂9
		Stainless	Supporter	6	CRA-X	11-6	Polymerization Preparation❂4｜Refined Solvent❂6
		Sussurro	Medic	4	PHY-X	4-7	RMA70-12❂10｜Loxic Kohl❂13	Paradox
		Suzuran	Supporter	6	DEC-X	TW-7	D32 Steel❂4｜Grindstone Pentahydrate❂8
		Suzuran	Supporter	6	DEC-Y	DM-EX-1	D32 Steel❂4｜Grindstone Pentahydrate❂8
		Swire	Guard	5	INS-X	S2-3	Sugar Lump❂7｜Polyester Pack❂17	Paradox
		Swire the Elegant Wit	Specialist	6	MER-X	5-2	D32 Steel❂4｜White Horse Kohl❂6
		Tachanka	Guard	5	SWO-X	S2-3	RMA70-24❂7｜Coagulating Gel❂12
		Texas	Vanguard	5	SOL-Y	CB-3	Polyester Lump❂8｜Orirock Cluster❂16	Paradox
		Texas the Omertosa	Specialist	6	EXE-Y	CB-8	Bipolar Nanoflake❂4｜Oriron Block❂7
		Toddifons	Sniper	5	SIE-X	4-6	Crystalline Circuit❂10｜Incandescent Alloy❂10	Paradox
		Tomimi	Caster	5	CCR-Y	RI-2	RMA70-24❂8｜Orirock Cluster❂14	Paradox
		Totter	Sniper	4	SIE-X	S3-5	Orirock Cluster❂24｜Coagulating Gel❂6	Paradox
		Tsukinogi	Supporter	5	BLS-X	S3-6	White Horse Kohl❂8｜Grindstone❂12	Paradox
		Tuye	Medic	5	PHY-X	4-6	Keton Colloid❂7｜Loxic Kohl❂15	Paradox
		Typhon	Sniper	6	SIE-X	S2-1	Polymerization Preparation❂4｜Refined Solvent❂7
		Utage	Guard	4	MUS-X	1-12	Aketon❂14｜Orirock Cluster❂14	Paradox
		Vendela	Medic	5	INC-X	IC-2	Keton Colloid❂5｜Grindstone❂13
		Verdant	Specialist	4	PUM-Y	9-3	Oriron Cluster❂15｜Fuscous Fiber❂9
		Vermeil	Sniper	4	MAR-X	3-1	Polyester Pack❂18｜Sugar Pack❂12	Paradox
		Vigil	Vanguard	6	TAC-X	3-8	Crystalline Electronic Unit❂3｜Optimized Device❂4
		Vigna	Vanguard	4	CHG-Y	3-3	Oriron Cluster❂16｜Orirock Cluster❂11	Paradox
		Vulcan	Defender	5	UNY-X	S4-4	Orirock Concentration❂8｜Aketon❂15	Paradox
		W	Sniper	6	ART-X	DM-3	Bipolar Nanoflake❂4｜Keton Colloid❂7
		W	Sniper	6	ART-Y	DM-7	Bipolar Nanoflake❂4｜Keton Colloid❂7
		Waai Fu	Specialist	5	EXE-X	4-7	RMA70-24❂7｜Orirock Cluster❂16	Paradox
		Wanqing	Vanguard	5	BEA-X	9-13	Cutting Fluid Solution❂9｜Coagulating Gel❂9
		Warfarin	Medic	5	PHY-X	2-10	Optimized Device❂5｜Sugar Pack❂17	Paradox
		Weedy	Specialist	6	PUS-X	BI-5	D32 Steel❂4｜Manganese Trihydrate❂6	Paradox
		Weedy	Specialist	6	PUS-Y	IC-6	D32 Steel❂4｜Manganese Trihydrate❂6	Paradox
		Whislash	Guard	5	INS-X	S3-6	Keton Colloid❂7｜Coagulating Gel❂12	Paradox
		Whisperain	Medic	5	WAH-X	2-5	Orirock Concentration❂9｜Crystalline Component❂13	Paradox
		Wild Mane	Vanguard	5	CHG-X	MN-2	Cutting Fluid Solution❂8｜Aketon❂11	Paradox
		Windflit	Supporter	5	CRA-X	11-9	Keton Colloid❂7｜Grindstone❂11	Paradox
		Zima	Vanguard	5	SOL-X	1-5	Sugar Lump❂7｜RMA70-12❂11	Paradox
		Zuo Le	Guard	6	MUS-X	WB-5	Polymerization Preparation❂4｜White Horse Kohl❂8
	""");
}
