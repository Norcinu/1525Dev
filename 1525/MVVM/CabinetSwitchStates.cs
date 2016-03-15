using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDTUtils.Native;

namespace PDTUtils.MVVM
{
    class CabinetSwitchStates : ObservableObject
    {
        #region Private Members 
        bool _running = false;
        bool _isDoorOpen = false;
        bool _isRefillMade = false;
        bool _hasSmartCard = false;
        bool _isTestSuiteRunning = false;
        bool _prepareForReboot = false;
        string _smartCardString = "";
        #endregion

        #region Public Properties
        public bool Running
        {
            get { return _running; }
            set { _running = value; RaisePropertyChangedEvent("Running"); }
        }
        
        public bool IsDoorOpen
        {
            get { return _isDoorOpen; }
            set { _isDoorOpen = value; RaisePropertyChangedEvent("IsDoorOpen"); }
        }

        public bool IsRefillMade
        {
            get { return _isRefillMade; }
            set { _isRefillMade = value; RaisePropertyChangedEvent("IsRefillMade"); }
        }
        public bool HasSmartCard
        {
            get { return _hasSmartCard; }
            set { _hasSmartCard = value; RaisePropertyChangedEvent("HasSmartCard"); }
        }
        public bool PrepareForReboot
        {
            get { return _prepareForReboot; }
            set { _prepareForReboot = value; RaisePropertyChangedEvent("PrepareForReboot"); }
        }

        public bool IsTestSuiteRunning
        {
            get { return _isTestSuiteRunning; }
            set { _isTestSuiteRunning = value; RaisePropertyChangedEvent("IsTestSuiteRunning"); }
        }
        #endregion

        public CabinetSwitchStates()
        {
        }
        
        public void Run()
        {
            while (_running)
            {

            }
        }
    }
}
