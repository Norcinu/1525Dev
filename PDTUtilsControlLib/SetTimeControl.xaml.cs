using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Timers;
using System.Diagnostics;

namespace PDTUtilsControlLib
{
	/// <summary>
	/// Interaction logic for SetTimeControl.xaml
	/// </summary>
	public partial class SetTimeControl : UserControl
	{
		DateTime m_currentDate;
		Timer t = new Timer(1000);

		public DateTime CurrentDate
		{
			get
			{
				var hour = Convert.ToInt32(txtHour.Text);
				var minute = Convert.ToInt32(txtMinute.Text);
				var second = Convert.ToInt32(txtSeconds.Text);
				return new DateTime(m_currentDate.Day, m_currentDate.Month, m_currentDate.Year,
					hour, minute, second);
			}
		}

		public static DependencyProperty TimeProperty = DependencyProperty.Register("CurrentDate", 
			typeof(DateTime), typeof(SetTimeControl));

		public SetTimeControl()
		{
			InitializeComponent();
			t.Enabled = true;
			var timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1.0)
			};
			
			timer.Tick += (o, e) =>
			{
				txtSeconds.Text = DateTime.Now.Second.ToString("00");
			};

			m_currentDate = DateTime.Now;
			txtHour.Text = m_currentDate.Hour.ToString("00");
			txtMinute.Text = m_currentDate.Minute.ToString("00");
			txtSeconds.Text = m_currentDate.Second.ToString("00");
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var hour = IncrementValue(txtHour);
			txtHour.Text = hour.ToString("00");
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var hour = DecrementValue(txtHour);
			txtHour.Text = hour.ToString("00");
		}

		private void btnMinuteInc_Click(object sender, RoutedEventArgs e)
		{
			var minute = IncrementValue(txtMinute, false);
			txtMinute.Text = minute.ToString("00");
		}

		private void btnMinuteDec_Click(object sender, RoutedEventArgs e)
		{
			var minute = DecrementValue(txtMinute, false);
			txtMinute.Text = minute.ToString("00");
		}

		int IncrementValue(TextBlock tb, bool hour = true)
		{
			var value = Convert.ToInt32(tb.Text);
			var maxValue = (hour == true) ? 23 : 59;

			if (value < maxValue)
				value++;
			else
				value = 0;

			return value;
		}

		int DecrementValue(TextBlock tb, bool hour = true)
		{
			var value = Convert.ToInt32(tb.Text);
			var maxValue = (hour == true) ? 23 : 59;

			if (value > 0)
				value--;
			else
				value = maxValue;

			return value;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SYSTEMTIME
		{
			public short year;
			public short month;
			public short dayOfWeek;
			public short day;
			public short hour;
			public short minute;
			public short second;
			public short milliseconds;
		}

		[DllImport("kernel32.dll")]
		private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

		[DllImport("kernel32.dll")]
		private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);
		private void btnSaveTime_Click(object sender, RoutedEventArgs e)
		{
			SYSTEMTIME newTime = new SYSTEMTIME();
			GetSystemTime(ref newTime);
			short hour = 0;
			short.TryParse(txtHour.Text, out hour);
			short minute = 0;
			short.TryParse(txtMinute.Text, out minute);
			newTime.hour = hour;
			newTime.minute = minute;
			SetSystemTime(ref newTime);

            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C EWFMGR C: -COMMIT";
            process.StartInfo = startInfo;
            process.Start();

            MessageBox.Show("New Time Saved.\n\nPlease reboot or changes will be lost.", "INFO", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
	}
}
