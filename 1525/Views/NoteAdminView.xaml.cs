using System.Windows.Controls;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for NoteAdminView.xaml
    /// </summary>
    public partial class NoteAdminView : UserControl
    {
        public NoteAdminView()
        {
            InitializeComponent();
            DataContext = new PDTUtils.MVVM.ViewModels.NoteAdminViewModel();
        }
        
        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // var dc = DataContext as PDTUtils.MVVM.ViewModels.NoteAdminViewModel
        }
        
    }
}
