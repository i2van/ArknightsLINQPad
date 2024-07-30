<Query Kind="Program">
  <Namespace>LINQPad.Controls</Namespace>
</Query>

// Arknights operations strategies.

#nullable enable

#load "./lib/Context.linq"
#load "./lib/Extensions.linq"
#load "./lib/WikiHyperlinq.linq"

void Main()
{
	// TODO: Add operations.
	var operations = new string[]
	{
		// Operation.
		//"H8-4",
	};

	const string title = "Arknights Operations Strategies";

	var operationsStrategies = operations
		.Select(static (o, i) =>
		new
		{
			ID        = i + 1,
			Operation = new WikiHyperlinq(o),
			Strategy  = new Hyperlinq($"https://www.youtube.com/results?search_query={Uri.EscapeDataString(o)}+Arknights", "YouTube")
		})
		.ToArray();

	(operationsStrategies.Any() ? (object)operationsStrategies : "No operations strategies found.").Dump(title);

	title.AsHyperlink(new WikiHyperlinq("Operation").Uri);
}
