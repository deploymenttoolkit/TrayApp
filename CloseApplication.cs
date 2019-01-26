using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.Modals;
using NLog;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeploymentToolkit.TrayApp
{
    public partial class CloseApplication : Form
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private string[] _applicationList;
        private Timer _checkApplicationsTimer;

        private int _forceCloseRemainingSeconds;
        private Timer _forceCloseTimer;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public CloseApplication(string[] applications, int timeUntilForceClose)
        {
            _logger.Trace("Initializing components...");
            InitializeComponent();

            _logger.Trace("Initializing applications...");
            _applicationList = applications;

            _logger.Trace("Initializing timer...");
            _checkApplicationsTimer = new Timer()
            {
                Interval = 2000
            };
            _checkApplicationsTimer.Tick += CheckApplications;
            _checkApplicationsTimer.Start();

            // Manually do the first tick to fill the list
            CheckApplications(null, null);

            _logger.Trace("Setting languages...");
            var language = LanguageManager.Language;
            ButtonClose.Text = language.ClosePrompt_ButtonClose;
            ButtonContinue.Text = language.ClosePrompt_ButtonContinue;
            LabelTop.Text = language.ClosePrompt_Message;

            if (timeUntilForceClose > 0)
            {
                _logger.Trace($"Force close is enabled. Forcing closeing in {timeUntilForceClose} seconds");
                LabelBottom.Text = $"{language.ClosePrompt_CountdownMessage} {timeUntilForceClose}";
                _forceCloseRemainingSeconds = timeUntilForceClose;

                _forceCloseTimer = new Timer()
                {
                    Interval = 1000
                };
                _forceCloseTimer.Tick += CloseTimerTick;
                _forceCloseTimer.Start();
            }

            _logger.Trace("Ready to display");
        }

        private void CloseTimerTick(object sender, EventArgs e)
        {
            if (--_forceCloseRemainingSeconds <= 0)
            {
                // No time left
                _logger.Trace("No time left on ForceCloseTimer");

                _logger.Trace("Stopping timer...");
                _forceCloseTimer.Stop();

                _logger.Trace("Executing close applications ...");
                OnButtonCloseClick(null, null);
            }
            else
            {
                // Update GUI with remaining time
                this.Invoke((Action)delegate ()
                {
                    LabelBottom.Text = $"{LanguageManager.Language.ClosePrompt_CountdownMessage} {_forceCloseRemainingSeconds}";
                });
            }
        }

        private void CheckApplications(object sender, EventArgs e)
        {
            _logger.Trace("Checking applications...");
            foreach(var application in _applicationList)
            {
                try
                {
                    var processName = application.ToLower().EndsWith(".exe") ? application.Substring(0, application.Length - 4) : application;
                    var processes = Process.GetProcessesByName(processName);
                    if (processes.Length > 0)
                    {
                        // application still running
                        foreach (var process in processes)
                        {
                            _logger.Trace($"Application [{process.Id}]{process.ProcessName} is still running");
                            if (ListViewCloseApplications.Items.ContainsKey(processName))
                                continue;

                            _logger.Trace($"Adding [{process.Id}]{process.ProcessName} to list");
                            var title = string.IsNullOrEmpty(process.MainWindowTitle) ? process.ProcessName : process.MainWindowTitle;
#if DEBUG
                            title = $"[DEBUG/{process.Id}/{process.ProcessName}] {title}";
#endif
                            ListViewCloseApplications.Items.Add(new ListViewItem()
                            {
                                Name = processName,
                                Text = title
                            });
                        }
                    }
                    else
                    {
                        _logger.Trace($"Application {processName} is not running");
                        if (!ListViewCloseApplications.Items.ContainsKey(processName))
                            continue;

                        _logger.Trace($"Removing {processName} from list");
                        ListViewCloseApplications.Items.RemoveByKey(processName);
                    }
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, $"Error processing {application}");
                }
            }

            ButtonContinue.Enabled = (ListViewCloseApplications.Items.Count == 0);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void OnResize(object sender, EventArgs e)
        {
            if(this.WindowState != FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void OnButtonCloseClick(object sender, EventArgs e)
        {
            if (sender != null)
                _logger.Trace("User decided to let us close the applications. Executing...");
            else
                _logger.Trace("ForceClose time ran out. Closeing applications ...");

            _logger.Trace("Hiding window...");
            this.Visible = false;

            _logger.Trace("Stopping timer...");
            _checkApplicationsTimer.Stop();
            _forceCloseTimer?.Stop();

            Task.Run(delegate
            {
                foreach (var application in _applicationList)
                {
                    try
                    {
                        var processName = application.ToLower().EndsWith(".exe") ? application.Substring(0, application.Length - 4) : application;
                        _logger.Trace($"Searching for running instances of {processName}");
                        var processes = Process.GetProcessesByName(processName);
                        _logger.Trace($"Found {processes.Length} instances of {processName}");

                        foreach (var process in processes)
                        {
                            process.Refresh();
                            if (process.HasExited)
                            {
                                _logger.Trace($"Skipping [{process.Id}]{process.ProcessName} as it seems to be closed");
                                continue;
                            }

                            try
                            {
                                _logger.Trace($"Trying to close [{process.Id}]{process.ProcessName} gracefully");
                                // Send a WM_CLOSE and wait for a gracefull exit
                                PostMessage(process.Handle, 0x0010, IntPtr.Zero, IntPtr.Zero);
                                if (!process.WaitForExit(5000))
                                {
                                    _logger.Trace($"[{process.Id}]{process.ProcessName} did not close after being instructed. Killing...");
                                    process.Kill();
                                }

                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex, $"Error closing [{process.Id}]{process.ProcessName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Error while processing {application}");
                    }
                }
            }).ContinueWith(delegate (Task task)
            {
                Program.SendMessage(new ContinueMessage()
                {
                    DeploymentStep = DeploymentStep.CloseApplications
                });
                Program.CloseForm(this);
            }); 
        }

        private void OnButtonContinueClick(object sender, EventArgs e)
        {
            _logger.Trace("User choose to continue with deployment...");
            Program.SendMessage(new ContinueMessage()
            {
                DeploymentStep = DeploymentStep.CloseApplications
            });
            Program.CloseForm(this);
        }
    }
}
