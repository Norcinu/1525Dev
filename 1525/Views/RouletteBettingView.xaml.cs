using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for RouletteBettingView.xaml
    /// </summary>
    public partial class RouletteBettingView : UserControl
    {
        public RouletteBettingView()
        {
            DataContext = new RouletteBettingViewModel();
            InitializeComponent();
        }
    }
}
