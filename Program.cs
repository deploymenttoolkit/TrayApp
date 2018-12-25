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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static AppList AppList;
        public static NotifyIcon TrayIcon;

        public static MenuItem ToggleVisibilityMenuItem;

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

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Logging.LogManager.Initialize();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Failed to initialize logging");
                Debug.WriteLine(ex);

                Environment.Exit(-1);
            }

            _logger.Info($"{Namespace} v{Version} initializing...");

            _logger.Trace("Creating AppList...");
            AppList = new AppList();

            _logger.Trace("Creating TrayIcon...");
            TrayIcon = new NotifyIcon
            {
                Icon = new Icon(SystemIcons.Shield, 40, 40),
                Text = "DeploymentToolkit TrayApp"
            };

            _logger.Trace("Creating ContextMenu...");
            var contextMenu = new ContextMenu();

            _logger.Trace("Creating MenuItem...");
            ToggleVisibilityMenuItem = new MenuItem
            {
                Index = 0,
                Text = "Show"
            };
            ToggleVisibilityMenuItem.Click += ToggleTrayAppClicked;
            contextMenu.MenuItems.Add(ToggleVisibilityMenuItem);

            _logger.Trace("Creating MenuItem...");
            var item = new MenuItem
            {
                Index = 1,
                Text = "Close"
            };
            item.Click += CloseTrayAppClicked;
            contextMenu.MenuItems.Add(item);

            _logger.Trace("Finishing TrayIcon...");
            TrayIcon.ContextMenu = contextMenu;
            TrayIcon.Visible = true;

            _logger.Trace("Checking commandline arguments...");
            if (args.Any(a => a.ToLower() == "--startup"))
            {
                _logger.Info("Detected '--startup' commandline. Starting application in Tray only");
                Application.Run();
            }
            else
            {
                _logger.Info("No valid commandline detected. Executing normal start");
                Application.Run(AppList);
            }
        }

        private static void CloseTrayAppClicked(object sender, EventArgs e)
        {
            _logger.Info("Close requested from TrayIcon. Closing...");
            try
            {
                AppList.Close();
                AppList.Dispose();
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
            if(AppList.IsDisposed || AppList == null)
            {
                _logger.Warn("AppList seems to be completly closed. Recreating form");
                AppList = null;
                AppList = new AppList();
            }

            if (ToggleVisibilityMenuItem.Text == "Show")
                ShowAppList();
            else
                HideAppList();
        }

        internal static void HideAppList()
        {
            _logger.Trace("Executing HideAppList");
            ToggleVisibilityMenuItem.Text = "Show";
            AppList.Hide();
            _logger.Trace("Executed HideAppList");
        }

        internal static void ShowAppList()
        {
            _logger.Trace("Executing ShowAppList");
            ToggleVisibilityMenuItem.Text = "Hide";
            AppList.Show();
            _logger.Trace("Executed ShowAppList");
        }
    }
}
