using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;
using AttachedCommandBehavior;

namespace PDTUtils.MVVM.ViewModels
{
    class ManufacturerBirthCertViewModel : BaseViewModel
    {
        readonly string _filename = Properties.Resources.birth_cert;

        public ManufacturerBirthCertViewModel(string name)
            : base(name)
        {
            try
            {
                ListBoxSelectionChanged = new SimpleCommand()
                {
                    ExecuteDelegate = x => DoListBoxSelectionChanged(x)
                };

                FactoryOnly = new ObservableCollection<BirthCertModel>();
                Operator = new ObservableCollection<BirthCertModel>();
                ParseIni();
            }
            finally
            {
            }
        }

        ObservableCollection<string> _theHelpMessages = new ObservableCollection<string>
        {
            @"RTP: Game Payout 88-94%",
            @"MinPlayerPointsBet: Minimum Player Points Bet Value. 5. 10. 20. 40. 60. 80. 100. 200. 300. 400. 500",
            @"NvFloatControl: 0 - Never Disable. Cent Value = Nv Off Float < Cent Value",
            @"RecyclerChannel: 2 - £10. 3 - £20",
            @"OverRideRecycler: 0 - Enable note payout. 1 - Disable note payout",
            @"DumpSwitchFitted: 0 = No Hopper Dumpswitch. 1 = Hopper Dumpswitch Active",
            @"Handpay Threshold: Handpay above this value (Pence)",
            @"Handpay Only: 0 - No Handpay. 1 - Handpay active",
            @"LH Divert Threshold: Value in € for Left Hopper Cashbox Divert",
            @"RH Divert Threshold: Value in € for Right Hopper Cashbox Divert",
            @"RefloatLH: Value to set left hopper float in Pence.",
            @"RefloatRH: Value to set right hopper float in Pence.",
            @"LProgressiveSys: 0 - No Local Progressive. 1 - Local Progressive"
        };

        public ObservableCollection<BirthCertModel> FactoryOnly { get; set; }
        public ObservableCollection<BirthCertModel> Operator { get; set; }
        public ObservableCollection<string> HelpValues { get; set; }

        public ICommand Parse { get { return new DelegateCommand(o => ParseIni()); } }
        void ParseIni()
        {
            ParseSection("FactoryOnly", FactoryOnly);
            ParseSection("Operator", Operator);
        }

        void ParseSection(string section, ObservableCollection<BirthCertModel> collection)
        {
            if (collection.Count == 0)
            {
                string[] config;
                IniFileUtility.GetIniProfileSection(out config, section, _filename);
                
                foreach (var str in config)
                {
                    if (str.StartsWith("#"))
                        break;

                    var pair = str.Split("=".ToCharArray());
                    var header = "";
                    if (pair[0] == "RTP")
                        header = " (%)";
                    else if (pair[0] == "Handpay Threshold")
                        header = " (p)";
                    else if (pair[0] == "LH Divert Threshold" || pair[0] == "RH Divert Threshold")
                        header = " (£)";
                    else if (pair[0] == "RefloatLH" || pair[0] == "RefloatRH")
                        header = " (Num of coins)";

                    collection.Add(new BirthCertModel(pair[0] + header, pair[1]));
                }
            }
        }
        
        public void SetHelpMessage(int index)
        {
            if (index > Operator.Count)
                return;
            else
            {
                if (HelpValues == null)
                    HelpValues = new ObservableCollection<string>();

                if (HelpValues.Count > 0)
                    HelpValues.RemoveAll();
                
                var temp = _theHelpMessages[index];
                var arr = temp.Split(":.".ToCharArray());

                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0 && !string.IsNullOrEmpty(arr[i]))
                        HelpValues.Add(arr[i]);
                }

                RaisePropertyChangedEvent("HelpValues");
            }
        }
        
        public ICommand ListBoxSelectionChanged { get; set; }
        void DoListBoxSelectionChanged(object o)
        {
            if (GlobalConfig.CantBarrageTheFarage)
            {
                var group = BoLib.getSmartCardGroup();
                if (group != 4 && group != 6)
                    return;
            }

            if (o == null)
                return;

            var index = o as int?;

            var key = Operator[(int)index].Field;
            var val = Operator[(int)index].Value;

            if (key.Contains("(%)") || key.Contains("(p)") || key.Contains("(£)") || key.Contains("(Num of coins)"))
                key = key.Substring(0, key.IndexOf("("));

            var bcw = new BirthCertSettingsWindow(key.ToString(), val.ToString());
            if (bcw.ShowDialog() != false) return;
            if (bcw.TxtNewValue.Text == val)
                return;
            else
            {
                Operator[(int)index].Value = bcw.TxtNewValue.Text;
                NativeWinApi.WritePrivateProfileString("Operator", key, Operator[(int)index].Value, Properties.Resources.birth_cert);
                GlobalConfig.ReparseSettings = true;

                RaisePropertyChangedEvent("OperatorESP");
            }
        }
    }
}
