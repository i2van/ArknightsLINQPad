<Query Kind="Program">
  <Namespace>static LINQPad.Util</Namespace>
  <Namespace>static System.Math</Namespace>
  <Namespace>static System.String</Namespace>
</Query>

/*
#define DUMP_ORIGINITE_PRIME_ORIGINAL_PACKS
//*/

#nullable enable

void Main()
{
	const string title = $"{nameof(OriginitePrime)} Packs";
	const string originalIitle = $"Original {title}";

	var uri = new WikiHyperlinq("Packs_Store").Uri;

	// Pack list can be found at https://arknights.wiki.gg/wiki/Packs_Store
	// TODO: Add packs.
	// TODO: See PrimePrice column, less is better.
	var originitePrime = new OriginitePrime[]
	{
		new("Piece",   1+2,      0.99),
		new("Cluster", 6+6,      4.99),
		new("Pile",    20+20,   14.99),
		new("Pack",    40+40,   29.99),
		new("Box",     66+66,   49.99),
		new("Crate",   130+130, 99.99),

		Pack.Is("Lin's Wallet",
			9.99).HasPrimes(16).HasLMD(700_000)
	}.Select(static (v, i) => { v.ID = i+1; return v; })
#if DUMP_ORIGINITE_PRIME_ORIGINAL_PACKS
	.Dump(originalIitle);
	originalIitle.AsHyperlink(uri);
#endif
	;

	originitePrime
		.Where(  static o => o.TotalPrice != 0)
		.OrderBy(static o => o.PrimePrice)
		.Dump(title);

	title.AsHyperlink(uri);
}

static object ToDump(object input) =>
	input switch
	{
		double doubleValue =>
			Round(doubleValue, 2),
		bool boolValue =>
			boolValue ? WithStyle("✅", "display: block; text-align: center") : Empty,
		_ => input
	};


record OriginitePrime(string Name, double Primes, double TotalPrice, bool Pack = false)
{
	public double PrimePrice { get; } = TotalPrice / Primes;

	public int ID { get; set; }
}

class Pack
{
	// TODO: Specify your level max sanity.
	private const double SanityPerPrime  = 135;
	// TODO: Specify LMD farm gain.
	private const double LMDFarmGain     = 10_000;
	// TODO: Specify LMD farm sanity.
	private const double LMDFarmSanity   = 36;

	private const int ChipFarmSanity     = 18;
	private const int ChipPackFarmSanity = 36;

	public string Name { get; }
	public double Price { get; }

	public double LMDSanity { get; private set; }
	public int ChipsSanity { get; private set; }
	public int ChipPacksSanity { get; private set; }

	public int Primes { get; private set; }

	public double TotalPrimes =>
		((LMDSanity + ChipsSanity + ChipPacksSanity) / SanityPerPrime) + Primes;

	private Pack(string name, double price) =>
		(Name, Price) = (name, price);

	public static Pack Is(string name, double price) =>
		new Pack(name, price);

	public Pack HasPrimes(int count)
	{
		Primes = count;
		return this;
	}

	public Pack HasLMD(int count)
	{
		LMDSanity = count * LMDFarmSanity / LMDFarmGain;
		return this;
	}

	public Pack HasChips(int count)
	{
		ChipsSanity = count * ChipFarmSanity;
		return this;
	}

	public Pack HasChipPacks(int count)
	{
		ChipPacksSanity = count * ChipPackFarmSanity;
		return this;
	}

	public static implicit operator OriginitePrime(Pack pack) =>
		new OriginitePrime(pack.Name, pack.TotalPrimes, pack.Price, true);
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
