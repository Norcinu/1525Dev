using System.Collections.ObjectModel;
using System.Windows.Input;
using AttachedCommandBehavior;


namespace PDTUtils.MVVM.ViewModels
{
    class TitoConfigView : BaseViewModel
    {
        bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                RaisePropertyChangedEvent("Enabled");
            }
        }

        int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChangedEvent("SelectedIndex");
            }
        }

        string _assetNumber;
        public string AssetNumber
        {
            get { return _assetNumber; }
            set
            {
                _assetNumber = value;
                RaisePropertyChangedEvent("AssetNumber");
            }
        }

        ObservableCollection<string> _hosts;
        public ObservableCollection<string> Hosts
        {
            get { return _hosts; }
        }

        PDTUtils.Logic.IniFile _birthCert;

        public TitoConfigView(string name)
            : base(name)
        {
            _hosts = new ObservableCollection<string>();
            _birthCert = new PDTUtils.Logic.IniFile(Properties.Resources.birth_cert);
            Enabled = _birthCert.GetBoolean("Operator", "TiToEnabled", false);
            SelectedIndex = _birthCert.GetInt32("Operator", "TiToHost", -1);
            AssetNumber = _birthCert.GetString("Operator", "AssetNo", "00000");

            WriteSettings = new SimpleCommand()
            {
                ExecuteDelegate = x => DoWriteSettings()
            };
        }

        public ICommand WriteSettings { get; set; }
        void DoWriteSettings()
        {
            if (AssetNumber.Equals("00000"))
            {
                var wd = new PDTUtils.Logic.WarningDialog("Please enter a valid Asset Number and try again.", "ERROR");
                wd.ShowDialog();
            }
            else
            {
                PDTUtils.Logic.GlobalConfig.RebootRequired = true;
                _birthCert.WriteValue("Operator", "TiToEnabled", Enabled);
                _birthCert.WriteValue("Operator", "TiToHost", SelectedIndex);
                _birthCert.WriteValue("Operator", "AssetNo", AssetNumber);
            }
        }
    }
}
