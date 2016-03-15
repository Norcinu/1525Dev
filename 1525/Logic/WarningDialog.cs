using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace PDTUtils.Logic
{
    class WarningDialog : Window
    {
        public string Message { get; set; }
        public string Caption { get; set; }

        public WarningDialog(string message, string caption)
        {
            Message = message;
            Caption = caption;

            var stp = new StackPanel();
            stp.Children.Add(new Label() { Content = Message, Margin = new Thickness(20, 20, 10, 20) });
            var btn = new Button() { Content = "CLOSE", FontSize = 26, Width = 100, Height = 50, Margin = new Thickness(0, 10, 0, 20) };
            btn.Click += new RoutedEventHandler(btn_Click);
            stp.Children.Add(btn);

            Title = Caption;
            Content = stp;
            SizeToContent = SizeToContent.WidthAndHeight;
            ResizeMode = ResizeMode.NoResize;
            Margin = new Thickness(20);
            Height = 640;  // just added to have a smaller control (Window)
            Width = 480;
            FontSize = 24;
            Background = Brushes.LightGray;
            Foreground = Brushes.Red;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            //var windows = Application.Current.Windows;
            //windows[1].Close();
        }

        public void ShowWindow(string message, string caption)
        {
            this.ShowDialog();

            /*Window window = new Window
            {
                Title = "ERROR",
                Content = stp,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Margin = new Thickness(20),
                Height = 640,  // just added to have a smaller control (Window)
                Width = 480,
                FontSize = 24,
                Background = Brushes.LightGray,
                Foreground = Brushes.Red,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };*/

            //window.ShowDialog();
        }
    }
}
