using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for RegionalSettings.xaml
    /// </summary>
    public partial class RegionalSettingsView : UserControl
    {
        public RegionalSettingsView()
        {
            InitializeComponent();
            this.DataContext = new RegionalSettingsViewModel();
        }

        private void lvStreetMarkets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}


