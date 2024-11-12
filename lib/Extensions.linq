<Query Kind="Statements">
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>static LINQPad.Util</Namespace>
  <Namespace>static System.Environment</Namespace>
  <Namespace>static System.Math</Namespace>
  <Namespace>static System.String</Namespace>
  <Namespace>static System.TimeSpan</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
</Query>

#nullable enable

static partial class Extensions
{
	private static readonly char[] Quotes = "\"'".ToCharArray();

	private static readonly TimeSpan AlmostSecond = TimeSpan.FromMicroseconds(999_999);

	public static string UnderscoreSpaces(this string str) =>
		str.Trim(Quotes).Replace("\" ", " ").Replace("\' ", " ").Replace(" \'", " ").Replace(' ', '_');

	public static string SpaceUnderscores(this string str) =>
		str.Replace('_', ' ');

	public static TimeSpan FloorSeconds(this TimeSpan timeSpan) =>
		timeSpan < Zero
			? timeSpan
			: timeSpan + AlmostSecond;

	public static Color InterpolateTo(this Color fromColor, double ratio, Color toColor)
	{
		var alt = 1.0 - ratio;

		var (xFrom, yFrom, zFrom) = (fromColor.R, fromColor.G, fromColor.B);
		var (xTo, yTo, zTo) = (toColor.R, toColor.G, toColor.B);

		var (magnitudeFrom, magnitudeTo) = (CalcMagnitude(xFrom, yFrom, zFrom), CalcMagnitude(xTo, yTo, zTo));
		var (x, y, z) = (CalcActual(xFrom, xTo), CalcActual(yFrom, yTo), CalcActual(zFrom, zTo));

		var magnitude = CalcActual(magnitudeFrom, magnitudeTo);
		var scale = magnitude / CalcMagnitude(x, y, z);

		return Color.FromArgb(ToColor(x), ToColor(y), ToColor(z));

		static Double CalcMagnitude(double x, double y, double z) =>
			Sqrt(x * x + y * y + z * z);

		double CalcActual(double from, double to) =>
			ratio * from + alt * to;

		int ToColor(double value) =>
			Max(0, Min(0xFF, (int)Round(value * scale)));
	}
}

static partial class CollectionExtensions
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
}

static partial class ControlsExtensions
{
	private static T SetAttribute<T>(this T control, string attribute, [CallerArgumentExpression(nameof(attribute))] string attributeName = "")
		where T : Control
	{
		control.HtmlElement[attributeName] = attribute;
		return control;
	}

	public static T SetTitle<T>(this T control, string title) where T : Control =>
		control.SetAttribute(title);
}

static partial class DataExtensions
{
	public static string Load(this string fileName) =>
		File.ReadAllText(Path.Join(Path.GetDirectoryName(Util.CurrentQueryPath), "data", fileName));
}

static partial class HtmlExtensions
{
	private const string H1ToA = $"{nameof(HtmlExtensions)}_{nameof(H1ToA)}";

	static HtmlExtensions() =>
		HtmlHead.AddScript($$"""
			function {{H1ToA}}(text, uri){
				const elem = [].find.call(document.getElementsByTagName("h1"), elem => elem.innerHTML === text);
				if(elem){
					elem.innerHTML = `<a href="${uri}" class="headingpresenter reference">${elem.innerHTML}</a>`;
				}
			}
			""");

	public static void AsHyperlink(this string text, string uri) =>
		InvokeScript(false, H1ToA, text, uri);
}

static partial class LINQExtensions
{
	public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) =>
		enumerable.Where(e => !predicate(e));

	public static IEnumerable<T> Select<T>(this IEnumerable<T> enumerable) =>
		enumerable.Select(static e => e);

	public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
		enumerable.SelectMany(static e => e);

	public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable) =>
		enumerable.OrderBy(static e => e);

	public static IEnumerable<IGrouping<T, T>> GroupBy<T>(this IEnumerable<T> enumerable) =>
		enumerable.GroupBy(static e => e);
}

enum VideoHostings
{
	All     = int.MaxValue,
	YouTube = 1
}

static partial class LINQPadExtensions
{
	private record VideoHosting(string Name, string? Uri);
	private sealed record Separator() : VideoHosting("｜", null);

	public static object GetSearchHyperlinqs(this VideoHostings videoHostings, params string[] search)
	{
		return Util.HorizontalRun(false, GetVideoHostings().Take((int)videoHostings).Select(ToHyperlinq));

		static IEnumerable<VideoHosting> GetVideoHostings()
		{
			yield return new("YouTube",  "https://www.youtube.com/results?search_query");
			yield return new Separator();
			yield return new("bilibili", "https://search.bilibili.com/all?keyword");
		}

		object ToHyperlinq(VideoHosting videoHosting) =>
			videoHosting is Separator
				? (object)videoHosting.Name
				: new Hyperlinq($"{videoHosting.Uri}={string.Join("+", search.Select(Uri.EscapeDataString))}", videoHosting.Name);
	}
}
