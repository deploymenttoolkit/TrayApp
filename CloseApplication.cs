using NLog;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DeploymentToolkit.TrayApp
{
    public partial class CloseApplication : Form
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private string[] _applicationList;
        private Timer _checkApplicationsTimer;

        public CloseApplication(string[] applications)
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

            _logger.Trace("Ready to display");
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
    }
}
