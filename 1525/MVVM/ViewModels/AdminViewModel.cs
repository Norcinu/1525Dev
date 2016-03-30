using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AttachedCommandBehavior;

namespace PDTUtils.MVVM.ViewModels
{
    class AdminViewModel : BaseViewModel
    {
        ObservableCollection<BaseViewModel> _pages = new ObservableCollection<BaseViewModel>();
        public ObservableCollection<BaseViewModel> Pages
        {
            get { return _pages; }
            set
            {
                if (_pages == null)
                    _pages = new ObservableCollection<BaseViewModel>();
                
                _pages = value;
                RaisePropertyChangedEvent("Pages");
            }
        }

        public ICommand TabSelectionChanged { get; set; }

        void DoTabSelectionChanged()
        {
            System.Diagnostics.Debug.WriteLine("Je frog?");
        }

        public AdminViewModel(string name)
            : base(name)
        {
            TabSelectionChanged = new SimpleCommand()
            {
                ExecuteDelegate = x => DoTabSelectionChanged()
            };
        }
    }
}
