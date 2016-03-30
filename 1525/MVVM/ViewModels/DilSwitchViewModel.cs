using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class DilSwitchViewModel : BaseViewModel
    {
        string _dilSwitchOne;
        string _dilSwitchTwo;
        string _dilSwitchThree;
        string _dilSwitchFour;

        
        public string DilSwitchOne
        {
            get { return _dilSwitchOne; }
            set
            {
                _dilSwitchOne = value;
                RaisePropertyChangedEvent("DilSwitchOne");
            }
        }

        public string DilSwitchTwo
        {
            get { return _dilSwitchTwo; }
            set
            {
                _dilSwitchTwo = value;
                RaisePropertyChangedEvent("DilSwitchOne");
            }
        }

        public string DilSwitchThree
        {
            get { return _dilSwitchThree; }
            set
            {
                _dilSwitchThree = value;
                RaisePropertyChangedEvent("DilSwitchThree");
            }
        }

        public string DilSwitchFour
        {
            get { return _dilSwitchFour; }
            set
            {
                _dilSwitchFour = value;
                RaisePropertyChangedEvent("DilSwitchFour");
            }
        }

        public DilSwitchViewModel(string name) : base("Dil Switch") 
        {
            CheckSwitches();
        }
        
        void CheckSwitches()
        {
            uint[] results = new uint[4] { 0, 0, 0, 0 };
            uint index = 0;

            for (var i = 1; i <= 8; i *= 2)
            {
                results[index] = BoLib.getSwitchStatus(4, (byte)i);
                index++;
            }

            if (results[0] > 0)
                DilSwitchOne = "Dil Switch #1 On";
            else
                DilSwitchOne = "Dil Switch #1 Off";

            if (results[1] > 0)
                DilSwitchTwo = "Dil Switch #2 On";
            else
                DilSwitchTwo = "Dil Switch #2 Off";

            if (results[2] > 0)
                DilSwitchThree = "Dil Switch #3 On";
            else
                DilSwitchThree = "Dil Switch #3 Off";
            
            if (results[3] > 0)
                DilSwitchFour = "Dil Switch #4 On";
            else
                DilSwitchFour = "Dil Switch #4 Off";
        }
    }
}
