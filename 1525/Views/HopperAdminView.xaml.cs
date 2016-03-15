using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for HopperAdminView.xaml
    /// </summary>
    public partial class HopperAdminView : UserControl
    {
        public HopperAdminView()
        {
            InitializeComponent();
            DataContext = new HopperViewModel();
        }
        
        //yes ok this breaks MVVM but yer kna its already broken in this app, so fuck it.
        void cmbHoppers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = DataContext as HopperViewModel;
            dc.RefreshLevels();
        }
    }
}
