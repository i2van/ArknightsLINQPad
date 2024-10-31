<Query Kind="Program">
  <NuGetReference>Avalonia.Desktop</NuGetReference>
  <NuGetReference>Avalonia.Themes.Fluent</NuGetReference>
  <NuGetReference>Avalonia.Win32.Interoperability</NuGetReference>
  <Namespace>Avalonia</Namespace>
  <Namespace>Avalonia.Controls</Namespace>
  <Namespace>Avalonia.Input</Namespace>
  <Namespace>Avalonia.Interactivity</Namespace>
  <Namespace>Avalonia.Layout</Namespace>
  <Namespace>Avalonia.Styling</Namespace>
  <Namespace>Avalonia.Themes.Fluent</Namespace>
  <Namespace>Avalonia.Threading</Namespace>
  <Namespace>Avalonia.VisualTree</Namespace>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Launch Arknights countdown timers using Hourglass: https://github.com/i2van/hourglass

#nullable enable

//#define DUMP_CONFIG_AND_EXIT

const string TitleOption = "-t ";
const string Hourglass   = nameof(Hourglass);

// TODO: Force the dark theme. Press Shift+F5 and then F5 after changing.
const bool ForceDarkTheme = false;

void Main()
{
	var config = new
	{
		// TODO: Use Hourglass executable placed next to this script.
		UseLocalHourglass = false,

		// TODO: Specify path to the Hourglass executable excluding executable name.
		// Leave it empty if Hourglass path is registered in PATH or App Paths.
		HourglassPath = @"",

		// TODO: Specify timers.
		Timers = SplitTrim("""
			Dormitory 1
			Dormitory 2
			Dormitory 3
			Dormitory 4
			Recruit
			Clue
			Clues
			Skill
			Dualchip
			Gold
			Misc
		"""),

		// TODO: Specify your timers presets.
		TimerPresets = new TimerPreset[]
		{
			new("Clue",    "7h59"),
			new("Recruit", "8h59"),
			new("Clues",   "23h59")
		},

		// TODO: Specify timer options: https://github.com/i2van/hourglass/blob/develop/Hourglass/Resources/Usage.txt
		// Hourglass FAQ: https://github.com/i2van/hourglass/blob/develop/FAQ.md
		Options = string.Join(" ", SplitTrim("""
			-n on
			-a on
			-g on
			-c on
			-v on
			-w minimized
			-i left+title
			-mt on
			-st on
		""")),

		// TODO: Auto clear timers after successfully launching Hourglass.
		AutoClear = true
	}
#if DUMP_CONFIG_AND_EXIT
	.Dump("Config");
	return
#endif
	;

	const double buttonWidth = 150;

	const int verticalSpacing         = 5;
	const int checkBoxVerticalSpacing = 10;
	const int horizontalSpacing       = 10;
	const int buttonHorizontalSpacing = 5;

	const PlacementMode flyoutPlacement = PlacementMode.BottomEdgeAlignedLeft;

	var verticalThickness          = new Thickness(0, verticalSpacing);
	var horizontalThickness        = new Thickness(horizontalSpacing, 0);
	var tabHorizontalThickness     = new Thickness(-horizontalSpacing, 0);
	var buttonHorizontalThickness  = new Thickness(buttonHorizontalSpacing, 0);
	var autoClearCheckBoxThickness = new Thickness(0, -checkBoxVerticalSpacing);

	// Controls.

	var tabControl = new TabControl
	{
		Margin = tabHorizontalThickness
	};

	var timersTextBox = new TextBox
	{
		Margin = verticalThickness
	};

	var launchTimersSplitButton = new SplitButton
	{
		Content = "Launch Timers"
	}
	.SetWidth(buttonWidth);

	var launchHourglassMenuItem = new MenuItem
	{
		Header = $"Launch {Hourglass}"
	};

	var launchCustomTitleMenuItem = new MenuItem
	{
		Header = new TimerPreset("Custom", "Title")
	};

	var aboutMenuItem = new MenuItem
	{
		Header = ExternalLinkHeader("About")
	};

	var aboutHourglassMenuItem = new MenuItem
	{
		Header = ExternalLinkHeader($"About {Hourglass}")
	};

	var hourglassFAQMenuItem = new MenuItem
	{
		Header = ExternalLinkHeader($"{Hourglass} FAQ")
	};

	var hourglassCommandLineMenuItem = new MenuItem
	{
		Header = ExternalLinkHeader($"{Hourglass} Command-line Reference")
	};

	var clearTimersSplitButton = new SplitButton
	{
		Content = "Clear Timers",
		Margin  = buttonHorizontalThickness
	}
	.SetWidth(buttonWidth);

	var autoClearCheckBox = new CheckBox
	{
		Content   = "Auto Clear Timers After Launch",
		IsChecked = config.AutoClear,
		Margin    = autoClearCheckBoxThickness
	};

	var clearTimersMenuFlyout = new MenuFlyout
	{
		Placement = flyoutPlacement
	}
	.AddItems(autoClearCheckBox);

	clearTimersSplitButton.Flyout = clearTimersMenuFlyout;

	var downloadHourglassButton = new HyperlinkButton
	{
		Content = $"Download {Hourglass}"
	}
	.SetWidth(buttonWidth);

	var errorTextBlock = new TextBlock
	{
		Text              = $"{Hourglass} could not be found. Install it and set path to executable in {nameof(config)}.{nameof(config.HourglassPath)} variable.",
		VerticalAlignment = VerticalAlignment.Center
	};

	var errorPanel = new StackPanel
	{
		IsVisible   = false,
		Orientation = Orientation.Horizontal
	}
	.AddChildren
	(
		errorTextBlock,
		downloadHourglassButton
	);

	var launchTimersMenuFlyout = new MenuFlyout
	{
		Placement = flyoutPlacement
	}
	.AddItems(config.TimerPresets.Select(AddTimerPresetMenuItem));

	launchTimersMenuFlyout.AddItems
	(
		launchCustomTitleMenuItem,
		new Separator(),
		launchHourglassMenuItem,
		new Separator(),
		aboutMenuItem,
		new Separator(),
		aboutHourglassMenuItem,
		hourglassFAQMenuItem,
		hourglassCommandLineMenuItem
	);

	launchTimersSplitButton.Flyout = launchTimersMenuFlyout;

	// Events.

	timersTextBox.TextChanged += delegate
	{
		Enable();
	};

	timersTextBox.KeyDown += (_, e) =>
	{
		if(e.Key == Key.Enter)
		{
			LaunchTimers();
		}
	};

	launchTimersSplitButton.Click += delegate
	{
		LaunchTimers();
	};

	void LaunchTimers()
	{
		var args = timersTextBox.Text!.Trim();

		var hourglassCommandLine = args.StartsWith(TitleOption)
			?  $"{config.Options} {args}"
			: $@"{config.Options} {TitleOption}""{((TabItem)tabControl.SelectedItem!).Header}"" {args}";

		if(RunHourglass(hourglassCommandLine) && autoClearCheckBox.IsChecked == true)
		{
			Clear();
		}

		Focus();
	}

	launchTimersSplitButton.Flyout.Closed += FlyoutClosed;

	clearTimersSplitButton.Click += delegate
	{
		Clear();
	};

	clearTimersSplitButton.Flyout.Closed += FlyoutClosed;

	launchCustomTitleMenuItem.Click += delegate
	{
		timersTextBox.Text = @"-t """;
	};

	launchHourglassMenuItem.Click += delegate
	{
		RunHourglass();
	};

	aboutMenuItem.Click += async delegate
	{
		await NavigateLaunchAsync("https://github.com/i2van/ArknightsLINQPad/blob/main/README.md#hourglasslinq");
	};

	aboutHourglassMenuItem.Click += async delegate
	{
		await NavigateHourglassBlobAsync("README.md");
	};

	hourglassFAQMenuItem.Click += async delegate
	{
		await NavigateHourglassBlobAsync("FAQ.md");
	};

	hourglassCommandLineMenuItem.Click += async delegate
	{
		await NavigateHourglassBlobAsync("Hourglass/Resources/Usage.txt");
	};

	tabControl.SelectionChanged += delegate
	{
		timersTextBox.Watermark = $"{((TabItem)tabControl.SelectedItem!).Header!} · {(
			config.Options.Contains("-mt on")
				? @"Specify multiple timers separated by spaces and press Enter. Use double quotation marks for the timers containing spaces, e.g., ""10 Oct 2024"""
				:  "Specify timer and press Enter"
			)}";
	};

	downloadHourglassButton.Click += async delegate
	{
		await NavigateHourglassAsync(downloadHourglassButton, "releases/latest");
	};

	void FlyoutClosed(object? sender, EventArgs e) =>
		Focus();

	// UI.

	new StackPanel
	{
		Margin = horizontalThickness
	}
	.AddChildren
	(
		tabControl.AddItems(config.Timers.Select(static timer => new TabItem{ Header = timer })),
		new StackPanel()
		.AddChildren
		(
			timersTextBox,
			errorPanel,
			new StackPanel
			{
				Orientation = Orientation.Horizontal
			}
			.AddChildren
			(
				launchTimersSplitButton,
				clearTimersSplitButton
			)
		)
	).Dump(Hourglass);

	RunOnce(Enable);
	RunOnce(AddRootHandles, 250, 1);

	void AddRootHandles()
	{
		if(launchTimersSplitButton.GetVisualRoot() is Control visualRoot)
		{
			visualRoot.AddHandler(InputElement.PointerPressedEvent, PointerPressedEventHandler, RoutingStrategies.Tunnel);
		}

		void PointerPressedEventHandler(object? sender, RoutedEventArgs e)
		{
			var control = e.Source as Control;

			if(	NotA<TextBox>() &&
				NotA<Button>() &&
				NotA<SplitButton>())
			{
				Focus();
			}

			bool NotA<T>() where T: Control =>
				control?.FindAncestorOfType<T>() is null;
		}
	}

	void Enable()
	{
		var text = timersTextBox.Text;

		launchTimersSplitButton.EnablePrimaryButton(!string.IsNullOrWhiteSpace(text));
		clearTimersSplitButton .EnablePrimaryButton(!string.IsNullOrEmpty(text));

		Focus();
	}

	bool RunHourglass(string args = "")
	{
		try
		{
			errorPanel.IsVisible = false;

			var hourglass = Path.Combine(
				config.UseLocalHourglass
					? Path.GetDirectoryName(Util.CurrentQueryPath) ?? string.Empty
					: config.HourglassPath,
				Hourglass);

			Process.Start(hourglass, args);

			return true;
		}
		catch(Win32Exception)
		{
			errorPanel.IsVisible = true;

			return false;
		}
	}

	MenuItem AddTimerPresetMenuItem(TimerPreset timerPreset)
	{
		var menuItem = new MenuItem
		{
			Header = timerPreset
		};

		menuItem.Click += delegate
		{
			timersTextBox.Text = timerPreset.ToHourglassCommandLine();

			LaunchTimers();
		};

		return menuItem;
	}

	void Clear() =>
		timersTextBox.Clear();

	void Focus()
	{
		RunOnce(Execute);

		void Execute()
		{
			timersTextBox.Focus();
			timersTextBox.CaretIndex = timersTextBox.Text?.Length ?? 0;
		}
	}

	async Task NavigateHourglassAsync<T>(T button, string uri) where T : Control
	{
		try
		{
			button.IsEnabled = false;

			await TopLevel.GetTopLevel(button)!.Launcher.LaunchUriAsync(
				uri.StartsWith("http")
					? new Uri(uri)
					: new Uri($"https://github.com/i2van/hourglass/{uri}"));
		}
		finally
		{
			button.IsEnabled = true;

			Focus();
		}
	}

	Task NavigateHourglassBlobAsync(string uri) =>
		NavigateLaunchAsync($"blob/develop/{uri}");

	Task NavigateLaunchAsync(string uri) =>
		NavigateHourglassAsync(launchTimersSplitButton, uri);

	static string ExternalLinkHeader(string name) =>
		"🔗 " + name;

	static void RunOnce(Action action, int retryMs = 25, int retries = 11)
	{
		DispatcherTimer.RunOnce(WithRetry, TimeSpan.FromMilliseconds(retryMs), DispatcherPriority.Background);

		void WithRetry()
		{
			try
			{
				action();
			}
			catch
			{
				if(retries > 0)
				{
					RunOnce(action, retryMs, --retries);
				}
			}
		}
	}

	static IReadOnlyCollection<string> SplitTrim(string s) =>
		s.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

sealed record TimerPreset(string Name, string Timer)
{
	public string ToHourglassCommandLine() =>
		$@"{TitleOption}""{Name}"" {Timer}";

	public override string ToString() =>
		$"{Name} ⏰ {Timer}";
}

void OnInit()
{
	AppBuilder
		.Configure<Application>()
		.UsePlatformDetect()
		.SetupWithoutStarting();

	Application.Current!.Styles.Add(new FluentTheme());

	if (Util.IsDarkThemeEnabled || ForceDarkTheme)
	{
		Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
	}
}

static class AvaloniaExtensions
{
	public static T SetWidth<T>(this T contentControl, double width) where T : ContentControl
	{
		contentControl.MinWidth = width;
		contentControl.HorizontalContentAlignment = HorizontalAlignment.Center;

		return contentControl;
	}

	public static void EnablePrimaryButton(this SplitButton splitButton, bool enable = true) =>
		splitButton.Uncapsulate()._primaryButton.IsEnabled = enable;

	public static T AddChildren<T>(this T panel, params Control[] children) where T : Panel
	{
		panel.Children.AddRange(children);

		return panel;
	}

	public static T AddItems<T>(this T itemsControl, params Control[] items) where T : ItemsControl =>
		itemsControl.AddItems((IEnumerable<Control>)items);

	public static T AddItems<T>(this T itemsControl, IEnumerable<Control> items) where T : ItemsControl
	{
		foreach (var child in items)
		{
			itemsControl.Items.Add(child);
		}

		return itemsControl;
	}

	public static MenuFlyout AddItems(this MenuFlyout menuFlyout, params Control[] items) =>
		menuFlyout.AddItems((IEnumerable<Control>)items);

	public static MenuFlyout AddItems(this MenuFlyout menuFlyout, IEnumerable<Control> items)
	{
		foreach (var child in items)
		{
			menuFlyout.Items.Add(child);
		}

		return menuFlyout;
	}
}
