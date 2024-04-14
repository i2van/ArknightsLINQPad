<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

// Arknights operators modules parser.

const StringSplitOptions StringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

using var httpClient = new HttpClient();

var wikiModules = await httpClient.GetStringAsync(GetUrl("Operator_Module"));

const char Endl  = '\n';
const char Comma = ',';

var regexTimeout = TimeSpan.FromMilliseconds(100);

const string XOperator = "|x operator =";
const string YOperator = "|y operator =";

var operatorModules =
	(await Task.WhenAll(
		wikiModules
			.Split(Endl)
			.Where( static s => s.StartsWith(XOperator) || s.StartsWith(YOperator))
			.Select(static s => s.Replace(XOperator, string.Empty).Replace(YOperator, string.Empty))
			.SelectMany(static s => s.Split(Comma, StringSplitOptions))
			.Distinct()
			.OrderBy(static _ => _)
			.Select(GetOperator)
		)
	).Select(static op => op.ToString());

await STATask.Run(() => Clipboard.SetText(string.Join(Environment.NewLine, operatorModules)));

"Operator modules have been copied to clipboard.".Dump();

Uri GetUrl(string uri) =>
	new($"https://arknights.wiki.gg/wiki/{uri}?action=raw");

async Task<Operator> GetOperator(string name)
{
	const string XTitle = "|title =";

	var wiki = await httpClient.GetStringAsync(GetUrl(name.Replace(' ', '_')));

	var wikiModules = wiki.Split("==Modules==", StringSplitOptions).Last().Split("==").First();

	var titles = wikiModules.Split(Endl, StringSplitOptions)
		.Where( static s => s.StartsWith(XTitle))
		.Skip(1) // Original
		.Select(static s => s.Replace(XTitle, string.Empty).Trim());

	var missions = wikiModules.Split(Endl, StringSplitOptions)
		.Where( static s => s.StartsWith("|mission2"))
		.Select(s => Regex.Match(s, @"\[\[([^]]+-[^]]+)\]\]", RegexOptions.None, regexTimeout).Groups[1].Value.Split('|').Last());

	return new(
		name,
		Regex.Match(wiki, @"\|rarity\s*=\s*(\d+)", RegexOptions.None, regexTimeout).Groups[1].Value,
		Regex.Match(wiki, @"\|simulation", RegexOptions.None, regexTimeout).Success,
		Enumerable.Zip(titles, missions, static (t, m) => new Module(t, m)).ToArray()
	);
}

sealed record Operator(string Name, string Stars, bool Paradox, IEnumerable<Module> Modules)
{
	public override string ToString() =>
		string.Join(Environment.NewLine, Modules.Select(m => $"{Name}\t{Stars}\t{m.Name}\t{m.Mission}{(Paradox ? $"\t{nameof(Paradox)}" : string.Empty)}"));
}

sealed record Module(string Name, string Mission);

public static class STATask
{
    public static Task Run(Action action)
    {
        var tcs = new TaskCompletionSource();

        var thread = new Thread(() =>
        {
            try
            {
                action();
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return tcs.Task;
    }
}
