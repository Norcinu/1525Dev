using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDTUTils;
using System.Windows.Input;

namespace PDTUtils.MVVM.ViewModels
{
    class EngineerViewModel : BaseViewModel
    {
        MachineLogsController _machineLogs;
        public MachineLogsController MachineLogs
        {
            get { return _machineLogs; }
            set 
            {
                if (_machineLogs == null)
                    _machineLogs = new MachineLogsController();
                _machineLogs = value;
                RaisePropertyChangedEvent("MachineLogs");
            }
        }

        public EngineerViewModel(string name)
            : base(name)
        {
            _machineLogs = new MachineLogsController();
            _machineLogs.SetErrorLog();
            _machineLogs.SetHandPayLog();
            _machineLogs.SetPlayedLog();
            _machineLogs.SetWinningLog();
        }
        
        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
