using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for ScreenshotView.xaml
    /// </summary>
    public partial class ScreenshotView : UserControl
    {
        public ScreenshotView()
        {
            InitializeComponent();
            this.DataContext = new ScreenshotViewModel();
        }
    }
}
