using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class GameSettingViewModel : ObservableObject
    {
        readonly ObservableCollection<GameSettingModel> _gameSettings = new ObservableCollection<GameSettingModel>();
        readonly int[] _ukStakeValues = { 25, 50, 100, 200 };
        readonly string _manifest = @"D:\machine\machine.ini";
        int[] _masterStakes = { 0, 0, 0, 0, 0, 0 };
        //ObservableCollection<int> _masterStakes = new ObservableCollection<int>();
        
        int _count = 0;
        int _currentFirstSel = -1;
        int _currentSecondSel = -1;
        uint _numberOfGames = 0;
        string _errorText = "";
        CultureInfo _currentCulture;

        List<string> _disabledModels = new List<string>();
        Brush [] _stakeColours = new Brush[3];

        public NumberFormatInfo Nfi { get; set; }
        
        #region properties
        public int ActiveCount
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChangedEvent("Count");
            }
        }
        
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                RaisePropertyChangedEvent("ErrorText");
            }
        }

        public CultureInfo SettingsCulture { get { return _currentCulture; } }
        public IEnumerable<GameSettingModel> Settings { get { return _gameSettings; } }
        int _selectedIndex;
        public bool SelectionChanged { get; set; }
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                if (_selectedIndex >= 0)
                {
                    SelectedModelNumber = _gameSettings[_selectedIndex].ModelNumber.ToString();
                    SelectedGameName = _gameSettings[_selectedIndex].Title;

                    RaisePropertyChangedEvent("IsActiveGame");
                    RaisePropertyChangedEvent("IsPromoGame");
                    RaisePropertyChangedEvent("StakeOne");
                    RaisePropertyChangedEvent("StakeTwo");
                    RaisePropertyChangedEvent("StakeThree");
                    RaisePropertyChangedEvent("StakeFour");
                    RaisePropertyChangedEvent("IsFirstPromo");
                    RaisePropertyChangedEvent("IsSecondPromo");
                    RaisePropertyChangedEvent("FirstPromo");
                    RaisePropertyChangedEvent("SecondPromo");
                }
            }
        }

        public string SelectedGameName
        {
            get
            {
                if (_selectedIndex == -1)
                    return "";
                return _gameSettings[SelectedIndex].Title;
            }
            set
            {
                _gameSettings[SelectedIndex].Title = value;
                RaisePropertyChangedEvent("SelectedGameName");
            }
        }
        
        public string SelectedModelNumber
        {
            get
            {
                if (SelectedIndex == -1)
                    return "";
                return _gameSettings[SelectedIndex].ModelNumber.ToString();
            }
            set
            {
                _gameSettings[SelectedIndex].ModelNumber = Convert.ToUInt32(value);
                RaisePropertyChangedEvent("SelectedModelNumber");
            }
        }

        int _numberOfPromos = 0;
        public int NumberOfPromos
        {
            get { return _numberOfPromos; }
            set
            {
                _numberOfPromos = value;
                RaisePropertyChangedEvent("NumberOfPromos");
            }
        }

        bool _isBritish = false;
        public bool IsBritishMachine
        {
            get { return _isBritish; }
            set
            {
                _isBritish = value;
                RaisePropertyChangedEvent("IsBritishMachine");
            }
        }
        
        public bool IsActiveGame
        {
            get
            {
                if (SelectedIndex >= 0)
                    return (bool)_gameSettings[SelectedIndex].Active;
                else
                    return false;
            }
            set
            {
                if (SelectedIndex >= 0)
                {
                    _gameSettings[SelectedIndex].Active = value;
                    RaisePropertyChangedEvent("IsActiveGame");
                }
            }
        }

        public bool IsPromoGame
        {
            get
            {
                if (SelectedIndex >= 0)
                    return _gameSettings[SelectedIndex].Promo;
                else
                    return false;
            }
            set
            {
                if (SelectedIndex >= 0)
                {
                    _gameSettings[SelectedIndex].Promo = value;
                    RaisePropertyChangedEvent("IsPromoGame");
                }
            }
        }
        
        public bool IsFirstPromo
        {
            get { return _gameSettings[SelectedIndex].IsFirstPromo; }
            set
            {
                if (_currentFirstSel != SelectedIndex)
                {
                    foreach (var gs in _gameSettings)
                    {
                        gs.Promo = false;
                        gs.IsFirstPromo = false;
                    }

                    _currentFirstSel = SelectedIndex;
                }
                _gameSettings[SelectedIndex].IsFirstPromo = value;
                RaisePropertyChangedEvent("IsFirstPromo");
            }
        }

        public bool IsSecondPromo
        {
            get { return _gameSettings[SelectedIndex].IsSecondPromo; }
            set
            {
                if (_currentSecondSel != SelectedIndex)
                {
                    _gameSettings[_currentFirstSel].IsSecondPromo = false;
                    _gameSettings[_currentSecondSel].Promo = false;
                    _currentSecondSel = SelectedIndex;
                }
                _gameSettings[SelectedIndex].IsSecondPromo = value;
                RaisePropertyChangedEvent("IsSecondPromo");
            }
        }

        public string FirstPromo
        {
            get;
            /*{
                if (_currentFirstSel != -1)
                    return _gameSettings[_currentFirstSel].Title;
                else
                    return "NOT SELECTED";
            }*/
            set;
        }

        public string SecondPromo
        {
            get;
            set;
            /*{
                if (_currentSecondSel != -1)
                    return _gameSettings[_currentSecondSel].Title;
                else
                    return "NOT SELECTED 2";
            }*/
        }

        public Brush StakeOneColour
        {
            get { return _stakeColours[0]; }
            set { _stakeColours[0] = value; RaisePropertyChangedEvent("StakeOneColour"); }
        }

        public Brush StakeTwoColour
        {
            get { return _stakeColours[1]; }
            set { _stakeColours[1] = value; RaisePropertyChangedEvent("StakeTwoColour"); }
        }

        public Brush StakeThreeColour
        {
            get { return _stakeColours[2]; }
            set { _stakeColours[2] = value; RaisePropertyChangedEvent("StakeThreeColour"); }
        }
        #endregion
        
        public ObservableCollection<string> Stakes { get; set; }
        BitArray _myBits = new BitArray(6, false);
        public BitArray MyBits { get { return _myBits; } }
        string[] _disabled;
        
        #region Commands
        public ICommand SetGameOptions
        {
            get { return new DelegateCommand(o => AddGame()); }
        }
        
        public ICommand SaveGameOptions
        {
            get { return new DelegateCommand(o => SaveChanges()); }
        }
        #endregion

        string[] _fields = 
        {
            "Number", "Title", "Active", "Stake1", "Stake2", "Stake3", "Stake4", 
            "StakeMask", "Promo", "ModelDirectory", "Exe", "HashKey"
        };
        
        //game number 1 - 100 for promo, I.E its index number
        public GameSettingViewModel()
        {
            SelectionChanged = false;
            IsBritishMachine = BoLib.getCountryCode() != BoLib.getSpainCountryCode();
            try
            {
                LoadMasterStakes();

                AddGame();

                Initialise();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            RaisePropertyChangedEvent("FirstPromo");
            RaisePropertyChangedEvent("SecondPromo");

            SelectedIndex = _gameSettings.Count > 0 ? 0 : -1;
        }
        
        void LoadMasterStakes()
        {
            if (Stakes == null)
                Stakes = new ObservableCollection<string>();

            //for (int i = 1; i < 7; i++)
            for (int i = 1; i < 7; i++ )
            {
                char[] temp = new char[5];
                NativeWinApi.GetPrivateProfileString("FactoryOnly", "MasterStake" + i, "", temp, 5, Properties.Resources.birth_cert);
                var a = Convert.ToInt32(new string(temp).Trim(" \0".ToCharArray()));
                if (a > 0)
                    _masterStakes[i /*- 1*/] = a;
                
                // if (a > 0)
                //     Stakes.Add(new string(temp).Trim(" \0".ToCharArray()));
                //_masterStakes[i - 1] = a;
                /*if (_masterStakes[i - 1] >= 100)
                    Stakes.Add(new string()*/
                //_masterStakes[i - 1]));
                /*else if (_masterStakes[i - 1] > 0)
                    Stakes.Add(new string(_masterStakes[i - 1].ToString() + Convert.ToChar("p")));*/

            }
        }
        
        public void AddGame()
        {
            if (!System.IO.File.Exists(_manifest))
            {
                var msg = new WpfMessageBoxService();
                msg.ShowMessage(_manifest, "ERROR");
                return;
            }
            
            if (_gameSettings.Count > 0)
                _gameSettings.Clear();
            
            _currentCulture = new CultureInfo("en-GB");

            Nfi = _currentCulture.NumberFormat;
            
            string[] modelNumber;
            IniFileUtility.GetIniProfileSection(out modelNumber, "Terminal", _manifest, true);
            _numberOfGames = Convert.ToUInt32(modelNumber[0]);
            
            IniFileUtility.GetIniProfileSection(out _disabled, "Disabled", Properties.Resources.disabled_games);

            var sb = new System.Text.StringBuilder(8); //dis be going wrong yo.
            NativeWinApi.GetPrivateProfileString("Operator", "PromoGame", "", sb, 8, Properties.Resources.birth_cert);
            var isPromo = sb.ToString();

            for (var i = 0; i < _numberOfGames; i++)
            {
                string[] model;
                IniFileUtility.GetIniProfileSection(out model, "Game" + (i + 1), _manifest, true);

                try
                {
                    var trimmedModel = model[0].Trim("\\".ToCharArray());
                    var m = new GameSettingModel
                    {
                        ModelNumber = Convert.ToUInt32(trimmedModel),
                        Title = model[1].Trim(" \"".ToCharArray()),
                        StakeMask = 0,
                        ModelDirectory = model[2],
                        Exe = model[3],
                        HashKey = ""
                    };

                    char[] trimmer = { '\0', ' ' };
                    if (_disabled != null)
                    {
                        var found = false;
                        for (var j = 0; j < _disabled.Length; j++)
                        {
                            char[] trim = { '=' };
                            var d = _disabled[j].Trim(trimmer).Split(new char[1] { '=' });

                            //if (_disabled[j] == trimmedModel)
                            if (d[0] == trimmedModel)
                            {
                                found = true;
                                _disabledModels.Add(d[0]);// _disabled[j]);
                                break;
                            }
                        }

                        if (!found)
                            m.Active = true;
                        else
                            m.Active = false;
                    }
                    else
                        m.Active = true;
                    
                    _gameSettings.Add(m);

                    if (i == Convert.ToInt32(isPromo) - 1)
                    {
                        m.Promo = true;
                        m.IsFirstPromo = true;
                        FirstPromo = m.Title;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        public void SaveChanges()
        {
            if (_gameSettings.Count <= 0) return;
            var promoCount = 0;
            foreach (var g in _gameSettings)
            {
                if (g.Promo && promoCount < 2)
                    ++promoCount;
                else if (promoCount >= 2)
                    g.Promo = false;
            }

            if (promoCount == 0)
            {
                var r = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                _gameSettings[r.Next(_gameSettings.Count)].Promo = true;
            }

            bool isFirstSet = false;
            bool isSecondSet = false;
            uint activeCount = _numberOfGames;

            BoLib.loadAndPlayFile(@"d:\1525\wav\SX_MISC1.wav");

            NativeWinApi.WritePrivateProfileString("Operator", "Stake1", _myBits[1].Equals(true) ? "25" : "0", Properties.Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "Stake2", _myBits[2].Equals(true) ? "50" : "0", Properties.Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "Stake3", _myBits[3].Equals(true) ? "100" : "0", Properties.Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "Stake4", _myBits[4].Equals(true) ? "200" : "0", Properties.Resources.birth_cert);
            //NativeWinApi.WritePrivateProfileString("Operator", "Stake5", _myBits[4].Equals(true) ? "200" : "0", Properties.Resources.birth_cert);

            /*try
            {
                if (_disabled != null)
                {
                    Array.Sort(_disabled);
                    for (int i = 1; i <= 100; i++)
                    {
                        NativeWinApi.WritePrivateProfileString("Disabled", i.ToString(), _disabled[i], Properties.Resources.disabled_games);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }*/
            IniFile disabledIni = new IniFile(Properties.Resources.disabled_games);
            disabledIni.DeleteSection("Disabled");

            foreach (var s in _gameSettings)
            {
                if (!s.Active)
                    NativeWinApi.WritePrivateProfileString("Disabled", s.ModelNumber.ToString(), s.ModelNumber.ToString(), Properties.Resources.disabled_games);
            }

            /* Commented out due to no longer writing to the machine ini. I'm not sure if I should take this out though. ELL-OH-ELL. */
            for (var i = 0; i < _numberOfGames; i++)
            {
                var m = _gameSettings[i];

                var temp = "Game" + (i + 1);
                var active = 0;

                if (m.Active)
                    active = 1;
                else
                {
                    active = 0;
                    activeCount--;
                }

                //NativeWinApi.WritePrivateProfileString(temp, _fields[0], m.ModelNumber.ToString(), _manifest);
                //NativeWinApi.WritePrivateProfileString(temp, _fields[1], m.Title, _manifest);

                if (FirstPromo == m.Title && m.Active)
                {
                    //NativeWinApi.WritePrivateProfileString(temp, _fields[8], "100", _manifest);
                    NativeWinApi.WritePrivateProfileString("Operator", "PromoGame", (i + 1).ToString(), Properties.Resources.birth_cert);
                    isFirstSet = true;
                }
                //else
                //{
                //NativeWinApi.WritePrivateProfileString(temp, _fields[8], "0", _manifest);
                //}
            }

            /*if (!isFirstSet)
            {
                for (int i = 0; i < _gameSettings.Count; i++)
                {
                    if (_gameSettings[i].Active)
                    {
                        NativeWinApi.WritePrivateProfileString("Game" + (i + 1), "Promo", "100", _manifest);
                        break;
                    }
                }
            }*/

            /*if (!isSecondSet)
            {
                for (int i = 0; i < _gameSettings.Count; i++)
                {
                    if (_gameSettings[i].Active && !_gameSettings[i].Promo)
                    {
                        NativeWinApi.WritePrivateProfileString("Game" + (i + 1), "Promo", "200", _manifest);
                        break;
                    }
                }
            }*/

            /*NativeWinApi.WritePrivateProfileString("General", "Update", "1", _manifest);
            NativeWinApi.WritePrivateProfileString("General", "NoActive", activeCount.ToString(), _manifest);*/

            IniFileUtility.HashFile(_manifest);

            isFirstSet = false;
            isSecondSet = false;

            GlobalConfig.RebootRequired = true;
        }
        
        
        void Initialise()
        {
            /*char[] trimmer = { '\0', ' ' };*/
            
            /*var buffer = new char[5];*/
            var number = NativeWinApi.GetPrivateProfileInt("Operator", "Stake1", 0, Properties.Resources.birth_cert);
            Stakes.Add((number > 0) ? number.ToString() + "p" : "25p");
            if (number > 0)
            {
                _myBits[1] = true;
                StakeOneColour = Brushes.Green;
            }
            else
                StakeOneColour = Brushes.Red;

            number = NativeWinApi.GetPrivateProfileInt("Operator", "Stake2", 0, Properties.Resources.birth_cert);
            Stakes.Add((number > 0) ? number.ToString() + "p" : "50p");
            if (number > 0)
            {
                _myBits[2] = true;
                StakeTwoColour = Brushes.Green;
            }
            else
                StakeTwoColour = Brushes.Red;
            
            number = NativeWinApi.GetPrivateProfileInt("Operator", "Stake3", 0, Properties.Resources.birth_cert);
            Stakes.Add((number > 0) ? "£" + (number / 100).ToString() : "£1");
            if (number > 0)
                _myBits[3] = true;
            
            number = NativeWinApi.GetPrivateProfileInt("Operator", "Stake4", 0, Properties.Resources.birth_cert);
            Stakes.Add((number > 0) ? "£" + (number / 100).ToString() : "£2");
            if (number > 0)
            {
                _myBits[4] = true;
                StakeThreeColour = Brushes.Green;
            }
            else
                StakeThreeColour = Brushes.Red;

            RaisePropertyChangedEvent("MyBits");
            RaisePropertyChangedEvent("StakeOneColour");
            RaisePropertyChangedEvent("StakeTwoColour");
            RaisePropertyChangedEvent("StakeThreeColour");
        }
        
        public ICommand ToggleStake { get { return new DelegateCommand(DoToggleStake); } }
        void DoToggleStake(object o)
        {
            var str = o as string;
            var index = -1;
            
            if (str.Equals("10"))
                index = 0;
            else if (str.Equals("25"))
            {
                index = 1;
                StakeOneColour = (_myBits[1]) ? Brushes.Green : Brushes.Red;
                RaisePropertyChangedEvent("StakeOneColour");
            }
            else if (str.Equals("50"))
            {
                index = 2;
                StakeTwoColour = (_myBits[2]) ? Brushes.Green : Brushes.Red;
                RaisePropertyChangedEvent("StakeTwoColour");
            }
            else if (str.Equals("100"))
                index = 3;
            else if (str.Equals("200"))
            {
                index = 4;
                StakeThreeColour = (_myBits[3]) ? Brushes.Green : Brushes.Red;
                RaisePropertyChangedEvent("StakeThreeColour");
            }
        }

        public void UpdatePromoSelection(GameSettingModel first, GameSettingModel second)
        {
            var a = BoLib.getPromoGame(0);
            var b = BoLib.getPromoGame(1);
            if (FirstPromo == first.Title)
            {
                var msg = new WpfMessageBoxService().ShowMessage(first.Title + " is already a promo game.\n Please select a different game", "ERROR");
                return;
            }
            
            //if (_gameSettings[a])
            //SecondPromo = first.Title;
            
            FirstPromo = first.Title;
            /*if (first.Title == SecondPromo) SecondPromo = "";
            if (second != null && second.Title != "")
            {
                SecondPromo = second.Title;
                if (second.Title == FirstPromo) SecondPromo = "";
            }*/
                
            
            foreach (var gsm in _gameSettings)
            {
                if (gsm.Title != first.Title)// && gsm.Title != second.Title)
                {
                    gsm.Promo = false;
                }
            }
            
            RaisePropertyChangedEvent("FirstPromo");
            RaisePropertyChangedEvent("SecondPromo");
        }
    }
}

