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

		private static readonly IMapper _buttonMapper = new MapperConfiguration(cfg =>
		{
			cfg.CreateMap<DefaultButtonSettings, ButtonSettings>();
			cfg.CreateMap<ButtonSettings, ButtonSettings>().ForAllMembers(o => o.Condition((source, destination, value) => value != null));
		}).CreateMapper();

		private static readonly IMapper _textBlockMapper = new MapperConfiguration(cfg =>
		{
			cfg.CreateMap<DefaultTextBlockSettings, TextBlockSettings>();
			cfg.CreateMap<TextBlockSettings, TextBlockSettings>().ForAllMembers(o => o.Condition((source, destination, value) => value != null));
		}).CreateMapper();

		public static void ApplyButtonThemes(this Page page, DefaultButtonSettings settings, Button[] buttons)
		{
			foreach(var button in buttons)
			{
				var buttonSettings = _buttonMapper.Map<ButtonSettings>(settings);

				var buttonConfig = settings.GetType().GetProperties().FirstOrDefault(p => p.Name == button.Name);
				if(buttonConfig != null)
				{
					var instance = (ButtonSettings)buttonConfig.GetValue(settings);
					if(instance != null)
					{
						buttonSettings = _buttonMapper.Map(instance, buttonSettings);
					}
				}

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
				var textBlockSettings = _textBlockMapper.Map<TextBlockSettings>(settings);

				var textBlockConfig = settings.GetType().GetProperties().FirstOrDefault(p => p.Name == textBlock.Name);
				if(textBlockConfig != null)
				{
					var instance = (TextBlockSettings)textBlockConfig.GetValue(settings);
					if(instance != null)
					{
						textBlockSettings = _textBlockMapper.Map(instance, textBlockSettings);
					}
				}

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
