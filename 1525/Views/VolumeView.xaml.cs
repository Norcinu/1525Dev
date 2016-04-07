using System;
using System.Windows;
using System.Windows.Controls;
using PDTUtils.Native;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for VolumeView.xaml
    /// </summary>
    public partial class VolumeView : UserControl
    {
        bool _initialSlider = true;

        public VolumeView()
        {
            InitializeComponent();
            MasterVolumeSlider.Value = BoLib.getLocalMasterVolume();
        }

        private void MasterVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var volume = Convert.ToUInt32(MasterVolumeSlider.Value);
            BoLib.setLocalMasterVolume(volume);
            if (!_initialSlider)
            {
                BoLib.justPlaySound(@"d:\1525\wav\volume.wav");
            }
            _initialSlider = false;
        }
    }
}
