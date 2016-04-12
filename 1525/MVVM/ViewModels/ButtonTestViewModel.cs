using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PDTUtils.Native;
using Timer = System.Timers.Timer;

namespace PDTUtils.MVVM.ViewModels
{
    class ButtonConfig
    {
        public List<string> Names { get; set; }
        protected ButtonConfig()
        {
            Names = new List<string>();
            Names.Add("Refill Key");
            Names.Add("Door Switch");
        }
    }

    class Advantage : ButtonConfig
    {
        public Advantage()
            : base()
        {
            Names.Add("Collect");
            Names.Add("More Games");
            Names.Add("Autoplay");
            Names.Add("Transfer");
            Names.Add("Stake");
            Names.Add("Start");
        }
    }

    class FortuneHunterXtra : ButtonConfig
    {
        public FortuneHunterXtra()
            : base()
        {
            Names.Add("Collect");
            Names.Add("Stake 1");
            Names.Add("Autoplay");
            Names.Add("Transfer");
            Names.Add("Stake 2");
            Names.Add("Start (TOP)");
            Names.Add("Start (FRONT)");
        }
    }

    class ButtonTestViewModel : BaseViewModel
    {
        #region Private Members
        bool _firstPass;

        readonly byte[] _specialMasks = new byte[2] { 0x10, 0x02 };
        readonly byte[] _buttonMasks = new byte[8] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
        readonly byte[] _lampMasks = new byte[8] { 128, 64, 32, 16, 8, 4, 2, 1 };

        string _bannerMessage;
        string _cabinetType;
        string _currentButtonStr;
        string _buttonResultSuccess;
        string _buttonResultError;
        Timer _testTimer;
        Brush _bannerBack;
        Brush _bannerForeground;

        int _currentButton;
        int _timerCounter;

        ButtonConfig _buttons;

        Visibility _startButtonActive;
        #endregion

        #region Public Properties
        public string BannerMessage
        {
            get { return _bannerMessage; }
            set
            {
                _bannerMessage = value;
                RaisePropertyChangedEvent("BannerMessage");
            }
        }

        public string CabinetType
        {
            get { return _cabinetType; }
            set
            {
                _cabinetType = value;
                RaisePropertyChangedEvent("CabinetType");
            }
        }

        public string CurrentButton
        {
            get { return _currentButtonStr; }
            set
            {
                _currentButtonStr = value;
                RaisePropertyChangedEvent("CurrentButton");
            }
        }

        public string ButtonResultSuccess
        {
            get { return _buttonResultSuccess; }
            set
            {
                _buttonResultSuccess = value;
                RaisePropertyChangedEvent("ButtonResultSuccess");
            }
        }

        public string ButtonResultError
        {
            get { return _buttonResultError; }
            set
            {
                _buttonResultError = value;
                RaisePropertyChangedEvent("ButtonResultError");
            }
        }

        public ICommand StartTest
        {
            get { return new DelegateCommand(o => DoStartTest()); }
        }

        public Brush BannerBackColour
        {
            get { return _bannerBack; }
            set
            {
                _bannerBack = value;
                RaisePropertyChangedEvent("BannerBackColour");
            }
        }

        public Brush BannerForeColour
        {
            get { return _bannerForeground; }
            set
            {
                _bannerForeground = value;
                RaisePropertyChangedEvent("BannerForeColour");
            }
        }

        public Visibility StartButtonActive
        {
            get { return _startButtonActive; }
            set
            {
                _startButtonActive = value;
                RaisePropertyChangedEvent("StartButtonActive");
            }
        }

        #endregion

        public ButtonTestViewModel(string name)
            : base(name)
        {
            _currentButton = 0;
            _timerCounter = 5;
            _bannerMessage = "Press Start to Continue";
            _cabinetType = BoLib.getCabinetType() != 5 ? "FortuneHunterXtra" : "ADVANTAGE"; //TODO !!! Expand this out for future cabs
            _testTimer = new Timer(1000);

            CurrentButton = "";
            ButtonResultSuccess = "";
            ButtonResultError = "";

            BannerBackColour = Brushes.Yellow;
            BannerForeColour = Brushes.HotPink;
            StartButtonActive = Visibility.Visible;

            _testTimer = new Timer() { Enabled = false, Interval = 1000 };
            _testTimer.Elapsed += new System.Timers.ElapsedEventHandler(_testTimer_Elapsed);

            if (_cabinetType.Equals("FortuneHunterXtra"))
                _buttons = new FortuneHunterXtra();
            else if (_cabinetType.Equals("ADVANTAGE"))
                _buttons = new Advantage();
        }

        void _testTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool someoneSomewhere = false;
            if (_currentButton < _buttons.Names.Count)
            {
                uint status = 0;
                if (_currentButton == 0) // key
                {
                    status = BoLib.getSwitchStatus(2, _specialMasks[_currentButton]);
                    if (!_firstPass)
                    {
                        CurrentButton = "Turn Refill Back";
                        _firstPass = true;
                    }
                    else
                    {
                        _currentButton++;
                        CurrentButton = "Hold Door Switch";
                        _firstPass = false;
                        someoneSomewhere = true;
                    }
                }
                else if (_currentButton == 1) // door
                {
                    someoneSomewhere = true;
                    _currentButton++;
                    CurrentButton = _buttons.Names[_currentButton];
                }
                else // button deck
                {
                    status = BoLib.getSwitchStatus(1, _buttonMasks[_currentButton - 2]);
                    BoLib.setLampStatus(1, _lampMasks[_currentButton - 2], 1);
                    if (status == 1 || _timerCounter == 0)
                    {
                        if (status == 0)
                            ButtonResultError += _buttons.Names[_currentButton] + "\n";
                        else
                            ButtonResultSuccess += _buttons.Names[_currentButton] + "\n";

                        _currentButton++;

                        if (_currentButton < _buttons.Names.Count)
                            CurrentButton = _buttons.Names[_currentButton];

                        someoneSomewhere = true;
                    }
                }
                
                if (_timerCounter > 0)
                    _timerCounter--;
                else
                    _timerCounter = 5;

                BannerMessage = "Counter " + _timerCounter;

                if (someoneSomewhere && _timerCounter != 5)
                    _timerCounter = 5;
            }
            else
            {
                _currentButton = 0;
                _timerCounter = 5;
                _testTimer.Enabled = false;
                BannerMessage = "Press Start to Continue";
                CurrentButton = "";
                StartButtonActive = Visibility.Visible;
            }
        }
        
        void DoStartTest()
        {
            if (_testTimer == null)
                _testTimer = new Timer() { Enabled = false, Interval = 5000 };
            
            _testTimer.Enabled = true;
            BannerMessage = "Counter " + _timerCounter;
            ButtonResultSuccess = "";
            ButtonResultError = "";
            CurrentButton = "Turn Refill Key";
            StartButtonActive = Visibility.Hidden;
        }
    }
}
