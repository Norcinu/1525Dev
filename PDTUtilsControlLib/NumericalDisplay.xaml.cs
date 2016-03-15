using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace PDTUtilsControlLib
{
	/// <summary>
	/// Interaction logic for NumericalDisplay.xaml
	/// </summary>
	public partial class NumericalDisplay : UserControl
	{
		uint _value = 0;
		public uint Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public NumericalDisplay()
		{
			Value = 0;
			InitializeComponent();
		}

		public string FormHeader { get { return label1.Content.ToString(); } set { label1.Content = value; } }

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			if (Value > 0)
				Value--;
			textBlock1.Text = Value.ToString();
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			if (Value < int.MaxValue)
				Value++;
			textBlock1.Text = Value.ToString();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			textBlock1.Text = Value.ToString();
		}
	}
}
