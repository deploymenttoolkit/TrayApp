using DeploymentToolkit.Messaging.Messages;
using DeploymentToolkit.Modals;
using DeploymentToolkit.Modals.Settings.Tray;
using DeploymentToolkit.TrayApp.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace DeploymentToolkit.TrayApp.Windows.Deployment
{
	public class RunningProcess
	{
		public string Title { get; set; }
		public string ProcessName { get; set; }
		public int ProcessId { get; set; }

		public override string ToString()
		{
			return Title;
		}
	}

	/// <summary>
	/// Interaction logic for CloseApplications.xaml
	/// </summary>
	public partial class CloseApplications : Page, IPageWindowSettings
	{
		public string PageName => "CloseApplications";
		public int? RequestedHeight => 480;
		public int? RequestedWidth => 580;
		public bool AllowMinimize => false;
		public bool AllowClose => false;

		private ObservableCollection<RunningProcess> _runningApplications = new();
		public ObservableCollection<RunningProcess> RunningApplications
		{
			get
			{
				return _runningApplications;
			}

			set
			{
				_runningApplications = value;
			}
		}

		private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		private readonly string[] _applicationList;
		private readonly Timer _checkApplicationsTimer;

		private int _forceCloseRemainingSeconds;
		private readonly Timer _forceCloseTimer;

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		public CloseApplications(string[] applications, int timeUntilForceClose = -1)
		{
			_logger.Trace("Initializing ...");
			InitializeComponent();

			_logger.Trace("Applying theme ...");
			this.ApplyButtonThemes(App.Settings.BrandingSettings.ButtonSettings, new[] { CloseApplicationsButton, CloseApplicationsContinueButton });
			this.ApplyTextBlockThemes(App.Settings.BrandingSettings.TextBlockSettings, new[] { CloseApplicationsTopTextBlock, CloseApplicationsBottomTextBlock });

			_applicationList = applications;
			DataContext = this;
			CloseApplicationsListView.ItemsSource = RunningApplications;

			_logger.Trace("Initializing timer...");
			_checkApplicationsTimer = new Timer()
			{
				Interval = 2000,
				AutoReset = true
			};
			_checkApplicationsTimer.Elapsed += CheckApplications;
			_checkApplicationsTimer.Start();

			// Manually do the first tick to fill the list
			CheckApplications(null, null);

			_logger.Trace("Setting languages...");
			var language = LanguageManager.Language;
			CloseApplicationsButton.Content = language.ClosePrompt_ButtonClose;
			CloseApplicationsContinueButton.Content = language.ClosePrompt_ButtonContinue;
			CloseApplicationsTopTextBlock.Text = language.ClosePrompt_Message;

			if(timeUntilForceClose > 0)
			{
				_logger.Trace($"Force close is enabled. Forcing closeing in {timeUntilForceClose} seconds");
				CloseApplicationsBottomTextBlock.Text = $"{language.ClosePrompt_CountdownMessage} {timeUntilForceClose}";
				_forceCloseRemainingSeconds = timeUntilForceClose;

				_forceCloseTimer = new Timer()
				{
					Interval = 1000,
					AutoReset = true
				};
				_forceCloseTimer.Elapsed += CloseTimerTick;
				_forceCloseTimer.Start();
			}
		}

		private void CloseTimerTick(object sender, EventArgs e)
		{
			if(--_forceCloseRemainingSeconds <= 0)
			{
				// No time left
				_logger.Trace("No time left on ForceCloseTimer");

				_logger.Trace("Stopping timer...");
				_forceCloseTimer.Stop();

				_logger.Trace("Executing close applications ...");
				CloseButton_Click(null, null);
			}
			else
			{
				// Update GUI with remaining time
				Dispatcher.Invoke(delegate () {
					CloseApplicationsBottomTextBlock.Text = $"{LanguageManager.Language.ClosePrompt_CountdownMessage} {_forceCloseRemainingSeconds}";
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
					if(processes.Length > 0)
					{
						// application still running
						foreach(var process in processes)
						{
							_logger.Trace($"Application [{process.Id}]{process.ProcessName} is still running");
							if(_runningApplications.Any(i => i.ProcessName == processName))
							{
								continue;
							}

							_logger.Trace($"Adding [{process.Id}]{process.ProcessName} to list");
							var title = string.IsNullOrEmpty(process.MainWindowTitle) ? process.ProcessName : process.MainWindowTitle;
#if DEBUG
							title = $"[DEBUG/{process.Id}/{process.ProcessName}] {title}";
#endif
							Dispatcher.Invoke(() =>
							{
								_runningApplications.Add(new RunningProcess()
								{
									ProcessName = processName,
									ProcessId = process.Id,
									Title = title
								});
							});
						}
					}
					else
					{
						_logger.Trace($"Application {processName} is not running");
						if(!_runningApplications.Any(i => i.ProcessName == processName))
						{
							continue;
						}

						_logger.Trace($"Removing {processName} from list");
						Dispatcher.Invoke(() =>
						{
							foreach(var app in _runningApplications.Where(i => i.ProcessName == processName).ToList())
							{
								_runningApplications.Remove(app);
							}
						});
					}
				}
				catch(Exception ex)
				{
					_logger.Error(ex, $"Error processing {application}");
				}
			}

			Dispatcher.Invoke(() =>
			{
				CloseApplicationsContinueButton.IsEnabled = (_runningApplications.Count == 0);
			});
		}

		private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if(sender != null)
			{
				_logger.Trace("User decided to let us close the applications. Executing...");
			}
			else
			{
				_logger.Trace("ForceClose time ran out. Closeing applications ...");
			}

			_logger.Trace("Hiding window...");
			// If invoked by force close we are NOT on GUI thread
			Dispatcher.Invoke(() =>
			{
				Visibility = System.Windows.Visibility.Hidden;
			});

			_logger.Trace("Stopping timer...");
			_checkApplicationsTimer.Stop();
			_forceCloseTimer?.Stop();

			Task.Run(delegate {
				foreach(var application in _applicationList)
				{
					try
					{
						var processName = application.ToLower().EndsWith(".exe") ? application.Substring(0, application.Length - 4) : application;
						_logger.Trace($"Searching for running instances of {processName}");
						var processes = Process.GetProcessesByName(processName);
						_logger.Trace($"Found {processes.Length} instances of {processName}");

						foreach(var process in processes)
						{
							try
							{
								process.Refresh();
								if(process.HasExited)
								{
									_logger.Trace($"Skipping [{process.Id}]{process.ProcessName} as it seems to be closed");
									continue;
								}

								_logger.Info($"Trying to close [{process.Id}]{process.ProcessName} gracefully");
								// Send a WM_CLOSE and wait for a gracefull exit
								PostMessage(process.Handle, 0x0010, IntPtr.Zero, IntPtr.Zero);
								if(!process.WaitForExit(5000))
								{
									_logger.Info($"[{process.Id}]{process.ProcessName} did not close after being instructed. Killing...");
									process.Kill();
								}
							}
							catch(Exception ex)
							{
								_logger.Error(ex, $"Error closing [{process.Id}]{process.ProcessName}");
							}
						}
					}
					catch(Exception ex)
					{
						_logger.Error(ex, $"Error while processing {application}");
					}
				}
			}).ContinueWith(delegate (Task task) {
				App.SendMessage(new ContinueMessage()
				{
					DeploymentStep = DeploymentStep.CloseApplications
				});
				App.Hide(this);
			});
		}

		private void ContinueButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_logger.Trace("User choose to continue with deployment...");

			Visibility = System.Windows.Visibility.Hidden;

			_logger.Trace("Stopping timer...");
			_checkApplicationsTimer.Stop();
			_forceCloseTimer?.Stop();

			App.SendMessage(new ContinueMessage()
			{
				DeploymentStep = DeploymentStep.CloseApplications
			});
			App.Hide(this);
		}

		private void Page_Unloaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_checkApplicationsTimer?.Stop();
			_checkApplicationsTimer?.Dispose();
			_forceCloseTimer?.Stop();
			_forceCloseTimer?.Dispose();
		}
	}
}
