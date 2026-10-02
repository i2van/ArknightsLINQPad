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
	}

	public static class Document
	{
		public const string MarginBottom = "6.5rem";

		public static class Styles
		{
			public const string FloatRight = "float: right";
		}

		public const string EventBackgroundUriTemplate = $$"""<span>{{HtmlExtensions.H1InnerHTMLTemplate}}<sup><a href="{{Url.Wiki}}/images/Site-background-dark.jpg" class="reference" style="{{Style.Sup}}" onmouseenter="{{Script.ShowPreview}}" onmouseleave="{{Script.HidePreview}}" onfocus="{{Script.ShowPreview}}" onblur="{{Script.HidePreview}}">bg</a></sup></span>""";

		private static class Style
		{
			public const string Sup = "margin-left: 0.3ex; font-size: 71%";
		}

		// Previews the image linked by the anchor; see also ItemImage.GetHyperlink.
		private static class Script
		{
			private const string PreviewId = "eventBackgroundPreview";
			private const string Margin    = "0.35rem";

			public const string ShowPreview =
				$"let i=document.getElementById('{PreviewId}');i||(i=document.body.appendChild(document.createElement('img')),i.id='{PreviewId}',i.src=this.href);const r=this.getBoundingClientRect();i.style.cssText='display:block;position:fixed;left:{Margin};top:calc('+r.bottom+'px + {Margin});z-index:2;max-width:calc(100vw - 2*{Margin});max-height:calc(100vh - '+r.bottom+'px - 2*{Margin});pointer-events:none'";

			public const string HidePreview =
				$"const i=document.getElementById('{PreviewId}');if(i)i.style.display='none'";
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
