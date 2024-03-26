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

#nullable enable

void Main()
{
	// TODO: Operator modules list can be found at https://arknights.wiki.gg/wiki/Operator_Module
	// TODO: Add operators modules.
	var items = new Items("""
		//	operator	stage	stars (1-6)
		//	Schwarz		2-1		6 // Y
			Ch'en		5-9		6
			Saga		WR-3	6
			Silence the Paradigmatic	6-3	6
			Grani		GT-6	5
			Mr. Nothing	3-7		5
			Tsukinogi	S3-6	5
			Purestream	3-7		4
			Vermeil		3-1		4
		""")
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
			Operators = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Operator, "Modules"))),
			v.Stars,
			v.Total,
			Paradox   = VerticalRun(v.Operators.Select(static v => new Hyperlinq($"https://www.youtube.com/results?search_query={v.Operator}+paradox+simulation", "YouTube")))
		}))
		.ToArray();

	const string title = "Joined Operators Modules Stage";

	(items.Any() ? (object)items : "No operators modules found.").Dump(title);

	title.AsHyperlink(new WikiHyperlinq("Operator_Module", "List").Uri);

	static T Pass<T>(T t) => t;
}

static class Extensions
{
	public static string JoinStrings<T, TV>(this IEnumerable<T> items, Func<T, TV> selector) =>
		 Join(NewLine, items.Select(selector));
}

record Item(string Operator, string Stage, int Stars);

class Items : IEnumerable<Item>
{
	private const string Comment = "//";

	private const string Operator = nameof(Operator);
	private const string Stage    = nameof(Stage);
	private const string Stars    = nameof(Stars);

	private static readonly Regex ItemRegex = new Regex($@"^(?<{Operator}>[^\t]+)\t+(?<{Stage}>[^\t]+)(\t+(?<{Stars}>[1-6]))?$", ExplicitCapture, FromSeconds(1));

	private readonly Lazy<Item[]> _items;

	public Items(string items) =>
		_items = new(() => Parse(items));

	public IEnumerator<Item> GetEnumerator() =>
		_items.Value.AsEnumerable().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		_items.Value.GetEnumerator();

	private static Item[] Parse(string values)
	{
		return values
			.Replace(Comment, NewLine + Comment)
			.Split(NewLine.ToCharArray())
			.Select(static v => v.Trim())
			.Where( static v => !IsNullOrEmpty(v))
			.Where( static v => !v.StartsWith(Comment))
			.Select(CreateItem)
			.ToArray();

		static Item CreateItem(string str)
		{
			var match = ItemRegex.Match(str);
			if(!match.Success)
			{
				Throw();
			}

			return new
			(
				GetString(Operator),
				GetString(Stage),
				GetNumber(Stars, FallbackValue(0))
			);

			string GetString(string key) =>
				match.Groups[key].Value;

			int GetNumber(string key, Func<int> fallbackValue) =>
				int.TryParse(GetString(key), Integer, InvariantCulture, out var value)
					? value
					: fallbackValue();

			static Func<T> FallbackValue<T>(T v) =>
				() => v;

			[DoesNotReturn]
			void Throw() =>
				throw new($"Invalid data: '{str}'. Expected: 'operator stage [stars (1-6)]'");
		}
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
