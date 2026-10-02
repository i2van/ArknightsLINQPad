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
  <Namespace>System.Collections.ObjectModel</Namespace>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Launch Arknights countdown timers using Hourglass: https://github.com/i2van/hourglass

#nullable enable

//#define DUMP_CONFIG_AND_EXIT

const string TitleOption = "-t ";
const string LoopOption  = "-l ";
const string On          = "on";
const string Off         = "off";

const string Any         = nameof(Any);
const string Clue        = nameof(Clue);
const string Dormitory   = nameof(Dormitory);
const string Reception   = nameof(Reception);
const string Recruitment = nameof(Recruitment);
const string Training    = nameof(Training);

const string Hourglass   = nameof(Hourglass);

void Main()
{
	Util.HideEditor();

	var config = new
	{
		// TODO: Use Hourglass executable placed next to this script.
		UseLocalHourglass = false,

		// TODO: Specify path to the Hourglass executable excluding executable name.
		// Leave it empty if Hourglass path is registered in PATH or App Paths.
		HourglassPath = @"",

		// TODO: Specify timers.
		Timers = SplitTrim($"""
			{Dormitory} 1
			{Dormitory} 2
			{Dormitory} 3
			{Dormitory} 4
			{Recruitment}
			{Clue}
			{Reception}
			{Training}
			{Any}
		"""),

		// TODO: Specify timer help URL.
		TimerHelpUrl = "https://arknights.wiki.gg/wiki",

		// TODO: Specify timer help URIs.
		TimerHelpUris = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
		{
			[Dormitory] = "Dormitory",
			[Reception] = "Reception_Room",
			[Training]  = "Training_Room"
		}),

		// TODO: Specify your timers presets.
		TimerPresets = new TimerPreset[]
		{
			new(Recruitment, "8h59"),
			new(Reception,   "23h59"),
			new(Clue,        "7h59")
		},

		// TODO: Specify timer options: https://github.com/i2van/hourglass/blob/main/Hourglass/Resources/Usage.txt
		// Hourglass FAQ: https://github.com/i2van/hourglass/blob/main/FAQ.md
		Options = string.Join(" ", SplitTrim($"""
			-n {On}
			-a {On}
			-g {On}
			-c {On}
			-v {On}
			-w minimized
			-i left+title
			-mt {On}
			-st {On}
		""")),

		// TODO: Use timer prefix.
		UseTimerPrefix = false,

		// TODO: Timer prefix.
		TimerPrefix = "Arknights: ",

		// TODO: Auto clear timers after successfully launching Hourglass.
		AutoClear = true
	}
#if DUMP_CONFIG_AND_EXIT
	.Dump("Config");
	return
#endif
	;

	const double buttonWidth     = 150;
	const double minColumnWidth  = buttonWidth / 2;
	const double titleWidthRatio = 0.3;

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

	var titleHistory = new SortedSet<string>(StringComparer.CurrentCultureIgnoreCase);

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

	var aboutSelectionMenuItem = new MenuItem
	{
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

	var loopCheckBox = new CheckBox
	{
		Content   = "Loop",
		IsVisible = false,
		Margin    = buttonHorizontalThickness
	};

	var titleTextBox = new TextBox
	{
		Watermark = $"Title · {Any} if not set",
		IsVisible = false,
		Margin    = verticalThickness
	};

	var titleSplitter = new GridSplitter
	{
		IsVisible       = false,
		Width           = buttonHorizontalSpacing,
		Margin          = verticalThickness,
		ResizeDirection = GridResizeDirection.Columns
	};

	var initialTimersColumnWidth = new GridLength(1 - titleWidthRatio, GridUnitType.Star);
	var initialTitleColumnWidth  = new GridLength(titleWidthRatio, GridUnitType.Star);

	var timersColumn     = new ColumnDefinition { Width = initialTimersColumnWidth, MinWidth = minColumnWidth };
	var titleColumn      = new ColumnDefinition(0, GridUnitType.Pixel);
	var titleColumnWidth = initialTitleColumnWidth;

	var timersPanel = new Grid
	{
		ColumnDefinitions = new ColumnDefinitions { timersColumn, new(GridLength.Auto), titleColumn }
	}
	.AddChildren
	(
		timersTextBox,
		titleSplitter,
		titleTextBox
	);

	Grid.SetColumn(titleSplitter, 1);
	Grid.SetColumn(titleTextBox,  2);

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
		aboutSelectionMenuItem,
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

	titleTextBox.KeyDown += (_, e) =>
	{
		if(e.Key == Key.Enter && !string.IsNullOrWhiteSpace(timersTextBox.Text))
		{
			LaunchTimers();
		}
	};

	titleTextBox.TextChanged += delegate
	{
		RunOnce(EnableButtons);
	};

	titleSplitter.DoubleTapped += delegate
	{
		titleColumnWidth   = initialTitleColumnWidth;
		timersColumn.Width = initialTimersColumnWidth;
		titleColumn.Width  = initialTitleColumnWidth;
	};

	launchTimersSplitButton.Click += delegate
	{
		LaunchTimers();
	};

	loopCheckBox.Click += delegate
	{
		Focus();
	};

	void LaunchTimers()
	{
		var args = timersTextBox.Text!.Trim();
		var loopOption = $"{LoopOption}{(loopCheckBox.IsChecked == true ? On : Off)}";

		var useCustomTitle = titleTextBox.IsVisible && !string.IsNullOrWhiteSpace(titleTextBox.Text);

		var title = useCustomTitle
			? titleTextBox.Text!.Trim()
			: GetSelectedItemHeader();

		var hasTitleOption = args.StartsWith(TitleOption);

		var hourglassCommandLine = hasTitleOption
			?  $"{config.Options} {loopOption} {args}"
			: $@"{config.Options} {loopOption} {TitleOption}""{(config.UseTimerPrefix ? config.TimerPrefix : string.Empty)}{title}"" {args}";

		if(RunHourglass(hourglassCommandLine))
		{
			if(useCustomTitle && !hasTitleOption)
			{
				RememberTitle(title);
			}

			if(autoClearCheckBox.IsChecked == true)
			{
				Clear();
			}
		}

		Focus();
	}

	launchTimersSplitButton.Flyout.Opened += FlyoutOpened;
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

	aboutSelectionMenuItem.Click += async delegate
	{
		await NavigateLaunchAsync((string)aboutSelectionMenuItem.Tag!);
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
		var selectedItemHeader = GetSelectedItemHeader();
		loopCheckBox.IsChecked = false;
		loopCheckBox.IsVisible = selectedItemHeader == Any;
		titleTextBox.Clear();
		titleTextBox.IsVisible  = loopCheckBox.IsVisible;
		titleSplitter.IsVisible = titleTextBox.IsVisible;

		if(titleTextBox.IsVisible)
		{
			titleColumn.MinWidth = minColumnWidth;
			titleColumn.Width    = titleColumnWidth;
		}
		else
		{
			if(titleColumn.Width.Value > 0)
			{
				titleColumnWidth = titleColumn.Width;
			}

			titleColumn.MinWidth = 0;
			titleColumn.Width    = new GridLength(0);
		}

		timersTextBox.Watermark = $"{selectedItemHeader} · {(
			config.Options.Contains("-mt on")
				? @"Specify multiple timers separated by spaces and press Enter. Use double quotation marks for the timers containing spaces, e.g., ""10 Oct 2024"""
				:  "Specify timer and press Enter"
			)}";
	};

	downloadHourglassButton.Click += async delegate
	{
		await NavigateHourglassAsync(downloadHourglassButton, "releases/latest");
	};

	void FlyoutOpened(object? sender, EventArgs e)
	{
		var title = GetSelectedItemHeader();

		aboutSelectionMenuItem.Header = ExternalLinkHeader($"About {title}");
		aboutSelectionMenuItem.Tag    = $"{config.TimerHelpUrl}/{(config.TimerHelpUris.Select(titleUri => title.StartsWith(titleUri.Key) ? titleUri.Value : null).FirstOrDefault(static uri => uri is not null) ?? title)}";
	}

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
			timersPanel,
			errorPanel,
			new StackPanel
			{
				Orientation = Orientation.Horizontal
			}
			.AddChildren
			(
				launchTimersSplitButton,
				clearTimersSplitButton,
				loopCheckBox
			)
		)
	).Dump(Hourglass);

	RunOnce(Enable);
	RunOnce(AddTitleHistoryContextMenu);
	RunOnce(AddRootHandles, 250, 1);

	void AddRootHandles()
	{
		if(TopLevel.GetTopLevel(launchTimersSplitButton) is Interactive interactiveRoot)
		{
			interactiveRoot.AddHandler(InputElement.PointerPressedEvent, PointerPressedEventHandler, RoutingStrategies.Tunnel);
		}

		void PointerPressedEventHandler(object? sender, RoutedEventArgs e)
		{
			var control = e.Source as Control;

			if(	NotA<TextBox>() &&
				NotA<Button>() &&
				NotA<SplitButton>() &&
				NotA<GridSplitter>())
			{
				Focus();
			}

			bool NotA<T>() where T: Control =>
				control?.FindAncestorOfType<T>() is null;
		}
	}

	void AddTitleHistoryContextMenu()
	{
		// The themed edit context flyout instance is shared by all text boxes.
		var titleContextMenuFlyout = (MenuFlyout)titleTextBox.ContextFlyout!;
		var titleHistoryMenuItems  = new List<Control>();

		titleContextMenuFlyout.Opening += delegate
		{
			foreach(var menuItem in titleHistoryMenuItems)
			{
				titleContextMenuFlyout.Items.Remove(menuItem);
			}

			titleHistoryMenuItems.Clear();

			if(titleContextMenuFlyout.Target != titleTextBox || titleHistory.Count == 0)
			{
				return;
			}

			titleHistoryMenuItems.AddRange(titleHistory.Select(CreateTitleHistoryMenuItem));
			titleHistoryMenuItems.Add(new Separator());

			for(var index = 0; index < titleHistoryMenuItems.Count; index++)
			{
				titleContextMenuFlyout.Items.Insert(index, titleHistoryMenuItems[index]);
			}
		};
	}

	void Enable()
	{
		EnableButtons();

		Focus();
	}

	void EnableButtons()
	{
		var text = timersTextBox.Text;

		launchTimersSplitButton.EnablePrimaryButton(!string.IsNullOrWhiteSpace(text));
		clearTimersSplitButton .EnablePrimaryButton(!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(titleTextBox.Text));
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

	MenuItem CreateTitleHistoryMenuItem(string title)
	{
		var menuItem = new MenuItem
		{
			Header = new TextBlock { Text = title }
		};

		menuItem.Click += delegate
		{
			titleTextBox.Text = title;

			Focus();
		};

		return menuItem;
	}

	void RememberTitle(string title)
	{
		titleHistory.Remove(title);
		titleHistory.Add(title);
	}

	string GetSelectedItemHeader() =>
		(string)((TabItem)tabControl.SelectedItem!).Header!;

	void Clear()
	{
		timersTextBox.Clear();
		titleTextBox.Clear();
		loopCheckBox.IsChecked = false;
	}

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
		NavigateLaunchAsync($"blob/main/{uri}");

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

	ApplyTheme();

	Util.ThemeChanged += delegate
	{
		Dispatcher.UIThread.Post(ApplyTheme);
	};

	static void ApplyTheme() =>
		Application.Current!.RequestedThemeVariant = Util.IsDarkThemeEnabled
			? ThemeVariant.Dark
			: ThemeVariant.Light;
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
