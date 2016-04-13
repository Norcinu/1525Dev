using System.Collections.ObjectModel;
using System.Windows.Input;
using AttachedCommandBehavior;

namespace PDTUtils.MVVM.ViewModels
{
    class ManufacturerViewModel : BaseViewModel
    {
        BaseViewModel _currentPage = null;
        ObservableCollection<BaseViewModel> _pages = new ObservableCollection<BaseViewModel>();
        public ICommand TabSelectionChanged { get; set; }

        public ManufacturerViewModel(string name)
            : base(name)
        {
            TabSelectionChanged = new SimpleCommand()
            {
                ExecuteDelegate = x => DoTabSelectionChanged(x)
            };

            //Pages.Add(new MachineIniViewModel("Manfacturer MI"));
            Pages.Add(new ManufacturerBirthCertViewModel("Manufacturer BC"));
            CurrentPage = Pages[0];
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
        
        public ObservableCollection<BaseViewModel> Pages
        {
            get
            {
                if (_pages == null)
                    _pages = new ObservableCollection<BaseViewModel>();
                return _pages;
            }
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
