<Query Kind="Statements" />

#nullable enable

#load "./Extensions.linq"
#load "./Parsable.linq"

record Operator(string Name, string Class, int Stars, string Module, string Stage, string E2Materials, bool Paradox)
{
	public string Uri { get; } = Name.Replace(" (", "/").Replace(")", string.Empty);
}

class Operators : Parsable<Operator>
{
	private const string Name        = nameof(Name);
	private const string Class       = nameof(Class);
	private const string Stars       = nameof(Stars);
	private const string Stage       = nameof(Stage);
	private const string Module      = nameof(Module);
	private const string Paradox     = nameof(Paradox);
	private const string E2Materials = nameof(E2Materials);

	protected override string Regex { get; } = $@"^(?<{Name}>[^\t]+)\t+(?<{Class}>[^\t]+)\t+(?<{Stars}>[1-6])\t+(?<{Stage}>[^\t]+)\t+(?<{Module}>[^\t]+)\t+(?<{E2Materials}>[^\t]+)(\t+(?<{Paradox}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = "Use OperatorModulesParser.linq script to update.";

	protected override Operator Create(Match match) =>
		new
		(
			GetString(match, Name),
			GetString(match, Class),
			GetNumber(match, Stars),
			GetString(match, Stage),
			GetString(match, Module),
			GetString(match, E2Materials),
			!string.IsNullOrWhiteSpace(GetString(match, Paradox))
		);

	public Operators(string operatorModules)
		: base(operatorModules)
	{
	}
}

enum ModuleType
{
	Unknown,
	All,
	X,
	Y,
	D
}

record OperatorModule(string Name, ModuleType ModuleType);

class OperatorModules : Parsable<OperatorModule>
{
	private const string Name   = nameof(Name);
	private const string Module = nameof(Module);

	protected override string Regex { get; } = $@"^(?<{Name}>[^\t]+)(\t+(?<{Module}>[^\t]+))?$";
	protected override string ErrorMessage { get; } = "Format: name <tab> type (X, Y, D or empty for all)";

	protected override OperatorModule Create(Match match)
	{
		var moduleType = GetString(match, Module).ToLowerInvariant() switch
		{
			"x" => ModuleType.X,
			"y" => ModuleType.Y,
			"d" => ModuleType.D,
			""  => ModuleType.All,
			_   => ModuleType.Unknown
		};

		if(moduleType == ModuleType.Unknown)
		{
			Throw(match.Value, ErrorMessage);
		}

		return new
		(
			GetString(match, Name),
			moduleType
		);
	}

	public OperatorModules(string operators)
		: base(operators)
	{
	}
}

static partial class OperatorData
{
	public const char MaterialSeparator = '｜';
	public const char CountSeparator    = '❂';

	// TODO: Run OperatorModulesParser.linq and paste from clipboard to the data/Operators.tsv file.
	public static readonly Operators Value = new("Operators.tsv".Load());
}
