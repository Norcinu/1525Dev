using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PDTUtils.Impls;
using PDTUtils.Native;

namespace PDTUtils.Logic
{
	/// <summary>
	/// Interaction logic for HopperUtilsWindow.xaml
	/// </summary>
	public partial class HopperUtilsWindow : Window
	{
		//DoorAndKeyStatus _keyDoor = new DoorAndKeyStatus();
		string[] _contentHeaders = new string[3] { "Set Hopper Floats", "Empty Hoppers", "Refill Hoppers" };
		bool[] _clearHoopers = new bool[2] { false, false };
		Timer _switchTimer = new Timer();
		HopperImpl _hopperImpl = new HopperImpl();
		bool _doLeft = true;
		
		private HopperUtilsWindow()
		{
			FontSize = 22;
			InitializeComponent();
			_switchTimer.Elapsed += timer_CheckHopperDumpSwitch;
		}

		public HopperUtilsWindow(DoorAndKeyStatus kd)
		{
			FontSize = 14;
			InitializeComponent();
		//	_keyDoor = kd;
			_switchTimer.Elapsed += timer_CheckHopperDumpSwitch;
			
			DataContext = kd;
			
			ChangeSelectedItem();
			EmptyLeftHopValue.Content = "£" + BoLib.getHopperFloatLevel(BoLib.getLeftHopper()).ToString("0.00");
			EmptyRightHopValue.Content = "£" + BoLib.getHopperFloatLevel(BoLib.getRightHopper()).ToString("0.00");
		}

		private void button_DoEvent(object sender, EventArgs e)
		{
			var button = sender as Button;
			if (button.Content.ToString() == _contentHeaders[0])
				DoSetFloats();
			else if (button.Content.ToString() == _contentHeaders[1])
				DoEmptyHoppers();
			else if (button.Content.ToString() == _contentHeaders[2])
			{

			}
		}

		private void button_EmptyEvent(object sender, EventArgs e)
		{
			//label1.Content = "Press and Hold Dump Switch";
			//label1.Foreground = Brushes.CadetBlue;
			_switchTimer.Enabled = true;
		}

		private void checkBox_Checked(object sender, EventArgs e)
		{
			var chkbox = sender as CheckBox;
			if (chkbox.Name == "Left")
				_clearHoopers[0] = true;
			else if (chkbox.Name == "Right")
				_clearHoopers[1] = true;
		}

		private void checkbox_UnChecked(object sender, EventArgs e)
		{
			var chkbox = sender as CheckBox;
			if (chkbox.Name == "Left")
				_clearHoopers[0] = false;
			else if (chkbox.Name=="Right")
				_clearHoopers[1] = false;
		}

		public void ChangeSelectedItem()
		{
			var first = -1;
			var open = BoLib.getDoorStatus();
			for (var i = 0; i < TabHoppers.Items.Count; i++)
			{
				var t = TabHoppers.Items[i] as TabItem;
				if (i < 2)
				{
					if (open > 0)
					{
						t.IsEnabled = true;
						if (first == -1)
							first = i;
					}
					else
						t.IsEnabled = false;
				}
				else
				{
					if (open > 0)
						t.IsEnabled = false;
					else
					{
						t.IsEnabled = true;
						if (first == -1)
							first = i;
						else
							t.IsEnabled = false;
					}
				}
				TabHoppers.SelectedIndex = first;
			}
		}

		private void DoSetFloats()
		{
			StackPanel1.IsEnabled = true;
			StackPanel1.Visibility = Visibility.Visible;
			LblLeftHopperValue.Content = "£" + BoLib.getHopperFloatLevel(BoLib.getLeftHopper()).ToString("0.00");
			LblRightHopperValue.Content = "£" + BoLib.getHopperFloatLevel(BoLib.getRightHopper()).ToString("0.00");
		}

		private void DoEmptyHoppers()
		{
			var leftLevel = BoLib.getHopperFloatLevel(BoLib.getLeftHopper());
			var rightLevel = BoLib.getHopperFloatLevel(BoLib.getRightHopper());

			var left = new Label() { Content = "£1 Hopper contains £ " + leftLevel.ToString("0.00"), Foreground = Brushes.Pink };
			var right = new Label() { Content = "10p Hopper contains £ " + rightLevel.ToString("0.00"), Foreground = Brushes.Pink };

			var chkLeft = new CheckBox() { Name = "Left", Content = "Empty the Left Hopper", Foreground = Brushes.DeepPink, FontSize = 22 };
			if (leftLevel == 0)
				chkLeft.IsEnabled = false;

			var chkRight = new CheckBox() { Name = "Right", Content = "Empty the Right Hopper", Foreground = Brushes.DeepPink, FontSize = 22 };
			if (rightLevel == 0)
				chkRight.IsEnabled = false;

			var empty = new Button() { Content = "Empty", Width = 75 };
			empty.Click += button_DoEvent;
		        
			StackPanel1.Children.Add(left);
			StackPanel1.Children.Add(right);
			StackPanel1.Children.Add(chkLeft);
			StackPanel1.Children.Add(chkRight);
			StackPanel1.Children.Add(empty);
		}

		private void timer_CheckHopperDumpSwitch(object sender, ElapsedEventArgs e)
		{
			if (_hopperImpl.DumpSwitchPressed == false)
			{
				if (BoLib.getHopperDumpSwitch() > 0)
				{
					_hopperImpl.DumpSwitchPressed = true;
					_switchTimer.Interval = 1000;
                    BoLib.setUtilRequestBitState((int)UtilBits.DumpLeftHopper);
				}
			}
			else
			{
				if (_doLeft)
				{
					var result = BoLib.getRequestEmptyLeftHopper();
					if (result == 0 && BoLib.getHopperFloatLevel(BoLib.getLeftHopper()) == 0)
					{
                        _doLeft = false;
                        BoLib.setUtilRequestBitState((int)UtilBits.DumpRightHopper);
					}
				}
				else
				{
					var result = BoLib.getRequestEmptyRightHopper();
					if (result == 0 && BoLib.getHopperFloatLevel(BoLib.getRightHopper()) == 0)
					{
						_doLeft = false;
						_switchTimer.Enabled = false;
						_switchTimer.Elapsed -= timer_CheckHopperDumpSwitch;
						BtnEmptyHoppers.IsEnabled = true;
					}
				}

				if (BoLib.getRequestEmptyLeftHopper() > 0)
				{
					EmptyLeftHopValue.Dispatcher.Invoke((DelegateUpdate)EmptyHoppers, EmptyLeftHopValue);
				}
				else if (BoLib.getRequestEmptyRightHopper() > 0)
				{
					EmptyRightHopValue.Dispatcher.Invoke((DelegateUpdate)EmptyHoppers, EmptyRightHopValue);
				}
			}
		}

		public delegate void DelegateUpdate(Label l);
		private void EmptyHoppers(Label l)
		{
			l.Foreground = Brushes.Aqua;
			l.Content = "Hopper Value : £" + BoLib.getHopperFloatLevel(
				_doLeft ? BoLib.getLeftHopper() : BoLib.getRightHopper()).ToString("0.00");
		}

		private void btnSetLeft_Click(object sender, RoutedEventArgs e)
		{
			var leftHopper = BoLib.getLeftHopper();
			BoLib.setHopperFloatLevel(leftHopper, BoLib.getHopperDivertLevel(leftHopper));
		}

		private void btnSetRight_Click(object sender, RoutedEventArgs e)
		{
			var rightHopper = BoLib.getRightHopper();
			BoLib.setHopperFloatLevel(rightHopper, BoLib.getHopperDivertLevel(rightHopper));
		}

		private void btnEmptyHoppers_Click(object sender, RoutedEventArgs e)
		{
			if (ChkEmptyLeft.IsChecked == true || ChkEmptyRight.IsChecked == true)
			{
				_switchTimer.Elapsed += timer_CheckHopperDumpSwitch;
				_switchTimer.Enabled = true;
				BtnEmptyHoppers.IsEnabled = false;
				LblGeneralMsg.Content = "Press and hold dump switch";
			}
			else
			{
				MessageBox.Show("Please select which Hopper(s) to empty", 
								"Select Hopper", MessageBoxButton.OK, 
								MessageBoxImage.Asterisk);
			}
		}
	}
}

