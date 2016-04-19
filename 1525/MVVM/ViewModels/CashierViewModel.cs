using System.Collections.ObjectModel;
using System.Windows.Input;
using AttachedCommandBehavior;

namespace PDTUtils.MVVM.ViewModels
{
    class CashierViewModel : BaseViewModel
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

        public CashierViewModel(string name)
            : base(name)
        {
            TabSelectionChanged = new SimpleCommand()
            {
                ExecuteDelegate = x => DoTabSelectionChanged(x)
            };

            Pages.Add(new MainPageViewModel("MainPage"));
            Pages.Add(new CashierLevelHistoryViewModel("CashierHistory"));

            CurrentPage = Pages[0];
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var p in Pages)
                p.Cleanup();
        }

        void DoTabSelectionChanged(object o)
        {
            if (o == null)
                return;

            var index = o as int?;

            if ((int)index < Pages.Count)
                CurrentPage = Pages[(int)index];
        }
    }
}
