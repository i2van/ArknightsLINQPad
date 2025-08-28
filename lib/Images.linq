<Query Kind="Program">
  <Namespace>ImageControl = LINQPad.Controls.Image</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
</Query>

#nullable enable

#load "./Context.linq"
#load "./Extensions.linq"
#load "./Parsable.linq"
#load "./WikiHyperlinq.linq"

class ItemImage
{
	public string Name { get; }
	public LazyImage Image { get; }

	public ItemImage(string name, string? pathUri, string? fileName = null, int? height = null) =>
		(Name, Image) = (name, new($"{DumpContext.Url.Wiki}/images{(string.IsNullOrWhiteSpace(pathUri) ? string.Empty : $"/{pathUri}")}/{(fileName ?? name).UnderscoreSpaces()}.png", height));

	public static Hyperlink GetHyperlink(string name)
	{
		var itemHyperlinq = GetItemHyperlinq(name);

		if(!ImageData.Value.TryGetValue(name, out var imageData))
		{
			imageData = LazyImage.NotAvailable;
		}

		var hyperlink = new Hyperlink(itemHyperlinq.Text, itemHyperlinq.Uri);
		var htmlElement = hyperlink.HtmlElement;

		htmlElement.AddEventListener("mouseenter", ShowImageEventHandler);
		htmlElement.AddEventListener("mouseout",   HideImageEventHandler);
		htmlElement.AddEventListener("focusin",    ShowImageEventHandler);
		htmlElement.AddEventListener("focusout",   HideImageEventHandler);

		return hyperlink;

		void ShowImageEventHandler(object? elem, EventArgs e)
		{
			var top = htmlElement.InvokeScript(true, "eval", "targetElement.getBoundingClientRect().top - document.body.getBoundingClientRect().top");

			Context.Containers.Image.Style   = $"position: absolute; top: {top}px; z-index: 2";
			Context.Containers.Image.Content = imageData;
		}

		static void HideImageEventHandler(object? elem, EventArgs e) =>
			Context.Containers.Image.ClearContent();

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

sealed class SkinImage : ItemImage
{
	public SkinImage(string name, string pathUri, string fileName)
		: base(name, pathUri, fileName, DumpContext.ImageHeight.Skin)
	{
	}
}

sealed class LazyImage
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

sealed class MaterialImages : Parsable<ItemImage>
{
	private const string PathUri  = nameof(PathUri);
	private const string Name     = nameof(Name);
	private const string FileName = nameof(FileName);

	protected override string Regex { get; } = $@"^(?<{Name}>[^\t]+)(\t+(?<{PathUri}>[^\t]+))?(\t+(?<{FileName}>[^\t]+))?$";
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

sealed record ImageUriMap(string Name, string Map);

sealed class ImageUriMaps : Parsable<ImageUriMap>
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

void Main()
{
}
