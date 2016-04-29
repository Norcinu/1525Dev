using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AttachedCommandBehavior;
using PDTUtils.Native;
using PDTUtils.Logic;

namespace PDTUtils.MVVM.ViewModels
{
    class CollectorViewModel : BaseViewModel
    {
        BaseViewModel _currentPage = null;
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

        public BaseViewModel CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    RaisePropertyChangedEvent("CurrentPage");
                }
            }
        }
        public ICommand TabSelectionChanged { get; set; }
        
        public CollectorViewModel(string name)
            : base(name)
        {
            TabSelectionChanged = new SimpleCommand()
            {
                ExecuteDelegate = x => DoTabSelectionChanged(x)
            };

            Pages.Add(new MetersViewModel());
            Pages.Add(new EmptyMachineViewModel());
            Pages.Add(new AuditViewModel());

            CurrentPage = Pages[0];
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var p in Pages)
                p.Cleanup();
        }

        public override void Refresh()
        {
            base.Refresh();
            foreach (var p in Pages)
                p.Refresh();
        }
        
        void DoTabSelectionChanged(object o)
        {
            if (o == null)
                return;

            var index = o as int?;

            if ((int)index < Pages.Count)
            {
                CurrentPage = Pages[(int)index];
                if (CurrentPage.Name == "Empty")
                {
                    var empty = CurrentPage as EmptyMachineViewModel;
                    empty.Refresh();
                }
            }
        }
    }
}
