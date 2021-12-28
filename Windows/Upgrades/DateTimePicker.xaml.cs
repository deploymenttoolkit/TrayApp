using DeploymentToolkit.Modals.Settings.Tray;
using DeploymentToolkit.TrayApp.Extensions;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace DeploymentToolkit.TrayApp.Windows.Upgrades
{
	/// <summary>
	/// Interaction logic for DateTimePicker.xaml
	/// </summary>
	public partial class DateTimePicker : Page, IPageWindowSettings
	{
		public static DateTimePicker Instance { get; set; }

		public string TimeResult { get; set; }
		public DateTime DateResult { get; set; }

		public string PageName => "DateTimePicker";
		public int? RequestedHeight => 450;
		public int? RequestedWidth => 800;
		public bool AllowMinimize => false;
		public bool AllowClose => false;

		private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		public DateTimePicker()
		{
			_logger.Trace("Initializing ...");
			InitializeComponent();
			DataContext = this;
			Instance = this;

			_logger.Trace("Applying theme ...");
			this.ApplyButtonThemes(App.Settings.BrandingSettings.ButtonSettings, new[] { DateTimePickerCancel, DateTimePickerConfirm });
			this.ApplyTextBlockThemes(App.Settings.BrandingSettings.TextBlockSettings, new[] { DateTimePickerTitle, DateTimePickerReminder });

			_logger.Trace("Setting language ...");
			var language = LanguageManager.Language;
			DateTimePickerConfirm.Content = language.UpgradePrompt_ButtonConfirm;
			DateTimePickerCancel.Content = language.UpgradePrompt_ButtonCancel;
			DateTimePickerTitle.Text = language.UpgradePrompt_ScheduleTitle;
			DateTimePickerOptionOne.Content = language.UpgradePrompt_ScheduleOptionOne;
			DateTimePickerOptionTwo.Content = language.UpgradePrompt_ScheduleOptionTwo;
			DateTimePickerReminder.Text = language.UpgradePrompt_ScheduleReminder;

			_logger.Trace("Configuring ...");

			ScheduledDate.DisplayDateStart = DateTime.Now;
			ScheduledDate.DisplayDateEnd = DateTime.Now.AddDays(7);

			DateResult = DateTime.Now.Date;
			TimeResult = "17:00";
		}

		private void DateTimePickerCancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			App.NavigateTo(new Scheduler());
		}

		private void DateTimePickerConfirm_Click(object sender, System.Windows.RoutedEventArgs e)
		{

		}
	}

	public class TimeValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if(DateTimePicker.Instance?.DateTimePickerConfirm != null)
			{
				DateTimePicker.Instance.DateTimePickerConfirm.IsEnabled = false;
			}

			var valueToTest = Convert.ToString(value);
			if(string.IsNullOrEmpty(valueToTest))
			{
				return new ValidationResult(false, $"Invalid time");
			}

			if(!TimeOnly.TryParse(valueToTest, out _))
			{
				return new ValidationResult(false, $"Invalid time");
			}

			// TODO: This only validates this prompt and not the Date. This needs to be changed
			if(DateTimePicker.Instance?.DateTimePickerConfirm != null)
			{
				DateTimePicker.Instance.DateTimePickerConfirm.IsEnabled = true;
			}

			return new ValidationResult(true, null);
		}
	}

	public class DateValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if(DateTimePicker.Instance?.DateTimePickerConfirm != null)
			{
				DateTimePicker.Instance.DateTimePickerConfirm.IsEnabled = false;
			}

			var selectedDate = (DateTime)value;
			if(selectedDate < DateTime.Now.Date)
			{
				return new ValidationResult(false, "Date cannot be in the past");
			}
			else if(selectedDate > DateTime.Now.AddDays(7))
			{
				return new ValidationResult(false, "Date cannot be more than 7 days in the future");
			}

			// TODO: This only validates this prompt and not the Time. This needs to be changed
			if(DateTimePicker.Instance?.DateTimePickerConfirm != null)
			{
				DateTimePicker.Instance.DateTimePickerConfirm.IsEnabled = true;
			}

			return new ValidationResult(true, null);
		}
	}
}
