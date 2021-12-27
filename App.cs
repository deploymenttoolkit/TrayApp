using DeploymentToolkit.Messaging;
using DeploymentToolkit.Messaging.Events;
using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.Modals;
using DeploymentToolkit.Modals.Settings;
using DeploymentToolkit.Modals.Settings.Tray;
using DeploymentToolkit.TrayApp.Windows;
using DeploymentToolkit.TrayApp.Windows.Deployment;
using DeploymentToolkit.TrayApp.Windows.Upgrades;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;

namespace DeploymentToolkit.TrayApp
{
	public partial class App : Application
	{
		internal static bool StartUp = false;
		internal static bool StartUpParameter = false;

		internal static TrayAppSettings Settings;
		internal static LanguageManager LanguageManager;

		internal static DeploymentInformationMessage DeploymentInformation;

		internal static new MainWindow MainWindow;
		internal static Page CurrentPage;

		public static Forms.NotifyIcon TrayIcon;
		public static Forms.ToolStripMenuItem MenuItemExit;
		public static Forms.ToolStripMenuItem MenuItemToggleVisibility;

		private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
		private static PipeServer _pipeServer;
		private static Process _blockerProcess;

		private static string _namespace;
		internal static string Namespace
		{
			get
			{
				if(string.IsNullOrEmpty(_namespace))
				{
					_namespace = typeof(App).Namespace;
				}

				return _namespace;
			}
		}

		private static Version _version;
		internal static Version Version
		{
			get
			{
				if(_version == null)
				{
					_version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				}

				return _version;
			}
		}

		private void AppStartup(object sender, StartupEventArgs eventArgs)
		{
			var args = eventArgs.Args;

			try
			{
				Logging.LogManager.Initialize("Tray");
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
#if !DEBUG
                MessageBox.Show($"Failed to initialize logging: {ex}");
                Environment.Exit(-1);
#else
				throw;
#endif
			}

			_logger.Info($"{Namespace} v{Version} initializing...");
			_logger.Debug($"ComamndLine: {Environment.CommandLine}");

			var ownProcess = Process.GetCurrentProcess();
			var openProcesses = Process.GetProcessesByName(ownProcess.ProcessName).Where(p => p.SessionId == ownProcess.SessionId);
			if(openProcesses.Count() > 1)
			{
				// There is already another process in this session. Just exit
				_logger.Info($"Another instance of {ownProcess.ProcessName} is already running. Exiting...");
				Environment.Exit(0);
			}

			_logger.Trace("Preparing Toasts...");
			ToastNotificationManagerCompat.OnActivated += OnToastNotification;

			_logger.Trace("Reading settings ...");
			Settings = ToolkitEnvironment.Settings.GetTrayAppSettings();

			_logger.Trace("Creating LanguageManager...");
			try
			{
				if(args.Any(a => a.ToLower() == "--force-lang"))
				{
					var language = args.FirstOrDefault((a) => a.Length == 2);
					LanguageManager = new LanguageManager(language);
				}
				else
				{
					LanguageManager = new LanguageManager();
				}
			}
			catch(Exception ex)
			{
				_logger.Fatal(ex, "Failed to create LanguageManager. Exiting...");
				MessageBox.Show($"Failed to initialize LanguageManager: {ex}");
				Environment.Exit(-1);
			}

			_logger.Trace("Creating TrayIcon...");
			try
			{
				TrayIcon = new Forms.NotifyIcon
				{
					Icon = new System.Drawing.Icon("./Assets/Tray.ico"),
					Text = "DeploymentToolkit TrayApp"
				};
			}
			catch(Exception ex)
			{
				_logger.Fatal(ex, "Failed to create tray icon");
				Environment.Exit(-2);
			}

			_logger.Trace("Creating ContextMenu...");
			var contextMenu = new Forms.ContextMenuStrip();

			//if(Settings.EnableAppList)
			//{
			//	_logger.Trace("Creating Show MenuItem...");
			//	MenuItemToggleVisibility = new Forms.ToolStripMenuItem
			//	{

			//		Text = "Show"
			//	};
			//	MenuItemToggleVisibility.Click += ToggleTrayAppClicked;
			//	contextMenu.Items.Add(MenuItemToggleVisibility);
			//}
			//else
			//{
			//	_logger.Debug("AppList not enabled. Not creating 'Show' MenuItem");
			//}

			_logger.Trace("Creating Close MenuItem...");
			MenuItemExit = new Forms.ToolStripMenuItem
			{
				Text = "Close"
			};
			MenuItemExit.Click += CloseTrayAppClicked;
			contextMenu.Items.Add(MenuItemExit);

#if DEBUG
			_logger.Trace("Creating debug MenuItems...");

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

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: View Restart Dialog"
				};
				item.Click += delegate (object sender, EventArgs e) {
					NavigateTo(new RestartPage());
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: View Restart Dialog (30 sec)"
				};
				item.Click += delegate (object sender, EventArgs e) {
					NavigateTo(new RestartPage(30));
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: Deferal (days)"
				};
				item.Click += delegate (object sender, EventArgs e) {
					NavigateTo(new DeploymentDeferal(7));
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: Deferal (time)"
				};
				item.Click += delegate (object sender, EventArgs e) {
					NavigateTo(new DeploymentDeferal(-1, DateTime.Now.AddDays(3)));
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: Close Apps (cmd.exe, notepad.exe)"
				};
				item.Click += delegate (object sender, EventArgs e) {
					NavigateTo(new CloseApplications(new[] { "cmd", "notepad" }));
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: Close Apps Forced (cmd.exe, notepad.exe)"
				};
				item.Click += delegate (object sender, EventArgs e) {
					NavigateTo(new CloseApplications(new[] { "cmd", "notepad" }, 30));
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: View Scheduler"
				};
				item.Click += delegate (object sender, EventArgs e) {
					NavigateTo(new Scheduler());
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: Rotate Logo"
				};
				item.Click += delegate (object sender, EventArgs e) {
					Settings.PageSettings.LogoPosition = Settings.PageSettings.LogoPosition switch
					{
						Alignment.Left => Alignment.Right,
						Alignment.Right => Alignment.Center,
						_ => Alignment.Left,
					};
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: Toast"
				};
				item.Click += delegate (object sender, EventArgs e) {
					new ToastContentBuilder()
						.AddArgument("action", "viewConversation")
						.AddText("Andrew sent you a picture")
						.AddText("Check this out, The Enchantments in Washington!")
						.AddInputTextBox("Test")
						.AddButton("Bye", ToastActivationType.Background, "SendBye")
						.Show();
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: Logoff"
				};
				item.Click += delegate (object sender, EventArgs e) {
					Util.PowerUtil.Logoff();
				};
				contextMenu.Items.Add(item);
			}

			{
				var item = new Forms.ToolStripMenuItem()
				{
					Text = "DEBUG: Restart"
				};
				item.Click += delegate (object sender, EventArgs e) {
					Util.PowerUtil.Restart();
				};
				contextMenu.Items.Add(item);
			}

#endif

			_logger.Trace("Creating PipeServer...");
			_pipeServer = new PipeServer(TrayIcon);
			_pipeServer.OnNewMessage += OnNewMessage;

			_logger.Trace("Creating MainWindow...");
			MainWindow = new MainWindow();

			_logger.Trace("Checking commandline arguments...");
			StartUpParameter = args.Any(a => a.ToLower() == "--requested");
			if(args.Any(a => a.ToLower() == "--startup") || !Settings.EnableAppList)
			{
				_logger.Info($"Detected '--startup' commandline or EnableAppList is set to false. Starting application in Tray only ({args.Any(a => a.ToLower() == "--startup")}/{StartUpParameter}/{Settings.EnableAppList})");
				StartUp = true;
			}
			else
			{
				_logger.Info("No valid commandline detected. Executing normal start");
				MainWindow.Show();
			}

			_logger.Trace("Finishing TrayIcon...");
			TrayIcon.ContextMenuStrip = contextMenu;
			TrayIcon.Visible = true;
		}

		private static void OnToastNotification(ToastNotificationActivatedEventArgsCompat eventArguments)
		{
			var arguments = ToastArguments.Parse(eventArguments.Argument);
			var userInput = eventArguments.UserInput;
		}

		private static void OnNewMessage(object sender, NewMessageEventArgs e)
		{
			// NOT ON GUI THREAD !!
			try
			{
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
						if(e.MessageId == MessageId.DeploymentStarted && DeploymentInformation.DisplaySettings.BlockScreensDuringInstallation)
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
						var icon = Forms.ToolTipIcon.Info;

						if(e.MessageId == MessageId.DeploymentStarted)
						{
							text += language.BalloonText_Start;
						}
						else if(e.MessageId == MessageId.DeploymentSuccess)
						{
							text += language.BalloonText_Complete;
						}
						else if(e.MessageId == MessageId.DeploymentError)
						{
							icon = Forms.ToolTipIcon.Error;
							text += language.BalloonText_Error;
						}

						_logger.Trace($"Icon: {icon}");
						_logger.Trace($"Final balloon tip text: {text}");

						MainWindow.Dispatcher.Invoke(delegate ()
						{
							TrayIcon.BalloonTipIcon = icon;
							TrayIcon.BalloonTipTitle = DeploymentInformation.DeploymentName;
							TrayIcon.BalloonTipText = text;
							TrayIcon.ShowBalloonTip(10000);

							if(e.MessageId == MessageId.DeploymentSuccess || e.MessageId == MessageId.DeploymentError)
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
						MainWindow.Dispatcher.Invoke(delegate ()
						{
#if !DEBUG
							// Disable exit of the program
							MenuItemExit.Enabled = false;
#endif

							NavigateTo(new RestartPage(message.TimeUntilForceRestart));
						});
					}
					break;

					case MessageId.DeploymentLogoff:
					{
						var message = e.Message as DeploymentLogoffMessage;

						MainWindow.Dispatcher.Invoke(delegate ()
						{
							TrayIcon.BalloonTipIcon = Forms.ToolTipIcon.Warning;
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
						MainWindow.Dispatcher.Invoke(delegate ()
						{
							NavigateTo(new CloseApplications(message.ApplicationNames, message.TimeUntilForceClose));
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
						MainWindow.Dispatcher.Invoke(delegate ()
						{
							NavigateTo(new DeploymentDeferal(message.RemainingDays, message.DeadLine));
						});
					}
					break;
				}
			}
			catch(Exception ex)
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
			if(_blockerProcess == null)
			{
				_logger.Trace("Not block process started. Skipping unblock ...");
				return;
			}

			if(!_blockerProcess.HasExited)
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
			catch(Exception ex)
			{
				_logger.Error(ex, "Failed to communicate with deployment");
			}
		}

		private static void CloseTrayAppClicked(object sender, EventArgs e)
		{
			_logger.Info("Close requested from TrayIcon");
			if(!MainWindow.AllowClose)
			{
				_logger.Warn("Cannot close. There is currently a windows being force displayed");
				return;
			}

			try
			{
				MainWindow?.Close();
				TrayIcon?.Dispose();
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
			//if(FormAppList.IsDisposed || FormAppList == null)
			//{
			//	_logger.Warn("AppList seems to be completly closed. Recreating form");
			//	FormAppList = null;
			//	FormAppList = new AppList();
			//}

			if(MenuItemToggleVisibility.Text == "Show")
				ShowAppList();
			else
				HideAppList();
		}

		internal static void HideAppList()
		{
			_logger.Trace("Executing HideAppList");
			if(Settings.EnableAppList)
				MenuItemToggleVisibility.Text = "Show";
			//FormAppList.Hide();
			_logger.Trace("Executed HideAppList");
		}

		internal static void ShowAppList()
		{
			_logger.Trace("Executing ShowAppList");
			if(Settings.EnableAppList)
				MenuItemToggleVisibility.Text = "Hide";
			//FormAppList.Show();
			_logger.Trace("Executed ShowAppList");
		}

		internal static void NavigateTo(Page page)
		{
			_logger.Info($"Navigating to {page.GetType().FullName}");

			if(CurrentPage != null)
			{
				_logger.Debug($"Already displaying page {CurrentPage.GetType().FullName}. Old page was not properly closed");
			}

			// Hide page for reconstruction
			MainWindow.Hide();

			if(page is not IPageWindowSettings settings)
			{
				throw new Exception("Invalid page configuration");
			}

			var theme = Settings.PageSettings.GetType().GetProperties().FirstOrDefault(p => p.Name == settings.PageName);
			if(theme != null)
			{
				MainWindow.ApplyTheme((PageSettings)theme.GetValue(Settings.PageSettings));
			}
			else
			{
				MainWindow.ApplyDefaultTheme();
			}

			if(!settings.AllowMinimize)
			{
				MainWindow.ResizeMode = ResizeMode.NoResize;
			}
			else
			{
				MainWindow.ResizeMode = ResizeMode.CanResize;
			}

			if(settings.RequestedHeight.HasValue)
			{
				MainWindow.Height = settings.RequestedHeight.Value;
			}
			if(settings.RequestedWidth.HasValue)
			{
				MainWindow.Width = settings.RequestedWidth.Value;
			}

			CurrentPage = page;

			MainWindow.Frame.Content = null;
			MainWindow.Frame.NavigationService.Navigate(page);

			// Show reconstructed page
			MainWindow.Show();
		}

		internal static void Hide(Page page)
		{
			_logger.Info($"Hiding by request of {page.GetType().FullName}");

			CurrentPage = null;

			// We may not be comming from a GUI thread here
			MainWindow.Dispatcher.Invoke(() =>
			{
				MainWindow.Frame.Content = null;
				MainWindow.Hide();
			});
		}
	}
}