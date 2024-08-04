<Query Kind="Program">
  <NuGetReference>Avalonia.Desktop</NuGetReference>
  <NuGetReference>Avalonia.Themes.Fluent</NuGetReference>
  <NuGetReference>Avalonia.Win32.Interoperability</NuGetReference>
  <Namespace>Avalonia</Namespace>
  <Namespace>Avalonia.Controls</Namespace>
  <Namespace>Avalonia.Input</Namespace>
  <Namespace>Avalonia.Layout</Namespace>
  <Namespace>Avalonia.Styling</Namespace>
  <Namespace>Avalonia.Themes.Fluent</Namespace>
  <Namespace>Avalonia.Threading</Namespace>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Launch Arknights countdown timers using Hourglass: https://github.com/i2van/hourglass

#nullable enable

//#define DUMP_CONFIG_AND_EXIT

const string TitleOption = "-t ";

void Main()
{
	var config = new
	{
		// TODO: Specify path to the Hourglass executable.
		Hourglass = @"Hourglass",

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
			new("Recruit", "8h59"),
			new("Clues",   "23h59")
		},

		// TODO: Specify timer options: https://github.com/i2van/hourglass/blob/develop/Hourglass/Resources/Usage.txt
		// Hourglass FAQ: https://chris.dziemborowicz.com/apps/hourglass/#faq
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

	const double buttonWidth = 140;

	const int verticalSpacing   = 5;
	const int horizontalSpacing = 10;

	var verticalThickness       = new Thickness(0, verticalSpacing);
	var horizontalThickness     = new Thickness(horizontalSpacing, 0);
	var halfHorizontalThickness = new Thickness(0, verticalSpacing/2);
	var tabHorizontalThickness  = new Thickness(-horizontalSpacing, 0);

	// Controls.

	var tabControl = new TabControl
	{
		Margin = tabHorizontalThickness
	};

	var timersTextBox = new TextBox
	{
		Margin = verticalThickness
	};

	var launchTimersButton = new SplitButton
	{
		Content   = "Launch Timers"
	}.SetWidth(buttonWidth);

	var timerPresetsMenuFlyout = new MenuFlyout
	{
		Placement = PlacementMode.BottomEdgeAlignedLeft
	}.AddItems(config.TimerPresets.Select(AddTimerPresetMenuItem));

	launchTimersButton.Flyout = timerPresetsMenuFlyout;

	var clearTimersButton = new Button
	{
		Content   = "Clear Timers",
		Margin    = horizontalThickness
	}.SetWidth(buttonWidth);

	var hourglassButton = new Button
	{
		Content   = "Launch Hourglass",
		Margin    = horizontalThickness
	}.SetWidth(buttonWidth);

	var downloadHourglassButton = new Button
	{
		Content   = "Download Hourglass",
		Margin    = horizontalThickness
	}.SetWidth(buttonWidth);

	var hourglassFAQButton = new Button
	{
		Content   = "Hourglass FAQ",
		Margin    = halfHorizontalThickness,
	}.SetWidth(buttonWidth);

	var autoClearCheckBox = new CheckBox
	{
		Content   = "Auto clear timers",
		IsChecked = config.AutoClear
	};

	var errorTextBlock = new TextBlock
	{
		Text              = "Hourglass could not be found. Install it and set path to executable in config.Hourglass variable.",
		VerticalAlignment = VerticalAlignment.Center
	};

	var errorPanel = new StackPanel
	{
		IsVisible   = false,
		Orientation = Orientation.Horizontal
	}
	.AddChildren
	(
		new TextBlock
		{
			Text              = $"Hourglass could not be found. Install it and specify path to executable in {nameof(config)}.{nameof(config.Hourglass)} variable.",
			VerticalAlignment = VerticalAlignment.Center
		},
		downloadHourglassButton
	);

	// Events.

	timersTextBox.TextChanged += delegate
	{
		Enable();
	};

	timersTextBox.KeyDown += (s, e) =>
	{
		if(e.Key == Key.Enter)
		{
			launchTimersButton.PerformPrimaryClick();
		}
	};

	launchTimersButton.Click += delegate
	{
		if(string.IsNullOrWhiteSpace(timersTextBox.Text))
		{
			launchTimersButton.PerformSecondaryClick();
			return;
		}

		var args = timersTextBox.Text.Trim();

		var hourglassCommandLine = args.StartsWith(TitleOption)
			?  $"{config.Options} {args}"
			: $@"{config.Options} {TitleOption}""{((TabItem)tabControl.SelectedItem!).Header}"" {args}";

		if(RunHourglass(hourglassCommandLine) && autoClearCheckBox.IsChecked == true)
		{
			Clear();
		}

		SelectAll();
	};

	launchTimersButton.Flyout.Closed += delegate
	{
		SelectAll();
	};

	clearTimersButton.Click += delegate
	{
		Clear();
	};

	hourglassButton.Click += delegate
	{
		RunHourglass();

		SelectAll();
	};

	hourglassFAQButton.Click += async delegate
	{
		await NavigateAsync(hourglassFAQButton, "https://chris.dziemborowicz.com/apps/hourglass/#faq");
	};

	tabControl.SelectionChanged += delegate
	{
		timersTextBox.Watermark = (string)((TabItem)tabControl.SelectedItem!).Header!;

		SelectAll();
	};

	downloadHourglassButton.Click += async delegate
	{
		await NavigateAsync(downloadHourglassButton, "https://github.com/i2van/hourglass/releases/latest");
	};

	// UI.

	new StackPanel
	{
		Margin = horizontalThickness
	}
	.AddChildren
	(
		tabControl.AddItems
		(
			config.Timers.Select(static timer => new TabItem{ Header = timer })
		),
		new StackPanel()
			.AddChildren
			(
				timersTextBox,
				errorPanel,
				new StackPanel { Orientation = Orientation.Horizontal }
					.AddChildren
					(
						launchTimersButton,
						clearTimersButton,
						autoClearCheckBox,
						hourglassButton,
						hourglassFAQButton
					)
			)
	).Dump("Hourglass");

	Enable();

	bool RunHourglass(string args = "")
	{
		try
		{
			errorPanel.IsVisible = false;

			Process.Start(config.Hourglass, args);

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

			launchTimersButton.PerformPrimaryClick();
		};

		return menuItem;
	}

	void Enable()
	{
		clearTimersButton.IsEnabled = !string.IsNullOrEmpty(timersTextBox.Text);

		Focus();
	}

	void Focus() =>
		timersTextBox.Focus();

	void Clear() =>
		timersTextBox.Clear();

	void SelectAll()
	{
		RunOnce(Execute);

		void Execute()
		{
			Focus();

			timersTextBox.SelectAll();
		}
	}

	async Task NavigateAsync<T>(T button, string uri) where T : Button
	{
		try
		{
			button.IsEnabled = false;

			await TopLevel.GetTopLevel(button)!.Launcher.LaunchUriAsync(new Uri(uri));
		}
		finally
		{
			button.IsEnabled = true;

			SelectAll();
		}
	}

	static void RunOnce(Action action) =>
		DispatcherTimer.RunOnce(action, TimeSpan.FromMilliseconds(250), DispatcherPriority.Background);

	static string[] SplitTrim(string s) =>
		s.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

record TimerPreset(string Name, string Timer)
{
	public string ToHourglassCommandLine() =>
		$@"{TitleOption}""{Name}"" {Timer}";

	public override string ToString() =>
		$"{Name} ⏲ {Timer}";
}

void OnInit()
{
	AppBuilder
		.Configure<Application>()
		.UsePlatformDetect()
		.SetupWithoutStarting();

	Application.Current!.Styles.Add(new FluentTheme());

	if (Util.IsDarkThemeEnabled)
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

	public static void PerformPrimaryClick(this SplitButton splitButton) =>
		splitButton.Uncapsulate()._primaryButton.PerformClick();

	public static void PerformSecondaryClick(this SplitButton splitButton) =>
		splitButton.Uncapsulate()._secondaryButton.PerformClick();

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

	public static MenuFlyout AddItems(this MenuFlyout menuFlyout, IEnumerable<Control> items)
	{
		foreach (var child in items)
		{
			menuFlyout.Items.Add(child);
		}

		return menuFlyout;
	}
}
