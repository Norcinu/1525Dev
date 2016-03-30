using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PDTUtils.Logic;
using PDTUtils.MVVM.ViewModels;
using PDTUtils.Native;
using Timer = System.Timers.Timer;
using System.Windows.Controls;
using System.Threading;



namespace PDTUtils.MVVM
{
    class ApplicationViewModel : ObservableObject
    {
        #region Private Member variables
        bool _libraryInitOk = false;
        bool _hasSmartCard = false;
        bool _doorStateChanged = false;

        int _currentPageIndex = 7; // None

        BaseViewModel _currentPage = null;
        ObservableCollection<BaseViewModel> _pages = new ObservableCollection<BaseViewModel>();
        ICommand _changePageCommand;
        WpfMessageBoxService _msg = new WpfMessageBoxService();
        
        string _errorMessage = "";
        string[] _strings = new string[8] {"Player", "Cashier", "Collector", "Engineer", "Administrator", 
                                           "Distributor", "Manufacturer", "None"};
        string _dateTimeStr = DateTime.Now.ToLongDateString() + " : " + DateTime.Now.ToLongTimeString();
        string _doorStateStr = "Closed";
        string _smartCardStatus = "NONE PRESENT";

        Timer _dateTimer;
        Timer _doorStateTimer;
        Timer _checkYourPrivilege;

        Brush _doorMsgBackground = Brushes.HotPink;
        Brush _doorMsgForeground = Brushes.Red;

        #endregion

        #region Public Properties
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

        public ICommand ExitApp
        {
            get
            {
                return new RelayCommand(o => DoAppExit());
            }
        }

        public ICommand VmContentRendered
        {
            get { return new RelayCommand(o => DoVmContentRendered()); }
        }

        public string DateTimeStr
        {
            get { return _dateTimeStr; }
            set
            {
                _dateTimeStr = value;
                RaisePropertyChangedEvent("DateTimeStr");
            }
        }

        public string DoorStateStr
        {
            get { return _doorStateStr; }
            set
            {
                _doorStateStr = value;
                RaisePropertyChangedEvent("DoorStateStr");
            }
        }

        public System.Windows.Media.Brush DoorMsgBackground
        {
            get { return _doorMsgBackground; }
            set
            {
                _doorMsgBackground = value;
                RaisePropertyChangedEvent("DoorMsgBackground");
            }
        }

        public System.Windows.Media.Brush DoorMsgForeground
        {
            get { return _doorMsgForeground; }
            set
            {
                _doorMsgForeground = value;
                RaisePropertyChangedEvent("DoorMsgForeground");
            }
        }

        public string AccessLevel
        {
            get { return _smartCardStatus; }
            set
            {
                _smartCardStatus = value;
                RaisePropertyChangedEvent("AccessLevel");
            }
        }
        #endregion

        #region Public Methods
        public ApplicationViewModel()
        {
            InitialiseBoLib();

            LoadSmartCardSetting();

            if (_hasSmartCard)
                _currentPageIndex = 0;

            //Pages.Add(new DefaultViewModel("Main"));
            Pages.Add(new CashierViewModel("Cashier"));
            Pages.Add(new CollectorViewModel("Collector"));
            Pages.Add(new EngineerViewModel("Engineer"));
            Pages.Add(new AdminViewModel("Admin"));

            foreach (var p in Pages)
                p.States.HasSmartCard = _hasSmartCard;

            _currentPage = Pages[0];
            
            _dateTimer = new Timer(1000.00);
            _dateTimer.Elapsed += new System.Timers.ElapsedEventHandler(_dateTimer_Elapsed);
            _dateTimer.Start();

            _doorStateTimer = new Timer(100.00);
            _doorStateTimer.Elapsed += new System.Timers.ElapsedEventHandler(_doorStateTimer_elapsed);
            _doorStateTimer.Start();

            _checkYourPrivilege = new Timer() { Enabled = true, Interval = 100 };
            _checkYourPrivilege.Elapsed += new System.Timers.ElapsedEventHandler(_checkYourPrivilege_Elapsed);
        }

        void _checkYourPrivilege_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var group = BoLib.getSmartCardGroup();
            if (group > 0 && group < 7)
            {
                if (_currentPageIndex > group)
                {
                    CurrentPage = Pages[0];
                }
            }
            else
            {
                if (CurrentPage != null)
                {
                    CurrentPage = Pages[0];
                }
            }
        }
        
        void _dateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTimeStr = DateTime.Now.ToLongDateString() + " : " + DateTime.Now.ToLongTimeString();
        }

        void _doorStateTimer_elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DoorStateStr = !BoLib.getUtilDoorAccess() ? "Closed" : "Open";

            if (DoorStateStr.Equals("Open"))
            {
                DoorMsgBackground = Brushes.LightBlue;
                DoorMsgForeground = Brushes.Yellow;

                if (!_doorStateChanged)
                {
                    _doorStateChanged = true;
                    var s = new System.Media.SoundPlayer(Properties.Resources.door_open_sound);
                    s.Play();
                }
            }
            else
            {
                DoorMsgBackground = Brushes.Black;
                DoorMsgForeground = Brushes.Red;
                _doorStateChanged = false;
            }

            if (_hasSmartCard)
            {
                var scStatus = BoLib.getSmartCardGroup() & 0xF;
                AccessLevel = PDTUTils.Logic.SmartCardStrings.Strings[scStatus];
            }
        }

        #endregion
        
        #region Private Methods
        void InitialiseBoLib()
        {
            var code = BoLib.setEnvironment();
            switch (code)
            {
                case 0:
                    _libraryInitOk = true;
                    BoLib.setUtilRequestBitState((int)UtilBits.Allow);
                    BoLib.InitDirectSound();
                    BoLib.addSound(@"d:\1525\wav\volume.wav");
                    break;
                case 1:
                    _libraryInitOk = false;
                    _errorMessage = "Could not connect to Shell. Check If Running.";
                    break;
                case 2:
                    _libraryInitOk = false;
                    _errorMessage = "Bo Lib Out of Date.";
                    break;
                case 3:
                    _libraryInitOk = false;
                    _errorMessage = "Unknown Error Occurred.";
                    break;
            }
        }
        
        void SetCurrentPage(int status, BaseViewModel newPage)
        {
            var scStatus = BoLib.getSmartCardGroup() & 0xF;
            var doorStatus = BoLib.getUtilDoorAccess();
            var refillStatus = BoLib.getUtilRefillAccess();

            if (scStatus >= status && scStatus != 7)
            {
                CurrentPage.States.Running = false;
                CurrentPage = newPage;
                CurrentPage.States.Running = true;
            }
            else if (status != 1 && scStatus <= 7)
            {
                string message = "INSUFFICIENT PRIVILEGES. PLEASE INSERT LEVEL " + status + " OR GREATER CARD.";
                WarningDialog warning = new WarningDialog(message, "ERROR");
                warning.ShowDialog();
                return;
            }
        }
        
        void ChangeViewModel(BaseViewModel newPage)
        {
            if (newPage == null) return;
            
            switch (newPage.Name)
            {
                case "Cashier":
                    _currentPageIndex = 1;
                    SetCurrentPage(1, newPage);
                    break;
                case "Collector":
                    _currentPageIndex = 2;
                    SetCurrentPage(2, newPage);
                    break;
                case "Engineer":
                    _currentPageIndex = 3;
                    SetCurrentPage(3, newPage);
                    break;
                case "Admin":
                    _currentPageIndex = 4;
                    SetCurrentPage(4, newPage);
                    break;
                case "Manufacturer": break;
            }

            //CurrentPage = newPage;
        }
        
        void DoAppExit()
        {
            //Do cleanup here
            _dateTimer.Stop();
            _doorStateTimer.Stop();
            _checkYourPrivilege.Stop();

            foreach (var p in Pages)
                p.Cleanup();

            if (BoLib.getUtilRequestBitState((int)UtilBits.Allow))
                BoLib.disableUtilsCoinBit();

            if (GlobalConfig.ReparseSettings)
                BoLib.setUtilRequestBitState((int)UtilBits.RereadBirthCert);

            if (GlobalConfig.RebootRequired)
                BoLib.setRebootRequired();
            
            BoLib.clearUtilRequestBitState((int)UtilBits.Allow);
            BoLib.closeSharedMemory();
            
            _libraryInitOk = false;

            Application.Current.MainWindow.Close();
        }

        void DoVmContentRendered()
        {
            if (!_libraryInitOk)
            {
                _msg.ShowMessage(_errorMessage, "ERROR");
                System.Threading.Thread.Sleep(20000);
            }
        }

        void LoadSmartCardSetting()
        {
            char[] val = new char[3];
            NativeWinApi.GetPrivateProfileString("Operator", "EnableCardReader", "", val, 3, Properties.Resources.birth_cert);
            _hasSmartCard = (val[0] == '1') ? true : false;
        }
        #endregion
    }
}
