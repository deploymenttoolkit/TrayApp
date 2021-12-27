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

			_logger.Trace("Loading message ...");
			using var document = File.Open("./Assets/Upgrade/en.rtf", FileMode.Open);
			RichTextUpgradeMessage.Selection.Load(document, DataFormats.Rtf);
		}

		private void UpgradeButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ScheduleButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			App.MainWindow.WindowState = WindowState.Minimized;
		}
	}
}
