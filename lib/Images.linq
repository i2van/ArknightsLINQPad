<Query Kind="Program">
  <Namespace>ImageControl = LINQPad.Controls.Image</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

#nullable enable

#load "./Extensions.linq"
#load "./Parsable.linq"

static DumpContext Context = null!;

void Main()
{
}

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

class ItemImage
{
	public string Name { get; }
	public LazyImage Image { get; }

	public ItemImage(string name, string pathUri, string? fileName = null, int? height = null) =>
		(Name, Image) = (name, new($"{DumpContext.Url.Wiki}/images/{pathUri}/{(fileName ?? name).UnderscoreSpaces()}.png", height));

	public static Hyperlink GetHyperlink(string name)
	{
		var itemHyperlinq = GetItemHyperlinq(name);

		if(!ImageData.Value.TryGetValue(name, out var imageData))
		{
			imageData = LazyImage.NotAvailable;
		}

		var hyperlink = new Hyperlink(itemHyperlinq.Text, itemHyperlinq.Uri);
		var htmlElement = hyperlink.HtmlElement;

		htmlElement.AddEventListener("mouseenter", ShowImageOnMouseEnterHandler);
		htmlElement.AddEventListener("mouseout",   static delegate { Context.Containers.Image.ClearContent(); });

		return hyperlink;

		void ShowImageOnMouseEnterHandler(object? elem, EventArgs e)
		{
			var top = htmlElement.InvokeScript(true, "eval", "targetElement.getBoundingClientRect().top - document.body.getBoundingClientRect().top");

			Context.Containers.Image.Style   = $"position: absolute; top: {top}px; z-index: 2";
			Context.Containers.Image.Content = imageData;
		}

		static Hyperlinq GetItemHyperlinq(string itemName)
		{
			var (uri, text) = GetUriName();

			return new WikiHyperlinq(uri, text);

			(string Uri, string Text) GetUriName()
			{
				const string token = "'s_Token";

				var itemUri = itemName.UnderscoreSpaces();

				return ImageUriMapData.Value.TryGetValue(itemName, out var mappedUri)
						? (mappedUri, itemName)
						: itemUri.Contains(token)
							? ($"Operator_Token/5-star#{itemUri[..(itemUri.Length - token.Length)]}", itemName)
							: (itemUri, itemName);
			}
		}
	}
}

class SkinImage : ItemImage
{
	public SkinImage(string name, string pathUri, string fileName)
		: base(name, pathUri, fileName, DumpContext.ImageHeight.Skin)
	{
	}
}

class LazyImage
{
	public static readonly LazyImage NotAvailable = new("https://upload.wikimedia.org/wikipedia/commons/a/ac/No_image_available.svg", DumpContext.ImageHeight.NotAvailable);

	private readonly Lazy<ImageControl> _value;

	private ImageControl Control => _value.Value;

	public LazyImage(string imageUri, int? height = null)
	{
		_value = new(CreateControl);

		ImageControl CreateControl()
		{
			var control = new ImageControl(new Uri(imageUri));

			if((height ??= DumpContext.ImageHeight.Item) > 0)
			{
				control.Height = height.Value;
			}

			return control;
		}
	}

	public object ToDump() =>
		Control;
}

class MaterialImages : Parsable<ItemImage>
{
	private const string PathUri  = nameof(PathUri);
	private const string Name     = nameof(Name);
	private const string FileName = nameof(FileName);

	protected override string Regex { get; } = $@"^(?<{Name}>[^\t]+)\t+(?<{PathUri}>[^\t]+)(\t+(?<{FileName}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = $"Expected: 'name pathUri [fileName]'";

	public MaterialImages(string items) :
		base(items)
	{
	}

	protected override ItemImage Create(Match match)
	{
		var name = GetString(match, Name);
		var pathUri = GetString(match, PathUri);
		var fileName = GetString(match, FileName);

		return fileName.Contains("_Skin")
			? new SkinImage(name, pathUri, fileName)
			: new ItemImage(name, pathUri, string.IsNullOrWhiteSpace(fileName) ? null : fileName);
	}
}

record ImageUriMap(string Name, string Map);

class ImageUriMaps : Parsable<ImageUriMap>
{
	private const string Name = nameof(Name);
	private const string Map  = nameof(Map);

	protected override string Regex { get; } = $@"^(?<{Name}>[^\t]+)\t+(?<{Map}>[^\t]+)$";
	protected override string ErrorMessage { get; } = $"Expected: 'name map'";

	public ImageUriMaps(string items) :
		base(items)
	{
	}

	protected override ImageUriMap Create(Match match) =>
		new(
			GetString(match, Name),
			GetString(match, Map)
		);
}

ref struct DisposableAction
{
	private readonly Action _action;

	public DisposableAction(Action action) =>
		_action = action;

	public void Dispose() =>
		_action?.Invoke();
}

class DumpContext
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

static class ImageData
{
	public static readonly ReadOnlyDictionary<string, LazyImage> Value = new(
		new MaterialImages("Images.tsv".Load()).ToDictionary(static v => v.Name, static v => v.Image)
	);
}

static class ImageUriMapData
{
	public static readonly ReadOnlyDictionary<string, string> Value = new(
		new ImageUriMaps("ImageUriMap.tsv".Load()).ToDictionary(static v => v.Name, static v => v.Map)
	);
}
