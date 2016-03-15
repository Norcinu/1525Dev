using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;
using PDTUtils.Properties;



namespace PDTUtils.MVVM.ViewModels
{
    class RegionalSettingsViewModel : ObservableObject
    {
        //bool _liveHasBeenEdited = false;
        bool _isSpanishMachine = true;
        bool _selectionChanged = false;
        int _arcadeSelectedIndex = -1;
        int _marketSelectedIndex = -1;
        uint _startUpMultiplier = 0;

        readonly ObservableCollection<KeyValuePair<string, uint>> _settingsView = new ObservableCollection<KeyValuePair<string, uint>>();
        readonly ObservableCollection<SpanishRegionalModel> _arcades = new ObservableCollection<SpanishRegionalModel>();
        readonly ObservableCollection<SpanishRegionalModel> _street = new ObservableCollection<SpanishRegionalModel>();
        readonly ObservableCollection<KeyValuePair<string, string>> _visualSettingsView = new ObservableCollection<KeyValuePair<string,string>>();
        
        SpanishRegionalModel _editableLiveRegion;
        SpainRegionSelection _selected = new SpainRegionSelection();
        
        readonly string _espRegionIni = Resources.esp_live_ini;
        readonly string[] _streetMarketRegions = new string[20]
        {
            "Andalucia", "Aragon", "Asturias", "Baleares", "Pais Vasco", "Cantabria", "Castilla-La Mancha",
            "Castilla Leon", "Catalonia", "Catalonia Light", "Extremadura", "Madrid", "Murcia", "Navarra",
            "La Rioja", "Valencia", "Valencia 500", "Valencia Light", "Canarias", "Galicia"
        };
        
        readonly string[] _arcadeRegions = new string[40]
        {
            "Andalucia-1000", "Aragon-1000","Aragon-2000","Aragon-6000","Asturia-1000","Asturias-2000","Baleares-1000",
            "Baleares-3000-(specialB)","Basque-1000","Basque-2000","Basque-3000-(specialBS)","Cantabria-1000","Cantabria-2000",
            "Castilla-la-Mancha-1000","Castilla-la-Mancha-2000","Castilla-La-Mancha-6000(special)","Castilla-La-Mancha-3000(special)",
            "Castilla-Leon-1000","Castilla-Leon-3000","Catalonia-2000","Catalonia-arcade-500","Extremadura-1000","Madrid-1000",
            "Madria-2000","Madrid-3000","Murcia-600","Murcia-6000-(arcade,reservate area)","Murcia-3000-(arcade,reservate area)",
            "Murcia-arcade-500","Navarra-1000","Navarra-2000","La Rioja-1000","La Rioja-2000","Valencia-2000","Valencia-3000",
            "Valencia-600","Valencia-1000","Canarias-1000","Galicia-3600","Galicia-1800"
        };
        
        readonly string[] _settingHeaders = new string[19] 
        {
            "Max Stake Credits", "Max Stake Bank", "Stake Mask", "Max Win Per Stake", "Max Credits", "Max Reserve Credits", 
            "Max Bank", "Max Player Points", "Escrow State", "RTP", "Game Time", "Give Change Threshold", 
            "Max Bank Note", "Allow Credit To Bank", "Convert To PP", "Cycle Size", "Fast Transfer", "GamesPerPeriod", ""
        };
        
        #region Properties
        public bool FirstScreen { get; set; }
        public bool SecondScreen { get; set; }

        public IEnumerable<SpanishRegionalModel> Arcades { get { return _arcades; } }
        public IEnumerable<SpanishRegionalModel> Street { get { return _street; } }
        public IEnumerable<KeyValuePair<string, uint>> SettingsView { get { return _settingsView; } }

        public SpainRegionSelection Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public SpanishRegionalModel EditableLiveRegion
        {
            get { return _editableLiveRegion; }
            set { _editableLiveRegion = value; }
        }
        
        public int ArcadeSelectedIndex
        {
            get { return _arcadeSelectedIndex; }
            set
            {
                if (_marketSelectedIndex >= 0)
                    MarketSelectedIndex = -1;
                _arcadeSelectedIndex = value;
                SetRegion();

                RaisePropertyChangedEvent("ArcadeSelectedIndex");
            }
        }

        public int MarketSelectedIndex
        {
            get { return _marketSelectedIndex; }
            set
            {
                if (_arcadeSelectedIndex >= 0)
                    ArcadeSelectedIndex = -1;
                _marketSelectedIndex = value;// +1;
                SetRegion();

                RaisePropertyChangedEvent("MarketSelectedIndex");
            }
        }
        
        public bool SelectionChanged
        {
            get { return _selectionChanged; }
            set
            {
                _selectionChanged = value;
                RaisePropertyChangedEvent("SelectionChanged");
            }
        }
        
        public uint _alwaysFichas = 0;
        public uint AlwaysFichas
        {
            get
            {
                return _alwaysFichas;
            }
            set
            {
                _alwaysFichas = value;
                RaisePropertyChangedEvent("AlwaysFichas");
            }
        }
        
        public uint _autoTransferStake = 0;
        public uint AutoTransferStake
        {
            get
            {
                return _autoTransferStake;
            }
            set
            {
                _autoTransferStake = value;
                RaisePropertyChangedEvent("AutoTransferStake");
            }
        }

        public bool IsSpanishMachine
        {
            get { return _isSpanishMachine; }
        }
        
        public  ObservableCollection<KeyValuePair<string, string>> VisualSettingsView
        {
            get{return _visualSettingsView;}
        }

        #endregion
 
        #region Commands
        public ICommand Save { get { return new DelegateCommand(o => SaveChanges()); } }
        public ICommand Load { get { return new DelegateCommand(o => LoadSettings()); } }
        public ICommand Increment { get { return new DelegateCommand(DoIncrement); } }
        public ICommand Decrement { get { return new DelegateCommand(DoDecrement); } }
        public ICommand ResetLiveToDefault { get { return new DelegateCommand(o => DoResetLiveToDefault()); } }
        public ICommand SetEscrow { get { return new DelegateCommand(DoSetEscrow); } }
        public ICommand SetFastTransfer { get { return new DelegateCommand(DoSetFastTransfer); } }
        public ICommand SetExtendedOption { get { return new DelegateCommand(DoSetExtendedOption); } }
        #endregion
        
        public RegionalSettingsViewModel()
        {
            if (BoLib.getCountryCode() != BoLib.getSpainCountryCode())
            {
                _isSpanishMachine = false;
                return; //not a spanish machine so dont load.
            }
            
            var i = 0;
            foreach (var s in _streetMarketRegions)
            {
                var sr = new SpanishRegional();
                //BoLib.getDefaultRegionValues(i, ref sr);
                _street.Add(new SpanishRegionalModel(_streetMarketRegions[i], sr));
                i++;
            }
            
            var smLength = _streetMarketRegions.Length - 1;
            i = 0;
            foreach (var arcade in _arcadeRegions)
            {
                var sr = new SpanishRegional();
                //BoLib.getDefaultRegionValues(smLength + i, ref sr);
                _arcades.Add(new SpanishRegionalModel(_arcadeRegions[i], sr));
                i++;
            }
            
            _editableLiveRegion = new SpanishRegionalModel("", new SpanishRegional());

            SelectionChanged = false;

            AlwaysFichas = 0;// BoLib.getLiveElement((int)EspRegionalExt.EspAlwaysFichas);
            AutoTransferStake = 0;// BoLib.getLiveElement((int)EspRegionalExt.EspAutoTfxToStake);
            
            LoadSettings();
            LoadSettingsView();
            
            _startUpMultiplier = _editableLiveRegion.GamesPerPeriod;

            RaisePropertyChangedEvent("EditableLiveRegion");
            RaisePropertyChangedEvent("Arcades");
            RaisePropertyChangedEvent("Street");
            RaisePropertyChangedEvent("Selected");
            RaisePropertyChangedEvent("IsSpanishMachine");
        }

        void LoadSettingsView()
        {
            if (_settingsView.Count > 0)
                _settingsView.Clear();
            if (_visualSettingsView.Count > 0)
                _visualSettingsView.Clear();
            
            PropertyInfo[] properties = _editableLiveRegion.GetType().GetProperties();
            int headerCtr = 0;
            try
            {
                foreach (var p in properties)
                {
                    if (headerCtr < 18)
                    {
                        if (headerCtr != 2)
                        {
                            if (_settingHeaders[headerCtr] == "RTP")
                            {
                                _settingsView.Add(new KeyValuePair<string, uint>(_settingHeaders[headerCtr],
                                    (uint)p.GetValue(_editableLiveRegion, null) / 100));

                                _visualSettingsView.Add(new KeyValuePair<string, string>(_settingHeaders[headerCtr],
                                    ((uint)p.GetValue(_editableLiveRegion, null) / 100).ToString() + "%"));
                            }
                            else
                            {
                                var str = "";
                                if (headerCtr == 0 || headerCtr == 1 || headerCtr == 3 || headerCtr == 4 ||
                                    headerCtr == 5 || headerCtr == 6 || headerCtr == 12)
                                {
                                    uint a = (uint)p.GetValue(_editableLiveRegion, null);
                                    if (a >= 100)
                                    {
                                        //a = (uint)p.GetValue(_editableLiveRegion, null) / 100;

                                        str = "€" + Convert.ToString(a /= 100);
                                    }
                                    else
                                    {
                                        //a = (uint)p.GetValue(_editableLiveRegion, null);
                                        str = Convert.ToString(a) + "¢";
                                    }
                                }
                                else
                                {
                                    str = p.GetValue(_editableLiveRegion, null).ToString();
                                }

                                _settingsView.Add(new KeyValuePair<string, uint>(_settingHeaders[headerCtr],
                                    (uint)p.GetValue(_editableLiveRegion, null)));

                                _visualSettingsView.Add(new KeyValuePair<string, string>(_settingHeaders[headerCtr], str));
                            }
                        }
                        headerCtr++;
                    }
                }
                _settingsView.Move(16, 11); //17,11
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
            }
            RaisePropertyChangedEvent("SettingsView");
            RaisePropertyChangedEvent("VisualSettingsView");
        }
        //
        public void SaveChanges()
        {
            NativeWinApi.WritePrivateProfileString("General", "Region", Selected.Community, _espRegionIni);
            NativeWinApi.WritePrivateProfileString("General", "VenueType", Selected.VenueType, _espRegionIni);
            
            Selected.Id = Selected.VenueType == "Street Market"
                ? Array.IndexOf(_streetMarketRegions, Selected.Community)
                : Array.IndexOf(_arcadeRegions, Selected.Community) + _streetMarketRegions.Length;
            
            NativeWinApi.WritePrivateProfileString("General", "CurrentRegion", Selected.Id.ToString(), _espRegionIni);
            
            NativeWinApi.WritePrivateProfileString("Settings", "MaxStakeCredits", _editableLiveRegion.MaxStakeCredits.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxStakeBank", _editableLiveRegion.MaxStakeBank.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "StakeMask", _editableLiveRegion.StakeMask.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "WinMax", _editableLiveRegion.MaxWinPerStake.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxCredits", _editableLiveRegion.MaxCredits.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxReserveCredits", _editableLiveRegion.MaxReserveCredits.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxBank", _editableLiveRegion.MaxBank.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "EscrowState", _editableLiveRegion.EscrowState.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "Rtp", (_editableLiveRegion.Rtp * 100).ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "GameTime", _editableLiveRegion.GameTime.ToString(), _espRegionIni);
            
            NativeWinApi.WritePrivateProfileString("Settings", "GamesPerPeriod", _editableLiveRegion.GamesPerPeriod.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "GiveChangeThreshold", _editableLiveRegion.GiveChangeThreshold.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxBankNote", _editableLiveRegion.MaxBankNote.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "AllowBank2Credit", _editableLiveRegion.AllowBank2Credit.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "ConvertToPlay", _editableLiveRegion.ConvertToPlay.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "FastTRansfer", _editableLiveRegion.FastTransfer.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "CycleSize", _editableLiveRegion.CycleSize.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxPlayerPoints", _editableLiveRegion.MaxPlayerPoints.ToString(), _espRegionIni);
            
            IniFileUtility.HashFile(_espRegionIni);
            
            RaisePropertyChangedEvent("EditableLiveRegion");
            
            GlobalConfig.RebootRequired = true;
        }

        /// <summary>
        /// Loads the settings
        /// </summary>
        public void LoadSettings()
        {
            string[] temp;
            IniFileUtility.GetIniProfileSection(out temp, "General", _espRegionIni);
            _selected.Id = Convert.ToInt32(temp[1].Substring(14));
            _selected.Community = temp[2].Substring(7).Trim();
            _selected.VenueType = temp[3].Substring(10).Trim();
            
            string[] liveSettings;
            IniFileUtility.GetIniProfileSection(out liveSettings, "Settings", _espRegionIni);
            
            _editableLiveRegion.MaxStakeCredits = Convert.ToUInt32(liveSettings[0].Substring(16));
            _editableLiveRegion.MaxStakeBank = Convert.ToUInt32(liveSettings[1].Substring(13));
            _editableLiveRegion.StakeMask = Convert.ToUInt32(liveSettings[2].Substring(10));
            _editableLiveRegion.MaxWinPerStake = Convert.ToUInt32(liveSettings[3].Substring(7));
            _editableLiveRegion.MaxCredits = Convert.ToUInt32(liveSettings[4].Substring(11));
            _editableLiveRegion.MaxReserveCredits = Convert.ToUInt32(liveSettings[5].Substring(18));
            _editableLiveRegion.MaxBank = Convert.ToUInt32(liveSettings[6].Substring(8));
            _editableLiveRegion.EscrowState = Convert.ToUInt32(liveSettings[7].Substring(12));
            _editableLiveRegion.Rtp = Convert.ToUInt32(liveSettings[8].Substring(4)) / 100;
            _editableLiveRegion.GameTime = Convert.ToUInt32(liveSettings[9].Substring(9));
            _editableLiveRegion.GamesPerPeriod = Convert.ToUInt32(liveSettings[10].Substring(15));
            _editableLiveRegion.GiveChangeThreshold = Convert.ToUInt32(liveSettings[11].Substring(20));
            _editableLiveRegion.MaxBankNote = Convert.ToUInt32(liveSettings[12].Substring(12));
            _editableLiveRegion.AllowBank2Credit = Convert.ToUInt32(liveSettings[13].Substring(17));
            _editableLiveRegion.ConvertToPlay = Convert.ToUInt32(liveSettings[14].Substring(14));
            _editableLiveRegion.FastTransfer = Convert.ToUInt32(liveSettings[15].Substring(13).TrimStart());
            _editableLiveRegion.CycleSize = Convert.ToUInt32(liveSettings[16].Substring(10));
            _editableLiveRegion.MaxPlayerPoints = Convert.ToUInt32(liveSettings[17].Substring(16));
            
            RaisePropertyChangedEvent("EditableLiveRegion");
            RaisePropertyChangedEvent("VisualMaxNote");
            RaisePropertyChangedEvent("VisualRTP");
            RaisePropertyChangedEvent("VisualGamesPerHour");
        }
        
        public void SetRegion()
        {
            if (_arcadeSelectedIndex == -1 && _marketSelectedIndex == -1)
                return;
            
            var id = 0;
            if (_arcadeSelectedIndex >= 0)
            {
                Selected.VenueType = "Arcade";
                Selected.Community = _arcadeRegions[_arcadeSelectedIndex];
                id = Array.IndexOf(_arcadeRegions, Selected.Community) + (_streetMarketRegions.Length + 1);
            }
            else
            {
                Selected.VenueType = "Street Market";
                Selected.Community = _streetMarketRegions[_marketSelectedIndex];
                id = Array.IndexOf(_streetMarketRegions, Selected.Community);// +1;
            }
            
            var sr = new SpanishRegional();
            //BoLib.getDefaultRegionValues(id, ref sr);
            
            _editableLiveRegion = new SpanishRegionalModel(Selected.Community, sr);
            //_editableLiveRegion.Rtp;// /= 100;
            SelectionChanged = true;
            
            SaveChanges();
            LoadSettings();
            LoadSettingsView();
            
            RaisePropertyChangedEvent("Selected");
            RaisePropertyChangedEvent("EditableLiveRegion");
        }
        
        void DoIncrement(object settingsName)
        {
            var setting = settingsName as string;
            if (setting == null) return;

            if ((setting.Equals("RTP") && _editableLiveRegion.Rtp < 10000))
                _editableLiveRegion.Rtp += 100;
            else if (setting.Equals("MaxBank"))
                _editableLiveRegion.MaxBank += 100;
            else if (setting.Equals("MaxCredits"))
                _editableLiveRegion.MaxCredits += 100;
            else if (setting.Equals("MaxReserve"))
                _editableLiveRegion.MaxReserveCredits += 1000;
            else if (setting.Equals("Cycle") && EditableLiveRegion.CycleSize < 500000)
                EditableLiveRegion.CycleSize += 1000;
            else if (setting.Equals("MaxPlayerPoints"))
                EditableLiveRegion.MaxPlayerPoints += 100;
            else if (setting.Equals("GiveChangeThreshold"))
                EditableLiveRegion.GiveChangeThreshold += 100;
            else if (setting.Equals("MaxStakeCredits"))
                EditableLiveRegion.MaxStakeCredits += 100;
            else if (setting.Equals("MaxStakeBank"))
                EditableLiveRegion.MaxStakeBank += 100;
            
            SaveChanges();
            LoadSettings();
            LoadSettingsView();
            
            RaisePropertyChangedEvent("EditableLiveRegion");
            RaisePropertyChangedEvent("VisualRTP");
        }
        
        void DoDecrement(object settingsName)
        {
            var setting = settingsName as string;
            if (setting == null) return;
            
            if (setting == "RTP")
            {
                if (_editableLiveRegion.Rtp > 1000)
                    _editableLiveRegion.Rtp -= 100;
            }
            else if (setting == "MaxBank" && _editableLiveRegion.MaxBank > 50)
                _editableLiveRegion.MaxBank -= 100;
            else if (setting == "MaxCredits" && _editableLiveRegion.MaxCredits > 50)
                _editableLiveRegion.MaxCredits -= 100;
            else if (setting == "MaxReserve" && _editableLiveRegion.MaxReserveCredits > 1000)
                _editableLiveRegion.MaxReserveCredits -= 1000;
            else if (setting.Equals("Cycle") && _editableLiveRegion.CycleSize > 0)
                EditableLiveRegion.CycleSize -= 1000;
            else if (setting.Equals("MaxPlayerPoints"))
            {
                if (EditableLiveRegion.MaxPlayerPoints >= 100)
                    EditableLiveRegion.MaxPlayerPoints -= 100;
            }
            else if (setting.Equals("GiveChangeThreshold"))
            {
                if (EditableLiveRegion.GiveChangeThreshold >= 100)
                    EditableLiveRegion.GiveChangeThreshold -= 100;
            }
            else if (setting.Equals("MaxStakeCredits"))
            {
                if (EditableLiveRegion.MaxStakeCredits >= 100)
                    EditableLiveRegion.MaxStakeCredits -= 100;
            }
            else if (setting.Equals("MaxStakeBank"))
            {
                if (EditableLiveRegion.MaxStakeBank >= 100)
                    EditableLiveRegion.MaxStakeBank -= 100;
            }
            
            SaveChanges();
            LoadSettings();
            LoadSettingsView();
            
            RaisePropertyChangedEvent("EditableLiveRegion");
            RaisePropertyChangedEvent("VisualRTP");
        }

        void DoResetLiveToDefault()
        {
            var id = (Selected.VenueType == "Street Market") ? Selected.Id : Selected.Id + 1;
            var sr = new SpanishRegional();
            //BoLib.getDefaultRegionValues(id, ref sr);
            _editableLiveRegion = new SpanishRegionalModel(Selected.Community, sr);
            
            SaveChanges();
            LoadSettings();
            LoadSettingsView();

            RaisePropertyChangedEvent("EditableLiveRegion");
        }
        
        void DoSetEscrow(object o)
        {
            var str = o as string;
            if (str == "Enable")
            {
                if (_editableLiveRegion.EscrowState == 0)
                {
                    _editableLiveRegion.EscrowState = 1;
                    SaveChanges();
                }
            }
            else if (str == "Disable")
            {
                if (_editableLiveRegion.EscrowState == 1)
                {
                    _editableLiveRegion.EscrowState = 0;
                    SaveChanges();
                }
            }
        }
        
        void DoSetFastTransfer(object o)
        {
            var str = o as string;
            if (str == "Enable")
            {
                if (_editableLiveRegion.FastTransfer == 0)
                {
                    _editableLiveRegion.FastTransfer = 1;
                    SaveChanges();
                }
            }
            else if (str == "Disable")
            {
                if (_editableLiveRegion.FastTransfer == 1)
                {
                    _editableLiveRegion.FastTransfer = 0;
                    SaveChanges();
                }
            }
        }
        
        void DoSetExtendedOption(object o)
        {
            var str = o as string;
            if (str == null) return;
            var tokens = str.Split("+".ToCharArray());
            switch (tokens[0])
            {
                case "AllowFish":
                    if (tokens[1] == "Enable" && AlwaysFichas == 0)
                    {
                        NativeWinApi.WritePrivateProfileString("Settings", "FichasOnly", "1", Properties.Resources.esp_live_ini);
                        AlwaysFichas = 1;
                    }
                    else if (tokens[1] == "Disable" && AlwaysFichas > 0)
                    {
                        NativeWinApi.WritePrivateProfileString("Settings", "FichasOnly", "0", Properties.Resources.esp_live_ini);
                        AlwaysFichas = 0;
                    }
                    break;
                case "AutoTransfer":
                    if (tokens[1] == "Enable" && AutoTransferStake == 0)
                    {
                        NativeWinApi.WritePrivateProfileString("Settings", "AutoTfx2Stake", "1", Properties.Resources.esp_live_ini);
                        AutoTransferStake = 1;
                    }
                    else if (tokens[1] == "Disable" && AutoTransferStake > 0)
                    {
                        NativeWinApi.WritePrivateProfileString("Settings", "AutoTfx2Stake", "0", Properties.Resources.esp_live_ini);
                        AutoTransferStake = 0;
                    }
                    break;
            }
        }

        public ICommand SetAllowBank2Credit
        {
            get { return new DelegateCommand(DoSetAllowBank2Credit); }
        }
        
        void DoSetAllowBank2Credit(object o)
        {
            var str = o as string;
            if (string.IsNullOrEmpty(str)) return;
            var tokens = str.Split("+".ToCharArray());

            if (tokens[1] == "Enable")
            {
                EditableLiveRegion.AllowBank2Credit = 1;
            }
            else
            {
                EditableLiveRegion.AllowBank2Credit = 0;
            }

            SaveChanges();
        }

        public uint VisualMaxNote
        {
            get { return EditableLiveRegion.MaxBankNote / 100; }
        }

        public ICommand SetMaxBankNote
        {
            get { return new DelegateCommand(DoSetMaxBankNote); }
        }

        void DoSetMaxBankNote(object o)
        {
            var str = o as string;
            if (string.IsNullOrEmpty(str)) return;
            if (str == "20")
                EditableLiveRegion.MaxBankNote = 2000;
            else
                EditableLiveRegion.MaxBankNote = 5000;

            SaveChanges();
            RaisePropertyChangedEvent("VisualMaxNote");
        }
        
        public uint VisualGamesPerHour
        {
            get
            {
                return /*EditableLiveRegion.GamesPerPeriod*/
                    _startUpMultiplier * EditableLiveRegion.GameTime;
            }
        }

        public uint VisualRTP
        {
            get { return EditableLiveRegion.Rtp / 100; }
        }
        
        public ICommand SetConvertToPP
        {
            get { return new DelegateCommand(DoSetConvertToPP); }
        }
        
        void DoSetConvertToPP(object o)
        {
            var str = o as string;
            if (string.IsNullOrEmpty(str)) return;
            if (str.Equals("Enable"))
            {
                EditableLiveRegion.ConvertToPlay = 1;
                NativeWinApi.WritePrivateProfileString("Settings", "ConvertToPlay", "1", Properties.Resources.esp_live_ini);
            }
            else
            {
                EditableLiveRegion.ConvertToPlay = 0;
                NativeWinApi.WritePrivateProfileString("Settings", "ConvertToPlay", "0", Properties.Resources.esp_live_ini);
            }

            RaisePropertyChangedEvent("EditableLiveRegion");
        }

        public ICommand EditTimeSettings
        {
            get { return new DelegateCommand(DoEditTimeSettings); }
        }

        void DoEditTimeSettings(object o)
        {
            var str = o as string;
            if (string.IsNullOrEmpty(str)) return;
            
            var arr = str.Split("+".ToCharArray());
            if (arr[0].Equals("GameTime"))
            {
                if (arr[1].Equals("Increase"))
                {
                    EditableLiveRegion.GameTime += 1;
                }
                else
                {
                    if (EditableLiveRegion.GameTime > 1)
                        EditableLiveRegion.GameTime -= 1;
                }
            }
            else if (arr[0].Equals("Duration"))
            {
                if (arr[1].Equals("Increase"))
                    EditableLiveRegion.GamesPerPeriod += 10;
                else
                {
                    if (EditableLiveRegion.GamesPerPeriod > 10)
                        EditableLiveRegion.GamesPerPeriod -= 10;
                }
            }
            
            SaveChanges();
            RaisePropertyChangedEvent("EditableLiveRegion");
            RaisePropertyChangedEvent("VisualGamesPerHour");
        }
    }
}
