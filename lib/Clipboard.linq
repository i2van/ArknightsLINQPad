<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#nullable enable

static class Clipboard
{
	public static Task SetClipboard(this IEnumerable<string> text) =>
		STATask.Run(() => System.Windows.Forms.Clipboard.SetText(string.Join(Environment.NewLine, text)));
}

static class STATask
{
	public static Task Run(Action action)
	{
		var tcs = new TaskCompletionSource();

		var thread = new Thread(() =>
		{
			try
			{
				action();
				tcs.SetResult();
			}
			catch (Exception ex)
			{
				tcs.SetException(ex);
			}
		});

		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();

		return tcs.Task;
	}
}
