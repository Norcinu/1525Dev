using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;

namespace PDTUtilsControlLib
{
    /// <summary>
    /// Interaction logic for SetDateControl.xaml
    /// </summary>
    public partial class SetDateControl : UserControl
    {
        DateTime m_currentDate;
		Timer t = new Timer(1000);

		public DateTime SystemDate
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

		public static DependencyProperty DateProperty = DependencyProperty.Register("SystemDate", 
			typeof(DateTime), typeof(SetTimeControl));

		public SetDateControl()
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
			txtHour.Text = m_currentDate.Day.ToString("00");
			txtMinute.Text = m_currentDate.Month.ToString("00");
			txtSeconds.Text = m_currentDate.Year.ToString("0000");
		}

		private void btnDayInc_Click(object sender, RoutedEventArgs e)
		{
			var hour = Convert.ToInt32(txtHour.Text);
            if (hour < 30)
                hour++;
			txtHour.Text = hour.ToString("00");
		}

		private void btnDayDec_Click_1(object sender, RoutedEventArgs e)
		{
			var hour = Convert.ToInt32(txtHour.Text);
            if (hour > 1)
                hour--;
			txtHour.Text = hour.ToString("00");
		}

		private void btnMonthInc_Click(object sender, RoutedEventArgs e)
		{
            var minute = Convert.ToInt32(txtMinute.Text);
            if (minute < 12)
                minute++;
            txtMinute.Text = minute.ToString("00");
		}

		private void btnMonthDec_Click(object sender, RoutedEventArgs e)
		{
            var minute = Convert.ToInt32(txtMinute.Text);
            if (minute > 1)
                minute--;
			txtMinute.Text = minute.ToString("00");
		}

		int IncrementValue(TextBlock tb, bool hour = true)
		{
			var value = Convert.ToInt32(tb.Text);
			var maxValue = (hour == true) ? 1 : 31;

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

        private void btnSaveDate_Click(object sender, RoutedEventArgs e)
        {
            SYSTEMTIME newTime = new SYSTEMTIME();
            GetSystemTime(ref newTime);
            short hour = 0;
            short.TryParse(txtHour.Text, out hour);
            short minute = 0;
            short.TryParse(txtMinute.Text, out minute);
            short year = 0;
            short.TryParse(txtSeconds.Text, out year);
            newTime.day = hour;
            newTime.month = minute;
            newTime.year = year;
            SetSystemTime(ref newTime);
            
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C EWFMGR C: -COMMIT";
            process.StartInfo = startInfo;
            process.Start();
            process.EnableRaisingEvents = true;
            MessageBox.Show("New Date Saved.\n\nPlease reboot or changes will be lost.", "INFO", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        
        private void btnSecondsInc_Click(object sender, RoutedEventArgs e)
        {
            var year = Convert.ToInt32(txtSeconds.Text);
            year++;
            txtSeconds.Text = year.ToString("0000");
        }

        private void btnSecondsDec_Click(object sender, RoutedEventArgs e)
        {
            var year = Convert.ToInt32(txtSeconds.Text);
            year--;
            txtSeconds.Text = year.ToString("0000");
        }
	}
}
