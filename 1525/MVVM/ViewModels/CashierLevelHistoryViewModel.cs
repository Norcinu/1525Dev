using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDTUtils.MVVM.ViewModels
{
    class CashierLevelHistoryViewModel : BaseViewModel
    {
        MachineLogsController _machineLogs;

        public MachineLogsController MachineLogs { get { return _machineLogs; } }
        
        public CashierLevelHistoryViewModel() : base()
        {
            _machineLogs = new MachineLogsController();
            _machineLogs.SetPlayedLog();
            _machineLogs.SetWinningLog();
        }
    }
}
