<Query Kind="Program">
  <Namespace>ImageControl = LINQPad.Controls.Image</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

#nullable enable

#load "./Extensions.linq"

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
	// REGEX:
	// ^.+/([a-z0-9]/[a-z0-9]{2})/(.+?)\.png.*$
	// new\("$2", "$1"\),
	public static readonly ReadOnlyDictionary<string, LazyImage> Value = new(new ItemImage[]
	{
		new SkinImage("Major Field", "4/46", "Earthspirit_Skin_1"),
		new("'Advance Patchwork'", "d/d1"),
		new("'Audible Redemption'", "9/9d"),
		new("'Cohesion'", "f/fe"),
		new("'Collection'", "c/c1", "Collection_(furniture)"),
		new("'Concealed Edge'", "4/4a"),
		new("'Cultivating Hope'", "9/9f"),
		new("'Duel With Oneself'", "9/9a"),
		new("'Elevations'", "4/41"),
		new("'Fowlbeast Accommodations'", "4/4f"),
		new("'Implantation'", "2/22"),
		new("'Indications'", "0/02"),
		new("'Nostalgic Habitations'", "b/b1"),
		new("'Purge of Flaws'", "4/49"),
		new("'Record of Suffering'", "d/d0"),
		new("'Simplicity'", "e/ec"),
		new("'Treading Sand'", "f/fd"),
		new("'Treeshade Lineations'", "7/79"),
		new("'Truck Passage'", "e/e7"),
		new("'Witness of Friendship'", "f/fa"),
		new("Aggregate Cyclicene", "2/2f"),
		new("Aketon", "d/d3"),
		new("Assembly Workbench", "2/2e"),
		new("Bipolar Nanoflake", "b/b6"),
		new("Bryophyta's Token", "e/eb"),
		new("Caster Chip", "9/94"),
		new("Chip Catalyst", "3/32"),
		new("Coagulating Gel", "b/bd"),
		new("Coldshot's Token", "9/9a"),
		new("Compound Cutting Fluid", "2/2c"),
		new("Cozy Plaid Rug", "e/eb"),
		new("Crystalline Circuit", "4/46"),
		new("Crystalline Component", "2/20"),
		new("Crystalline Electronic Unit", "d/d0"),
		new("Cured Fiberboard", "6/67"),
		new("Cutting Fluid Solution", "9/93"),
		new("Cyclicene Prefab", "e/ea"),
		new("D32 Steel", "6/68"),
		new("Data Supplement Instrument", "6/6d"),
		new("Data Supplement Stick", "f/f8"),
		new("Defender Chip", "2/24"),
		new("Device", "a/a4"),
		new("Dizzy Spinning Chair", "6/63"),
		new("Exhibition Room Ceiling Light (Bright)", "d/de"),
		new("Exhibition Room Ceiling Light (Dim)", "0/0b"),
		new("Explosion-proof Fluorescent Lamp", "e/e0"),
		new("Frontline Battle Record", "f/f7"),
		new("Furniture Part", "0/0d"),
		new("Fuscous Fiber", "d/d6"),
		new("Fuze's Token", "b/ba"),
		new("Grindstone Pentahydrate", "a/a5"),
		new("Grindstone", "6/69"),
		new("Guard Chip", "6/63"),
		new("Harold's Token", "8/8b"),
		new("Headhunting Permit", "0/0b"),
		new("Incandescent Alloy Block", "a/a3"),
		new("Incandescent Alloy", "f/f4"),
		new("Information Fragment", "1/1d"),
		new("Insider's Token", "b/bc"),
		new("Integrated Device", "f/fb"),
		new("Jieyun's Token", "0/03"),
		new("Keton Colloid", "a/ae"),
		new("LMD", "e/e7"),
		new("Loxic Kohl", "4/4a"),
		new("Manganese Ore", "6/6a"),
		new("Manganese Trihydrate", "c/cd"),
		new("Medic Chip", "e/eb"),
		new("Mischievously Spliced Table", "9/9a"),
		new("Module Data Block", "6/66"),
		new("Nucleic Crystal Sinter", "7/7e"),
		new("Optimized Device", "f/fd"),
		new("Orirock Cluster", "4/4d"),
		new("Orirock Concentration", "d/d4"),
		new("Orirock Cube", "d/d8"),
		new("Oriron Block", "5/52"),
		new("Oriron Cluster", "8/87"),
		new("Oriron", "4/44"),
		new("Patching Planks", "8/8c"),
		new("Polyester Lump", "e/ed"),
		new("Polyester Pack", "f/f3"),
		new("Polyester", "3/3a"),
		new("Polyketon", "9/96"),
		new("Polymerization Preparation", "9/9c"),
		new("Polymerized Gel", "6/66"),
		new("Projectile Interception System", "a/a0"),
		new("Public Bookshelf", "5/54"),
		new("Pure Gold", "0/0e"),
		new("Pure White Stone-Tiled Table", "1/10"),
		new("Puzzle's Token", "6/68"),
		new("Rattan Pendant Lamp", "7/74"),
		new("Recruitment Permit", "3/3b"),
		new("Refined Solvent", "9/9f"),
		new("Reinforced Work Chair", "a/ae"),
		new("Reinforcement Debris", "7/7e"),
		new("RMA70-12", "1/10"),
		new("RMA70-24", "f/f1"),
		new("Semi-Synthetic Solvent", "5/58"),
		new("Shock-proof Pillar", "2/22"),
		new("Signal Disruptor", "3/33"),
		new("Silence the Paradigmatic's Token", "2/20"),
		new("Skill Summary - 2", "b/b0", "Skill_Summary_Volume_2"),
		new("Skill Summary - 3", "a/a7", "Skill_Summary_Volume_3"),
		new("Sniper Chip", "c/cd"),
		new("Solidified Fiber Board", "e/e2"),
		new("Specialist Chip", "e/e2"),
		new("Strategic Battle Record", "5/55"),
		new("Stumpy Little Fridge", "f/f1"),
		new("Sugar Lump", "d/d2"),
		new("Sugar Pack", "2/23"),
		new("Sugar", "a/a9"),
		new("Supporter Chip", "d/d7"),
		new("Surveillance Camera", "6/65"),
		new("Tachanka's Token", "a/a9"),
		new("Tactical Battle Record", "a/a7"),
		new("Throwable Discharge Device", "7/7a"),
		new("Toasty Shop Lamp", "6/6c"),
		new("Transmuted Salt Agglomerate", "b/bd"),
		new("Transmuted Salt", "b/bd"),
		new("Used Folding Chair", "c/c1"),
		new("Vanguard Chip", "d/df"),
		new("Wall-mounted Newspaper Rack", "9/91"),
		new("Wanqing's Token", "f/fc"),
		new("White Horse Kohl", "4/4a"),
		new("Wooden Coat Hanger", "7/7e"),
		new("Yellowy Gauze Curtain", "9/9d")
	}.ToDictionary(static v => v.Name, static v => v.Image));
}

static class ImageUriMapData
{
	public static readonly ReadOnlyDictionary<string, string> Value = new(new Dictionary<string, string>
	{
		// Furniture.
		["'Collection'"] = "Collection_(furniture)",
		// Skins.
		["Major Field"]  = "Earthspirit/Gallery#Major_Field",
		// 6-star tokens.
		["Silence the Paradigmatic's Token"] = "Operator_Token/6-star#Silence_the_Paradigmatic"
	});
}
