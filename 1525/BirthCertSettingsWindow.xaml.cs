using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;


namespace PDTUtils
{
    public class HelpMessageWindow : Window
    {
        public string Message { get; set; }

        public HelpMessageWindow()
        {
        }

        public HelpMessageWindow(string message)
        {
            Background = System.Windows.Media.Brushes.HotPink;
            SizeToContent = SizeToContent.Width;
            Message = message;
            this.AddChild(new Label()
            {
                Content = Message,
                Margin = new Thickness(15),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 24,
                Background = System.Windows.Media.Brushes.Red,
                Foreground = System.Windows.Media.Brushes.Yellow
            });
        }
    }
    
    public partial class BirthCertSettingsWindow : Window
    {
        #region options
        public string OptionValue { get; set; }
        public string OptionField { get; set; }
        #endregion

        public ChangeType RetChangeType { get; set; }

        public BirthCertSettingsWindow()
        {
            InitializeComponent();
        }

        public BirthCertSettingsWindow(string f, string v)//, int index)
        {
            InitializeComponent();
            OptionField = f;
            OptionValue = v;
            TxtNewValue.Text = OptionValue;
            RetChangeType = ChangeType.None;

            if (OptionField[0] == '#')
                BtnComment.Content = "Enable";
            else
                BtnComment.Content = "Disable";

            Left = (1920 / 2) - (300 / 2);
            Top = (1080 / 2) - (136 / 2);

        }

        void button2_Click(object sender, RoutedEventArgs e)
        {
            RetChangeType = ChangeType.Cancel;
            Close();
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            RetChangeType = ChangeType.Amend;
            OptionValue = TxtNewValue.Text;
            Close();
        }
        
        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            if (OptionField[0] == '#')
            {
                OptionField = OptionField.Substring(1);
                RetChangeType = ChangeType.Uncomment;
                Close();
            }
            else
            {
                OptionField = OptionField.Insert(0, "#");
                RetChangeType = ChangeType.Comment;
                Close();
            }
        }
    }
}
