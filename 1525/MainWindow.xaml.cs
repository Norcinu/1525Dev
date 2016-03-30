using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.Access;
using PDTUtils.Logic;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils
{
	/// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
	{
        bool _initialSlider = true;
	    bool _sharedMemoryOnline = false;
        bool _directSoundOnline = false;
        double WindowHeight { get; set; }
        string _errorMessage = "";
	    		
        System.Timers.Timer _doorStatusTimer;
		System.Timers.Timer _uiUpdateTimer;
		Thread _keyDoorThread;
        
		MachineErrorLog _errorLogText = new MachineErrorLog();
		MachineGameStatistics _gameStatistics = new MachineGameStatistics();
		ShortTermMeters _shortTerm = new ShortTermMeters();
		LongTermMeters _longTerm = new LongTermMeters();
        TitoMeters _titoMeter = new TitoMeters();
        
        readonly MachineIni _machineIni = new MachineIni();
        readonly UniqueIniCategory _uniqueIniCategory = new UniqueIniCategory();
        readonly ServiceEnabler _enabler = new ServiceEnabler();
        readonly GamesList _gamesList = new GamesList();
        readonly MachineLogsController _logController = new MachineLogsController();
        readonly UserSoftwareUpdate _updateFiles;

        
        public MainWindow()
        {
            RequiresSave = false;
            FullyLoaded = false;
            try
            {
                InitialiseBoLib();

                InitializeComponent();

                BoLib.InitDirectSound();
                BoLib.addSound(@"d:\1525\wav\volume.wav");

                MachineDescription.IsSpanish = false;
                MachineDescription.IsBritish = true;
                MachineDescription.CountryCode = BoLib.getCountryCode();

                var ci = new CultureInfo("en-GB");

                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
                
                _updateFiles = new UserSoftwareUpdate(this);
                WindowHeight = Height;
                
                if (_keyDoorWorker.HasSmartCard)
                {
                    if (_keyDoorWorker.FirstAccessLevelTest())
                    {
                        Enabler.EnableCategory("MainPage");
                        UcMainPage.IsEnabled = true;
                        UcMainPage.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    KeyDoorWorker.CheckDoorOpenOnStart();
                    Enabler.EnableCategory("MainPage");
                    UcMainPage.IsEnabled = true;
                    UcMainPage.Visibility = Visibility.Visible;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error: " + err.ToString());
                Application.Current.Shutdown();
            }
            RowOne.Height = new GridLength(75);
            ColumnOne.Width = new GridLength(200);
            Loaded += WindowMain_Loaded;
            FullyLoaded = true;
        }
		
        #region Properties
		
		public MachineLogsController LogController { get { return _logController; } }
		/*public*/ UserSoftwareUpdate UpdateFiles { get { return _updateFiles; } }
		
		public GamesList GamesList
		{
			get { return _gamesList; }
		}
		
		DoorAndKeyStatus _keyDoorWorker = new DoorAndKeyStatus();
		public DoorAndKeyStatus KeyDoorWorker
		{
			get { return _keyDoorWorker; }
			private set { _keyDoorWorker = value; }
		}
	        
		public bool RequiresSave { get; set; }

	    public MachineIni GetMachineIni
		{ 
			get { return _machineIni; } 
		}
        
		public UniqueIniCategory GetUniqueCategories
		{
			get { return _uniqueIniCategory; }
		}
        
		public ServiceEnabler Enabler
		{
			get { return _enabler; }
		}
        
		public MachineGameStatistics GameStatistics
		{
			get { return _gameStatistics; }
            set { _gameStatistics = value; }
		}
        
		public ShortTermMeters ShortTerm
		{
			get { return _shortTerm; }
			set { _shortTerm = value; }
		}
        
		public LongTermMeters LongTerm
		{
			get { return _longTerm; }
			set { _longTerm = value; }
		}
        
        public TitoMeters TitoMeter
        {
            get { return _titoMeter; }
            set { _titoMeter = value; }
        }
        
        public bool FullyLoaded { get; set; }
        
		#endregion
	    
		void btnExit_Click(object sender, RoutedEventArgs e)
        {
            //if (BoLib.refillKeyStatus() > 0)
            if (BoLib.getUtilRefillAccess())
            {
                var msg = new PDTUtils.MVVM.WpfMessageBoxService();
                msg.ShowMessage("Please Turn Key to Exit.", "INFO");
                return;
            }
			Application.Current.Shutdown();
        }
        //we dont really need it to say that do we?
		void btnHoppers_Click(object sender, RoutedEventArgs e)
		{
			Enabler.ClearAll();
		}
        
        void btnLogfiles_Click(object sender, RoutedEventArgs e)
		{
            Enabler.EnableCategory(Categories.Logfile);

            UcMainPage.Visibility = Visibility.Hidden;
            
            if (KeyDoorWorker.IsDoorClosed)
                MyLogfiles.SelectedIndex = 3;
            
            if (!LogController.IsLoaded)
            {
                LogController.SetErrorLog();
                LogController.SetWarningLog();
                LogController.SetPlayedLog();
                LogController.SetWinningLog();
                LogController.SetHandPayLog();
                //LogController.SetCashlessLibLog();
                //LogController.SetVizTechLog();
                LogController.IsLoaded = true;
            }
		}
        
        void Games_Click(object sender, RoutedEventArgs e)
        {
            _gameStatistics.ParsePerfLog();
            Enabler.EnableCategory(Categories.GameStatistics);
            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;
            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;
            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;
            UcAudit.IsEnabled = false;
            UcAudit.Visibility = Visibility.Hidden;
        }

        void GetSystemUptime()
        {
            var ticks = Stopwatch.GetTimestamp();
            var uptime = ((double)ticks) / Stopwatch.Frequency;
            var uptimeSpan = TimeSpan.FromSeconds(uptime);
            var u = uptimeSpan.ToString().Split(".".ToCharArray());
            LblUptime.Content = u[0];
        }
        
        void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _keyDoorThread = new Thread(new ThreadStart(_keyDoorWorker.Run));
                _keyDoorThread.Start();
                while (!_keyDoorThread.IsAlive) ;
                Thread.Sleep(2);
            }
            catch (Exception err)
            {
                MessageBox.Show("Error: " + err.ToString());
            }
        }

        void WindowMain_Closing(object sender, CancelEventArgs e)
        {
            if (_keyDoorWorker.Running)
            {
                _keyDoorWorker.Running = false;
                Thread.Sleep(20);
            }

            if (_keyDoorThread != null)
            {
                try
                {
                    if (_keyDoorThread.IsAlive)
                    {
                        //_keyDoorThread.Abort();
                        //_keyDoorThread.Join();
                        //keep an eye on this - _keyDoorThread.Join() seemed to be getting into a race condition.
                        //this should work fine as we *have* shutdown thread above, sleep should let it catch up.
                        while (_keyDoorThread.IsAlive) Thread.Sleep(2);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            if (BoLib.getUtilRequestBitState((int)UtilBits.Allow))
                BoLib.disableUtilsCoinBit();

            if (GlobalConfig.ReparseSettings)
                BoLib.setUtilRequestBitState((int)UtilBits.RereadBirthCert);

            if (GlobalConfig.RebootRequired)
                BoLib.setRebootRequired();

            BoLib.clearUtilRequestBitState((int)UtilBits.Allow);

            if (!_sharedMemoryOnline) return;
            _sharedMemoryOnline = false;
            BoLib.closeSharedMemory();
        }
        //part suspended
        void btnSetup_Click(object sender, RoutedEventArgs e)
        {
            //PlaySoundOnEvent(@"d:\1525\wav\paytrans.wav");
            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;

            UcAudit.IsEnabled = false;
            UcAudit.Visibility = Visibility.Hidden;

            var note = myNoteAdmin.DataContext as PDTUtils.MVVM.ViewModels.NoteAdminViewModel;
            note.Refresh();

            Enabler.ClearAll();
            Enabler.EnableCategory(Categories.Setup);
            
            if (KeyDoorWorker.HasSmartCard)
            {
                switch (GlobalAccess.Level)
                {
                    case (int)SmartCardLevels.Player:
                    case (int)SmartCardLevels.Cashier:
                        break;
                    case (int)SmartCardLevels.Collector:
                        TabSetup.SelectedIndex = 4;
                        break;
                    case (int)SmartCardLevels.Engineer:
                        TabSetup.SelectedIndex = 1;
                        break;
                    case (int)SmartCardLevels.Administrator:
                    case (int)SmartCardLevels.Distributor:
                    case (int)SmartCardLevels.Manufacturer:
                        TabSetup.SelectedIndex = 0;
                        break;
                }
            }
            else
            {
                if (KeyDoorWorker.DoorStatus)
                    TabSetup.SelectedIndex = 0;
                else
                    TabSetup.SelectedIndex = 4;
            }
            
            MasterVolumeSlider.Value = BoLib.getLocalMasterVolume();
        }
        /// 
        /// <summary>
        /// TODO: validates the new ini setting.
        /// </summary>
        /// <returns>true for valid settting.</returns>
		bool ValidateNewIniSetting()
		{   
			return true;
		}
        
        void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            UpdateIniItem(sender);
		}
        
        void UpdateIniItem(object sender)
        {
            var l = sender as ListView;
            if (l.SelectedIndex == -1)
                return;
           
            var c = l.Items[l.SelectedIndex] as IniElement;
            var items = l.ItemsSource;
            
            var w = new IniSettingsWindow(c.Field, c.Value);
            if (w.ShowDialog() != false) return;
            switch (w.RetChangeType)
            {
                case ChangeType.Amend:
                    AmendOption(w, sender, ref c);
                    break;
                case ChangeType.Cancel:
                    break;
                case ChangeType.Comment:
                    CommentEntry(w, sender, ref c);
                    break;
                case ChangeType.Uncomment:
                    UnCommentEntry(w, sender, ref c);
                    break;
                default:
                    break;
            }
            l.SelectedIndex = -1;
        }
        
        void AmendOption(IniSettingsWindow w, object sender, ref IniElement c)
        {
            var newValue = w.OptionValue;
            Debug.WriteLine(newValue);
            var listView = sender as ListView;
            var current = listView.Items[listView.SelectedIndex] as IniElement;

            if (newValue == c.Value && (newValue != c.Value || current.Field[0] != '#')) return;
            current.Value = newValue;
            if (current.Field[0] == '#')
            {
                var ini = new IniFile(Properties.Resources.machine_ini);
                ini.DeleteKey(current.Category, current.Field);
                current.Field = current.Field.Substring(1);
            }
            current.Value = newValue;
            listView.Items.Refresh();

            GetMachineIni.WriteMachineIni(current.Category, current.Field);
            GetMachineIni.ChangesPending = true;
        }

        void CommentEntry(IniSettingsWindow w, object sender, ref IniElement c)
        {
            var listView = sender as ListView;
            var current = listView.Items[listView.SelectedIndex] as IniElement;

            if (current.Field[0] == '#') return;
            var ini = new IniFile(Properties.Resources.machine_ini);
            ini.DeleteKey(current.Category, current.Field);
            current.Field = "#" + current.Field;

            listView.Items.Refresh();
            GetMachineIni.WriteMachineIni(current.Category, current.Field);
            GetMachineIni.ChangesPending = true;
        }
        
        void UnCommentEntry(IniSettingsWindow w, object sender, ref IniElement c)
        {
            var listView = sender as ListView;
            var current = listView.Items[listView.SelectedIndex] as IniElement;

            if (current != null && current.Field[0] != '#') return;
            listView.Items.Refresh();
            GetMachineIni.WriteMachineIni(current.Category, current.Field);
            GetMachineIni.ChangesPending = true;

            current.Field = current.Field.Substring(1);

            listView.Items.Refresh();
            GetMachineIni.WriteMachineIni(current.Category, current.Field);
            GetMachineIni.ChangesPending = true;
        }
        
        void RemoveChildrenFromStackPanel()
        {
            var childCount = StpButtonPanel.Children.Count;
            if (childCount > 0)
            {
                StpButtonPanel.Children.RemoveRange(0, childCount);
            }
        }
		
		void MasterVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var volume = Convert.ToUInt32(MasterVolumeSlider.Value);
            BoLib.setLocalMasterVolume(volume);
            if (!_initialSlider)
            {
                BoLib.justPlaySound(@"d:\1525\wav\volume.wav");
            }
            _initialSlider = false;
		}
		
		void btnReadMeters_Click(object sender, RoutedEventArgs e)
		{
           // PlaySoundOnEvent(@"d:\1525\wav\paytrans.wav");
            Enabler.ClearAll();
            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            UcAudit.IsEnabled = false;
            UcAudit.Visibility = Visibility.Hidden;

            UcPerformance.IsEnabled = !UcPerformance.IsEnabled;
            if (UcPerformance.IsEnabled)
            {
                UcPerformance.RefreshMang();
                UcPerformance.Visibility = Visibility.Visible;
            }
            else
                UcPerformance.Visibility = Visibility.Hidden;

            /*StpGameStatsView.IsEnabled = false;
            StpGameStatsView.Visibility = Visibility.Hidden;*/
		}
		
		void btnFunctionalTests_Click(object sender, RoutedEventArgs e)
		{
           // PlaySoundOnEvent(@"d:\1525\wav\paytrans.wav");

            if (BoLib.getDoorStatus() == 1)
            {
                _enabler.ClearAll();

                UcMainPage.IsEnabled = false;
                UcMainPage.Visibility = Visibility.Hidden;
                
                UcDiagnostics.IsEnabled = false;
                UcDiagnostics.Visibility = Visibility.Hidden;

                UcPerformance.IsEnabled = false;
                UcPerformance.Visibility = Visibility.Hidden;

                UcAudit.IsEnabled = false;
                UcAudit.Visibility = Visibility.Hidden;

                /*StpGameStatsView.IsEnabled = false;
                StpGameStatsView.Visibility = Visibility.Hidden;*/

                _keyDoorWorker.TestSuiteRunning = true;
                var ts = new TestSuiteWindow();
                ts.ShowDialog();
                _keyDoorWorker.TestSuiteRunning = false;
            }
            else
            {
                MessageBox.Show("Please Open Door.", "INFO", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
		}
		
		private void btnSystem_Click(object sender, RoutedEventArgs e)
		{
           // PlaySoundOnEvent(@"d:\1525\wav\paytrans.wav");

            GamesList.GetGamesList();
            
            Enabler.ClearAll();
            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            UcAudit.IsEnabled = false;
            UcAudit.Visibility = Visibility.Hidden;

            /*StpGameStatsView.IsEnabled = false;
            StpGameStatsView.Visibility = Visibility.Hidden;*/

            Enabler.EnableCategory(Categories.System);
		}
        
		
        ICommand EnableScreenshot
        {
            get
            {
                return new DelegateCommand(o => DoScreenshot());
            }
        }
        
        void DoScreenshot() { }
        
        void btnScreenShots_Click(object sender, RoutedEventArgs e)
        {
          //  PlaySoundOnEvent(@"d:\1525\wav\paytrans.wav");
            Enabler.ClearAll();

            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;

            UcAudit.IsEnabled = false;
            UcAudit.Visibility = Visibility.Hidden;

            /*StpGameStatsView.IsEnabled = false;
            StpGameStatsView.Visibility = Visibility.Hidden;*/

            var w = new ScreenshotWindow();
            w.ShowDialog();
        }
        
        void btnCreditManage_Click(object sender, RoutedEventArgs e)
        {
            //BoLib.justPlaySound(@"d:\1525\wav\util_click.wav");
            //PlaySoundOnEvent(@"d:\1525\wav\paytrans.wav");
            UcMainPage.IsEnabled = true;//!UcMainPage.IsEnabled;
            if (UcMainPage.IsEnabled)
                UcMainPage.Visibility = Visibility.Visible;
            else
                UcMainPage.Visibility = Visibility.Hidden;

            LogController.ClearAllLogs();
            
            Enabler.ClearAll();

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;
            
            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;

            UcAudit.IsEnabled = false;
            UcAudit.Visibility = Visibility.Hidden;

            /*StpGameStatsView.IsEnabled = false;
            StpGameStatsView.Visibility = Visibility.Hidden;*/
        }
        
        void btnDiagnostics_Click(object sender, RoutedEventArgs e)
        {
            //PlaySoundOnEvent(@"d:\1525\wav\paytrans.wav");
            //if (UcDiagnostics.AccessLevel)
            UcDiagnostics.IsEnabled = !UcDiagnostics.IsEnabled;
            UcDiagnostics.Visibility = UcDiagnostics.IsEnabled ? Visibility.Visible : Visibility.Hidden;

            Enabler.ClearAll();

            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;
    
            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;

            UcAudit.IsEnabled = false;
            UcAudit.Visibility = Visibility.Hidden;

            /*StpGameStatsView.IsEnabled = false;
            StpGameStatsView.Visibility = Visibility.Hidden;*/
        }
        
        void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
        }
        
        // refactor this 
        // set globalconfig.reboot = true
        void btnReboot_Click(object sender, RoutedEventArgs e)
        {
            //CHECK THIS TODODODOD
            _keyDoorWorker.PrepareForReboot = true;
            //if (BoLib.refillKeyStatus() == 1 && BoLib.getDoorStatus() == 1)
            /*if (BoLib.getUtilRefillAccess() && !BoLib.getUtilDoorAccess())
            {
                MessageBox.Show("Please Turn Refill Key and Close Door", "INFO");
            }
            else if (BoLib.getUtilRefillAccess())
            {
                MessageBox.Show("Please Turn Refill Key.", "INFO");
            }
            else if (!BoLib.getUtilDoorAccess())*/
            if (BoLib.refillKeyStatus() == 1 && BoLib.getDoorStatus() == 1)
            {
                MessageBox.Show("Please Turn Refill Key and Close Door", "INFO");
            }
            else if (BoLib.refillKeyStatus() == 1)
            {
                MessageBox.Show("Please Turn Refill Key.", "INFO");
            }
            else if (BoLib.getDoorStatus() == 1)
            {
                MessageBox.Show("Please Close the Door", "INFO");
            }
            
            if (BoLib.refillKeyStatus() != 0 || BoLib.getDoorStatus() != 0) return;
            _keyDoorWorker.PrepareForReboot = false;
            DiskCommit.SaveAndReboot();       
        }
       
	    void ButtonBase_OnClick(object sender, RoutedEventArgs e)
	    {
            Application.Current.Shutdown();
	    }
        
        void Audit_Click(object sender, RoutedEventArgs e)
        {
            /*var path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            Process.Start("explorer", path);*/
            
            if (!Enabler.Audit)
            {
                Enabler.ClearAll();
                Enabler.EnableCategory(Categories.Audit);
                UcAudit.IsEnabled = true;
                UcAudit.Visibility = Visibility.Visible;
                
                UcDiagnostics.IsEnabled = false;
                UcDiagnostics.Visibility = Visibility.Hidden;
                UcMainPage.IsEnabled = false;
                UcMainPage.Visibility = Visibility.Hidden;
                UcPerformance.IsEnabled = false;
                UcPerformance.Visibility = Visibility.Hidden;

                /*StpGameStatsView.IsEnabled = false;
                StpGameStatsView.Visibility = Visibility.Hidden;*/
            }
        }
    }
}
