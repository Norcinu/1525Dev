using System;
using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class MachineBettingViewModel : ObservableObject
    {
        readonly uint TOTAL_STAKES = 11;
        uint[] _validBetValues = new uint[11] { 5, 10, 20, 40, 60, 80, 100, 200, 300, 400, 500 };
        uint _currentBetValue = BoLib.getPlayerPointsMinBet();
        int _currentBetIndex = 0;

        #region PROPERTIES
        public uint CurrentBetValue
        {
            get { return _currentBetValue; }
            set
            {
                _currentBetValue = value;
                RaisePropertyChangedEvent("CurrentBetValue");
            }
        }

        public string CurrentBetAsString
        {
            get
            {
                if (_currentBetValue < 100)
                    return (_currentBetValue < 10) ? "0.0" + _currentBetValue.ToString() : "0." + _currentBetValue.ToString();
                else
                    return (_currentBetValue / 100).ToString() + ".00";
            }
        }

        #endregion

        public MachineBettingViewModel()
        {
            _currentBetIndex = Array.IndexOf(_validBetValues, _currentBetValue, 0, (int)TOTAL_STAKES - 1);
            if (_currentBetIndex == -1)
            {
                _currentBetIndex = 0;
                _currentBetValue = _validBetValues[_currentBetIndex];
            }
        }

        public ICommand ForwardHo
        {
            get { return new DelegateCommand(CycleArray); }
        }
        
        void CycleArray(object o)
        {
            var direction = o as string;
            if (direction == "forward")
            {
                if (_currentBetIndex < TOTAL_STAKES - 1)
                    ++_currentBetIndex;
                else
                    _currentBetIndex = 0;
            }
            else
            {
                if (_currentBetIndex > 0)
                    --_currentBetIndex;
                else
                    _currentBetIndex = (int)TOTAL_STAKES - 1;
            }
            
            CurrentBetValue = _validBetValues[_currentBetIndex];
            NativeWinApi.WritePrivateProfileString("Operator", "MinPlayerPointsBet", _currentBetValue.ToString(), 
                Properties.Resources.birth_cert);
            
            // birth cert needs to be re-parsed to affect changes.
            PDTUtils.Logic.GlobalConfig.ReparseSettings = true;
            //BoLib.setUtilRequestBitState((int)UtilBits.RereadBirthCert);
        }
    }
}
