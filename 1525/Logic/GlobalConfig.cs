using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.Logic
{
    //TODO: Shit name - rename this.
    enum ControlRunning
    {
        MainWindow = 1,
        MainPage,
        RegionalSetup,
        History,   
    }
    
    static class GlobalConfig
    {
        public static bool RebootRequired { get; set; }
        public static bool CantBarrageTheFarage { get; set; }
        public static bool LockControlNavigation { get; set; }
        public static bool ReparseSettings { get; set; }
        public static ControlRunning CurrentActive { get; set; }
    }
}
    