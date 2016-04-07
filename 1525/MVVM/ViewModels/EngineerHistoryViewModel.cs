using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDTUtils.MVVM.ViewModels
{
    class EngineerHistoryViewModel : BaseViewModel
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

        public EngineerHistoryViewModel(string name)
            : base(name)
        {
            _machineLogs = new MachineLogsController();
            _machineLogs.SetErrorLog();
            _machineLogs.SetHandPayLog();
            _machineLogs.SetPlayedLog();
            _machineLogs.SetWinningLog();
        }

        public void Cleanup()
        {

        }
    }
}
