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
        
        BaseViewModel _currentPage = null;
        public BaseViewModel CurrentPage
        {
            get
            {
                if (_currentPage == null)
                    return null;

                return _currentPage;
            }

            set
            {
                _currentPage = value;
                RaisePropertyChangedEvent("CurrentPage");
            }
        }

        public ICommand TabSelectionChanged { get; set; }
        
        void DoTabSelectionChanged(object o)
        {
            if (o == null)
                return;
            
            var index = o as int?;

            if ((int)index < Pages.Count)
                CurrentPage = Pages[(int)index];
        }
        
        public AdminViewModel(string name)
            : base(name)
        {
            TabSelectionChanged = new SimpleCommand()
            {
                ExecuteDelegate = x => DoTabSelectionChanged(x)
            };
            
            Pages.Add(new GameSettingViewModel("GameSettings"));
            Pages.Add(new ConfigureViewModel("Configure"));
            Pages.Add(new GameStatisticsViewModel());
            Pages.Add(new CashMatchViewModel("CashMatch"));
            Pages.Add(new BirthCertViewModel("BirthCert"));
            Pages.Add(new GeneralSettingsViewModel("General"));
            Pages.Add(new DateTimeViewModel("DateTime"));
            Pages.Add(new VolumeViewModel("Volume"));
            CurrentPage = Pages[0];
        }
    }
}
