using System.Collections.ObjectModel;
using System.Windows.Input;
using AttachedCommandBehavior;
using System.Xml;
using System;
using PDTUtils.Native;


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
            SelectedIndex = _birthCert.GetInt32("Operator", "TiToHost", -1) - 1;
            AssetNumber = _birthCert.GetString("Operator", "AssetNo", "00000");
            
            WriteSettings = new SimpleCommand()
            {
                ExecuteDelegate = x => DoWriteSettings()
            };
            
            ParseHostFile();
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
                _birthCert.WriteValue("Operator", "TiToEnabled", Convert.ToInt32(Enabled));
                _birthCert.WriteValue("Operator", "TiToHost", SelectedIndex + 1);
                _birthCert.WriteValue("Operator", "AssetNo", AssetNumber);
                UpdateTitoSettings();
            }
        }
        
        void ParseHostFile()
        {
            try
            {
                using (var xml = XmlReader.Create(Properties.Resources.tito_host_file))
                {
                    while (xml.Read())
                    {
                        if (xml.HasAttributes)
                        {
                            string attr = "";
                            if (xml.Name.Equals("full"))
                            {
                                _hosts.Add(xml.GetAttribute("value"));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        
        void UpdateTitoSettings()
        {
            const string bnvType = "6";
            var printerType = "4";

            if (Enabled)
            {
                BoLib.setFileAction();
                
                BoLib.setTitoState(1);
                _birthCert.WriteValue("FactoryOnly", "PayoutType", "1");
                BoLib.setTerminalType(1);
                //nice little pup <3
                _birthCert.WriteValue("FactoryOnly", "PrinterType", printerType);
                BoLib.setPrinterType(Convert.ToByte(printerType));
                _birthCert.WriteValue("FactoryOnly", "BnvType", bnvType);
                BoLib.setBnvType(Convert.ToByte(bnvType));
                _birthCert.WriteValue("Operator", "RecyclerChannel", "0");
                BoLib.setRecyclerChannel(0);
                
                BoLib.clearFileAction();
            }
            else // disable
            {
                BoLib.setFileAction();

                BoLib.setTitoState(1);
                _birthCert.WriteValue("FactoryOnly", "PayoutType", "0");
                BoLib.setTerminalType(1); //printer

                _birthCert.WriteValue("FactoryOnly", "PrinterType", printerType);
                BoLib.setPrinterType(Convert.ToByte(printerType));
                _birthCert.WriteValue("FactoryOnly", "BnvType", bnvType);
                BoLib.setBnvType(Convert.ToByte(bnvType));
                _birthCert.WriteValue("Operator", "RecyclerChannel", "0");
                BoLib.setRecyclerChannel(0);
                
                BoLib.clearFileAction();
            }
        }
    }
}
