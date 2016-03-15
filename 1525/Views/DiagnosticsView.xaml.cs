using System.Collections.ObjectModel;
using System.Windows.Controls;
using PDTUtils.Logic;
using PDTUtils.MVVM.ViewModels;
 
namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for Diagnostics.xaml
    /// </summary>
    public partial class DiagnosticsView
    {
        public DiagnosticsView()
        {
            InitializeComponent();
            var d = new DiagnosticViewModel(new MachineInfo());
            if (!d.AccessLevel)
                this.Visibility = System.Windows.Visibility.Hidden;
            else
                this.Visibility = System.Windows.Visibility.Visible;

            DataContext = d;//new DiagnosticViewModel(new MachineInfo());
        }
        
        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
