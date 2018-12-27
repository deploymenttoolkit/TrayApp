﻿using DeploymentToolkit.Messaging;
using DeploymentToolkit.Messaging.Events;
using DeploymentToolkit.Messaging.Messages;
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

        public static NotifyIcon TrayIcon;

        public static MenuItem MenuItemExit;
        public static MenuItem MenuItemToggleVisibility;

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
                case MessageId.CloseApplications:
                    {
                        var message = e.Message as CloseApplicationsMessage;
                        FormAppList.Invoke((Action)delegate ()
                        {
                            // Disable exit of the program
                            MenuItemExit.Enabled = false;

                            if (FormCloseApplication != null && !FormCloseApplication.IsDisposed)
                                FormCloseApplication.Dispose();

                            FormCloseApplication = new CloseApplication(message.ApplicationNames);
                            FormCloseApplication.Show();
                        });
                    }
                    break;
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
            _logger.Info("Application ready to close");
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
    }
}
