<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
</Query>

// Arknights operators modules parser.

#nullable enable

#load "./lib/Clipboard.linq"
#load "./lib/Extensions.linq"
#load "./lib/Operators.linq"
#load "./lib/Parsable.linq"

const StringSplitOptions StringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

using var httpClient = new HttpClient().Configure();

var wikiModules = await httpClient.GetStringAsync(GetUrl("Operator_Module"));

const char Endl  = '\n';
const char Comma = ',';

var regexTimeout = TimeSpan.FromMilliseconds(100);

var operators = "xyd".ToCharArray().Select(static m => $"{m} operator").ToArray();

var operatorModules =
	(await Task.WhenAll(
		wikiModules
			.Split("|")
			.Where( s => operators.Any(o => s.StartsWith(o)))
			.Select(s => operators.Aggregate(s, static (s, o) => s.Replace(o, string.Empty)).TrimStart().TrimStart('='))
			.SelectMany(static s => s.Split(Comma, StringSplitOptions))
			.Distinct()
			.OrderBy()
			.Select(GetOperator)
		)
	)
	.Where( static op => op.Modules.Any())
	.Select(static op => op.ToString());

await operatorModules.SetClipboard();

"Operator modules have been copied to clipboard.".Dump();

Uri GetUrl(string uri) =>
	new($"https://arknights.wiki.gg/wiki/{uri}?action=raw");

async Task<OperatorWithModules> GetOperator(string name)
{
	var url = GetUrl(GetOperatorUri(" (Medic)").Replace(' ', '_'));

	const string Title = "|title =";

	var wiki = await httpClient.GetStringAsync(url);

	var e2Materials = string.Join(OperatorData.MaterialSeparator, Regex.Matches(wiki, @"\|e2\s+m[2-9]\s*=\s*([^\r\n}]+)", RegexOptions.None, regexTimeout)
		.Select(static match => match.Groups)
		.Select(static group => string.Join(OperatorData.CountSeparator, group[1].Value.Split(',', StringSplitOptions)))
	);

	if(string.IsNullOrEmpty(e2Materials))
	{
		var noMaterial = $"Missing promotion material{OperatorData.CountSeparator}1";
		e2Materials = $"{noMaterial}{OperatorData.MaterialSeparator}{noMaterial}";
	}

	var wikiModules = wiki.Split("==Operator Modules==", StringSplitOptions).Last().Split("==").First();

	var titles = wikiModules.Split(Endl, StringSplitOptions)
		.Where( static s => s.StartsWith(Title))
		.Skip(1) // Original
		.Select(static s => s.Replace(Title, string.Empty).Trim());

	var missions = wikiModules.Split(Endl, StringSplitOptions)
		.Where( static s => s.StartsWith("|mission2"))
		.Select(s => Regex.Match(s, @"\[\[([^]]+-[^]]+)\]\]", RegexOptions.None, regexTimeout).Groups[1].Value.Split('|').Last());

	return new(
		name,
		Regex.Match(wiki, @"\|class\s*=\s*([^\r\n]+)", RegexOptions.None, regexTimeout).Groups[1].Value,
		Regex.Match(wiki, @"\|rarity\s*=\s*(\d+)", RegexOptions.None, regexTimeout).Groups[1].Value,
		e2Materials,
		Regex.Match(wiki, @"\|simulation", RegexOptions.None, regexTimeout).Success,
		Enumerable.Zip(titles, missions, static (t, m) => new Module(t, m)).ToArray()
	);

	string GetOperatorUri(string operatorTag)
	{
		var index = name.IndexOf(operatorTag);
		return index < 0
			? name
			: $"{name.Substring(0, index)}/{operatorTag.Trim(" ()".ToCharArray())}";
	}
}

sealed record OperatorWithModules(string Name, string Class, string Stars, string E2Materials, bool Paradox, IEnumerable<Module> Modules)
{
	public override string ToString() =>
		string.Join(Environment.NewLine, Modules.Select(m => $"{Name}\t{Class}\t{Stars}\t{m.Name}\t{m.Mission}\t{E2Materials}{(Paradox ? $"\t{nameof(Paradox)}" : string.Empty)}"));
}

sealed record Module(string Name, string Mission);
