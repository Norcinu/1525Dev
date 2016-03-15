using System.Windows.Controls;
using System.Collections.Generic;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for GameChartView.xaml
    /// </summary>
    public partial class GameChartView : UserControl
    {
        public GameChartView()
        {
            InitializeComponent();
            DataContext = new GameChartViewModel();
        }
    }
}
