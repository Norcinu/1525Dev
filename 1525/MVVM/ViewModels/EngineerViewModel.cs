using System.Collections.ObjectModel;
using System.Windows.Input;
using AttachedCommandBehavior;
using PDTUtils.Logic;

namespace PDTUtils.MVVM.ViewModels
{
    class EngineerViewModel : BaseViewModel
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

        public EngineerViewModel(string name)
            : base(name)
        {
            TabSelectionChanged = new SimpleCommand()
            {
                ExecuteDelegate = x => DoTabSelectionChanged(x)
            };

            Pages.Add(new EngineerHistoryViewModel("EngineerHistory"));
            Pages.Add(new BirthCertViewModel("Birth Cert"));
            Pages.Add(new TestSuiteViewModel());
            Pages.Add(new DiagnosticViewModel("Diagnostics", new MachineInfo()));
            Pages.Add(new NetworkSettingsViewModel("Network"));
            Pages.Add(new TitoConfigView("TitoConfig"));
            Pages.Add(new UserSoftwareUpdate("Update"));
            Pages.Add(new UsbFileUploaderViewModel("Upload"));

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
