<Query Kind="Program" />

#nullable enable

#load "./Context.linq"
#load "./Extensions.linq"

class WikiHyperlinq : Hyperlinq
{
	private const string WikiUri = $"{DumpContext.Url.Wiki}/wiki/";

	private readonly string? _metadata;

	public WikiHyperlinq(string uri, string text, string? metadata = null)
		: base(WikiUri + uri.UnderscoreSpaces(), text) =>
		_metadata = metadata;

	public WikiHyperlinq(string text)
		: this(text, text)
	{
	}

	public string? ProcessMetadata(Func<string, string> process) =>
		_metadata is null ? null : process(_metadata!);
}

void Main()
{
}
