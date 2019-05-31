using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.TrayApp.Extensions;
using NLog;
using System;
using System.Windows.Forms;

namespace DeploymentToolkit.TrayApp
{
    public partial class RestartDialog : Form
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private int _restartRemainingSeconds;
        private Timer _restartTimer;

        public RestartDialog(int timeUntilForcedRestart)
        {
            _logger.Trace("Initializing components...");
            InitializeComponent();
            this.AddLogo(PictureLogo);

            _logger.Trace("Adding language...");
            var language = LanguageManager.Language;
            ButtonNow.Text = language.RestartPrompt_ButtonRestartNow;
            ButtonLater.Text = language.RestartPrompt_ButtonRestartLater;
            LabelTop.Text = language.RestartPrompt_Message;
            LabelCenter.Text = "";
            LabelBottom.Text = "";
            
            if(timeUntilForcedRestart > 0)
            {
                _logger.Trace($"{timeUntilForcedRestart} seconds until forced restart");

                LabelCenter.Text = $"{language.RestartPrompt_MessageTime}\n{language.RestartPrompt_MessageRestart}";
                LabelBottom.Text = $"{language.RestartPrompt_TimeRemaining}\n{timeUntilForcedRestart}";

                ButtonLater.Enabled = false;

                _restartRemainingSeconds = timeUntilForcedRestart;

                _logger.Trace("Setting up timer ...");
                _restartTimer = new Timer()
                {
                    Interval = 1000
                };
                _restartTimer.Tick += RestartTimerTick;
                _restartTimer.Start();
            }

            _logger.Trace("Ready to display");
        }

        private void RestartTimerTick(object sender, EventArgs e)
        {
            if(--_restartRemainingSeconds <= 0)
            {
                _restartTimer.Stop();
                _restartTimer.Dispose();

                ButtonNow_Click(null, null);
            }
            else
            {
                this.Invoke((Action)delegate ()
                {
                    LabelBottom.Text = $"{LanguageManager.Language.RestartPrompt_TimeRemaining}\n{_restartRemainingSeconds}";
                });
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void ButtonNow_Click(object sender, EventArgs e)
        {
            if (sender != null)
                _logger.Trace("User choose to do the restart now");
            else
                _logger.Trace("Time ran out. Restarting ...");

            _restartTimer?.Dispose();

            Program.SendMessage(new ContinueMessage()
            {
                DeploymentStep = Modals.DeploymentStep.Restart
            });
            Program.CloseForm(this);
        }
    }
}
