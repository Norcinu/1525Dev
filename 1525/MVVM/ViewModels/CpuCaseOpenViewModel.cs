using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    struct CpuEvents
    {
        string TimeStamp { get; set; }
        string EventCode { get; set; }
        string EventString { get; set; }
    }
    
    class CpuCaseOpenViewModel : ObservableObject
    {
        bool _showListView = false;
        readonly int _numberOfEvents = 255;
        ObservableCollection<string> _eventList = new ObservableCollection<string>();
        public ObservableCollection<string> EventList { get { return _eventList; } }
        public bool ShowListView
        {
            get { return _showListView; }
            set
            {
                _showListView = value;
                RaisePropertyChangedEvent("ShowListView");
            }
        }
        
        public CpuCaseOpenViewModel()
        {
            BoLib.setUtilRequestBitState((int)UtilBits.ReadCpuEventsBit);
        }
        
        public ICommand LoadEvents { get { return new DelegateCommand(o => DoLoadEvents()); } }
        void DoLoadEvents()
        {
            if (_eventList.Count > 0)
                _eventList.Clear();

            ShowListView = true;
            
            for (int i = 0; i < _numberOfEvents; i++)
            {
                char[] _line = new char[1024];
                NativeWinApi.GetPrivateProfileString("EventLog", (i + 1).ToString(), "", _line, _line.Length, 
                    Properties.Resources.cpu_event_log);
                var str = new string(_line).Trim("\0".ToCharArray());
                if (string.IsNullOrEmpty(str) && i == 0)
                {
                    _eventList.Add("No Events in Logfile.");
                    break;
                }
                _eventList.Add(str);
            }
            RaisePropertyChangedEvent("EventList");
        }
    }
}
