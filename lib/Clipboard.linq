<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#nullable enable

static class Clipboard
{
	public static Task SetClipboard(this IEnumerable<string> text)
	{
		return STATask.Run(() => Try(() => System.Windows.Forms.Clipboard.SetText(string.Join(Environment.NewLine, text))));

		static void Try(Action action)
		{
			const int MaxTries = 3;

			var tryNumber = 0;

			while(tryNumber++ < MaxTries)
			{
				try
				{
					action();
					return;
				}
				catch
				{
					if(tryNumber >= MaxTries)
					{
						throw;
					}

					Thread.Sleep(TimeSpan.FromSeconds(tryNumber));
					continue;
				}
			}
		}
	}
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
