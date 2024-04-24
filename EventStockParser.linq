<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
</Query>

// Arknights event stock parser.

#nullable enable

#load "./lib/Extensions.linq"
#load "./lib/Clipboard.linq"

// TODO: Specify the event URI including /Rerun if present.
var eventUri = "Come_Catastrophes_or_Wakes_of_Vultures".Split('#').First();

const StringSplitOptions StringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

using var httpClient = new HttpClient();

var wiki = await httpClient.GetStringAsync($"https://arknights.wiki.gg/wiki/{eventUri}?action=raw");

const char Endl  = '\n';
const char Comma = ',';

const string EventStoreCell = "{{Event store cell|";

var regexTimeout = TimeSpan.FromMilliseconds(100);

var eventName  = Regex.Match(wiki, @"\|name\s*=\s*(.+?)\n", RegexOptions.None, regexTimeout).Groups[1].Value.Trim();
var eventStock = Regex.Match(wiki, @"=+?([^=]+)=+?[^=]+\{\{Event\s+store\s+head", RegexOptions.None, regexTimeout).Groups[1].Value.Trim();

string? eventCurrency = null;

var stockItems = wiki.Split(Endl)
	.Where( static s => s.StartsWith(EventStoreCell))
	.Select(static s => s.Replace(EventStoreCell, string.Empty))
	.Select(static s => s.Split(Comma, StringSplitOptions))
	.Select(GetStockItem)
	.Where( static s => !string.IsNullOrEmpty(s))
	.ToArray();

await new string[]{
	$@"[new(""{eventUri}#{(string.IsNullOrWhiteSpace(eventStock) ? "EVENT_STOCK" : eventStock.UnderscoreSpaces())}"", ""{eventName}"", ""{(eventCurrency ?? "EVENT_CURRENCY").UnderscoreSpaces()}"")] = new(""""""{Environment.NewLine}// {eventName}",
	string.Join(Environment.NewLine, stockItems),
	$@""""""")"
}.SetClipboard();

"Event stock has been copied to clipboard.".Dump();

string GetStockItem(IEnumerable<string> stockItems)
{
	const char Or = '|';

	var name = stockItems.First();

	if(name.StartsWith("ware="))
	{
		name = name.Split(Or).Skip(1).First();
	}
	else
	{
		eventCurrency ??= stockItems.Skip(1).First().Split(Or).Last();
	}

	var priceCount = stockItems.Last().Split(Or).ToArray();
	var count = priceCount.Last().Trim('}');

	return count == "-1"
			? string.Empty
			: $"{priceCount.First()}\t{(count == "1" ? string.Empty : count)}\t{name.Replace("“", @"""").Replace("”", @"""")}";
}
