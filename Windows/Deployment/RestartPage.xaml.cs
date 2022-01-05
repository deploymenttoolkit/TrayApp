using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.Modals.Settings.Tray;
using DeploymentToolkit.TrayApp.Extensions;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace DeploymentToolkit.TrayApp.Windows.Deployment
{
	/// <summary>
	/// Interaction logic for RestartPage.xaml
	/// </summary>
	public partial class RestartPage : Page, IPageWindowSettings
	{
		public string PageName => "Restart";
		public int? RequestedHeight => 480;
		public int? RequestedWidth => 580;
		public bool AllowMinimize => false;
		public bool AllowClose => false;

		private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		private int _restartRemainingSeconds;
		private readonly Timer _restartTimer;

		public RestartPage(int timeUntilForcedRestart = 0)
		{
			_logger.Trace("Initializing ...");
			InitializeComponent();

			_logger.Trace("Applying theme ...");
			this.ApplyButtonThemes(App.Settings.BrandingSettings.ButtonSettings, new[] { RestartNowButton, RestartLaterButton });
			this.ApplyTextBlockThemes(App.Settings.BrandingSettings.TextBlockSettings, new[] { RestartTopTextBlock, RestartMiddleTextBlock, RestartBottomTextBlock });

			_logger.Trace("Setting language ...");
			var language = LanguageManager.Language;
			RestartNowButton.Content = language.RestartPrompt_ButtonRestartNow;
			RestartLaterButton.Content = language.RestartPrompt_ButtonRestartLater;
			RestartTopTextBlock.Text = language.RestartPrompt_Message;

			if(timeUntilForcedRestart > 0)
			{
				_logger.Trace($"{timeUntilForcedRestart} seconds until forced restart");

				RestartMiddleTextBlock.Text = $"{language.RestartPrompt_MessageTime}\n{language.RestartPrompt_MessageRestart}";
				RestartBottomTextBlock.Text = $"{language.RestartPrompt_TimeRemaining}\n{timeUntilForcedRestart}";

				RestartLaterButton.Visibility = Visibility.Hidden;

				_restartRemainingSeconds = timeUntilForcedRestart;

				_logger.Trace("Setting up timer ...");
				_restartTimer = new Timer()
				{
					Interval = 1000,
					AutoReset = true
				};
				_restartTimer.Elapsed += RestartTimerTick;
				_restartTimer.Start();
			}
		}

		private void RestartTimerTick(object sender, EventArgs e)
		{
			if(--_restartRemainingSeconds <= 0)
			{
				_restartTimer.Stop();
				_restartTimer.Dispose();

				NowButton_Click(null, null);
			}
			else
			{
				Dispatcher.Invoke(delegate () {
					RestartBottomTextBlock.Text = $"{LanguageManager.Language.RestartPrompt_TimeRemaining}\n{_restartRemainingSeconds}";
				});
			}
		}

		private void NowButton_Click(object sender, RoutedEventArgs e)
		{
			if(sender != null)
			{
				_logger.Info("User initiated restart");
			}
			else
			{
				_logger.Info("Countdown initiated restart");
			}

			App.SendMessage(new ContinueMessage()
			{
				DeploymentStep = Modals.DeploymentStep.Restart
			});
		}

		private void LaterButton_Click(object sender, RoutedEventArgs e)
		{
			App.SendMessage(new AbortMessage()
			{
				DeploymentStep = Modals.DeploymentStep.Restart
			});
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			_restartTimer?.Stop();
			_restartTimer?.Dispose();
		}
	}
}
