using AutoMapper;
using DeploymentToolkit.Modals.Settings.Tray;
using System.Windows;
using System.Windows.Controls;

namespace DeploymentToolkit.TrayApp.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal static bool AllowClose => (App.CurrentPage as IPageWindowSettings)?.AllowClose ?? true;
		internal static bool AllowMinimize => (App.CurrentPage as IPageWindowSettings)?.AllowMinimize ?? true;

		private static readonly IMapper _pageMapper = new MapperConfiguration(cfg =>
		{
			cfg.CreateMap<DefaultPageSettings, PageSettings>();
			cfg.CreateMap<PageSettings, PageSettings>().ForAllMembers(o => o.Condition((source, destination, value) => value != null));
		}).CreateMapper();

		private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		public MainWindow()
		{
			_logger.Trace("Initializing ...");
			InitializeComponent();

			_logger.Trace("Setting language ...");
			var language = LanguageManager.Language;
			TextBlockCompanyBrandingText.Text = language.DTK_BrandingTitle;

			ApplyDefaultTheme();
		}

		internal void ApplyDefaultTheme()
		{
			ApplyTheme(App.Settings.PageSettings);
		}

		internal void ApplyTheme(IPageSettings settings)
		{
			var path = new System.Uri("./Assets/Logo.png", System.UriKind.Relative);
			ImageCompanyBrandingLogo.Source = System.Windows.Media.Imaging.BitmapFrame.Create(path);

			var pageSettings = _pageMapper.Map<PageSettings>(App.Settings.PageSettings);
			if (settings is PageSettings customPageSettings)
			{
				pageSettings = _pageMapper.Map(customPageSettings, pageSettings);
			}

			if(pageSettings.LogoPosition == Alignment.Disabled)
			{
				ImageCompanyBrandingLogo.Visibility = Visibility.Hidden;
				if(pageSettings.TitleAlignment != Alignment.Disabled)
				{
					TextBlockCompanyBrandingText.SetValue(Grid.ColumnSpanProperty, 2);
				}
			}
			else if(pageSettings.LogoPosition == Alignment.Left)
			{
				TextBlockCompanyBrandingText.SetValue(Grid.ColumnProperty, 1);
				ImageCompanyBrandingLogo.Visibility = Visibility.Visible;
				ImageCompanyBrandingLogo.HorizontalAlignment = HorizontalAlignment.Left;
				ImageCompanyBrandingLogo.SetValue(Grid.ColumnProperty, 0);
				ImageCompanyBrandingLogo.SetValue(Grid.ColumnSpanProperty, 1);
			}
			else if(pageSettings.LogoPosition == Alignment.Right)
			{
				TextBlockCompanyBrandingText.SetValue(Grid.ColumnProperty, 0);
				ImageCompanyBrandingLogo.Visibility = Visibility.Visible;
				ImageCompanyBrandingLogo.HorizontalAlignment = HorizontalAlignment.Right;
				ImageCompanyBrandingLogo.SetValue(Grid.ColumnProperty, 1);
				ImageCompanyBrandingLogo.SetValue(Grid.ColumnSpanProperty, 1);
			}
			else if(pageSettings.LogoPosition == Alignment.Center)
			{
				TextBlockCompanyBrandingText.Visibility = Visibility.Hidden;
				ImageCompanyBrandingLogo.SetValue(Grid.ColumnProperty, 0);
				ImageCompanyBrandingLogo.SetValue(Grid.ColumnSpanProperty, 2);
				ImageCompanyBrandingLogo.HorizontalAlignment = HorizontalAlignment.Center;
				ImageCompanyBrandingLogo.Visibility = Visibility.Visible;
			}

			if(pageSettings.TitleAlignment == Alignment.Disabled)
			{
				TextBlockCompanyBrandingText.Visibility = Visibility.Hidden;
				if(pageSettings.LogoPosition != Alignment.Disabled)
				{
					ImageCompanyBrandingLogo.SetValue(Grid.ColumnProperty, 0);
					ImageCompanyBrandingLogo.SetValue(Grid.ColumnSpanProperty, 2);
				}
			}
			else if(pageSettings.TitleAlignment == Alignment.Right)
			{
				TextBlockCompanyBrandingText.TextAlignment = TextAlignment.Right;
			}
			else if(pageSettings.TitleAlignment == Alignment.Left)
			{
				TextBlockCompanyBrandingText.TextAlignment = TextAlignment.Left;
			}
			else if(pageSettings.TitleAlignment == Alignment.Center)
			{
				TextBlockCompanyBrandingText.TextAlignment = TextAlignment.Center;
			}

			if(
				pageSettings.LogoPosition != Alignment.Center &&
				pageSettings.TitleAlignment != Alignment.Disabled &&
				TextBlockCompanyBrandingText.Visibility != Visibility.Visible)
			{
				TextBlockCompanyBrandingText.Visibility = Visibility.Visible;
			}

			if(pageSettings.LogoAlignment != Alignment.Automatic)
			{
				ImageCompanyBrandingLogo.HorizontalAlignment = pageSettings.LogoAlignment switch
				{
					Alignment.Left => HorizontalAlignment.Left,
					Alignment.Right => HorizontalAlignment.Right,
					Alignment.Center => HorizontalAlignment.Center,
					_ => ImageCompanyBrandingLogo.HorizontalAlignment
				};
			}

			UpdateLayout();
			InvalidateVisual();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = !AllowClose;
		}

		private void Window_StateChanged(object sender, System.EventArgs e)
		{
			// Force Window to be displayed
			if(AllowMinimize)
			{
				return;
			}

			if(WindowState == WindowState.Minimized)
			{
				WindowState = WindowState.Normal;
			}
		}
	}
}
