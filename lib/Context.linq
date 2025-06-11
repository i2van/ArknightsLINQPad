<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

#nullable enable

#load "./Extensions.linq"

static DumpContext Context = null!;

sealed class DumpContext
{
	public static readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;

	public static class Url
	{
		public const string Wiki = "https://arknights.wiki.gg";
	}

	public static class ImageHeight
	{
		public const int Item         = -96; // <= 0 for as is size.
		public const int Skin         = 480;
		public const int NotAvailable = Skin / 2;
	}

	public static class Document
	{
		public const string MarginBottom = "6.5rem";

		public static class Styles
		{
			public const string FloatRight = "float: right";
		}

		public const string EventBackgroundUriTemplate = $$"""<span>{{HtmlExtensions.H1InnerHTMLTemplate}}<sup><a href="{{Url.Wiki}}/images/5/51/Wiki-background.png" class="reference" title="Event background" style="{{Style.Sup}}">bg</a></sup></span>""";

		private static class Style
		{
			public const string Sup = "margin-left: 0.3ex; font-size: 71%";
		}
	}

	public static class Glyphs
	{
		public const char Circle = '⬤';
	}

	public class DumpContainers
	{
		public readonly DumpContainer Image = new();
	}

	public readonly DumpContainers Containers = new();
}

void Main()
{
}
