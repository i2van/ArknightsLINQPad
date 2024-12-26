<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
</Query>

// Arknights event stock scraper.

#nullable enable

#load "./lib/Clipboard.linq"
#load "./lib/Extensions.linq"

// TODO: Specify the event URI including /Rerun if present.
var eventUri = """
So Long, Adele Rerun
"""
	.Trim()
	.Replace(" Rerun", "/Rerun")
	.Replace(" ", "_")
	.Split('#')
	.First();

const StringSplitOptions StringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

using var httpClient = new HttpClient().Configure();

var wiki = await httpClient.GetStringAsync($"https://arknights.wiki.gg/wiki/{eventUri}?action=raw");

var regexTimeout = TimeSpan.FromMilliseconds(100);

var eventName  = Regex.Match(wiki, @"\|name\s*=\s*(.+?)\n", RegexOptions.None, regexTimeout).Groups[1].Value.Trim();
var eventStock = Regex.Match(wiki, @"=+?([^=]+)=+?[^=]+\{\{Event\s+store\s+head", RegexOptions.None, regexTimeout).Groups[1].Value.Trim();

string? eventCurrency = null;

var stockItems = wiki
	.Replace("\n", string.Empty)
	.Split("}}", StringSplitOptions)
	.SkipWhile(static s => !s.Contains("{{Event store head"))
	.TakeWhile(static s => !s.StartsWith("{{Table end"))
	.Select(GetStockItem)
	.WhereNot(string.IsNullOrEmpty)
	.ToArray();

if(!stockItems.Any())
{
	throw new("No event stock items found");
}

await new string[]{
	$@"[new(""{eventUri}#{(string.IsNullOrWhiteSpace(eventStock) ? "EVENT_STOCK" : eventStock.UnderscoreSpaces())}"", ""{eventName}"", ""{(eventCurrency ?? "EVENT_CURRENCY").UnderscoreSpaces()}"")] = new(""""""{Environment.NewLine}// {eventName}",
	string.Join(Environment.NewLine, stockItems),
	$@""""""")"
}.SetClipboard();

"Event stock has been copied to clipboard.".Dump();

string GetStockItem(string stockItem)
{
	const string EventStoreCell = "{{Event store cell|";

	const string Name     = nameof(Name);
	const string Currency = nameof(Currency);
	const string Price    = nameof(Price);
	const string Stock    = nameof(Stock);

	var regex = new Regex(@$"^\s*{Regex.Escape(EventStoreCell)}\s*[a-z]+\s*=\s*(?<{Name}>[^,\|]+).*\|[a-z]+\s*=\s*(?<{Currency}>[^,]+).+?(?<{Price}>[0-9]+)\|[a-z]+\s*=\s*(?<{Stock}>[0-9]+)\s*$", RegexOptions.ExplicitCapture, regexTimeout);

	var match = regex.Match(stockItem);
	if(!match.Success)
	{
		return string.Empty;
	}

	eventCurrency ??= getValue(Currency);

	var stock = getValue(Stock);
	if(stock == "1")
	{
		stock = string.Empty;
	}

	return $"{getValue(Price)}\t{stock}\t{getValue(Name).Replace("“", @"""").Replace("”", @"""")}";

	string getValue(string name) =>
		match.Groups[name].Value.Trim();
}
