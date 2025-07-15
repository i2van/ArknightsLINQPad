<Query Kind="Program" />

// Arknights operators modules missions.

#nullable enable

#load "./lib/Context.linq"
#load "./lib/Extensions.linq"
#load "./lib/Images.linq"
#load "./lib/Operators.linq"
#load "./lib/Parsable.linq"
#load "./lib/WikiHyperlinq.linq"

#define DUMP_UNKNOWN_OPERATORS

/*
#define DUMP_OPERATORS
#define DUMP_ALL_OPERATOR_MODULES
//*/

void Main()
{
	// TODO: Add operator(s) with (optionally) X or Y modules.
	var operators = new OperatorModules("""
		// name <tab> type (X, Y, D or empty for all)
		/*** Vanguard ***/


		/*** Guard ***/


		/*** Defender ***/


		/*** Sniper ***/


		/*** Caster ***/


		/*** Medic ***/


		/*** Supporter ***/


		/*** Specialist ***/
	""")
#if DUMP_OPERATORS
		.OrderBy(static op => op.Name)
		.Dump("Operators")
#endif
	;

	var operatorModules = OperatorData.Value
#if !DUMP_ALL_OPERATOR_MODULES
		.OnlyFor(operators)
#endif
		.OrderBy(static v => v.Name)
		.GroupBy(static v => v.Stage)
		.Select( static v => new
		{
			Stage     = v.Key,
			Names     = v.JoinStrings(static v => v.Name),
			Operators = v.Select(Pass),
			Stars     = v.JoinStrings(static v => v.Stars),
			MaxStars  = v.Max(static v => v.Stars),
			Total     = v.Count()
		})
		.OrderByDescending(static v => v.Total)
		.ThenByDescending( static v => v.MaxStars)
		.ThenBy(static v => v.Names)
		.Select((v, i) => HighlightIf(v.Total > 1, new
		{
			ID          = i + 1,
			Stage       = new WikiHyperlinq(v.Stage),
			Operator    = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Uri, v.Name))),
			Class       = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq(v.Class))),
			v.Stars,
			Module      = VerticalRun(v.Operators.Select(static v => new WikiHyperlinq($"{v.Uri}#Operator_Modules", v.Module))),
			v.Total,
			Paradox     = VerticalRun(v.Operators.Select(static v => v.Paradox ? GetParadoxHyperlinq(v.Name) : Empty)),
			E2Materials = VerticalRun(v.Operators.Select(static v => GetMaterials(v.Uri, v.E2Materials)))
		}))
		.ToArray();

#if DUMP_UNKNOWN_OPERATORS
	var unknownOperators = operators.Select(static op => op.Name)
		.Except(OperatorData.Value.Select(static om => om.Name), StringComparer.OrdinalIgnoreCase)
		.Select(static (name, i) => new
		{
			ID      = i + 1,
			Name    = new WikiHyperlinq(name),
			Paradox = GetParadoxHyperlinq(name)
		})
		.ToArray();

	if(unknownOperators.Any())
	{
		const string unknownOperatorsTitle = "Unknown Operators";
		unknownOperators.Dump(unknownOperatorsTitle);
		unknownOperatorsTitle.AsHyperlink(new WikiHyperlinq("Operator").Uri);
	}
#endif

	const string title = "Joined Operators Modules";

	Context = new();

	HorizontalRun(false,
		(operatorModules.Any() ? (object)operatorModules : "No operators modules found."),
		Context.Containers.Image
	).Dump(title);

	new DumpContainer(){ Style = $"height: {DumpContext.Document.MarginBottom}" }.Dump();

	title.AsHyperlink(new WikiHyperlinq("Operator_Module", "List").Uri);

	static T Pass<T>(T t) => t;
}

static object GetParadoxHyperlinq(string name) =>
	VideoHostings.YouTube.GetSearchHyperlinqs(name, "paradox", "simulation");

static object GetMaterials(string op, string materials)
{
	const StringSplitOptions StringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

	return HorizontalRun(false,
		materials
			.Split(OperatorData.MaterialSeparator, StringSplitOptions)
			.SelectMany(GetMaterialLinks)
			.InsertAfterEach(3, OperatorData.MaterialSeparator)
	);

	IEnumerable<object> GetMaterialLinks(string materialCount)
	{
		var material = materialCount.Split(OperatorData.CountSeparator, StringSplitOptions);

		yield return ItemImage.GetHyperlink(material.First());
		yield return OperatorData.CountSeparator;
		yield return new WikiHyperlinq($"{op}#Promotion", material.Last());
	}
}

static partial class CollectionExtensions
{
	public static IEnumerable<Operator> OnlyFor(this IEnumerable<Operator> operatorModules, IEnumerable<OperatorModule> operators)
	{
		var stringComparer   = StringComparer.OrdinalIgnoreCase;
		var stringComparison = StringComparison.OrdinalIgnoreCase;

		var operatorsModuleTypes = operators
			.ToLookup(static op => op.Name, static op => op.ModuleType, stringComparer)
			.ToDictionary(static g => g.Key, static g => g, stringComparer);

		return operatorModules.Where(operatorModule =>
				operatorsModuleTypes.TryGetValue(operatorModule.Name, out var moduleTypes) &&
				moduleTypes.Any(moduleType => HasModule(moduleType, operatorModule.Module)));

		bool HasModule(ModuleType moduleType, string module) =>
			moduleType switch
			{
				ModuleType.All or ModuleType.Unknown => true,
				ModuleType.X when module.EndsWith("X", stringComparison) => true,
				ModuleType.Y when module.EndsWith("Y", stringComparison) => true,
				ModuleType.D when module.EndsWith("Δ", stringComparison) => true,
				_ => false
			};
	}
}
