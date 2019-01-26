﻿using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.Modals;
using DeploymentToolkit.TrayApp.Extensions;
using NLog;
using System;
using System.Windows.Forms;

namespace DeploymentToolkit.TrayApp
{
    public partial class DeploymentDeferal : Form
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public DeploymentDeferal(int remainingDays, DateTime deadline)
        {
            _logger.Trace("Initializing components...");
            InitializeComponent();
            this.AddLogo(PictureLogo);

            _logger.Trace("Setting languages...");
            var language = LanguageManager.Language;
            ButtonContinue.Text = language.ClosePrompt_ButtonContinue;
            ButtonDefer.Text = language.ClosePrompt_ButtonDefer;
            LabelTop.Text = $"{language.DeferPrompt_WelcomeMessage}\n{Program.DeploymentInformation.DeploymentName}";
            
            var text = $"{language.DeferPrompt_ExpiryMessage}\n";
            if (remainingDays > 0)
            {
                text += $"{language.DeferPrompt_RemainingDeferrals} {remainingDays}\n";
            }
            if(deadline != DateTime.MinValue)
            {
                text += $"{language.DeferPrompt_Deadline} {deadline.ToShortDateString()}";
            }
            LabelCenter.Text = text;

            LabelBottom.Text = language.DeferPrompt_WarningMessage;

            _logger.Trace("Ready to display");
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

        private void ButtonDefer_Click(object sender, EventArgs e)
        {
            _logger.Trace("User choose to defer installation");

            ButtonDefer.Enabled = false;
            ButtonContinue.Enabled = false;

            Program.SendMessage(new DeferMessage());
            Program.CloseForm(this);
        }

        private void ButtonContinue_Click(object sender, EventArgs e)
        {
            _logger.Trace("User choose to continue with installation");

            ButtonDefer.Enabled = false;
            ButtonContinue.Enabled = false;

            Program.SendMessage(new ContinueMessage()
            {
                DeploymentStep = DeploymentStep.DeferDeployment
            });
            Program.CloseForm(this);
        }
    }
}
