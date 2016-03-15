using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for CreditManagementView.xaml
    /// </summary>
    public partial class CreditManagementView : UserControl
    {
        public CreditManagementView()
        {
            InitializeComponent();
            this.DataContext = new CreditManagementViewModel();
        }
    }
}
