using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDTUtils.MVVM
{
    class BaseViewModel : ObservableObject
    {
        string _name = "";
        CabinetSwitchStates _states;

        public string Name
        {
            get { return _name; }
            set
            {
                if (!_name.Equals(value))
                {
                    _name = value;
                    RaisePropertyChangedEvent(_name);
                }
            }
        }

        public CabinetSwitchStates States
        {
            get { return _states; }
            set
            {
                if (_states == null)
                    _states = new CabinetSwitchStates();
                _states = value;
                RaisePropertyChangedEvent("States");
            }
        }
        
        protected BaseViewModel() : base()
        {
            Name = "";
            _states = new CabinetSwitchStates();
        }
        
        protected BaseViewModel(string name) : base()
        {
            Name = name;
            _states = new CabinetSwitchStates();
        }

        //Shut down threads and stuff.
        public virtual void Cleanup()
        {
            
        }
    }
}
