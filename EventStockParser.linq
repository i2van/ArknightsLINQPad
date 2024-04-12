<Query Kind="Statements">
  <Namespace>System.Windows.Forms</Namespace>
</Query>

// Arknights event stock parser.

#nullable enable

// Get browser extension at https://merribithouse.net/copytables/
// TODO: Go to Arknights event stock table at https://arknights.wiki.gg/wiki/Event
// TODO: Table... > Copy... > As is
const string stock = """
Ware	Price	Stock
Coldshot's Token
1
200 Shoddy Fuel.png	1
Coldshot's Token
1
240 Shoddy Fuel.png	1
Coldshot's Token
1
280 Shoddy Fuel.png	1
Coldshot's Token
1
320 Shoddy Fuel.png	1
Coldshot's Token
1
360 Shoddy Fuel.png	1
Headhunting Permit
1
150 Shoddy Fuel.png	3
Module Data Block
2
75 Shoddy Fuel.png	2
Bipolar Nanoflake
1
100 Shoddy Fuel.png	5
Oriron Block
1
40 Shoddy Fuel.png	10
Grindstone Pentahydrate
1
35 Shoddy Fuel.png	10
Orirock Concentration
1
25 Shoddy Fuel.png	10
Advance Patchwork.png	50 Shoddy Fuel.png	1
Assembly Workbench.png	50 Shoddy Fuel.png	1
Shock-proof Pillar.png	30 Shoddy Fuel.png	1
Explosion-proof Fluorescent Lamp.png	25 Shoddy Fuel.png	1
Reinforced Work Chair.png	25 Shoddy Fuel.png	1
Data Supplement Instrument
1
15 Shoddy Fuel.png	10
Data Supplement Stick
1
5 Shoddy Fuel.png	60
Semi-Synthetic Solvent
1
12 Shoddy Fuel.png	10
Loxic Kohl
1
8 Shoddy Fuel.png	15
LMD
5K
7 Shoddy Fuel.png	100
Strategic Battle Record
2
5 Shoddy Fuel.png	25
Tactical Battle Record
2
3 Shoddy Fuel.png	50
Frontline Battle Record
2
1 Shoddy Fuel.png	150
Skill Summary - 3
1
4 Shoddy Fuel.png	25
Skill Summary - 2
1
2 Shoddy Fuel.png	50
Device
1
4 Shoddy Fuel.png	25
Oriron
1
3 Shoddy Fuel.png	25
Orirock Cube
1
2 Shoddy Fuel.png	25
Sniper Chip
1
6 Shoddy Fuel.png	5
Furniture Part
10
2 Shoddy Fuel.png	200
LMD
20
1 Shoddy Fuel.png	Unlimited
""";

Clipboard.SetText(
	$@"[new(""EVENT_URI#EVENT_STOCK"", ""EVENT_NAME"", ""EVENT_CURRENCY"")] = new(""""""{Environment.NewLine}// EVENT_NAME{Environment.NewLine}" +
	string.Join(Environment.NewLine,
		GetResources()
			.Select(static r => Regex.Replace( r, @"^(.+)\t(.+)\t(.+)$", "$2\t$3\t$1"))
			.Select(static r => Regex.Replace( r, @"\t1\t", "\t\t"))
	) +
	$@"{Environment.NewLine}"""""")");

"Event stock has been copied to clipboard.".Dump();

IEnumerable<string> GetResources()
{
	var resources = stock
		.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		.Skip(1) // Table header.
		.Where( static r => !Regex.IsMatch(r, @"\s+Unlimited"))
		.Where( static r => !Regex.IsMatch(r, @"^\d+K?$"))
		.Select(static r => Regex.Replace( r, @"\.png\s+Automatically\s+exchanged\s+for\s+Intelligence\s+Certificate\s*$", string.Empty))
		.Select(static r => Regex.Replace( r, @"(\d)\s+[^\d]+\.png\s+(\d)", "$1\t$2"))
		.Select(static r => Regex.Replace( r, @"\.png", string.Empty))
		.Select(static r => Regex.Replace( r, @"[”“]", @""""))
		.Select(static r => r.Trim())
		.ToArray();

	var digits = resources
		.Where(StartsWithDigit)
		.ToArray();

	var i = 0;

	return resources
			.Where(static r => !StartsWithDigit(r))
			.TakeWhile(_ => i < digits.Length)
			.Select(r => Regex.IsMatch(r, @"^.+\s+\d+\s+\d+$") ? r : $"{r}\t{digits[i++]}");

	static bool StartsWithDigit(string r) =>
		r[0] is >= '0' and <= '9';
}
