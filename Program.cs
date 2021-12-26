using DeploymentToolkit.Messaging;
using DeploymentToolkit.Messaging.Events;
using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.Modals;
using DeploymentToolkit.Modals.Settings;
using NLog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DeploymentToolkit.TrayApp
{
    static class Program
    {
        public static bool StartUp = false;
        public static bool StartUpParameter = false;

        public static TrayAppSettings Settings;

        public static Form CurrentDialog;

        public static AppList FormAppList;
        public static CloseApplication FormCloseApplication;
        public static DeploymentDeferal FormDeploymentDeferal;
        public static RestartDialog FormRestart;

        public static LanguageManager LanguageManager;

        public static NotifyIcon TrayIcon;

        public static ToolStripMenuItem MenuItemExit;
        public static ToolStripMenuItem MenuItemToggleVisibility;

        public static DeploymentInformationMessage DeploymentInformation;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static string _namespace;
        internal static string Namespace
        {
            get
            {
                if (string.IsNullOrEmpty(_namespace))
                    _namespace = typeof(Program).Namespace;
                return _namespace;
            }
        }

        private static Version _version;
        internal static Version Version
        {
            get
            {
                if (_version == null)
                    _version = Assembly.GetExecutingAssembly().GetName().Version;
                return _version;
            }
        }

        private static PipeServer _pipeServer;

        private static Process _blockerProcess;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Logging.LogManager.Initialize("Tray");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Failed to initialize logging");
                Debug.WriteLine(ex);
#endif
                MessageBox.Show($"Failed to initialize logging: {ex}");

                Environment.Exit(-1);
            }

            _logger.Info($"{Namespace} v{Version} initializing...");
            _logger.Debug($"ComamndLine: {Environment.CommandLine}");

            var ownProcess = Process.GetCurrentProcess();
            var openProcesses = Process.GetProcessesByName(ownProcess.ProcessName).Where(p => p.SessionId == ownProcess.SessionId);
            if (openProcesses.Count() > 1)
            {
                // There is already another process in this session. Just exit
                _logger.Info($"Another instance of {ownProcess.ProcessName} is already running. Exiting...");
                Environment.Exit(0);
            }

            _logger.Trace("Reading settings ...");
            Settings = ToolkitEnvironment.Settings.GetTrayAppSettings();

            _logger.Trace("Creating LanguageManager...");
            try
            {
                LanguageManager = new LanguageManager();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to create LanguageManager. Exiting...");
                MessageBox.Show($"Failed to initialize LanguageManager: {ex}");
                Environment.Exit(-1);
            }

            _logger.Trace("Creating AppList...");
            FormAppList = new AppList();

            _logger.Trace("Creating TrayIcon...");
            try
            {
                TrayIcon = new NotifyIcon
                {
                    Icon = new Icon("Tray.ico"),
                    Text = "DeploymentToolkit TrayApp"
                };
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to create tray icon");
                Environment.Exit(-2);
            }

            _logger.Trace("Creating ContextMenu...");
            var contextMenu = new ContextMenuStrip();

            if (Settings.EnableAppList)
            {
                _logger.Trace("Creating Show MenuItem...");
                MenuItemToggleVisibility = new ToolStripMenuItem
                {
                    
                    Text = "Show"
                };
                MenuItemToggleVisibility.Click += ToggleTrayAppClicked;
                contextMenu.Items.Add(MenuItemToggleVisibility);
            }
            else
            {
                _logger.Debug("AppList not enabled. Not creating 'Show' MenuItem");
            }

            _logger.Trace("Creating Close MenuItem...");
            MenuItemExit = new ToolStripMenuItem
            {
                Text = "Close"
            };
            MenuItemExit.Click += CloseTrayAppClicked;
            contextMenu.Items.Add(MenuItemExit);

#if DEBUG
            _logger.Trace("Creating debug MenuItems...");

            {
                var item = new ToolStripMenuItem()
                {
                    Text = "DEBUG: View CloseApplication (cmd.exe)"
                };
                item.Click += delegate (object sender, EventArgs e)
                {
                    FormCloseApplication?.Dispose();
                    FormCloseApplication = new CloseApplication(new[] { "cmd.exe" }, 0);
                    ShowForm(FormCloseApplication);
                };
                contextMenu.Items.Add(item);
            }

            {
                var item = new ToolStripMenuItem()
                {
                    Text = "DEBUG: View DeploymentDeferal"
                };
                item.Click += delegate (object sender, EventArgs e)
                {
                    FormDeploymentDeferal?.Dispose();
                    FormDeploymentDeferal = new DeploymentDeferal(1, DateTime.Now.AddDays(7));
                    ShowForm(FormDeploymentDeferal);
                };
                contextMenu.Items.Add(item);

                // Make sure this is never null as we may test dialogs
                DeploymentInformation = new DeploymentInformationMessage()
                {
                    DeploymentName = "DEBUG",
                    SequenceType = SequenceType.Installation,
                    DisplaySettings = new DisplaySettings()
                    {
                        BlockScreensDuringInstallation = false,
                        PersistentPrompt = false
                    }
                };
            }

            {
                var item = new ToolStripMenuItem()
                {
                    Text = "DEBUG: Logoff"
                };
                item.Click += delegate (object sender, EventArgs e)
                {
                    Util.PowerUtil.Logoff();
                };
                contextMenu.Items.Add(item);
            }

            {
                var item = new ToolStripMenuItem()
                {
                    Text = "DEBUG: Restart"
                };
                item.Click += delegate (object sender, EventArgs e)
                {
                    Util.PowerUtil.Restart();
                };
                contextMenu.Items.Add(item);
            }

#endif

            _logger.Trace("Finishing TrayIcon...");
            TrayIcon.ContextMenuStrip = contextMenu;
            TrayIcon.Visible = true;

            _logger.Trace("Creating PipeServer...");
            _pipeServer = new PipeServer(TrayIcon);
            _pipeServer.OnNewMessage += OnNewMessage;

            _logger.Trace("Checking commandline arguments...");
            StartUpParameter = args.Any(a => a.ToLower() == "--requested");
            if (args.Any(a => a.ToLower() == "--startup") || !Settings.EnableAppList)
            {
                _logger.Info($"Detected '--startup' commandline or EnableAppList is set to false. Starting application in Tray only ({args.Any(a => a.ToLower() == "--startup")}/{StartUpParameter}/{Settings.EnableAppList})");
                StartUp = true;
                // create handles
                var handle = FormAppList.Handle;
                Application.Run(FormAppList);
            }
            else
            {
                _logger.Info("No valid commandline detected. Executing normal start");
                Application.Run(FormAppList);
            }
        }

        private static void OnNewMessage(object sender, NewMessageEventArgs e)
        {
            // NOT ON GUI THREAD !!
            try
            {
                switch (e.MessageId)
                {
                    case MessageId.DeploymentInformationMessage:
                        {
                            _logger.Trace("Received DeploymentInformationMessage");
                            DeploymentInformation = e.Message as DeploymentInformationMessage;

                            _logger.Trace($"Deploymentmethod: {DeploymentInformation.SequenceType}");
                            _logger.Trace($"Deploymentname: {DeploymentInformation.DeploymentName}");
                        }
                        break;

                    case MessageId.DeploymentStarted:
                    case MessageId.DeploymentSuccess:
                    case MessageId.DeploymentError:
                        {
                            if (e.MessageId == MessageId.DeploymentStarted && DeploymentInformation.DisplaySettings.BlockScreensDuringInstallation)
                            {
                                _logger.Trace("BlockScreensDuringInstallation specified. Blocking ...");
                                BlockScreen();
                            }
                            else
                            {
                                UnblockScreen();
                            }

                            var language = LanguageManager.Language;
                            var text = DeploymentInformation.SequenceType == SequenceType.Installation
                                ? language.DeploymentType_Install
                                : language.DeploymentType_UnInstall;
                            text += " ";
                            var icon = ToolTipIcon.Info;

                            if (e.MessageId == MessageId.DeploymentStarted)
                            {
                                text += language.BalloonText_Start;
                            }
                            else if (e.MessageId == MessageId.DeploymentSuccess)
                            {
                                text += language.BalloonText_Complete;
                            }
                            else if (e.MessageId == MessageId.DeploymentError)
                            {
                                icon = ToolTipIcon.Error;
                                text += language.BalloonText_Error;
                            }

                            _logger.Trace($"Icon: {icon}");
                            _logger.Trace($"Final balloon tip text: {text}");

                            FormAppList.Invoke((Action)delegate ()
                            {
                                TrayIcon.BalloonTipIcon = icon;
                                TrayIcon.BalloonTipTitle = DeploymentInformation.DeploymentName;
                                TrayIcon.BalloonTipText = text;
                                TrayIcon.ShowBalloonTip(10000);

                                if (e.MessageId == MessageId.DeploymentSuccess || e.MessageId == MessageId.DeploymentError)
                                {
                                    // Enable exit again
                                    MenuItemExit.Enabled = true;
                                }
                            });
                        }
                        break;

                    case MessageId.DeploymentRestart:
                        {
                            var message = e.Message as DeploymentRestartMessage;
                            FormAppList.Invoke((Action)delegate ()
                            {
#if !DEBUG
                                // Disable exit of the program
                                MenuItemExit.Enabled = false;
#endif

                                if (FormRestart != null && !FormRestart.IsDisposed)
                                    FormRestart.Dispose();

                                FormRestart = new RestartDialog(message.TimeUntilForceRestart);
                                ShowForm(FormRestart);
                            });
                        }
                        break;

                    case MessageId.DeploymentLogoff:
                        {
                            var message = e.Message as DeploymentLogoffMessage;

                            FormAppList.Invoke((Action)delegate ()
                            {
                                TrayIcon.BalloonTipIcon = ToolTipIcon.Warning;
                                TrayIcon.BalloonTipTitle = DeploymentInformation.DeploymentName;
                                TrayIcon.BalloonTipText = $"You will be logged off in {message.TimeUntilForceLogoff} seconds";
                                TrayIcon.ShowBalloonTip(10000);

                                System.Threading.Tasks.Task.Factory.StartNew(async () =>
                                {
                                    await System.Threading.Tasks.Task.Delay(message.TimeUntilForceLogoff * 1000);
                                    Util.PowerUtil.Logoff();
                                });
                            });
                        }
                        break;

                    case MessageId.CloseApplications:
                        {
                            var message = e.Message as CloseApplicationsMessage;
#if !DEBUG
                            // Disable exit of the program
                            MenuItemExit.Enabled = false;
#endif
                            FormAppList.Invoke((Action)delegate ()
                            {
                                if (FormCloseApplication != null && !FormCloseApplication.IsDisposed)
                                    FormCloseApplication.Dispose();

                                FormCloseApplication = new CloseApplication(message.ApplicationNames, message.TimeUntilForceClose);
                                ShowForm(FormCloseApplication);
                            });
                        }
                        break;

                    case MessageId.DeferDeployment:
                        {
                            var message = e.Message as DeferMessage;
#if !DEBUG
                            // Disable exit of the program
                            MenuItemExit.Enabled = false;
#endif
                            FormAppList.Invoke((Action)delegate ()
                            {
                                if (FormDeploymentDeferal != null && !FormDeploymentDeferal.IsDisposed)
                                    FormDeploymentDeferal.Dispose();

                                FormDeploymentDeferal = new DeploymentDeferal(message.RemainingDays, message.DeadLine);
                                ShowForm(FormDeploymentDeferal);
                            });
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to process message");
            }
        }

        private static void BlockScreen()
        {
            _blockerProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ToolkitEnvironment.EnvironmentVariables.DeploymentToolkitBlockerExePath
                }
            };

            _blockerProcess.Start();

            _logger.Trace($"Startec blocker with process id {_blockerProcess.Id}");
        }

        private static void UnblockScreen()
        {
            if (_blockerProcess == null)
            {
                _logger.Trace("Not block process started. Skipping unblock ...");
                return;
            }

            if (!_blockerProcess.HasExited)
            {
                _logger.Trace("Killing blocker ...");
                _blockerProcess.Kill();
            }
        }

        internal static void SendMessage(IMessage message)
        {
            try
            {
                _pipeServer.SendMessage(message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to communicate with deployment");
            }
        }

        private static void CloseTrayAppClicked(object sender, EventArgs e)
        {
            _logger.Info("Close requested from TrayIcon. Closing...");
            try
            {
                FormAppList?.Dispose();
                TrayIcon?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to gracefully close application");
            }
            _logger.Info("Application exited");
            Environment.Exit(0);
        }

        private static void ToggleTrayAppClicked(object sender, EventArgs e)
        {
            if (FormAppList.IsDisposed || FormAppList == null)
            {
                _logger.Warn("AppList seems to be completly closed. Recreating form");
                FormAppList = null;
                FormAppList = new AppList();
            }

            if (MenuItemToggleVisibility.Text == "Show")
                ShowAppList();
            else
                HideAppList();
        }

        internal static void HideAppList()
        {
            _logger.Trace("Executing HideAppList");
            if (Settings.EnableAppList)
                MenuItemToggleVisibility.Text = "Show";
            FormAppList.Hide();
            _logger.Trace("Executed HideAppList");
        }

        internal static void ShowAppList()
        {
            _logger.Trace("Executing ShowAppList");
            if (Settings.EnableAppList)
                MenuItemToggleVisibility.Text = "Hide";
            FormAppList.Show();
            _logger.Trace("Executed ShowAppList");
        }

        internal static void ShowForm(Form form)
        {
            if (CurrentDialog != null)
            {
                CloseForm(CurrentDialog);
            }

            if (StartUpParameter)
            {
                // I don't know why the fuck this is necessary.
                // But when started by the Toolkit (and not autostart) the first form is just not shown. Who the fuck knows why
                // So we just show it twice. And that works. Why? IDK.
                StartUpParameter = false;
                ShowForm(form);
            }

            var type = form.GetType();
            var name = type.Name;

            form.Show();
            CurrentDialog = form;

            _logger.Info($"Showing form {name}");
        }

        internal static void CloseForm(Form form)
        {
            if (form == null)
                return;
            var type = form.GetType();
            var name = type.Name;

            form.Dispose();
            CurrentDialog = null;

            _logger.Info($"Closing form {name}");
        }
    }
}
