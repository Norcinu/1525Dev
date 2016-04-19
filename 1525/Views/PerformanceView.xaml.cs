using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for PerformanceView.xaml
    /// </summary>
    public partial class PerformanceView : UserControl
    {
        public PerformanceView()
        {
            InitializeComponent();
            /*this.DataContext = new MetersViewModel();*/
        }

        public void RefreshMang()
        {
            var mvm = DataContext as MetersViewModel;
            mvm.Refresh();
        }
    }
}
