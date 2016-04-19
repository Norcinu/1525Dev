/*
 * Massive TODO:
 * Get rid of all the new string/Convert to Ints.
 * Use GetPrivateProfileInt!!!
 */ 

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;
using PDTUtils.Properties;
using GlobalConfig = PDTUtils.Logic.GlobalConfig;
using Timer = System.Timers.Timer;
using Visibility = System.Windows.Visibility;

namespace PDTUtils.MVVM.ViewModels
{
    class MessageBoxAccess
    {
        public bool Show { get; set; }
        public MessageBoxAccess(bool show)
        {
            Show = show;
        }
    }
    
    class HopperViewModel : ObservableObject
    {
        bool _leftHopperEnabled;
        bool _rightHopperEnabled;
        public bool LeftHopperEnabled
        {
            get { return _leftHopperEnabled; }
            set 
            { 
                _leftHopperEnabled = value;
                RaisePropertyChangedEvent("LeftHopperEnabled");
            }
        }
        
        public bool RightHopperEnabled
        {
            get { return _rightHopperEnabled; }
            set 
            { 
                _rightHopperEnabled = value;
                RaisePropertyChangedEvent("RightHopperEnabled");
            }
        }
        
        bool _hasTwoHoppers;
        bool _enabled;
        bool _emptyingHoppers;
        bool _needToSync;
        bool _syncLeft;
        bool _syncRight;
        bool _isSpanish;
        bool _isBritish; //yeah alright the convertors arent working correctly.
        bool _dumpSwitchPressed = false;
        byte _currentHopperDumping;
        
        int _selectedTabIndex;
        int _espLeftHopper;
        int _espRightHopper;
        
        int _leftHopperTryCount = 0;
        int _rightHopperTryCount = 0;
        int _rightHopperValue = 0;

        readonly int TRY_COUNT = 3;
        
        string _leftRefillMsg;
        string _rightRefillMsg;
        /*string _leftCoinsAdded;
        string _rightCoinsAdded;*/
        string _refloatLeft;
        string _refloatRight;
        string _hopperPayingValue = "";
        string _floatLevelLeft = "";
        string _floatLevelRight = "";

        /*string _newLeftFillValue;
        string _newRightFillValue;*/
        
        Timer EmptyLeftTimer;
        Timer EmptyRightTimer;
        //Timer RefillTimer;
        Timer SpanishEmpty;

        NumberFormatInfo Nfi { get; set; }
        //CultureInfo CurrentCulture { get; set; }

        //MessageBoxAccess _msgAccess = new MessageBoxAccess(false);
        WpfMessageBoxService _msg = new WpfMessageBoxService();

        Visibility _isLeftVisible;// = Visibility.Hidden;
        public Visibility IsLeftVisible
        {
            get { return _isLeftVisible; }
            set
            {
                _isLeftVisible = value;
                RaisePropertyChangedEvent("IsLeftVisible");
            }
        }

        Visibility _isRightVisible;// = Visibility.Hidden;
        public Visibility IsRightVisible
        {
            get { return _isRightVisible; }
            set
            {
                _isRightVisible = value;
                RaisePropertyChangedEvent("IsRightVisible");
            }
        }

        #region Properties
        public bool EndRefill { get; set; }
        public bool NotRefilling { get; set; }          // Disabling tabs.
        public HopperModel LeftHopper { get; set; }     // £1 Hopper
        public HopperModel RightHopper { get; set; }    // 10p Hopper

        public string DivertLeftMessage { get; set; }
        public string DivertRightMessage { get; set; }

        public string SelHopperValue { get; set; }
        
        public bool EmptyingHoppers
        {
            get { return _emptyingHoppers; }
            set { _emptyingHoppers = value; }
        }
        
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                RaisePropertyChangedEvent("Enabled");
                RaisePropertyChangedEvent("LeftRefillMsg");
                RaisePropertyChangedEvent("RightRefillMsg");
            }
        }
                
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                if (!_emptyingHoppers)
                    _selectedTabIndex = value;
            }
        }
        
        public string DumpSwitchMessage { get; set; }
        //public List<string> HopperList { get; set; }
        public ObservableCollection<string> HopperList { get; set; }

        public ObservableCollection<HopperModel> _hopperModels = new ObservableCollection<HopperModel>();
        public ObservableCollection<HopperModel> HopperModels
        {
            get { return _hopperModels; }
            set
            {
                _hopperModels = value;
                RaisePropertyChangedEvent("HopperModels");
            }
        }
        
        public string LeftRefillMsg
        {
            get
            {
                return _leftRefillMsg;
            }
            set
            {
                _leftRefillMsg = "Left Hopper Coins Added: " + Convert.ToDecimal(value).ToString("C",
                    Thread.CurrentThread.CurrentCulture.NumberFormat);
                RaisePropertyChangedEvent("LeftRefillMsg");
            }
        }

        public string RightRefillMsg
        {
            get
            {
                return _rightRefillMsg;
            }
            set
            {
                _rightRefillMsg = "Right Hopper Coins Added: " + Convert.ToDecimal(value).ToString("C",
                    Thread.CurrentThread.CurrentCulture.NumberFormat);
                RaisePropertyChangedEvent("RightRefillMsg");
            }
        }

        public bool AreWeRefilling { get; set; }

        public string RefloatRight
        {
            get { return _refloatRight; }
            set
            {
                _refloatRight = value;
                RaisePropertyChangedEvent("RefloatRight");
            }
        }

        public string RefloatLeft
        {
            get { return _refloatLeft; }
            set
            {
                _refloatLeft = value;
                RaisePropertyChangedEvent("RefloatLeft");
            }
        }

        public bool NeedToSync
        {
            get { return _needToSync; }
            set
            {
                _needToSync = value;
                RaisePropertyChangedEvent("NeedToSync");
            }
        }

        string _currentSelHopper = "";
        public string CurrentSelHopper
        {
            get { return _currentSelHopper; }
            set
            {
                _currentSelHopper = value;
                if (_currentSelHopper == "LEFT HOPPER" || string.IsNullOrEmpty(_currentSelHopper))
                    SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString();
                else
                    SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Right).ToString();

                RaisePropertyChangedEvent("SelHopperValue");
                RaisePropertyChangedEvent("CurrentSelHopper");
            }
        }

    /*    public string NewLeftFillValue
        {
            get { return _newLeftFillValue; }
            set { _newLeftFillValue = value; RaisePropertyChangedEvent("NewLeftFillValue"); }
        }
       
        
        public string NewRightFillValue
        {
            get { return _newRightFillValue; }
            set { _newRightFillValue = value; RaisePropertyChangedEvent("NewRightFillValue"); }
        }*/

        public bool IsSpanish
        {
            get { return _isSpanish; }
            set { _isSpanish = value; RaisePropertyChangedEvent("IsSpanish"); }
        }

        public bool IsBritish
        {
            get { return _isBritish; }
            set { _isBritish = value; RaisePropertyChangedEvent("IsBritish"); }
        }

        public int EspLeftHopper
        {
            get { return _espLeftHopper; }
            set
            {
                _espLeftHopper = value;
                RaisePropertyChangedEvent("EspLeftHopper");
            }
        }

        public int EspRightHopper
        {
            get { return _espRightHopper; }
            set
            {
                _espRightHopper = value;
                RaisePropertyChangedEvent("EspRightHopper");
            }
        }

        public string FloatLevelLeft
        {
            get { return _floatLevelLeft; }
            set
            {
                _floatLevelLeft = value;
                RaisePropertyChangedEvent("FloatLevelLeft");
            }
        }

        public string FloatLevelRight
        {
            get { return _floatLevelRight; }
            set
            {
                _floatLevelRight = value;
                RaisePropertyChangedEvent("FloatLevelRight");
            }
        }
        #endregion

        public HopperViewModel()
        {
            //CurrentCulture = new CultureInfo("en-GB");

            // incase no hoppers
            char[] buffer = new char[3];
            NativeWinApi.GetPrivateProfileString("FactoryOnly", "NumberOfHoppers", "", buffer, 3, Properties.Resources.birth_cert);
            if (buffer[0] == '0')
                return;

            _hasTwoHoppers = (buffer[0] == '1') ? false : true;
            var hopperCount = Convert.ToUInt32(new string(buffer, 0, buffer.Length));

           // Nfi = CurrentCulture.NumberFormat;
            
            RefloatLeft = "";
            RefloatRight = "";
            _enabled = false;
            EndRefill = false;
            _emptyingHoppers = false;
            NotRefilling = true;
            unchecked { _currentHopperDumping = (byte)Hoppers.NoHopper; }
            LeftHopper = new HopperModel();
            RightHopper = new HopperModel();
            DumpSwitchMessage = (BoLib.getHopperDumpSwitchActive() > 0) ? DumpSwitchMessage = "** INFO: Please press dump switch when emptying. **" : "";
            AreWeRefilling = false;
            SelectedTabIndex = 0;
            DivertLeftMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Left).ToString();
            DivertRightMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Right).ToString();
            IsLeftVisible = Visibility.Collapsed;
            IsRightVisible = Visibility.Collapsed;
            _rightHopperValue = NativeWinApi.GetPrivateProfileInt("FactoryOnly", "PayoutCoin2", 0, Properties.Resources.birth_cert);

            IsSpanish = false;
            IsBritish = true;

            CheckDirAndIniExist();

            char[] LOL_WUTERMELON = new char[10];
            HopperList = new ObservableCollection<string>();

            if (hopperCount >= 1)
            {
                FloatLevelLeft = BoLib.getHopperFloatLevel((int)Hoppers.Left).ToString();
                IsLeftVisible = Visibility.Visible;

                NativeWinApi.GetPrivateProfileString("Hoppers", "Left", "", LOL_WUTERMELON, 10, Properties.Resources.utils_config);
                EspLeftHopper = Convert.ToInt32(new string(LOL_WUTERMELON));

                Array.Clear(LOL_WUTERMELON, 0, 10);
                
                HopperList.Add("LEFT HOPPER");
            }
            else
                FloatLevelLeft = "0";

            if (hopperCount == 2)
            {
                FloatLevelRight = BoLib.getHopperFloatLevel((int)Hoppers.Right).ToString();
                IsRightVisible = Visibility.Visible;

                NativeWinApi.GetPrivateProfileString("Hoppers", "Right", "", LOL_WUTERMELON, 10, Properties.Resources.utils_config);
                EspRightHopper = Convert.ToInt32(new string(LOL_WUTERMELON));

                HopperList.Add("RIGHT HOPPER");
            }
            else
                FloatLevelRight = "0";
            
            
            NeedToSync = false;
            _syncLeft = false;
            _syncRight = false;

            SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString();

            InitRefloatLevels();
            
            RaisePropertyChangedEvent("DumpSwitchMessage");
            RaisePropertyChangedEvent("DivertLeftMessage");
            RaisePropertyChangedEvent("DivertRightMessage");
            RaisePropertyChangedEvent("NotRefilling");
            RaisePropertyChangedEvent("AreWeReflling");
            RaisePropertyChangedEvent("SelHopperValue");
            
            
            //convert hopperview to use 1 hopper only/intelligent
            //you need to condition lots with IF WE HAVE 2 HOPPERS THEN ALLOW IT BRUV.
                        
            
            //RaisePropertyChangedEvent("HopperList");
            
            const uint initial = 0;
            try
            {
                LeftRefillMsg = "Left Hopper Coins Added: " + initial.ToString();
                RightRefillMsg = "Right Hopper Coins Added: " + initial.ToString();
            }
            catch (FormatException)
            {
                LeftRefillMsg = initial.ToString();
                RightRefillMsg = initial.ToString();
            }
        }
        
        public ICommand ToggleRefillStatus
        {
            get { return new DelegateCommand(o => NotRefilling = !NotRefilling); }
        }

        public void TabControlSelectionChanged(object sender)
        {

        }

        public ICommand DoEmptyHopper
        {
            get { return new DelegateCommand(EmptyHopper); }
        }

        void EmptyHopper(object hopper)
        {
            //var cb = hopper as ComboBox;
            var cb = hopper as string;
            var dumpSwitchPressed = false;
            _emptyingHoppers = true;

            if (EmptyLeftTimer == null)
                EmptyLeftTimer = new Timer(100.0);

            switch (cb)//(cb.SelectedIndex)
            {
                //case 0:
                case "left":
                    if (EmptyRightTimer != null && EmptyRightTimer.Enabled) EmptyRightTimer.Enabled = false;
                    
                    EmptyLeftTimer.Elapsed += (sender, e) =>
                    {
                        if (BoLib.getHopperDumpSwitchActive() > 0)
                        {
                            DumpSwitchMessage = "Press Coin Dump Button to Continue"; //reinstate this?
                            RaisePropertyChangedEvent("DumpSwitchMessage");
                            dumpSwitchPressed = true;//debug
                            if (BoLib.refillKeyStatus() > 0 && BoLib.getDoorStatus() > 0)
                            {
                                /*if (BoLib.getSwitchStatus(2, 0x20) > 0)
                                {
                                    DumpSwitchMessage = "Dump Button Pressed OK";
                                    RaisePropertyChangedEvent("DumpSwitchMessage");
                                    dumpSwitchPressed = true;
                                }*/
                                
                                Thread.Sleep(2);
                            }
                        }
                        
                        if (BoLib.refillKeyStatus() <= 0 || BoLib.getDoorStatus() <= 0 ||
                            (!dumpSwitchPressed && BoLib.getHopperDumpSwitchActive() > 0)) return;

                        BoLib.setUtilRequestBitState((int)UtilBits.DumpLeftHopper);

                        if (BoLib.getRequestEmptyLeftHopper() > 0 && BoLib.getHopperFloatLevel((byte)Hoppers.Left) > 0)
                        {
                            Thread.Sleep(2000);
                            SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString();
                            RaisePropertyChangedEvent("SelHopperValue");
                        }
                        else if (BoLib.getHopperFloatLevel(0) == 0)
                        {
                            SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString();
                            DumpSwitchMessage = "Hopper Empty";
                            RaisePropertyChangedEvent("DumpSwitchMessage");
                            var t = sender as Timer;
                            t.Enabled = false;
                            dumpSwitchPressed = false;
            
                            var hopperValueStr = "LEFT HOPPER (£1 COINS)\n";
                            var _msg = new WpfMessageBoxService();
                            _msg.ShowMessage(hopperValueStr + "COINS REMOVED = " + SelHopperValue, "HOPPER EMPTYING");
                        }
                    };
                    EmptyLeftTimer.Enabled = true;

                    break;
                    
                //case 1:
                case "right":
                    if (EmptyLeftTimer != null && EmptyLeftTimer.Enabled) EmptyLeftTimer.Enabled = false;

                    if (EmptyRightTimer == null)
                    {
                        EmptyRightTimer = new Timer(100.0);
                        EmptyRightTimer.Elapsed += (sender, e) =>
                        {
                            if (BoLib.getHopperDumpSwitchActive() > 0)
                            {
                                DumpSwitchMessage = "Press Coin Dump Button to Continue";
                                RaisePropertyChangedEvent("DumpSwitchMessage");
                                
                                if (BoLib.refillKeyStatus() > 0 && BoLib.getDoorStatus() > 0)
                                {
                                    if (BoLib.getSwitchStatus(2, 0x20) > 0)
                                    {
                                        DumpSwitchMessage = "Dump Button Pressed OK";
                                        RaisePropertyChangedEvent("DumpSwitchMessage");
                                        dumpSwitchPressed = true;
                                    }
                                    Thread.Sleep(2);
                                }
                            }

                            if (BoLib.refillKeyStatus() <= 0 || BoLib.getDoorStatus() <= 0 ||
                                (!dumpSwitchPressed && BoLib.getHopperDumpSwitchActive() > 0)) return;

                            BoLib.setUtilRequestBitState((int)UtilBits.DumpRightHopper);

                            if (BoLib.getRequestEmptyRightHopper() > 0 && BoLib.getHopperFloatLevel((byte)Hoppers.Right) > 0)
                            {
                                Thread.Sleep(2000);
                                SelHopperValue =/* "Hopper Level: " +*/ BoLib.getHopperFloatLevel((byte)Hoppers.Right).ToString();
                                RaisePropertyChangedEvent("SelHopperValue");
                            }
                            else if (BoLib.getHopperFloatLevel((byte)Hoppers.Right) == 0)
                            {
                                SelHopperValue =/* "Hopper Level: " +*/ BoLib.getHopperFloatLevel((byte)Hoppers.Right).ToString();
                                DumpSwitchMessage = "Hopper Empty";
                                RaisePropertyChangedEvent("DumpSwitchMessage");
                                RaisePropertyChangedEvent("SelHopperValue");
                                var t = sender as Timer;
                                t.Enabled = false;
                                dumpSwitchPressed = false;
                                var hopperValueStr = (_rightHopperValue >= 100) ? "RIGHT HOPPER £" + (_rightHopperValue / 100).ToString() : "RIGHT HOPPER " + _rightHopperValue.ToString() + "(p COINS)\n";
                                var _msg = new WpfMessageBoxService();
                                _msg.ShowMessage(hopperValueStr +  "COINS REMOVED = " + SelHopperValue, "HOPPER EMPTYING");
                            }

                            //Debug.WriteLine("IS HOPPER HOPPING: ", BoLib.getIsHopperHopping(1).ToString());
                        };
                    }

                    EmptyRightTimer.Enabled = true;
                    break;
            }
        }
        
        void InitRefloatLevels()
        {
            char[] refloatValue = new char[10];
            NativeWinApi.GetPrivateProfileString("Operator", "RefloatLH", "", refloatValue, 10, Resources.birth_cert);
            RefloatLeft = new string(refloatValue).Trim("\0".ToCharArray());
            CheckForSync(Convert.ToUInt32(RefloatLeft), "left");
            NativeWinApi.GetPrivateProfileString("Operator", "RefloatRH", "", refloatValue, 10, Resources.birth_cert);
            RefloatRight = new string(refloatValue).Trim("\0".ToCharArray());
            CheckForSync(Convert.ToUInt32(RefloatRight), "right");
        }
        
        public ICommand PerformSync { get { return new DelegateCommand(o => DoPerformSync()); } }
        void DoPerformSync()
        {
            if (!_syncLeft && !_syncRight)
            {
                NeedToSync = false;
                return;
            }
            
            NSync(ref _syncLeft, ref _refloatLeft, 0);
            NSync(ref _syncRight, ref _refloatRight, 2);

            RaisePropertyChangedEvent("RefloatLeft");
            RaisePropertyChangedEvent("RefloatRight");
        }
        
        /// <summary>
        /// Bye Bye Bye
        /// </summary>
        /// <param name="shouldSync">Its gonna be me</param>
        /// <param name="refloatValue">Tearing up my heart</param>
        /// <param name="whichHopper">This I promise you</param>7
        void NSync(ref bool shouldSync, ref string refloatValue, byte whichHopper)
        {
            if (shouldSync)
            {
                var divert = BoLib.getHopperDivertLevel(whichHopper);
                refloatValue = divert.ToString();
                shouldSync = false;
                var key = (whichHopper == (byte)Hoppers.Left) ? "RefloatLH" : "RefloatRH";
                NativeWinApi.WritePrivateProfileString("Operator", key, refloatValue, Resources.birth_cert);
                shouldSync = false;
            }
        }
        
        void CheckForSync(UInt32 newRefloatValue, string hopper)
        {
            if (hopper == "left")
            {
                if (newRefloatValue > BoLib.getHopperDivertLevel((byte)Hoppers.Left))
                {
                    NeedToSync = true;
                    _syncLeft = true;
                }
                else
                    _syncLeft = false;
            }
            else 
            {
                if (newRefloatValue > BoLib.getHopperDivertLevel((byte)Hoppers.Right))
                {
                    NeedToSync = true;
                    _syncRight = true;
                }
                else
                    _syncRight = false;
            }
            
            if (!_syncLeft && !_syncRight)
                NeedToSync = false; 
        }
        
        public ICommand SetRefloatLevel { get { return new DelegateCommand(DoSetRefloatLevel); } }
        void DoSetRefloatLevel(object o)
        {
            var format = o as string;
            var tokens = format.Split("+".ToCharArray());
            const int denom = 50;
            string key = (tokens[0] == "left") ? "RefloatLH" : "RefloatRH";

            char[] refloatValue = new char[10];
            NativeWinApi.GetPrivateProfileString("Operator", key, "", refloatValue, 10, Resources.birth_cert);

            var newRefloatValue = Convert.ToUInt32(new string(refloatValue).Trim("\0".ToCharArray()));
            if (tokens[1] == "increase") newRefloatValue += denom;
            else if (tokens[1] == "decrease" && newRefloatValue > denom) newRefloatValue -= denom;

            if (tokens[0] == "left")
                RefloatLeft = newRefloatValue.ToString();
            else
                RefloatRight = newRefloatValue.ToString();

            NativeWinApi.WritePrivateProfileString("Operator", key, newRefloatValue.ToString(), Resources.birth_cert);

            CheckForSync(newRefloatValue, tokens[0]);

            RaisePropertyChangedEvent(key);
        }
        
        public ICommand ChangeLeftDivert { get { return new DelegateCommand(DoChangeDivert); } }
        void DoChangeDivert(object o)
        {
            const uint changeAmount = 10;
            var actionType = o as string;
            char[] divert = new char[10];
            NativeWinApi.GetPrivateProfileString("Operator", "LH Divert Threshold", "", divert, 10, Resources.birth_cert);
            var currentThreshold = Convert.ToUInt32(new string(divert));//BoLib.getHopperDivertLevel(0);
            var newValue = currentThreshold;
            
            if (actionType == "increase")
            {
                newValue += changeAmount;
            }
            else if (actionType == "decrease" && currentThreshold > 200)
            {
                newValue -= changeAmount;
                if (newValue < 200)
                    newValue = 0;
            }
            
            //BoLib.setHopperDivertLevel(BoLib.getLeftHopper(), newValue);
            GlobalConfig.ReparseSettings = true;
            NativeWinApi.WritePrivateProfileString("Operator", "LH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            //PDTUtils.Logic.IniFileUtility.HashFile(Resources.birth_cert);
            BoLib.setHopperDivertLevel((byte)Hoppers.Left, newValue);

            DivertLeftMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertLeftMessage");
        }
        
        public ICommand ChangeRightDivert { get { return new DelegateCommand(DoChangeDivertRight); } }
        void DoChangeDivertRight(object o)
        {
            var actionType = o as string;
            //var currentThreshold = BoLib.getHopperDivertLevel((byte)Hoppers.Right);
            char[] divert = new char[10];
            NativeWinApi.GetPrivateProfileString("Operator", "RH Divert Threshold", "", divert, 10, Resources.birth_cert);
            var currentThreshold = Convert.ToUInt32(new string(divert));
            const uint changeAmount = 10;
            var newValue = currentThreshold;

            if (actionType == "increase")
            {
                newValue += changeAmount;
            }
            else if (actionType == "decrease" && currentThreshold > 50)
            {
                newValue -= changeAmount;
                if (newValue < 50)
                    newValue = 50;
            }
            
            
            //BoLib.setHopperDivertLevel(BoLib.getRightHopper(), newValue);
            GlobalConfig.ReparseSettings = true;
            NativeWinApi.WritePrivateProfileString("Operator", "RH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            //PDTUtils.Logic.IniFileUtility.HashFile(Resources.birth_cert);
            BoLib.setHopperDivertLevel((byte)Hoppers.Right, newValue);

            DivertRightMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertRightMessage");
        }

        
        public ICommand LoadDefaults { get { return new DelegateCommand(o => DoLoadDefaults()); } }
        void DoLoadDefaults()
        {
            uint[] refloatDefaults = new uint[2] { 600, 50 };
            uint[] divertDefaults = new uint[2] { 800, 250 };
            
            BoLib.setHopperFloatLevel((byte)Hoppers.Left, refloatDefaults[0]);
            BoLib.setHopperFloatLevel((byte)Hoppers.Right, refloatDefaults[1]);
            
            RefloatLeft = refloatDefaults[0].ToString();
            RefloatRight = refloatDefaults[1].ToString();

            BoLib.setHopperDivertLevel((byte)Hoppers.Left, divertDefaults[0]);
            BoLib.setHopperDivertLevel((byte)Hoppers.Right, divertDefaults[1]);

            DivertLeftMessage = divertDefaults[0].ToString();
            DivertRightMessage = divertDefaults[1].ToString();
            
            NeedToSync = false;
            _syncLeft = false;
            _syncRight = false;

            NativeWinApi.WritePrivateProfileString("Operator", "RefloatLH", RefloatLeft, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "RefloatRH", RefloatRight, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "LH Divert Threshold", DivertLeftMessage, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "RH Divert Threshold", DivertRightMessage, Resources.birth_cert);

            RaisePropertyChangedEvent("DivertLeftMessage");
            RaisePropertyChangedEvent("DivertRightMessage");
        }
        
        public ICommand RefloatTheHoppers { get { return new DelegateCommand(o => DoRefloatTheHoppers()); } }
        void DoRefloatTheHoppers()
        {
            char[] refloatLeft = new char[10];
            char[] refloatRight = new char[10];

            NativeWinApi.GetPrivateProfileString("Operator", "RefloatLH", "", refloatLeft, 10, Resources.birth_cert);
            NativeWinApi.GetPrivateProfileString("Operator", "RefloatRH", "", refloatRight, 10, Resources.birth_cert);
            
            RefloatLeft = new string(refloatLeft).Trim("\0".ToCharArray());
            RefloatRight = new string(refloatRight).Trim("\0".ToCharArray());
            
            if (BoLib.getCountryCode() == BoLib.getSpainCountryCode())
            {
                var leftFloat = BoLib.getHopperFloatLevel((byte)Hoppers.Left);
                var rightFloat = BoLib.getHopperFloatLevel((byte)Hoppers.Right);
                
                BoLib.setHopperFloatLevel((byte)Hoppers.Left, leftFloat + Convert.ToUInt32(RefloatLeft));
                BoLib.setHopperFloatLevel((byte)Hoppers.Right, rightFloat + Convert.ToUInt32(RefloatRight));
            }
            else
            {
                BoLib.setHopperFloatLevel((byte)Hoppers.Left, Convert.ToUInt32(RefloatLeft));
                BoLib.setHopperFloatLevel((byte)Hoppers.Right, Convert.ToUInt32(RefloatRight));
            }

            SelHopperValue = (_currentSelHopper.Equals("LEFT HOPPER")) ? BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString()
                                                                       : BoLib.getHopperFloatLevel((byte)Hoppers.Right).ToString();
            
            RaisePropertyChangedEvent("CurrentSelHopper");
            RaisePropertyChangedEvent("SelHopperValue");
        }

        /// <summary>
        /// Called via tab selection. If we have refilled elsewhere we will still show old levels.
        /// </summary>
        public void RefreshLevels()
        {
            CurrentSelHopper = CurrentSelHopper;
        }
        
        /*
         * 
         * Spanish Hopper Emptying Methods
         * 
         */
        public ICommand SpanishEmptyOne { get { return new DelegateCommand(DoSpanishEmptyOne); } }
        void DoSpanishEmptyOne(object o)
        {
            if (BoLib.refillKeyStatus() == 0)
            {
                var msg = new WpfMessageBoxService();
                msg.ShowMessage("Please turn Refill Key before continuing.", "Warning");
                return;
            }

            //!!!!WARNING: EXPERIMENTAL LOL DOES THIS WORK GREIGHT MEIGHT?
            new Thread(() =>
           {
               while (!_dumpSwitchPressed)
               {
                   if (BoLib.getHopperDumpSwitchActive() > 0)
                   {
                       if (BoLib.getSwitchStatus(2, 0x20) == 0)
                       {
                           //System.Diagnostics.Debug.WriteLine(DumpSwitchMessage);
                           //RaisePropertyChangedEvent("DumpSwitchMessage");
                           Thread.Sleep(2);
                       }
                       else
                       {
                           _dumpSwitchPressed = true;
                       }
                   }
                   else
                       _dumpSwitchPressed = true; //Dump switch not installed.
               }

               if (_dumpSwitchPressed)
               {
                   Thread.Sleep(500);
                   var which = o as string;
                   var currentCredits = BoLib.getBank() + BoLib.getCredit() + (int)BoLib.getReserveCredits();

                   bool isLeftHopper = which.Equals("left") ? true : false;

                   if (which.Equals("left"))
                   {
                       if (BoLib.getHopperFloatLevel(0) == 0)
                       {
                           _leftHopperTryCount++;
                           //    new WpfMessageBoxService().ShowMessage("The hopper selected is already empty.", "Payout Info");
                           //    return;
                       }

                       if (_leftHopperTryCount <= TRY_COUNT)
                       {
                           BoLib.setUtilRequestBitState((int)UtilBits.DumpLeftHopper);
                           _currentHopperDumping = (byte)Hoppers.Left;
                       }
                   }
                   else
                   {
                       if (!_hasTwoHoppers)
                       {
                           _msg.ShowMessage("Right Hand Hopper not installed", "ERROR");
                           return;
                       }

                       if (BoLib.getHopperFloatLevel(2) == 0)
                       {
                           _rightHopperTryCount++;
                       }

                       if (_rightHopperTryCount <= TRY_COUNT)
                       {
                           BoLib.setUtilRequestBitState((int)UtilBits.DumpRightHopper);
                           _currentHopperDumping = (byte)Hoppers.Right;
                       }
                   }
               }
               /*if (SpanishEmpty == null)
               {
                   SpanishEmpty = new Timer() { Enabled = true, Interval = 100 };
                   SpanishEmpty.Elapsed += new System.Timers.ElapsedEventHandler(TimerSpainEmpty);
               }
               else if (!SpanishEmpty.Enabled)
                   SpanishEmpty.Enabled = true;*/

           }).Start();
            //}

            while (!BoLib.getIsHopperHopping())
            {
                Thread.Sleep(2);
            }
            //Thread.Sleep(2000);

            if (SpanishEmpty == null)
            {
                SpanishEmpty = new Timer() { Enabled = true, Interval = 100 };
                SpanishEmpty.Elapsed += new System.Timers.ElapsedEventHandler(TimerSpainEmpty);
            }
            else if (!SpanishEmpty.Enabled)
                SpanishEmpty.Enabled = true;
          }
        
        void TimerSpainEmpty(object sender, System.Timers.ElapsedEventArgs e)
        {
            //lock (this)
            {
                if (!BoLib.getIsHopperHopping())
                //if (!BoLib.getUtilRequestBitState((int)UtilBits.DumpLeftHopper) && !BoLib.getUtilRequestBitState((int)UtilBits.DumpRightHopper))
                //if (!BoLib.getRequestHopperPayout())
                {
                    SetHopperPayingValue();
                    SpanishEmpty.Enabled = false;
                    var hopperValue = (_currentHopperDumping == (byte)Hoppers.Left) ? "LEFT HOPPER INFO" : "RIGHT HOPPER INFO";
                    var floatLevel = BoLib.getHopperFloatLevel(_currentHopperDumping);
                    _msg.ShowMessage("FINISHED EMPTYING.\n" + _hopperPayingValue + " Coins Paid Out: " + floatLevel, hopperValue);
                    //var w = new PDTUtils.Logic.WarningDialog("FINISHED EMPTYING.\n" + _hopperPayingValue + " Coins Paid Out: " + floatLevel, "FLOAT");
                    BoLib.setHopperFloatLevel(_currentHopperDumping, floatLevel); //0);
                    unchecked { _currentHopperDumping = (byte)Hoppers.NoHopper; }
                    FloatLevelLeft = BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString();
                    FloatLevelRight = BoLib.getHopperFloatLevel((byte)Hoppers.Right).ToString();
                    _dumpSwitchPressed = false;
                }
            }
        }
        
        /// <summary>
        /// Checks to see if directory + utils config exist.
        /// If not create them.
        /// </summary>
        void CheckDirAndIniExist()
        {
            if (!File.Exists(Properties.Resources.utils_config))
            {
                try
                {
                    if (!Directory.Exists(@"D:\1525\config"))
                        Directory.CreateDirectory(@"D:\1525\config");
                    
                    //File.Create(Properties.Resources.utils_config);
                    string contents = "######### General Config for 1525 Utilities.\r\n\r\n[Hoppers]\r\nLeft=300\r\nRight=100";
                    File.WriteAllText(Properties.Resources.utils_config, contents);
                }
                catch (Exception ex)
                {
                    var window = new WpfMessageBoxService();
                    window.ShowMessage(ex.Message, "ERROR");
                }
            }
        }
        
        public ICommand EspChangeRefillAmount { get { return new DelegateCommand(DoEspChangeRefillAmount); } }
        void DoEspChangeRefillAmount(object o)
        {
            var str = o as string;
            var tokens = str.Split('+');
            int increment = 10;//50;//100;
            const int MAX_LIMIT = 1000;
            const int MIN_LIMIT = 200;
            
            CheckDirAndIniExist();

            //<Left/Right> + <increase/decrease>
            if (tokens[0].ToLower().Equals("left"))
            {
                if (tokens[1].ToLower().Equals("increase"))
                {
                    if (EspLeftHopper + increment <= MAX_LIMIT)
                        EspLeftHopper += increment;
                    else
                        EspLeftHopper = MAX_LIMIT;
                }
                else if (tokens[1].ToLower().Equals("decrease"))
                {
                    if (EspLeftHopper - increment > MIN_LIMIT)
                        EspLeftHopper -= increment;
                    else
                        EspLeftHopper = MIN_LIMIT;
                }
            }
            else if (tokens[0].ToLower().Equals("right"))
            {
                if (tokens[1].ToLower().Equals("increase"))
                {
                    if (EspRightHopper + increment <= MAX_LIMIT)
                        EspRightHopper += increment;
                    else
                        EspRightHopper = MAX_LIMIT;
                }
                else if (tokens[1].ToLower().Equals("decrease"))
                {
                    if (EspRightHopper - increment > MIN_LIMIT)
                        EspRightHopper -= increment;
                    else
                        EspRightHopper = MIN_LIMIT;
                }
            }

            NativeWinApi.WritePrivateProfileString("Hoppers", "Left", EspLeftHopper.ToString(), Properties.Resources.utils_config);
            NativeWinApi.WritePrivateProfileString("Hoppers", "Right", EspRightHopper.ToString(), Properties.Resources.utils_config);
        }
        
        public ICommand ZeroHopperFloat { get { return new DelegateCommand(DoHopperZero); } }
        void DoHopperZero(object o)
        {
            if (o == null) return;
            //TODO: !!!! Need to update the visual float level here !!!!
            var str = o as string;
            if (str.Equals("left"))
            {
                BoLib.setHopperFloatLevel((int)Hoppers.Left, 0);
                FloatLevelLeft = 0.ToString();
            }
            else if (str.Equals("right"))
            {
                BoLib.setHopperFloatLevel((int)Hoppers.Right, 0);
                FloatLevelRight = 0.ToString();
            }
        }
        
        public ICommand EspHopperRefill { get { return new DelegateCommand(DoEspHopperRefill); } }
        void DoEspHopperRefill(object o)
        {
           /* if (!_hasTwoHoppers)
            {
                _msg.ShowMessage("Right Hand Hopper not installed", "ERROR");
                return;
            }*/

            var str = o as string;
            var key = str.Equals("left") ? "Left" : "Right";
            byte hopper = str.Equals("left") ? (byte)Hoppers.Left : (byte)Hoppers.Right;
            uint newFloat = (uint)NativeWinApi.GetPrivateProfileInt("Hoppers", key, 0, Properties.Resources.utils_config);
            BoLib.setHopperFloatLevel(hopper, newFloat);
            if (BoLib.getHopperFloatLevel(hopper) > 0)
            {
                var msg = new WpfMessageBoxService();
                msg.ShowMessage("New Float Level is " + newFloat.ToString() + " Coins", "Payout Info");
            }
            
            if (hopper == (byte)Hoppers.Left)
            {
                _leftHopperTryCount = 0;
                FloatLevelLeft = newFloat.ToString();
            }
            else
            {
                _rightHopperTryCount = 0;
                FloatLevelRight = newFloat.ToString();
            }
        }
        
        void SetHopperPayingValue()
        {
            if (_currentHopperDumping >= 0)
            {
                char[] iniValue = new char[5];
                string key = (_currentHopperDumping == (byte)Hoppers.Left) ? "PayoutCoin1" : "PayoutCoin2";

                NativeWinApi.GetPrivateProfileString("FactoryOnly", key, "", iniValue, iniValue.Length, Properties.Resources.birth_cert);
                var args = (key.Equals("PayoutCoin1")) ? "\0 0" : "\0";
                _hopperPayingValue = new string(iniValue, 0, iniValue.Length).Trim(args.ToCharArray());
                
                if (key.Equals("PayoutCoin1"))
                    _hopperPayingValue = _hopperPayingValue.Trim("0".ToCharArray());
                
                if (BoLib.getCountryCode() == BoLib.getSpainCountryCode())
                    _hopperPayingValue = (key == "PayoutCoin1") ? _hopperPayingValue.Insert(0, "€") : 
                                                                  _hopperPayingValue.Insert(_hopperPayingValue.Length, "¢");
                else
                    _hopperPayingValue = (key == "PayoutCoin1") ? _hopperPayingValue.Insert(0, "£") : 
                                                                  _hopperPayingValue.Insert(_hopperPayingValue.Length, "p");
            }
        }
    }
}
