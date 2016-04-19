using System.Windows.Input;
using PDTUtils.Native;
using PDTUtils.Logic;
using System.Diagnostics;

namespace PDTUtils.MVVM.ViewModels
{
    class CashMatchViewModel : BaseViewModel
    {
        readonly byte MIN_LOYALTY = 1;
        readonly byte MAX_LOYALTY = 3;

        readonly string CM_HEADER1 = "CMatchTDC"; //Total Daily Count - no. of games
        readonly string CM_HEADER2 = "CMatchTDV"; //Total Daily Value - daily Spend
        readonly string DM_HEADER1 = "DPlayTDC";  //Amount of Demo plays  
        readonly string DM_HEADER2 = "DPlaySize"; //Games in demo session
        readonly string LY_HEADER1 = "RewardRTP";

        bool _cashMatchActive = true;
        bool _demoActive = true;
        bool _loyaltyActive = true;
        byte _loyaltyPayback = 1;
        uint _maximumEvents = 0;
        uint _maxSpendPerDay = 0;
        uint _demoEventsPerDay = 0;
        uint _demoEventNumOfGames = 0;

        int _oldLength = 0;

        string _sitePrepend = "FHX_";
        string _siteCode = "";

        #region PROPERTIES
        public byte LoyaltyPayback
        {
            get { return _loyaltyPayback; }
            set
            {
                _loyaltyPayback = value;
                RaisePropertyChangedEvent("LoyaltyPayback");
            }
        }

        public uint MaximumEvents
        {
            get { return _maximumEvents; }
            set
            {
                _maximumEvents = value;
                RaisePropertyChangedEvent("MaximumEvents");
                RaisePropertyChangedEvent("VisualMaximumEvents");
            }
        }

        public string VisualMaximumEvents
        {
            get
            {
                if (_maximumEvents == 0)
                    return "0 (Disabled)";
                else if (_maximumEvents == (BoLib.getCashMatchEventMax() + 1))
                    return "∞";
                else
                    return _maximumEvents.ToString();
            }
        }

        public uint MaxSpendPerDay
        {
            get { return _maxSpendPerDay; }
            set
            {
                _maxSpendPerDay = value;
                RaisePropertyChangedEvent("MaxSpendPerDay");
                RaisePropertyChangedEvent("VisualMaxSpendPerDay");
            }
        }

        public string VisualMaxSpendPerDay
        {
            get
            {
                if (_maxSpendPerDay == 0)
                    return "0";
                else if (_maxSpendPerDay == (BoLib.getCashMatchEventMaxValue() + 1))
                    return "∞";
                else
                    return "£" + _maxSpendPerDay.ToString();
            }
        }

        public string DemoEventsPerDay
        {
            get
            {
                if (_demoEventsPerDay == (BoLib.getDemoEventsMax() + 1))
                    return "∞";
                if (_demoEventsPerDay > 0)
                    return _demoEventsPerDay.ToString();
                else
                    return "0";
            }
            /*set
            {
                _demoEventsPerDay = value;
                RaisePropertyChangedEvent("DemoEventsPerDay");
            }*/
        }

        public uint DemoEventsNumOfGames
        {
            get { return _demoEventNumOfGames; }
            set
            {
                _demoEventNumOfGames = value;
                RaisePropertyChangedEvent("DemoEventsNumOfGames");
            }
        }

        public bool CashMatchActive
        {

            get { return _cashMatchActive; }
            set
            {
                _cashMatchActive = value;
                RaisePropertyChangedEvent("CashMatchActive");
            }
        }

        public bool DemoEventsActive
        {
            get { return _demoActive; }
            set
            {
                _demoActive = value;
                RaisePropertyChangedEvent("DemoEventsActive");
            }
        }

        public bool LoyaltyActive
        {
            get { return _loyaltyActive; }
            set
            {
                _loyaltyActive = value;
                RaisePropertyChangedEvent("LoyaltyActive");
            }
        }

        public string SiteCode
        {
            get { return _siteCode; }
            set
            {
                if (value.Length > 6)
                {
                    if (value.Length > _oldLength)
                    {
                        new WpfMessageBoxService().ShowMessage("Site Code should be less than 6 numbers", "Error");
                        _oldLength = value.Length;
                    }

                    return;
                }

                _oldLength = value.Length;
                _siteCode = value;
                RaisePropertyChangedEvent("SiteCode");
            }
        }

        #endregion

        public CashMatchViewModel(string name)
            : base(name)
        {
            var ini = new PDTUtils.Logic.IniFile(Properties.Resources.birth_cert);
            var cm1 = (uint)ini.GetInt32("Operator", CM_HEADER1, 0);
            var cm2 = (uint)ini.GetInt32("Operator", CM_HEADER2, 0);
            var dm1 = (uint)ini.GetInt32("Operator", DM_HEADER1, 0);
            var dm2 = (uint)ini.GetInt32("Operator", DM_HEADER2, 0);
            var loy = (uint)ini.GetInt32("Operator", LY_HEADER1, 1);

            _maximumEvents = cm1;
            _maxSpendPerDay = cm2 / 100;

            _demoEventNumOfGames = (dm2 > 0) ? dm2 : 20;
            _demoEventsPerDay = dm1;

            LoyaltyPayback = BoLib.getSmartCardPointsRTP();

            CashMatchActive = (_maximumEvents > 0) ? true : false;
            DemoEventsActive = (_demoEventsPerDay > 0) ? true : false;
            LoyaltyActive = (LoyaltyPayback > 0) ? true : false;

            var sc = ini.GetString("Operator", "SCVenue", "");
            SiteCode = sc.Split(new char[1] { '_' })[1];
        }
        
        public ICommand EditLoyalty { get { return new DelegateCommand(DoEditLoyalty); } }
        void DoEditLoyalty(object o)
        {
            if (!_loyaltyActive) return;

            var str = o as string;
            if (str.Equals("increase"))
            {
                if (LoyaltyPayback < MAX_LOYALTY)
                    LoyaltyPayback++;
            }
            else if (str.Equals("decrease"))
            {
                if (LoyaltyPayback > MIN_LOYALTY)
                    LoyaltyPayback--;
            }

            NativeWinApi.WritePrivateProfileString("Operator", "RewardRTP", LoyaltyPayback.ToString(), Properties.Resources.birth_cert);
            BoLib.setSmartCardPointsRTP(_loyaltyPayback);
        }

        public ICommand EditMaxEvents
        {
            get { return new DelegateCommand(DoEditMaxEvents); }
        }
        void DoEditMaxEvents(object o)
        {
            if (!_cashMatchActive) return;

            var cashMatchEventMax = BoLib.getCashMatchEventMax() + 1;
            var str = o as string;
            if (str.Equals("increase"))
            {
                if (MaximumEvents == 50)
                {
                    MaximumEvents = cashMatchEventMax;
                }
                else
                {
                    if (MaximumEvents < 25 && MaximumEvents != 0)
                    {
                        MaximumEvents += 5;
                    }
                    else if (MaximumEvents != 0 && MaximumEvents != cashMatchEventMax)
                    {
                        MaximumEvents *= 2;
                    }
                }
            }
            else if (str.Equals("decrease"))
            {
                if (MaximumEvents == cashMatchEventMax)
                {
                    MaximumEvents = 50;
                }
                else
                {
                    if (MaximumEvents == 50)
                    {
                        MaximumEvents = MaximumEvents / 2;
                    }
                    else
                    {
                        if (MaximumEvents > 5)
                            MaximumEvents -= 5;
                    }
                }
            }

            if (MaximumEvents == 0)
            {
                BoLib.setCashMatchEventMaxTotalCount(cashMatchEventMax);
                NativeWinApi.WritePrivateProfileString("Operator", CM_HEADER1, cashMatchEventMax.ToString(), Properties.Resources.birth_cert);
            }
            else
            {
                BoLib.setCashMatchEventMaxTotalCount(MaximumEvents);
                NativeWinApi.WritePrivateProfileString("Operator", CM_HEADER1, MaximumEvents.ToString(), Properties.Resources.birth_cert);
            }
        }

        public ICommand EditDailyMaxSpend
        {
            get { return new DelegateCommand(DoEditDailyMaxSpend); }
        }
        
        void DoEditDailyMaxSpend(object o)
        {
            if (!_cashMatchActive) return;

            var str = o as string;
            var infinite = BoLib.getCashMatchEventMaxValue() + 1;
            if (str.Equals("increase"))
            {
                if (MaxSpendPerDay == 200)
                {
                    MaxSpendPerDay = infinite;
                }
                else
                {
                    if (MaxSpendPerDay < 100 && MaxSpendPerDay != infinite)
                    {
                        MaxSpendPerDay += 25;
                    }
                    else if (MaxSpendPerDay != 0 && MaxSpendPerDay != infinite)
                    {
                        MaxSpendPerDay += 50;
                    }
                }
            }
            else if (str.Equals("decrease"))
            {
                if (MaxSpendPerDay == infinite)
                {
                    MaxSpendPerDay = 200;
                }
                else
                {
                    if (MaxSpendPerDay > 100)
                    {
                        MaxSpendPerDay -= 50;
                    }
                    else
                    {
                        if (MaxSpendPerDay > 25)
                        {
                            MaxSpendPerDay -= 25;
                        }
                    }
                }
            }
            
            if (MaxSpendPerDay == 0)
            {
                BoLib.setCashMatchEventMaxTotalValue(infinite);
                NativeWinApi.WritePrivateProfileString("Operator", CM_HEADER2, infinite.ToString(), Properties.Resources.birth_cert);
            }
            else
            {
                BoLib.setCashMatchEventMaxTotalValue(MaxSpendPerDay * 100);
                NativeWinApi.WritePrivateProfileString("Operator", CM_HEADER2, (MaxSpendPerDay * 100).ToString(), Properties.Resources.birth_cert);
            }
        }
        
        /*
         * 
         * Demo Play Section
         * 
         */
        public ICommand EditDemoEvents { get { return new DelegateCommand(DoEditDemoEvents); } }
        void DoEditDemoEvents(object o)
        {
            if (!_demoActive) return;
            
            var demoMax = BoLib.getDemoEventsMax() + 1;
            var str = o as string;
            if (str.Equals("increase"))
            {
                if (_demoEventsPerDay == 50)
                {
                    _demoEventsPerDay = demoMax;
                }
                else if (_demoEventsPerDay < demoMax)
                {
                    if (_demoEventsPerDay < 25 && _demoEventsPerDay != 0)
                    {
                        _demoEventsPerDay += 5;
                    }
                    else if (_demoEventsPerDay != 0)
                    {
                        _demoEventsPerDay *= 2;
                    }
                }
            }
            else if (str.Equals("decrease"))
            {
                if (_demoEventsPerDay == demoMax)
                {
                    _demoEventsPerDay = 50;
                }
                else
                {
                    if (_demoEventsPerDay == 50)
                    {
                        _demoEventsPerDay = _demoEventsPerDay / 2;
                    }
                    else
                    {
                        if (_demoEventsPerDay > 5)
                            _demoEventsPerDay -= 5;
                    }
                }
            }

            if (_demoEventsPerDay == 0)
            {
                BoLib.setDemoPlayEventMaxTotalCount(demoMax);
                NativeWinApi.WritePrivateProfileString("Operator", DM_HEADER1, demoMax.ToString(), Properties.Resources.birth_cert);
            }
            else
            {
                BoLib.setDemoPlayEventMaxTotalCount(_demoEventsPerDay);
                NativeWinApi.WritePrivateProfileString("Operator", DM_HEADER1, _demoEventsPerDay.ToString(), Properties.Resources.birth_cert);
            }

            RaisePropertyChangedEvent("DemoEventsPerDay");
        }
        
        public ICommand EditDemoPlays { get { return new DelegateCommand(DoEditDemoPlays); } }
        void DoEditDemoPlays(object o)
        {
            if (!_demoActive) return;

            var str = o as string;
            if (str.Equals("increase"))
            {
                if (DemoEventsNumOfGames == 20)
                {
                    DemoEventsNumOfGames = 50;
                }
                else if (DemoEventsNumOfGames == 50)
                {
                    DemoEventsNumOfGames = 100;
                }
            }
            else if (str.Equals("decrease"))
            {
                if (DemoEventsNumOfGames == 50)
                {
                    DemoEventsNumOfGames = 20;
                }
                else if (DemoEventsNumOfGames == 100)
                {
                    DemoEventsNumOfGames = 50;
                }
            }

            BoLib.setDemoPlayEventMaxTotalValue(DemoEventsNumOfGames);
            NativeWinApi.WritePrivateProfileString("Operator", DM_HEADER2, DemoEventsNumOfGames.ToString(), Properties.Resources.birth_cert);
        }
        
        public ICommand ToggleLoyalty
        {
            get
            {
                return new DelegateCommand(o => DoToggleLoyalty());
            }
        }
        void DoToggleLoyalty()
        {
            if (!LoyaltyActive)
            {
                LoyaltyActive = true;
                if (LoyaltyPayback == 0)
                    LoyaltyPayback = 1;
                BoLib.setSmartCardPointsRTP(_loyaltyPayback);
                NativeWinApi.WritePrivateProfileString("Operator", "RewardRTP", LoyaltyPayback.ToString(), Properties.Resources.birth_cert);
            }
            else
            {
                LoyaltyActive = false;
                BoLib.setSmartCardPointsRTP(0);
                NativeWinApi.WritePrivateProfileString("Operator", "RewardRTP", "0", Properties.Resources.birth_cert);
            }
        }
        
        public ICommand ToggleCashMatch
        {
            get { return new DelegateCommand(o => DoToggleCashMatch()); }
        }
        void DoToggleCashMatch()
        {
            if (!CashMatchActive)
            {
                CashMatchActive = true;
                if (_maxSpendPerDay == 0)
                    MaxSpendPerDay = 200;
                if (_maximumEvents == 0)
                    MaximumEvents = 100;

                BoLib.setCashMatchEventMaxTotalCount(_maximumEvents);
                NativeWinApi.WritePrivateProfileString("Operator", CM_HEADER2, _maximumEvents.ToString(), Properties.Resources.birth_cert);
            }
            else
            {
                CashMatchActive = false;
                BoLib.setCashMatchEventMaxTotalCount(0);
                NativeWinApi.WritePrivateProfileString("Operator", CM_HEADER2, "0", Properties.Resources.birth_cert);
            }
        }

        public ICommand ToggleDemoEvents { get { return new DelegateCommand(o => DoToggleDemoEvents()); } }
        void DoToggleDemoEvents()
        {
            if (!DemoEventsActive)
            {
                DemoEventsActive = true;
                if (_demoEventsPerDay == 0)
                {
                    _demoEventsPerDay = 5;
                    RaisePropertyChangedEvent("DemoEventsPerDay");
                }

                BoLib.setDemoPlayEventMaxTotalCount(_demoEventsPerDay);
                NativeWinApi.WritePrivateProfileString("Operator", DM_HEADER1, DemoEventsPerDay, Properties.Resources.birth_cert);
            }
            else
            {
                DemoEventsActive = false;

                BoLib.setDemoPlayEventMaxTotalCount(0);
                NativeWinApi.WritePrivateProfileString("Operator", DM_HEADER1, "0", Properties.Resources.birth_cert);
            }
        }

        public ICommand Save { get { return new DelegateCommand(o => DoSave()); } }
        void DoSave()
        {
            BoLib.loadAndPlayFile(@"d:\1525\wav\SX_MISC1.wav");
            var returnCode = false;

            if (_siteCode.Length < 6)
            {
                try
                {
                    WarningDialog warning = new WarningDialog("Please enter the full 6 digit code", "Error");
                    warning.ShowDialog();
                    returnCode = true;
                }
                finally
                {
                    Debug.WriteLine("Warning Dialog 6 Digit Code Error Thrown.");
                    returnCode = true;
                }
            }

            if (returnCode)
                return;

            if (_siteCode.Length > 6)
                _siteCode = _siteCode.Remove(6);

            NativeWinApi.WritePrivateProfileString("Operator", "SCVenue", _sitePrepend + _siteCode, Properties.Resources.birth_cert);
            GlobalConfig.ReparseSettings = true;
        }
    }
}

