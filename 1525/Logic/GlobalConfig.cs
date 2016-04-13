using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.Logic
{   
    static class GlobalConfig
    {
        public static bool RebootRequired { get; set; }
        public static bool CantBarrageTheFarage { get; set; }
        public static bool LockControlNavigation { get; set; }
        public static bool ReparseSettings { get; set; }
        public static bool TestSuiteRunning { get; set; }
    }
}
