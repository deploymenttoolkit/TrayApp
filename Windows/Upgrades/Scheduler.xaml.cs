using DeploymentToolkit.Modals.Settings.Tray;
using DeploymentToolkit.TrayApp.Extensions;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DeploymentToolkit.TrayApp.Windows.Upgrades
{
	/// <summary>
	/// Interaction logic for Scheduler.xaml
	/// </summary>
	public partial class Scheduler : Page, IPageWindowSettings
	{
		public string PageName => "UpgradeSchedule";
		public int? RequestedHeight => 450;
		public int? RequestedWidth => 800;
		public bool AllowMinimize => true;
		public bool AllowClose => false;

		private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		public Scheduler()
		{
			_logger.Trace("Initializing ...");
			InitializeComponent();

			_logger.Trace("Applying theme ...");
			this.ApplyButtonThemes(App.Settings.BrandingSettings.ButtonSettings, new[] { UpgradeButton, ScheduleButton, MinimizeButton });

			_logger.Trace("Setting language ...");
			var language = LanguageManager.Language;
			UpgradeButton.Content = language.UpgradePrompt_ButtonStartNow;
			ScheduleButton.Content = language.UpgradePrompt_ButtonSchedule;
			MinimizeButton.Content = language.UpgradePrompt_ButtonMinimize;

			_logger.Trace("Loading message ...");
			if(string.IsNullOrEmpty(language.PathToUpgradeRtf))
			{
				throw new FileNotFoundException("No language found to display message in :(");
			}
			using var document = File.Open(language.PathToUpgradeRtf, FileMode.Open);
			RichTextUpgradeMessage.Selection.Load(document, DataFormats.Rtf);
		}

		private void UpgradeButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ScheduleButton_Click(object sender, RoutedEventArgs e)
		{
			App.NavigateTo(new DateTimePicker());
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			App.MainWindow.WindowState = WindowState.Minimized;
		}
	}
}
