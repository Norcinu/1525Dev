using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;


namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for GeneralSettings.xaml
    /// </summary>
    public partial class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView()
        {
            InitializeComponent();
            this.DataContext = new GeneralSettingsViewModel();
        }
    }
}
