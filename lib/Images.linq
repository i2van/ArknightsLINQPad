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

	public ItemImage(string name, string? fileName = null, int? height = null, bool invertOnLightTheme = false) =>
		(Name, Image) = (name, new($"{DumpContext.Url.Wiki}/images/{(fileName ?? name).UnderscoreSpaces()}.png", height, invertOnLightTheme));

	public static Hyperlink GetHyperlink(string name)
	{
		if(!ImageData.Value.TryGetValue(name, out var imageData))
		{
			imageData = new ItemImage(name).Image;
		}

		return GetHyperlink(GetItemHyperlinq(name), imageData);

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

	public static Hyperlink GetHyperlink(Hyperlinq hyperlinq, LazyImage image)
	{
		var hyperlink = new Hyperlink(hyperlinq.Text, hyperlinq.Uri);
		var htmlElement = hyperlink.HtmlElement;

		htmlElement.AddEventListener("mouseenter", ShowImageEventHandler);
		htmlElement.AddEventListener("mouseout",   HideImageEventHandler);
		htmlElement.AddEventListener("focusin",    ShowImageEventHandler);
		htmlElement.AddEventListener("focusout",   HideImageEventHandler);

		return hyperlink;

		void ShowImageEventHandler(object? elem, EventArgs e)
		{
			// Returns "top|isDarkTheme", e.g. "123.4|1". Theme is detected from the body background luminance.
			const string script = """
				(function(){
					const top = targetElement.getBoundingClientRect().top;
					const rgba = getComputedStyle(document.body).backgroundColor.match(/[\d.]+/g) || [];
					const isDarkTheme = rgba.length > 2 && (rgba.length < 4 || +rgba[3] > 0) && +rgba[0]*.299 + +rgba[1]*.587 + +rgba[2]*.114 < 128;
					return top + '|' + (isDarkTheme ? 1 : 0);
				})()
				""";

			var topTheme = $"{htmlElement.InvokeScript(true, "eval", script)}".Split('|');
			var top = topTheme[0];

			var isDarkTheme = topTheme.Length > 1 && topTheme[1] == "1";
			var filter = image.InvertOnLightTheme && !isDarkTheme ? "; filter: invert(1)" : "";

			// Fixed position does not affect the document size, so no scroll bars appear.
			// The image inherits max-height and is capped to the viewport space below the row.
			var maxHeight = image.MaxHeight is { } maxImageHeight
				? $"min({maxImageHeight}px, 100vh - {top}px - 4px)"
				: $"calc(100vh - {top}px - 4px)";

			Context.Containers.Image.Style   = $"position: fixed; top: {top}px; z-index: 2; max-height: {maxHeight}{filter}";
			Context.Containers.Image.Content = image;
		}

		static void HideImageEventHandler(object? elem, EventArgs e) =>
			Context.Containers.Image.ClearContent();
	}
}

sealed class SkinImage : ItemImage
{
	public SkinImage(string name, string fileName)
		: base(name, fileName, DumpContext.ImageHeight.Skin)
	{
	}
}

sealed class StageImage : ItemImage
{
	public StageImage(string name)
		: base(name, $"{name} map", DumpContext.ImageHeight.Skin)
	{
	}

	public static new Hyperlink GetHyperlink(string name) =>
		GetHyperlink(name, name);

	public static Hyperlink GetHyperlink(string uri, string name) =>
		GetHyperlink(new WikiHyperlinq(uri, name), new StageImage(uri).Image);
}

sealed class OperatorImage : ItemImage
{
	public OperatorImage(string name)
		: base(name, $"{name} icon")
	{
	}

	public static new Hyperlink GetHyperlink(string name) =>
		GetHyperlink(name, name);

	public static Hyperlink GetHyperlink(string uri, string name) =>
		GetHyperlink(new WikiHyperlinq(uri, name), new OperatorImage(name).Image);
}

sealed class ModuleImage : ItemImage
{
	public ModuleImage(string name)
		: base(name, $"{name} module", invertOnLightTheme: true)
	{
	}

	public static Hyperlink GetHyperlink(string uri, string name) =>
		GetHyperlink(new WikiHyperlinq(uri, name), new ModuleImage(name).Image);
}

sealed class ClassImage : ItemImage
{
	public ClassImage(string name)
		: base(name, invertOnLightTheme: true)
	{
	}

	public static new Hyperlink GetHyperlink(string name) =>
		GetHyperlink(new WikiHyperlinq(name), new ClassImage(name).Image);
}

sealed class LazyImage
{
	private readonly Lazy<ImageControl> _value;

	private ImageControl Control => _value.Value;

	public bool InvertOnLightTheme { get; }

	public int? MaxHeight { get; }

	public LazyImage(string imageUri, int? height = null, bool invertOnLightTheme = false)
	{
		InvertOnLightTheme = invertOnLightTheme;

		var maxHeight = height ?? DumpContext.ImageHeight.Item;
		MaxHeight = maxHeight > 0 ? maxHeight : null;

		_value = new(CreateControl);

		ImageControl CreateControl()
		{
			var control = new ImageControl(new Uri(imageUri));

			// LINQPad theme styles the image background; keep the original transparency.
			control.Styles["background"] = "transparent";

			// Fit into the hover container to avoid the scroll bars; see ItemImage.GetHyperlink.
			control.Styles["max-width"]  = "100%";
			control.Styles["max-height"] = "inherit";

			return control;
		}
	}

	public object ToDump() =>
		Control;
}

sealed class MaterialImages : Parsable<ItemImage>
{
	private const string Name     = nameof(Name);
	private const string FileName = nameof(FileName);

	protected override string Regex { get; } = $@"^(?<{Name}>[^\t]+)(\t+(?<{FileName}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = $"Expected: 'name [fileName]'";

	public MaterialImages(string items) :
		base(items)
	{
	}

	protected override ItemImage Create(Match match)
	{
		var name = GetString(match, Name);
		var fileName = GetString(match, FileName);

		return fileName.Contains("_Skin")
			? new SkinImage(name, fileName)
			: new ItemImage(name, string.IsNullOrWhiteSpace(fileName) ? null : fileName);
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
		new MaterialImages("Images.tsv".LoadOptional()).ToDictionary(static v => v.Name, static v => v.Image)
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
