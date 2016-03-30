using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDTUtils;

namespace PDTUtils.MVVM.ViewModels
{
    class GameStatisticsViewModel : BaseViewModel
    {
        MachineGameStatistics _gameStatistics;
        public MachineGameStatistics GameStatistics
        {
            get { return _gameStatistics; }
            set
            {
                _gameStatistics = value;
                RaisePropertyChangedEvent("GameStatistics");
            }
        }
        public GameStatisticsViewModel() : base("GameStatisticsViewModel")
        {
            GameStatistics = new MachineGameStatistics();
            GameStatistics.ParsePerfLog();
        }
    }
}
