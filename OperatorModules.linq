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
		// name <tab> type (X, Y, D or empty for all)
	""")
#if DUMP_OPERATORS
	.Dump("Operators")
#endif
	;

	// TODO: Run OperatorModulesParser.linq and paste below.
	var operatorModules = new OperatorModules("""
		Aak	6	GEE-X	2-8	4❂D32 Steel｜7❂Polymerized Gel	Paradox
		Absinthe	5	CCR-X	3-4	10❂Orirock Concentration｜10❂Incandescent Alloy
		Aciddrop	4	ARC-X	2-8	10❂RMA70-12｜8❂Integrated Device	Paradox
		Akafuyu	5	MUS-X	3-8	8❂Polymerized Gel｜15❂Aketon	Paradox
		Almond	5	HOK-X	BI-2	6❂Oriron Block｜13❂Manganese Ore	Paradox
		Ambriel	4	DEA-X	4-7	16❂Oriron Cluster｜6❂RMA70-12
		Amiya	5	CCR-Y	3-8	10❂Orirock Concentration｜10❂Loxic Kohl
		Andreana	5	DEA-X	3-2	8❂Grindstone Pentahydrate｜8❂RMA70-12
		Angelina	6	DEC-X	2-7	4❂Bipolar Nanoflake｜5❂Sugar Lump	Paradox
		Angelina	6	DEC-Y	4-4	4❂Bipolar Nanoflake｜5❂Sugar Lump	Paradox
		April	5	MAR-Y	5-3	9❂Polymerized Gel｜9❂RMA70-12	Paradox
		Archetto	6	MAR-X	SV-4	4❂Polymerization Preparation｜8❂Orirock Concentration	Paradox
		Archetto	6	MAR-Y	2-9	4❂Polymerization Preparation｜8❂Orirock Concentration	Paradox
		Aroma	5	BLA-X	GT-HX-3	7❂RMA70-24｜21❂Loxic Kohl
		Ascalon	6	AMB-X	11-6	4❂D32 Steel｜6❂Keton Colloid
		Ash	6	MAR-X	SV-8	4❂D32 Steel｜6❂Polymerized Gel
		Ash	6	MAR-Y	1-12	4❂D32 Steel｜6❂Polymerized Gel
		Ashlock	5	FOR-X	7-9	8❂Polymerized Gel｜13❂Compound Cutting Fluid
		Astgenne	5	CHA-Y	4-3	7❂RMA70-24｜10❂Coagulating Gel	Paradox
		Aurora	5	HES-X	3-8	9❂White Horse Kohl｜19❂Orirock Cluster	Paradox
		Bagpipe	6	CHG-X	4-5	4❂Polymerization Preparation｜9❂Orirock Concentration	Paradox
		Bagpipe	6	CHG-Y	9-5	4❂Polymerization Preparation｜9❂Orirock Concentration	Paradox
		Bassline	5	GUA-Y	LE-4	7❂Cyclicene Prefab｜15❂Manganese Ore
		Beanstalk	4	TAC-Y	S3-7	13❂Coagulating Gel｜10❂Manganese Ore	Paradox
		Beehunter	4	FGT-Y	7-4	19❂Sugar Pack｜7❂RMA70-12	Paradox
		Beeswax	5	PLX-X	DH-6	5❂Optimized Device｜18❂Loxic Kohl	Paradox
		Bena	5	PUM-Y	2-3	6❂Oriron Block｜15❂Loxic Kohl	Paradox
		Bibeak	5	SWO-X	3-7	8❂Manganese Trihydrate｜8❂RMA70-12	Paradox
		Bison	5	PRO-X	3-7	7❂Grindstone Pentahydrate｜11❂RMA70-12	Paradox
		Blacknight	5	TAC-X	3-1	8❂Incandescent Alloy Block｜15❂Loxic Kohl	Paradox
		Blaze	6	CEN-X	11-13	4❂D32 Steel｜6❂Optimized Device	Paradox
		Blemishine	6	GUA-X	MN-4	4❂D32 Steel｜7❂RMA70-24	Paradox
		Blemishine	6	GUA-Y	MN-2	4❂D32 Steel｜7❂RMA70-24	Paradox
		Blitz	5	SPT-X	3-4	8❂Manganese Trihydrate｜8❂RMA70-12
		Blue Poison	5	MAR-Y	3-4	8❂Manganese Trihydrate｜8❂Integrated Device	Paradox
		Breeze	5	RIN-X	4-1	5❂Optimized Device｜18❂Loxic Kohl	Paradox
		Broca	5	CEN-X	7-8	7❂Sugar Lump｜13❂Grindstone	Paradox
		Bryophyta	5	INS-X	9-3	7❂Refined Solvent｜15❂Aketon
		Bubble	4	PRO-X	4-6	16❂Coagulating Gel｜8❂Loxic Kohl
		Carnelian	6	PLX-X	7-3	4❂D32 Steel｜7❂RMA70-24	Paradox
		Carnelian	6	PLX-Y	4-6	4❂D32 Steel｜7❂RMA70-24	Paradox
		Cement	5	HES-X	IW-EX-6	7❂Manganese Trihydrate｜13❂Grindstone
		Ceobe	6	CCR-X	RI-6	4❂Bipolar Nanoflake｜5❂Incandescent Alloy Block	Paradox
		Ceobe	6	CCR-Y	11-7	4❂Bipolar Nanoflake｜5❂Incandescent Alloy Block	Paradox
		Ceylon	5	WAH-X	OF-3	7❂Oriron Block｜10❂Aketon	Paradox
		Ch'en	6	SWO-X	5-8	4❂Polymerization Preparation｜6❂White Horse Kohl	Paradox
		Ch'en	6	SWO-Y	5-9	4❂Polymerization Preparation｜6❂White Horse Kohl	Paradox
		Chiave	5	SOL-Y	1-1	7❂Manganese Trihydrate｜13❂Grindstone	Paradox
		Chongyue	6	FGT-X	WB-7	4❂Polymerization Preparation｜5❂Incandescent Alloy
		Click	4	FUN-Y	11-7	15❂Manganese Ore｜9❂Oriron Cluster	Paradox
		Cliffheart	5	HOK-X	MN-5	6❂Oriron Block｜13❂Manganese Ore	Paradox
		Conviction	4	DRE-X	2-4	11❂Integrated Device｜9❂Coagulating Gel
		Corroserum	5	BLA-X	BI-6	5❂Optimized Device｜10❂RMA70-12	Paradox
		Courier	4	SOL-X	S2-2	11❂Integrated Device｜10❂Aketon
		Croissant	5	PRO-Y	2-8	8❂RMA70-24｜8❂Integrated Device	Paradox
		Cuora	4	PRO-X	2-10	14❂Grindstone｜10❂Loxic Kohl	Paradox
		Cutter	4	SWO-Y	2-7	12❂Coagulating Gel｜11❂Manganese Ore	Paradox
		Dagda	5	FGT-Y	11-5	7❂Incandescent Alloy Block｜21❂Orirock Cluster	Paradox
		Deepcolor	4	SUM-Y	2-4	20❂Orirock Cluster｜9❂Manganese Ore	Paradox
		Degenbrecher	6	SWO-X	BI-6	3❂Nucleic Crystal Sinter｜7❂Grindstone Pentahydrate
		Delphine	5	MSC-Y	WD-6	8❂Cyclicene Prefab｜10❂Semi-Synthetic Solvent
		Dobermann	4	INS-Y	1-12	15❂Manganese Ore｜11❂Loxic Kohl
		Doc	5	INS-Y	2-5	8❂Polymerized Gel｜11❂Integrated Device
		Dorothy	6	TRP-Y	4-5	4❂Polymerization Preparation｜8❂Cutting Fluid Solution	Paradox
		Dusk	6	SPC-X	1-6	4❂Crystalline Electronic Unit｜6❂Manganese Trihydrate	Paradox
		Dusk	6	SPC-Y	4-3	4❂Crystalline Electronic Unit｜6❂Manganese Trihydrate	Paradox
		Earthspirit	4	DEC-Y	DM-2	20❂Sugar Pack｜7❂Grindstone	Paradox
		Ebenholz	6	MSC-X	3-7	4❂D32 Steel｜7❂Refined Solvent	Paradox
		Ebenholz	6	MSC-Y	3-6	4❂D32 Steel｜7❂Refined Solvent	Paradox
		Ebenholz	6	MSC-Δ	11-12	4❂D32 Steel｜7❂Refined Solvent	Paradox
		Elysium	5	BEA-X	S3-6	7❂Incandescent Alloy Block｜16❂Aketon	Paradox
		Enforcer	5	PUS-X	GA-EX-1	8❂Polymerized Gel｜12❂Compound Cutting Fluid	Paradox
		Erato	5	SIE-X	S5-9	5❂Optimized Device｜11❂RMA70-12	Paradox
		Estelle	4	CEN-Y	7-4	12❂RMA70-12｜8❂Grindstone	Paradox
		Ethan	4	AMB-X	TW-3	17❂Sugar Pack｜14❂Orirock Cluster	Paradox
		Eunectes	6	HES-X	RI-EX-4	4❂Bipolar Nanoflake｜7❂Polymerized Gel
		Eunectes	6	HES-Y	RI-7	4❂Bipolar Nanoflake｜7❂Polymerized Gel
		Executor the Ex Foedere	6	REA-X	7-14	3❂Nucleic Crystal Sinter｜7❂Polymerized Gel	Paradox
		Exusiai	6	MAR-X	4-10	4❂Polymerization Preparation｜5❂Sugar Lump	Paradox
		Eyjafjalla	6	CCR-X	OF-5	4❂Polymerization Preparation｜5❂Optimized Device	Paradox
		Fartooth	6	DEA-X	S2-2	4❂D32 Steel｜7❂Cutting Fluid Solution
		Fartooth	6	DEA-Y	2-5	4❂D32 Steel｜7❂Cutting Fluid Solution
		FEater	5	PUS-Y	WR-7	8❂Grindstone Pentahydrate｜15❂Polyester Pack	Paradox
		Fiammetta	6	ART-X	GA-4	3❂Crystalline Electronic Unit｜6❂Grindstone Pentahydrate	Paradox
		Fiammetta	6	ART-Y	S5-9	3❂Crystalline Electronic Unit｜6❂Grindstone Pentahydrate	Paradox
		Firewatch	5	DEA-Y	2-10	7❂Polyester Lump｜15❂Loxic Kohl	Paradox
		Firewhistle	5	FOR-X	S4-6	7❂Grindstone Pentahydrate｜13❂Coagulating Gel	Paradox
		Flamebringer	5	DRE-Y	S4-3	9❂White Horse Kohl｜13❂Manganese Ore
		Flametail	6	SOL-X	NL-5	4❂Bipolar Nanoflake｜9❂Orirock Concentration
		Flametail	6	SOL-Y	NL-3	4❂Bipolar Nanoflake｜9❂Orirock Concentration
		Flint	5	FGT-Y	RI-9	8❂Orirock Concentration｜15❂Grindstone	Paradox
		Folinic	5	PHY-Y	5-2	8❂Keton Colloid｜8❂Integrated Device
		Franka	5	DRE-X	9-3	6❂Oriron Block｜18❂Sugar Pack	Paradox
		Frost	5	TRP-Y	4-4	8❂Grindstone Pentahydrate｜17❂Orirock Cluster
		Fuze	5	CEN-Y	3-1	8❂Incandescent Alloy Block｜16❂Sugar Pack
		Gavial	4	PHY-Y	S4-5	13❂Integrated Device｜6❂Oriron Cluster
		Gavial the Invincible	6	CEN-X	9-2	4❂Bipolar Nanoflake｜6❂Polymerized Gel	Paradox
		Gitano	4	SPC-X	1-10	17❂Sugar Pack｜14❂Orirock Cluster	Paradox
		Gladiia	6	HOK-X	SV-EX-5	4❂Crystalline Electronic Unit｜6❂Polymerized Gel	Paradox
		Glaucus	5	DEC-X	5-7	7❂Keton Colloid｜10❂Integrated Device	Paradox
		Gnosis	6	UMD-X	BI-7	3❂Crystalline Electronic Unit｜7❂Incandescent Alloy Block
		Goldenglow	6	FUN-X	R8-8	4❂Bipolar Nanoflake｜5❂Manganese Trihydrate
		Grain Buds	5	DEC-X	9-11	7❂Crystalline Circuit｜11❂Compound Cutting Fluid
		Grani	5	CHG-X	GT-6	7❂RMA70-24｜13❂Oriron Cluster	Paradox
		Gravel	4	EXE-X	4-8	18❂Polyester Pack｜13❂Orirock Cluster	Paradox
		GreyThroat	5	MAR-Y	2-10	7❂Oriron Block｜9❂Integrated Device
		Greyy	4	SPC-Y	2-3	15❂Manganese Ore｜9❂Aketon	Paradox
		Greyy the Lightningbearer	5	BOM-X	1-3	7❂Grindstone Pentahydrate｜16❂Incandescent Alloy	Paradox
		Gummy	4	GUA-X	4-2	13❂RMA70-12｜7❂Manganese Ore	Paradox
		Harmonie	5	MSC-X	6-5	6❂RMA70-24｜15❂Oriron Cluster
		Haze	4	CCR-X	S2-4	19❂Orirock Cluster｜8❂RMA70-12	Paradox
		Heavyrain	5	PRO-Y	2-7	9❂Orirock Concentration｜14❂Oriron Cluster	Paradox
		Hellagur	6	MUS-X	5-10	4❂Bipolar Nanoflake｜7❂Polyester Lump	Paradox
		Hellagur	6	MUS-Y	7-10	4❂Bipolar Nanoflake｜7❂Polyester Lump	Paradox
		Hibiscus the Purifier	5	INC-X	LE-4	9❂White Horse Kohl｜18❂Orirock Cluster	Paradox
		Highmore	5	REA-X	SN-2	9❂Orirock Concentration｜13❂Semi-Synthetic Solvent
		Ho'olheyak	6	CCR-X	OF-7	4❂D32 Steel｜7❂Transmuted Salt Agglomerate
		Ho'olheyak	6	CCR-Y	4-3	4❂D32 Steel｜7❂Transmuted Salt Agglomerate
		Horn	6	FOR-X	7-15	4❂D32 Steel｜7❂Oriron Block	Paradox
		Hoshiguma	6	PRO-X	6-4	4❂Polymerization Preparation｜5❂Grindstone Pentahydrate
		Hoshiguma	6	PRO-Y	3-1	4❂Polymerization Preparation｜5❂Grindstone Pentahydrate
		Humus	4	REA-X	7-8	11❂RMA70-12｜12❂Sugar Pack	Paradox
		Hung	5	GUA-Y	S2-4	7❂Incandescent Alloy Block｜15❂Aketon	Paradox
		Iana	5	PUM-X	3-7	7❂Transmuted Salt Agglomerate｜12❂Aggregate Cyclicene
		Ifrit	6	BLA-X	1-5	4❂D32 Steel｜7❂Polyester Lump	Paradox
		Indigo	4	MSC-X	SV-5	14❂Oriron Cluster｜7❂RMA70-12	Paradox
		Indra	5	FGT-Y	11-5	7❂Keton Colloid｜16❂Polyester Pack	Paradox
		Insider	5	MAR-X	3-4	9❂Polymerized Gel｜15❂Sugar Pack
		Irene	6	SWO-Y	SV-4	4❂Bipolar Nanoflake｜7❂RMA70-24
		Iris	5	MSC-X	2-2	6❂Oriron Block｜11❂Integrated Device	Paradox
		Istina	5	DEC-Y	2-3	5❂Optimized Device｜9❂RMA70-12	Paradox
		Jackie	4	FGT-X	CB-4	19❂Orirock Cluster｜12❂Loxic Kohl	Paradox
		Jaye	4	MER-X	S3-2	14❂Grindstone｜8❂Aketon	Paradox
		Jessica	4	MAR-Y	SV-4	20❂Loxic Kohl｜7❂Oriron Cluster	Paradox
		Jessica the Liberated	6	SPT-X	3-8	4❂Crystalline Electronic Unit｜4❂Optimized Device
		Jieyun	5	ART-Y	S2-2	7❂Keton Colloid｜11❂RMA70-12
		Kafka	5	EXE-Y	2-7	8❂Polymerized Gel｜15❂Oriron Cluster	Paradox
		Kal'tsit	6	PHY-X	5-10	4❂Crystalline Electronic Unit｜4❂Optimized Device
		Kal'tsit	6	PHY-Y	5-10	4❂Crystalline Electronic Unit｜4❂Optimized Device
		Kazemaru	5	PUM-X	3-1	9❂Orirock Concentration｜13❂Semi-Synthetic Solvent	Paradox
		Kirara	5	AMB-X	3-1	7❂Incandescent Alloy Block｜11❂Integrated Device	Paradox
		Kirin R Yato	6	EXE-X	3-7	3❂Nucleic Crystal Sinter｜6❂Keton Colloid
		Kjera	5	FUN-Y	BI-7	8❂Grindstone Pentahydrate｜13❂Incandescent Alloy
		Kroos the Keen Glint	5	MAR-X	3-8	7❂Crystalline Circuit｜10❂Oriron Cluster
		La Pluma	5	REA-X	11-6	7❂Keton Colloid｜13❂Manganese Ore	Paradox
		Lava the Purgatory	5	SPC-X	WR-4	8❂White Horse Kohl｜13❂Grindstone
		Lee	6	MER-X	3-1	4❂Polymerization Preparation｜9❂White Horse Kohl	Paradox
		Lee	6	MER-Y	IW-EX-1	4❂Polymerization Preparation｜9❂White Horse Kohl	Paradox
		Leizi	5	CHA-X	S3-6	7❂RMA70-24｜13❂Coagulating Gel	Paradox
		Leonhardt	5	SPC-X	3-5	7❂Keton Colloid｜15❂Loxic Kohl	Paradox
		Lessing	6	DRE-X	7-11	4❂Crystalline Electronic Unit｜5❂Cyclicene Prefab
		Lin	6	PLX-X	11-6	4❂D32 Steel｜8❂Cutting Fluid Solution
		Ling	6	SUM-Y	3-4	4❂D32 Steel｜5❂Crystalline Circuit	Paradox
		Liskarm	5	SPT-X	4-6	7❂Grindstone Pentahydrate｜15❂Aketon	Paradox
		Lumen	6	WAH-X	6-9	4❂Crystalline Electronic Unit｜5❂Optimized Device	Paradox
		Lumen	6	WAH-Y	OF-5	4❂Crystalline Electronic Unit｜5❂Optimized Device	Paradox
		Lunacub	5	DEA-Y	2-5	8❂Refined Solvent｜13❂Aketon
		Lutonada	4	UNY-X	3-1	14❂Semi-Synthetic Solvent｜7❂Integrated Device
		Magallan	6	SUM-X	2-5	4❂Polymerization Preparation｜6❂Manganese Trihydrate	Paradox
		Magallan	6	SUM-Y	2-2	4❂Polymerization Preparation｜6❂Manganese Trihydrate	Paradox
		Manticore	5	AMB-X	1-9	8❂Manganese Trihydrate｜12❂Sugar Pack	Paradox
		Matoimaru	4	DRE-Y	S2-10	16❂Aketon｜10❂Sugar Pack
		Matterhorn	4	PRO-Y	3-6	14❂Manganese Ore｜7❂Integrated Device	Paradox
		May	4	MAR-X	4-4	14❂Oriron Cluster｜12❂Polyester Pack	Paradox
		Mayer	5	SUM-X	2-9	6❂Oriron Block｜11❂RMA70-12	Paradox
		Melanite	5	ARC-Y	S3-5	8❂Refined Solvent｜15❂Loxic Kohl
		Meteor	4	MAR-X	2-7	14❂Oriron Cluster｜12❂Polyester Pack	Paradox
		Meteorite	5	ART-Y	2-4	7❂RMA70-24｜14❂Manganese Ore	Paradox
		Minimalist	5	FUN-Y	IC-8	6❂Crystalline Circuit｜12❂Oriron Cluster	Paradox
		Mint	5	PLX-X	BI-2	9❂Incandescent Alloy Block｜14❂Orirock Cluster	Paradox
		Mizuki	6	AMB-X	4-3	4❂Polymerization Preparation｜6❂Crystalline Circuit	Paradox
		Mizuki	6	AMB-Y	4-5	4❂Polymerization Preparation｜6❂Crystalline Circuit	Paradox
		Morgan	5	DRE-X	4-5	8❂Cutting Fluid Solution｜11❂Transmuted Salt	Paradox
		Mostima	6	SPC-Y	CB-5	4❂Bipolar Nanoflake｜7❂Grindstone Pentahydrate	Paradox
		Mountain	6	FGT-Y	MB-EX-3	4❂Crystalline Electronic Unit｜8❂Polymerized Gel	Paradox
		Mr. Nothing	5	MER-X	3-7	6❂Optimized Device｜10❂Manganese Ore	Paradox
		Mudrock	6	UNY-X	11-7	4❂Crystalline Electronic Unit｜5❂Incandescent Alloy Block
		Muelsyse	6	TAC-X	3-4	4❂Bipolar Nanoflake｜4❂Grindstone Pentahydrate	Paradox
		Myrrh	4	PHY-X	6-11	14❂Aketon｜12❂Polyester Pack	Paradox
		Myrtle	4	BEA-X	6-3	12❂Grindstone｜8❂Integrated Device	Paradox
		Nearl	5	GUA-X	1-12	9❂White Horse Kohl｜16❂Polyester Pack
		Nearl the Radiant Knight	6	DRE-X	MN-8	4❂Polymerization Preparation｜8❂Polymerized Gel	Paradox
		Nearl the Radiant Knight	6	DRE-Y	NL-10	4❂Polymerization Preparation｜8❂Polymerized Gel	Paradox
		Nian	6	PRO-X	WR-9	4❂Polymerization Preparation｜7❂Incandescent Alloy Block	Paradox
		Nian	6	PRO-Y	S6-2	4❂Polymerization Preparation｜7❂Incandescent Alloy Block	Paradox
		Nightingale	6	RIN-X	3-6	4❂D32 Steel｜6❂Keton Colloid	Paradox
		Nightmare	5	CCR-Y	S4-1	7❂Sugar Lump｜14❂Manganese Ore
		Nine-Colored Deer	5	BLS-X	IW-3	7❂Manganese Trihydrate｜14❂Crystalline Component
		Pallas	6	INS-X	3-6	4❂Crystalline Electronic Unit｜6❂White Horse Kohl
		Pallas	6	INS-Y	4-3	4❂Crystalline Electronic Unit｜6❂White Horse Kohl
		Passenger	6	CHA-X	2-2	4❂Bipolar Nanoflake｜5❂Oriron Block
		Passenger	6	CHA-Y	5-10	4❂Bipolar Nanoflake｜5❂Oriron Block
		Penance	6	UNY-X	CB-4	4❂D32 Steel｜8❂White Horse Kohl
		Perfumer	4	RIN-Y	3-4	19❂Loxic Kohl｜8❂Aketon	Paradox
		Phantom	6	EXE-X	3-6	4❂Polymerization Preparation｜9❂Polymerized Gel	Paradox
		Phantom	6	EXE-Y	DM-5	4❂Polymerization Preparation｜9❂Polymerized Gel	Paradox
		Platinum	5	MAR-X	5-7	8❂Grindstone Pentahydrate｜15❂Loxic Kohl	Paradox
		Podenco	4	DEC-X	4-6	15❂Incandescent Alloy｜5❂Grindstone	Paradox
		Poncirus	5	SOL-X	OF-5	8❂Incandescent Alloy Block｜18❂Orirock Cluster
		Pozëmka	6	ARC-Y	2-10	3❂Crystalline Electronic Unit｜9❂Orirock Concentration	Paradox
		Pramanix	5	UMD-X	2-3	7❂Keton Colloid｜11❂Grindstone	Paradox
		Projekt Red	5	EXE-Y	S4-5	7❂Manganese Trihydrate｜14❂Oriron Cluster
		Provence	5	ARC-X	S5-1	9❂Sugar Lump｜7❂Integrated Device	Paradox
		Proviso	5	DEC-X	4-7	7❂Oriron Block｜12❂Manganese Ore	Paradox
		Ptilopsis	5	RIN-X	4-9	9❂Orirock Concentration｜10❂Grindstone	Paradox
		Pudding	4	CHA-Y	2-10	11❂RMA70-12｜3❂Cutting Fluid Solution	Paradox
		Purestream	4	WAH-Y	3-7	11❂Integrated Device｜9❂Coagulating Gel	Paradox
		Qanipalaat	5	CCR-Y	2-3	7❂Oriron Block｜12❂Manganese Ore	Paradox
		Quercus	5	BLS-X	9-2	6❂Oriron Block｜13❂Manganese Ore	Paradox
		Rathalos S Noir Corne	5	MUS-X	3-8	8❂Transmuted Salt Agglomerate｜11❂Crystalline Component
		Reed	5	CHG-Y	9-5	9❂Orirock Concentration｜12❂Manganese Ore	Paradox
		Reed the Flame Shadow	6	INC-X	11-6	3❂Nucleic Crystal Sinter｜9❂Orirock Concentration
		Roberta	4	CRA-X	3-8	14❂Semi-Synthetic Solvent｜7❂Integrated Device	Paradox
		Robin	5	TRP-Y	BI-EX-2	8❂Incandescent Alloy Block｜11❂Aketon
		Rockrock	5	FUN-X	RI-EX-4	7❂Cutting Fluid Solution｜14❂Aketon	Paradox
		Rope	4	HOK-X	3-3	15❂Oriron Cluster｜11❂Sugar Pack	Paradox
		Rosa	6	SIE-X	7-9	4❂Bipolar Nanoflake｜6❂Optimized Device
		Rosmontis	6	BOM-X	4-1	4❂D32 Steel｜5❂Keton Colloid
		Saga	6	SOL-X	WR-3	4❂Bipolar Nanoflake｜6❂Incandescent Alloy Block
		Saga	6	SOL-Y	WR-1	4❂Bipolar Nanoflake｜6❂Incandescent Alloy Block
		Saileach	6	BEA-X	9-2	4❂Crystalline Electronic Unit｜6❂Refined Solvent	Paradox
		Santalla	5	SPC-Y	BI-2	8❂White Horse Kohl｜17❂Polyester Pack
		Saria	6	GUA-X	MB-3	4❂Bipolar Nanoflake｜5❂Manganese Trihydrate
		Saria	6	GUA-Y	4-6	4❂Bipolar Nanoflake｜5❂Manganese Trihydrate
		Savage	5	CEN-X	4-5	9❂Orirock Concentration｜18❂Sugar Pack	Paradox
		Scavenger	4	SOL-X	1-3	20❂Loxic Kohl｜6❂Integrated Device	Paradox
		Scene	5	SUM-X	3-1	9❂White Horse Kohl｜12❂Manganese Ore	Paradox
		Schwarz	6	ARC-X	OF-7	4❂D32 Steel｜5❂Oriron Block	Paradox
		Schwarz	6	ARC-Y	2-1	4❂D32 Steel｜5❂Oriron Block	Paradox
		Sesa	5	ART-X	S3-6	8❂Grindstone Pentahydrate｜18❂Orirock Cluster	Paradox
		Shamare	5	UMD-X	6-5	8❂Orirock Concentration｜17❂Incandescent Alloy	Paradox
		Shaw	4	PUS-Y	2-8	12❂Integrated Device｜11❂Polyester Pack	Paradox
		Shining	6	PHY-X	NL-5	4❂Bipolar Nanoflake｜5❂Oriron Block
		Shining	6	PHY-Y	5-8	4❂Bipolar Nanoflake｜5❂Oriron Block
		Shirayuki	4	ART-Y	S2-11	15❂Aketon｜9❂Oriron Cluster	Paradox
		Shu	6	GUA-X	11-11	3❂Crystalline Electronic Unit｜6❂Cutting Fluid Solution
		Siege	6	SOL-X	1-7	4❂Bipolar Nanoflake｜6❂Orirock Concentration
		Siege	6	SOL-Y	11-2	4❂Bipolar Nanoflake｜6❂Orirock Concentration
		Silence	5	PHY-Y	4-8	7❂Keton Colloid｜18❂Orirock Cluster	Paradox
		Silence the Paradigmatic	6	BLS-X	6-3	3❂Crystalline Electronic Unit｜6❂RMA70-24
		Skadi	6	DRE-X	GT-4	4❂D32 Steel｜9❂Orirock Concentration	Paradox
		Skadi	6	DRE-Y	GT-6	4❂D32 Steel｜9❂Orirock Concentration	Paradox
		Skyfire	5	SPC-Y	S3-5	7❂Polyester Lump｜13❂Grindstone	Paradox
		Snowsant	5	HOK-X	4-6	8❂Polymerized Gel｜15❂Oriron Cluster	Paradox
		Specter	5	CEN-X	SN-EX-1	8❂White Horse Kohl｜15❂Aketon	Paradox
		Specter the Unchained	6	PUM-X	SV-EX-1	4❂Polymerization Preparation｜6❂Keton Colloid
		Specter the Unchained	6	PUM-Y	S4-1	4❂Polymerization Preparation｜6❂Keton Colloid
		Spuria	5	GEE-X	DM-2	7❂Crystalline Circuit｜9❂Compound Cutting Fluid
		Stainless	6	CRA-X	11-6	4❂Polymerization Preparation｜6❂Refined Solvent
		Sussurro	4	PHY-X	4-7	10❂RMA70-12｜13❂Loxic Kohl	Paradox
		Suzuran	6	DEC-X	TW-7	4❂D32 Steel｜8❂Grindstone Pentahydrate
		Suzuran	6	DEC-Y	DM-EX-1	4❂D32 Steel｜8❂Grindstone Pentahydrate
		Swire	5	INS-X	S2-3	7❂Sugar Lump｜17❂Polyester Pack	Paradox
		Swire the Elegant Wit	6	MER-X	5-2	4❂D32 Steel｜6❂White Horse Kohl
		Tachanka	5	SWO-X	S2-3	7❂RMA70-24｜12❂Coagulating Gel
		Texas	5	SOL-Y	CB-3	8❂Polyester Lump｜16❂Orirock Cluster	Paradox
		Texas the Omertosa	6	EXE-Y	CB-8	4❂Bipolar Nanoflake｜7❂Oriron Block
		Toddifons	5	SIE-X	4-6	10❂Crystalline Circuit｜10❂Incandescent Alloy	Paradox
		Tomimi	5	CCR-Y	RI-2	8❂RMA70-24｜14❂Orirock Cluster	Paradox
		Totter	4	SIE-X	S3-5	24❂Orirock Cluster｜6❂Coagulating Gel	Paradox
		Tsukinogi	5	BLS-X	S3-6	8❂White Horse Kohl｜12❂Grindstone	Paradox
		Tuye	5	PHY-X	4-6	7❂Keton Colloid｜15❂Loxic Kohl	Paradox
		Typhon	6	SIE-X	S2-1	4❂Polymerization Preparation｜7❂Refined Solvent
		Utage	4	MUS-X	1-12	14❂Aketon｜14❂Orirock Cluster	Paradox
		Vendela	5	INC-X	IC-2	5❂Keton Colloid｜13❂Grindstone
		Verdant	4	PUM-Y	9-3	15❂Oriron Cluster｜9❂Fuscous Fiber
		Vermeil	4	MAR-X	3-1	18❂Polyester Pack｜12❂Sugar Pack	Paradox
		Vigil	6	TAC-X	3-8	3❂Crystalline Electronic Unit｜4❂Optimized Device
		Vigna	4	CHG-Y	3-3	16❂Oriron Cluster｜11❂Orirock Cluster	Paradox
		Vulcan	5	UNY-X	S4-4	8❂Orirock Concentration｜15❂Aketon	Paradox
		W	6	ART-X	DM-3	4❂Bipolar Nanoflake｜7❂Keton Colloid
		W	6	ART-Y	DM-7	4❂Bipolar Nanoflake｜7❂Keton Colloid
		Waai Fu	5	EXE-X	4-7	7❂RMA70-24｜16❂Orirock Cluster	Paradox
		Wanqing	5	BEA-X	9-13	9❂Cutting Fluid Solution｜9❂Coagulating Gel
		Warfarin	5	PHY-X	2-10	5❂Optimized Device｜17❂Sugar Pack	Paradox
		Weedy	6	PUS-X	BI-5	4❂D32 Steel｜6❂Manganese Trihydrate	Paradox
		Weedy	6	PUS-Y	IC-6	4❂D32 Steel｜6❂Manganese Trihydrate	Paradox
		Whislash	5	INS-X	S3-6	7❂Keton Colloid｜12❂Coagulating Gel	Paradox
		Whisperain	5	WAH-X	2-5	9❂Orirock Concentration｜13❂Crystalline Component	Paradox
		Wild Mane	5	CHG-X	MN-2	8❂Cutting Fluid Solution｜11❂Aketon	Paradox
		Windflit	5	CRA-X	11-9	7❂Keton Colloid｜11❂Grindstone	Paradox
		Zima	5	SOL-X	1-5	7❂Sugar Lump｜11❂RMA70-12	Paradox
		Zuo Le	6	MUS-X	WB-5	4❂Polymerization Preparation｜8❂White Horse Kohl
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
			ID          = i + 1,
			Stage       = new WikiHyperlinq(v.Stage, "Information"),
			Operators   = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Operator))),
			v.Stars,
			Module      = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Operator, v.Module, v.Module))),
			v.Total,
			Paradox     = VerticalRun(v.Operators.Select(static v => v.Paradox ? new Hyperlinq($"https://www.youtube.com/results?search_query={Uri.EscapeDataString(v.Operator)}+paradox+simulation", "YouTube") : (object)Empty)),
			E2Materials = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Operator, "Promotion", v.E2Materials)))
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
