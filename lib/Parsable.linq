<Query Kind="Statements">
  <Namespace>static System.Environment</Namespace>
  <Namespace>static System.Globalization.CultureInfo</Namespace>
  <Namespace>static System.Globalization.NumberStyles</Namespace>
  <Namespace>static System.String</Namespace>
  <Namespace>static System.Text.RegularExpressions.RegexOptions</Namespace>
  <Namespace>static System.TimeSpan</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
</Query>

#nullable enable

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
		var regex = new Regex(Regex, ExplicitCapture, FromMilliseconds(100));

		var match = regex.Match(str);
		if(match.Success)
		{
			return Create(match);
		}

		Throw(str, ErrorMessage);
		return default!;
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

	[DoesNotReturn]
	protected void Throw() =>
		throw new(ErrorMessage);

	protected static Func<TV> FallbackAction<TV>(Action func) =>
		delegate { func(); return default!; };

	protected static Func<TV> FallbackValue<TV>(TV v) =>
		() => v;
}
