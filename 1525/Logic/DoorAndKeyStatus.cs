using System;
using System.ComponentModel;
using System.Threading;
using PDTUtils.Access;
using PDTUtils.Native;


namespace PDTUtils
{
    /// <summary>
    /// Handles status of door open/closed, key turns and card reader status.
    /// </summary>
	public class DoorAndKeyStatus : INotifyPropertyChanged
    {
        string[] _strings = new string[8] {"Player", "Cashier", "Collector", "Engineer", "Administrator", 
                                           "Distributor", "Manufacturer", "None"};


        volatile bool _doorStatus;
		volatile bool _running;
		volatile bool _hasChanged;
		volatile bool _isTestSuiteRunning;
        volatile bool _prepareForReboot;
        volatile bool _hasSmartCard;
        
        string _smartCardString = "";
        string _commandProperty = "";
        
		#region Properties
		public bool TestSuiteRunning
		{
			get { return _isTestSuiteRunning; }
			set { _isTestSuiteRunning = value; }
		}
        
		public bool HasChanged
		{
			get { return _hasChanged; }
			set { _hasChanged = value; }
		}
        
        public bool DoorStatus
        {
            get { return _doorStatus; }
            set
            {
                _doorStatus = value;
                OnPropertyChanged("DoorStatus");
                OnPropertyChanged("IsDoorClosed");
            }
        }
        
        /// <summary>
        /// I cant be bothered writing a negate converter.
        /// </summary>
        public bool IsDoorOpen
        {
            get { return _doorStatus; }
        }
        
        public bool IsDoorClosed
        {
	        get { return !_doorStatus; }
        }
        
		public bool Running
		{
			get { return _running; }
			set { _running = value; }
		}
        
        public bool PrepareForReboot
        {
            get { return _prepareForReboot; }
            set { _prepareForReboot = value; }
        }
        
        public string SmartCardString
        {
            get { return _smartCardString; }
        }
        
        public string CommandProperty { get { return _commandProperty; } }
        

        bool _isCashier = false;
        public bool IsCashier { get { return _isCashier; } set { _isCashier = value; OnPropertyChanged("IsCashier"); } }
        
        bool _isCollector = false;
        public bool IsCollector { get { return _isCollector; } set { _isCollector = value; OnPropertyChanged("IsCollector"); } }
        
        bool _isAdmin = false;
        public bool IsAdmin { get { return _isAdmin; } set { _isAdmin = value; OnPropertyChanged("IsAdmin"); } }

        bool _isEngineer = false;
        public bool IsEngineer { get { return _isEngineer; } set { _isEngineer = value; OnPropertyChanged("IsEngineer"); } }

        bool _isManufacturer = false;
        public bool IsManufacturer { get { return _isManufacturer; } set { _isManufacturer = value; OnPropertyChanged("IsManufacturer"); } }

        bool _anyAuthedCard = false;
        public bool AnyAuthedCard { get { return _anyAuthedCard; } set { _anyAuthedCard = value; OnPropertyChanged("AnyAuthedCard"); } }
        public bool HasSmartCard
        {
            get { return _hasSmartCard; }
            /*set
            {
                _hasSmartCard
            }*/
        }
        
        #endregion

        public DoorAndKeyStatus()
        {
            _doorStatus = false;
            _running = true;
            _hasChanged = false;
            _isTestSuiteRunning = false;

            AnyAuthedCard = false;

            char[] val = new char[3];
            NativeWinApi.GetPrivateProfileString("Operator", "EnableCardReader", "", val, 3, Properties.Resources.birth_cert);
            _hasSmartCard = (val[0] == '1') ? true : false;
        }
        
        public void Run()
        {
            while (_running)
            {
                var r = new Random();
                if (r.Next(1000) < 100 && !_isTestSuiteRunning)
                {
                    if (!_hasSmartCard)
                    {
                        _smartCardString = "via Door";
                        if (!BoLib.getUtilDoorAccess())
                        {
                            //TODO : ADD OTHER PROPERTIES HERE JUST IN CASE.
                            if (_doorStatus)
                            {
                                _doorStatus = false;
                                _hasChanged = true;
                                IsCashier = true;

                                IsCollector = false;
                                IsManufacturer = false;
                                IsAdmin = false;
                                IsEngineer = false;
                                AnyAuthedCard = true;

                                OnPropertyChanged("DoorStatus");
                                OnPropertyChanged("IsDoorClosed");
                                OnPropertyChanged("IsDoorOpen");
                            }
                        }
                        else
                        {
                            if (!_doorStatus)
                            {
                                _doorStatus = true;
                                _hasChanged = true;

                                IsCashier = true;
                                IsCollector = true;
                                IsEngineer = true;
                                IsAdmin = true;
                                IsManufacturer = true;
                                AnyAuthedCard = true;

                                OnPropertyChanged("DoorStatus");
                                OnPropertyChanged("IsDoorClosed");
                                OnPropertyChanged("IsDoorOpen");
                            }
                        }
                    }
                }

                if (_hasSmartCard)
                {
                    GlobalAccess.Level = BoLib.getUtilsAccessLevel() & 0x0F;
                    _smartCardString = _strings[GlobalAccess.Level];
                    GlobalAccess.Level = GlobalAccess.Level;

                    //Check door status.
                    if (!BoLib.getUtilDoorAccess())
                    {
                        if (!_doorStatus)
                        {
                            _doorStatus = false;
                            _hasChanged = true;
                            OnPropertyChanged("DoorStatus");
                            OnPropertyChanged("IsDoorClosed");
                            OnPropertyChanged("IsDoorOpen");
                        }
                    }
                    else // door closed.
                    {
                        if (_doorStatus)
                        {
                            _doorStatus = true;
                            _hasChanged = true;
                            OnPropertyChanged("DoorStatus");
                            OnPropertyChanged("IsDoorClosed");
                            OnPropertyChanged("IsDoorOpen");
                        }
                    }

                    if (GlobalAccess.Level == 1) //Cashier
                    {
                        if (BoLib.getSmartCardSubGroup() == (byte)SmartCardSubGroups.NONE)
                        {
                            IsCashier = true;//access
                            AnyAuthedCard = true;
                        }
                        else
                        {
                            IsCashier = false;
                            AnyAuthedCard = false;
                            _smartCardString = "No Access";
                        }

                        //no-access
                        IsCollector = false;
                        IsEngineer = false;
                        IsManufacturer = false;
                        IsAdmin = false;
                    }
                    else if (GlobalAccess.Level == 2) //Collector
                    {
                        //access
                        IsCashier = true;
                        IsCollector = true;
                        AnyAuthedCard = true;
                        //no-access
                        IsEngineer = false;
                        IsAdmin = false;
                        IsManufacturer = false;
                    }
                    else if (GlobalAccess.Level == 3) //(v6)I'm an Engineer!
                    {
                        //access
                        IsCashier = true;
                        IsCollector = true;
                        IsEngineer = true;
                        AnyAuthedCard = true;
                        //no access
                        IsAdmin = false;
                        IsManufacturer = false;
                    }
                    else if (GlobalAccess.Level == 4) //Administrator
                    {
                        //access
                        IsCashier = true;
                        IsCollector = true;
                        IsEngineer = true;
                        AnyAuthedCard = true;
                        IsAdmin = true;
                        //no access
                        IsManufacturer = false;
                    }
                    else if (GlobalAccess.Level == 6) //Manufacturer/Factory
                    {
                        //access
                        IsCashier = true;
                        IsCollector = true;
                        IsEngineer = true;
                        IsAdmin = true;
                        IsManufacturer = true;
                        AnyAuthedCard = true;
                    }
                    else //nothing
                    {
                        //no access
                        IsCashier = false;
                        IsCollector = false;
                        IsEngineer = false;
                        IsManufacturer = false;
                        IsAdmin = false;
                        AnyAuthedCard = false;
                    }

                    OnPropertyChanged("DoorStatus");
                    OnPropertyChanged("IsDoorClosed");
                    OnPropertyChanged("IsDoorOpen");
                }

                OnPropertyChanged("KeyDoorWorker");
                Thread.Sleep(150);
            }
        }

        public bool FirstAccessLevelTest()
        {
            if (_hasSmartCard)
            {
                GlobalAccess.Level = BoLib.getUtilsAccessLevel() & 0x0F;
                _smartCardString = _strings[GlobalAccess.Level];
                GlobalAccess.Level = GlobalAccess.Level;
                return true;
            }
            return false;
        }

        public void CheckDoorOpenOnStart()
        {
            if (!_hasSmartCard)
            {
                if (BoLib.getDoorStatus() == 0)
                    _doorStatus = true;
            }
        }

        public void Clone(DoorAndKeyStatus kd)
		{
			DoorStatus = kd.DoorStatus;
			HasChanged = kd.HasChanged;
			Running = kd.Running;
		}
        
        
		#region Property Changed events
		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#endregion
	}
}
