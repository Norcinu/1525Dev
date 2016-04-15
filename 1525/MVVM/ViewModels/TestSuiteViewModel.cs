using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class TestSuiteViewModel : BaseViewModel
    {
        #region Private members
        ICommand _changePageCommand;

        ObservableCollection<BaseViewModel> _pages = new ObservableCollection<BaseViewModel>();
        BaseViewModel _currentPage;
        #endregion

        #region Public properties
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

        public ObservableCollection<BaseViewModel> Pages
        {
            get
            {
                if (_pages == null)
                    _pages = new ObservableCollection<BaseViewModel>();
                return _pages;
            }
        }
        
        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(p => ChangeViewModel((BaseViewModel)p), p => p is BaseViewModel);
                }

                return _changePageCommand;
            }
        }
        #endregion

        public TestSuiteViewModel()
            : base("TestSuiteViewModel")
        {
            Pages.Add(new ButtonTestViewModel("Buttons"));
            Pages.Add(new PrinterTestViewModel("Printer"));
            Pages.Add(new LampTestViewModel("Lamps"));
            Pages.Add(new DilSwitchViewModel("Dil Switches"));
            Pages.Add(new NoteTestViewModel("Note"));
            Pages.Add(new CoinTestViewModel("Coin"));
        }

        void ChangeViewModel(BaseViewModel vm)
        {
            if (vm == null) return;

            if (CurrentPage != null)
            {
                if (CurrentPage.Name.Equals("Note"))
                {
                    var temp = CurrentPage as NoteTestViewModel;
                    temp.Cleanup();
                }
                else if (CurrentPage.Name.Equals("Coin"))
                {
                    var temp = CurrentPage as CoinTestViewModel;
                    temp.Cleanup();
                }
            }

            CurrentPage = vm;

            if (CurrentPage.Name.Equals("Note"))
            {
                var temp = CurrentPage as NoteTestViewModel;
                temp.StartThread();
            }
            else if (CurrentPage.Name.Equals("Coin"))
            {
                var temp = CurrentPage as CoinTestViewModel;
                temp.StartThread();
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var vm in Pages)
            {
                vm.Cleanup();
            }
            BoLib.clearUtilRequestBitState((int)UtilBits.TestSwitch);
        }
    }
}
