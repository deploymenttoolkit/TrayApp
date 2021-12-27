using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.Modals;
using DeploymentToolkit.Modals.Settings.Tray;
using DeploymentToolkit.TrayApp.Extensions;
using System;
using System.Windows.Controls;

namespace DeploymentToolkit.TrayApp.Windows.Deployment
{
	/// <summary>
	/// Interaction logic for DeploymentDeferal.xaml
	/// </summary>
	public partial class DeploymentDeferal : Page, IPageWindowSettings
	{
		public string PageName => "Deferal";
		public int? RequestedHeight => 480;
		public int? RequestedWidth => 580;
		public bool AllowMinimize => false;
		public bool AllowClose => false;


		private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
		public DeploymentDeferal(int remainingDays = -1, DateTime deadline = default)
		{
			_logger.Trace("Initializing ...");
			InitializeComponent();

			_logger.Trace("Applying theme ...");
			this.ApplyButtonThemes(App.Settings.BrandingSettings.ButtonSettings, new[] { DeferButton, ContinueButton });
			this.ApplyTextBlockThemes(App.Settings.BrandingSettings.TextBlockSettings, new [] { DeferalTopTextBlock, DeferalMiddleTextBlock, DeferalBottomTextBlock });

			_logger.Trace("Setting languages...");
			var language = LanguageManager.Language;
			ContinueButton.Content = language.ClosePrompt_ButtonContinue;
			DeferButton.Content = language.ClosePrompt_ButtonDefer;

			DeferalTopTextBlock.Text = $"{language.DeferPrompt_WelcomeMessage}\n{App.DeploymentInformation.DeploymentName}";

			var text = $"{language.DeferPrompt_ExpiryMessage}\n";
			if(remainingDays > 0)
			{
				text += $"{language.DeferPrompt_RemainingDeferrals} {remainingDays}\n";
			}
			if(deadline != default)
			{
				text += $"{language.DeferPrompt_Deadline} {deadline.ToShortDateString()}";
			}
			DeferalMiddleTextBlock.Text = text;

			DeferalBottomTextBlock.Text = language.DeferPrompt_WarningMessage;
		}

		private void LaterButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_logger.Trace("User choose to defer installation");

			DeferButton.IsEnabled = false;
			ContinueButton.IsEnabled = false;

			App.SendMessage(new DeferMessage());
			App.Hide(this);
		}

		private void NowButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_logger.Trace("User choose to continue with installation");

			DeferButton.IsEnabled = false;
			ContinueButton.IsEnabled = false;

			App.SendMessage(new ContinueMessage()
			{
				DeploymentStep = DeploymentStep.DeferDeployment
			});
			App.Hide(this);
		}
	}
}
