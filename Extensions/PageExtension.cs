using AutoMapper;
using DeploymentToolkit.Modals.Settings.Tray;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DeploymentToolkit.TrayApp.Extensions
{
	public static class PageExtension
	{
		private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		private static readonly IMapper _defaultMapper = new MapperConfiguration(cfg =>
		{
			cfg.CreateMap<DefaultButtonSettings, ButtonSettings>();
			cfg.CreateMap<ButtonSettings, ButtonSettings>().ForAllMembers(o => o.Condition((source, destination, value) => value != null));
			cfg.CreateMap<DefaultTextBlockSettings, TextBlockSettings>();
			cfg.CreateMap<TextBlockSettings, TextBlockSettings>().ForAllMembers(o => o.Condition((source, destination, value) => value != null));
		}).CreateMapper();

		private static T2 MergeSettings<T1, T2>(T1 defaultSettings, string controlName)
		{
			// Merge default settings
			var mergedSettings = _defaultMapper.Map<T2>(defaultSettings);
			// Get settings
			var controlConfig = defaultSettings.GetType().GetProperties().FirstOrDefault(c => c.Name == controlName);
			if(controlConfig != null)
			{
				var instance = (T2)controlConfig.GetValue(defaultSettings);
				if(instance != null)
				{
					mergedSettings = _defaultMapper.Map(instance, mergedSettings);
				}
			}
			return mergedSettings;
		}

		public static void ApplyButtonThemes(this Page page, DefaultButtonSettings settings, Button[] buttons)
		{
			foreach(var button in buttons)
			{
				var buttonSettings = MergeSettings<DefaultButtonSettings, ButtonSettings>(settings, button.Name);

				if(buttonSettings.Height.HasValue)
				{
					button.Height = buttonSettings.Height.Value;
				}
				if(buttonSettings.Width.HasValue)
				{
					button.Width = buttonSettings.Width.Value;
				}
				if(!string.IsNullOrEmpty(buttonSettings.FontColor))
				{
					button.Foreground = new BrushConverter().ConvertFromString(buttonSettings.FontColor) as SolidColorBrush;
				}
				if(!string.IsNullOrEmpty(buttonSettings.FontWeight))
				{
					var fontWeight = typeof(FontWeights).GetProperties().FirstOrDefault(p => p.Name == buttonSettings.FontWeight);
					if(fontWeight != null)
					{
						button.FontWeight = (FontWeight)fontWeight.GetValue(typeof(FontWeights), null);
					}
				}
				if(!string.IsNullOrEmpty(buttonSettings.BackgroundColor))
				{
					button.Background = new BrushConverter().ConvertFromString(buttonSettings.BackgroundColor) as SolidColorBrush;
				}
				if(!string.IsNullOrEmpty(buttonSettings.BorderColor))
				{
					button.BorderBrush = new BrushConverter().ConvertFromString(buttonSettings.BorderColor) as SolidColorBrush;
				}
			}
		}

		public static void ApplyTextBlockThemes(this Page page, DefaultTextBlockSettings settings, TextBlock[] textBlocks)
		{
			foreach(var textBlock in textBlocks)
			{
				var textBlockSettings = MergeSettings<DefaultTextBlockSettings, TextBlockSettings>(settings, textBlock.Name);

				if(!string.IsNullOrEmpty(textBlockSettings.FontColor))
				{
					textBlock.Foreground = new BrushConverter().ConvertFromString(textBlockSettings.FontColor) as SolidColorBrush;
				}
				if(!string.IsNullOrEmpty(textBlockSettings.FontWeight))
				{
					var fontWeight = typeof(FontWeights).GetProperties().FirstOrDefault(p => p.Name == textBlockSettings.FontWeight);
					if(fontWeight != null)
					{
						textBlock.FontWeight = (FontWeight)fontWeight.GetValue(typeof(FontWeights), null);
					}
				}
				if(!string.IsNullOrEmpty(textBlockSettings.BackgroundColor))
				{
					textBlock.Background = new BrushConverter().ConvertFromString(textBlockSettings.BackgroundColor) as SolidColorBrush;
				}
			}
		}
	}
}
