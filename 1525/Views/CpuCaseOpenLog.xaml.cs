﻿using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for CpuCaseOpenLog.xaml
    /// </summary>
    public partial class CpuCaseOpenLog : UserControl
    {
        public CpuCaseOpenLog()
        {
            InitializeComponent();
            DataContext = new CpuCaseOpenViewModel();
        }
    }
}
