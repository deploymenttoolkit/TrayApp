using DeploymentToolkit.Messaging;
using DeploymentToolkit.Messaging.Events;
using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.Modals;
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
        public static AppList FormAppList;
        public static CloseApplication FormCloseApplication;
        public static DeploymentDeferal FormDeploymentDeferal;
        public static RestartDialog FormRestart;

        public static LanguageManager LanguageManager;

        public static NotifyIcon TrayIcon;

        public static MenuItem MenuItemExit;
        public static MenuItem MenuItemToggleVisibility;

        public static DeploymentInformationMessage DeploymentInformation;

        private static Logger _logger = LogManager.GetCurrentClassLogger();

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

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Logging.LogManager.Initialize("Tray");
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Failed to initialize logging");
                Debug.WriteLine(ex);

                Environment.Exit(-1);
            }

            _logger.Info($"{Namespace} v{Version} initializing...");

            var ownProcess = Process.GetCurrentProcess();
            var openProcesses = Process.GetProcessesByName(ownProcess.ProcessName).Where(p => p.SessionId == ownProcess.SessionId);
            if(openProcesses.Count() > 1)
            {
                // There is already another process in this session. Just exit
                _logger.Info($"Another instance of {ownProcess.ProcessName} is already running. Exiting...");
                Environment.Exit(0);
            }

            _logger.Trace("Creating LanguageManager...");
            try
            {
                LanguageManager = new LanguageManager();
            }
            catch(Exception ex)
            {
                _logger.Fatal(ex, "Failed to create LanaugeManager. Exiting...");
                Environment.Exit(-1);
            }

            _logger.Trace("Creating AppList...");
            FormAppList = new AppList();

            _logger.Trace("Creating TrayIcon...");
            TrayIcon = new NotifyIcon
            {
                Icon = new Icon(SystemIcons.Shield, 40, 40),
                Text = "DeploymentToolkit TrayApp"
            };

            _logger.Trace("Creating ContextMenu...");
            var contextMenu = new ContextMenu();

            _logger.Trace("Creating MenuItem...");
            MenuItemToggleVisibility = new MenuItem
            {
                Index = 0,
                Text = "Show"
            };
            MenuItemToggleVisibility.Click += ToggleTrayAppClicked;
            contextMenu.MenuItems.Add(MenuItemToggleVisibility);

            _logger.Trace("Creating MenuItem...");
            MenuItemExit = new MenuItem
            {
                Index = 1,
                Text = "Close"
            };
            MenuItemExit.Click += CloseTrayAppClicked;
            contextMenu.MenuItems.Add(MenuItemExit);

#if DEBUG
            _logger.Trace("Creating debug MenuItems...");

            {
                var item = new MenuItem()
                {
                    Index = 2,
                    Text = "DEBUG: View CloseApplication (cmd.exe)"
                };
                item.Click += delegate (object sender, EventArgs e)
                {
                    FormCloseApplication?.Dispose();
                    FormCloseApplication = new CloseApplication(new[] { "cmd.exe" }, 0);
                    FormCloseApplication.Show();
                };
                contextMenu.MenuItems.Add(item);
            }

            {
                var item = new MenuItem()
                {
                    Index = 3,
                    Text = "DEBUG: View DeploymentDeferal"
                };
                item.Click += delegate(object sender, EventArgs e)
                {
                    FormDeploymentDeferal?.Dispose();
                    FormDeploymentDeferal = new DeploymentDeferal(1, DateTime.Now.AddDays(7));
                    FormDeploymentDeferal.Show();
                };
                contextMenu.MenuItems.Add(item);

                // Make sure this is never null as we may test dialogs
                DeploymentInformation = new DeploymentInformationMessage()
                {
                    DeploymentName = "DEBUG",
                    SequenceType = SequenceType.Installation
                };
            }

            {
                var item = new MenuItem()
                {
                    Index = 4,
                    Text = "DEBUG: Logoff"
                };
                item.Click += delegate (object sender, EventArgs e)
                {
                    Utils.PowerUtil.Logoff();
                };
                contextMenu.MenuItems.Add(item);
            }

            {
                var item = new MenuItem()
                {
                    Index = 5,
                    Text = "DEBUG: Restart"
                };
                item.Click += delegate (object sender, EventArgs e)
                {
                    Utils.PowerUtil.Restart();
                };
                contextMenu.MenuItems.Add(item);
            }

#endif

            _logger.Trace("Finishing TrayIcon...");
            TrayIcon.ContextMenu = contextMenu;
            TrayIcon.Visible = true;

            _logger.Trace("Creating PipeServer...");
            _pipeServer = new PipeServer(TrayIcon);
            _pipeServer.OnNewMessage += OnNewMessage;

            _logger.Trace("Checking commandline arguments...");
            if (args.Any(a => a.ToLower() == "--startup"))
            {
                _logger.Info("Detected '--startup' commandline. Starting application in Tray only");
                Application.Run();
            }
            else
            {
                _logger.Info("No valid commandline detected. Executing normal start");
                Application.Run(FormAppList);
            }
        }

        private static void OnNewMessage(object sender, NewMessageEventArgs e)
        {
            // You are not in the Main form thread here !!!
            
            switch(e.MessageId)
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
                        else if(e.MessageId == MessageId.DeploymentSuccess)
                        {
                            text += language.BalloonText_Complete;
                        }
                        else if(e.MessageId == MessageId.DeploymentError)
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
                            FormRestart.Show();
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
                                Utils.PowerUtil.Logoff();
                            });
                        });
                    }
                    break;

                case MessageId.CloseApplications:
                    {
                        var message = e.Message as CloseApplicationsMessage;
                        FormAppList.Invoke((Action)delegate ()
                        {
#if !DEBUG
                            // Disable exit of the program
                            MenuItemExit.Enabled = false;
#endif

                            if (FormCloseApplication != null && !FormCloseApplication.IsDisposed)
                                FormCloseApplication.Dispose();

                            FormCloseApplication = new CloseApplication(message.ApplicationNames, message.TimeUntilForceClose);
                            FormCloseApplication.Show();
                        });
                    }
                    break;

                case MessageId.DeferDeployment:
                    {
                        var message = e.Message as DeferMessage;
                        FormAppList.Invoke((Action)delegate ()
                        {
#if !DEBUG
                            // Disable exit of the program
                            MenuItemExit.Enabled = false;
#endif

                            if (FormDeploymentDeferal != null && !FormDeploymentDeferal.IsDisposed)
                                FormCloseApplication.Dispose();

                            FormDeploymentDeferal = new DeploymentDeferal(message.RemainingDays, message.DeadLine);
                            FormDeploymentDeferal.Show();
                        });
                    }
                    break;
            }
        }

        internal static void SendMessage(IMessage message)
        {
            try
            {
                _pipeServer.SendMessage(message);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Failed to communicate with deployment");
            }
        }

        private static void CloseTrayAppClicked(object sender, EventArgs e)
        {
            _logger.Info("Close requested from TrayIcon. Closing...");
            try
            {
                FormAppList.Close();
                FormAppList.Dispose();
                TrayIcon.Dispose();
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Failed to gracefully close application");
            }
            _logger.Info("Application exited");
            Environment.Exit(0);
        }

        private static void ToggleTrayAppClicked(object sender, EventArgs e)
        {
            if(FormAppList.IsDisposed || FormAppList == null)
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
            MenuItemToggleVisibility.Text = "Show";
            FormAppList.Hide();
            _logger.Trace("Executed HideAppList");
        }

        internal static void ShowAppList()
        {
            _logger.Trace("Executing ShowAppList");
            MenuItemToggleVisibility.Text = "Hide";
            FormAppList.Show();
            _logger.Trace("Executed ShowAppList");
        }

        internal static void CloseForm(Form form)
        {
            if (form == null)
                return;
            var type = form.GetType();
            var name = type.Name;
            _logger.Info($"Closing form {name}");
            form.Dispose();
        }
    }
}
