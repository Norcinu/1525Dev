using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils.MVVM.ViewModels
{
    class GeneralSettingsViewModel : BaseViewModel
    {
        public bool RebootRequired { get; set; }
        public bool IsCatC { get; set; }
        public bool TiToEnabled { get; set; }
        public bool IsBritish { get; set; }
        public bool HandPayState { get; set; }

        public string RtpMessage { get; set; }
        public string HandPayLevel { get; set; }
        public string DivertLeftMessage { get; set; }
        public string DivertRightMessage { get; set; }
        public string TerminalAssetMsg { get; set; }
        public string HandPayStateMsg { get; set; }
        readonly string _titoDisabledMsg = "Warning: TiTo DISABLED";
        readonly string _titoEnabledMsg = "TiTo ENABLED";

        public GeneralSettingsViewModel(string name)
            : base(name)
        {
            try
            {

                if (BoLib.getCountryCode() != BoLib.getSpainCountryCode())
                {
                    IsBritish = true;
                    RaisePropertyChangedEvent("IsBritish");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
                }
                else
                {
                    IsBritish = false;
                    RaisePropertyChangedEvent("IsBritish");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
                }

                /*   if (BoLib.getCountryCode() != BoLib.getUkCountryCodeC())
                   {
                       RtpMessage = "CAT C NOT ENABLED";
                       IsCatC = false;
                   }
                   else
                   {*/
                RtpMessage = BoLib.getTargetPercentage().ToString() + "%";
                //IsCatC = true;
                //                }

                TiToEnabled = BoLib.getTitoEnabledState();

                var sb = new StringBuilder(20);
                NativeWinApi.GetPrivateProfileString("Operator", "AssetNo", "", sb, sb.Capacity, Resources.birth_cert);


                TerminalAssetMsg = (TiToEnabled) ? /*_titoEnabledMsg*/ sb.ToString() : _titoDisabledMsg;

                HandPayLevel = (BoLib.getHandPayThreshold() / 100).ToString();
                DivertLeftMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Left).ToString();
                DivertRightMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Right).ToString();

                RebootRequired = false;

                char[] buffer = new char[3];
                NativeWinApi.GetPrivateProfileString("Config", "PayoutType", "", buffer, buffer.Length, Resources.birth_cert);
                var str = new string(buffer).Trim("\0".ToCharArray());
                if (str.Equals("0"))
                {
                    HandPayState = false;
                    HandPayStateMsg = "Hopper Only";
                    RaisePropertyChangedEvent("HandPayState");
                }
                else
                {
                    if (str.Equals("1"))
                        HandPayStateMsg = "Printer";
                    else
                        HandPayStateMsg = "Combined";

                    HandPayState = true;
                    RaisePropertyChangedEvent("HandPayState");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            RaisePropertyChangedEvent("IsCatC");
            RaisePropertyChangedEvent("HandPayLevel");
            RaisePropertyChangedEvent("DivertMessage");
            RaisePropertyChangedEvent("TerminalAssetMsg");
            RaisePropertyChangedEvent("RebootRequired");
        }
        
        public ICommand SetRtp
        {
            get { return new DelegateCommand(ChangeRtp); }
        }
        
        void ChangeRtp(object newRtp)
        {          
            var rtp = NativeWinApi.GetPrivateProfileInt("Operator", "RTP", 0, Properties.Resources.birth_cert);
            var s = newRtp as string;
            if (s.Equals("up"))
            {
                var max = BoLib.getCountryCode() == BoLib.getUkCountryCodeC() ? 90 : 94;
                if (rtp < max) 
                    rtp = rtp + 1;
                
                GlobalConfig.RebootRequired = true;
            } 
            else if (s.Equals("down"))
            {
                var min = BoLib.getCountryCode() == BoLib.getUkCountryCodeC() ? 86 : 90;
                if (rtp > min) 
                    rtp = rtp - 1;
                
                GlobalConfig.RebootRequired = true;
            }
            NativeWinApi.WritePrivateProfileString("Operator", "RTP", rtp.ToString(), Properties.Resources.birth_cert);
            RtpMessage = rtp.ToString() + "%";
            RaisePropertyChangedEvent("RtpMessage");
            /*if (BoLib.getCountryCode() == BoLib.getUkCountryCodeC())
            {
                var rtp = Convert.ToInt32(newRtp as string);
                NativeWinApi.WritePrivateProfileString("Operator", "RTP", rtp.ToString(), Resources.birth_cert);
                IniFileUtility.HashFile(Resources.machine_ini);
                
                RtpMessage = rtp.ToString() + "%";
                RaisePropertyChangedEvent("RtpMessage");
            }*/
        }
        
        public ICommand ChangeHandPayThreshold
        {
            get { return new DelegateCommand(DoChangeHandPayThreshold); }
        }
        
        public void DoChangeHandPayThreshold(object o)
        {
            if (!BoLib.canPerformHandPay() || BoLib.getTerminalType() == 1)
            {
                var _msgBox = new WpfMessageBoxService();
                _msgBox.ShowMessage("UNABLE TO CHANGE HANDPAY THRESHOLD. CHECK PRINTER OR COUNTRY SETTINGS", "ERROR");
                return;
            }
            
            var type = o as string;
            var current = (int)BoLib.getHandPayThreshold();
            var newVal = current;
            
            var maxHandPay = (int)BoLib.getMaxHandPayThreshold();
            var denom = maxHandPay - current;
            var amount = (denom < 1000) ? denom : 1000;//5000

            if (type == "increment")
            {
                BoLib.setHandPayThreshold((uint)current + (uint)amount);
                newVal += amount;
                NativeWinApi.WritePrivateProfileString("Operator", "Handpay Threshold", (newVal).ToString(), Resources.birth_cert);
            }
            else
            {
                var thresh = BoLib.getHandPayThreshold();

                if (thresh == 0)
                    return;

                if (amount == 0 && thresh >= 1000)
                    amount = 1000; //5000

                if (thresh < 1000)
                {
                    amount = 0;
                    for (int i = 0; i < thresh; i += 100)
                    {
                        amount += 100;
                    }
                }

                BoLib.setHandPayThreshold((uint)current - (uint)amount);
                newVal -= amount;
                NativeWinApi.WritePrivateProfileString("Operator", "Handpay Threshold", (newVal).ToString(), Resources.birth_cert);
            }
            
            //IniFileUtility.HashFile(Resources.birth_cert);

            HandPayLevel = (newVal / 100).ToString();
            RaisePropertyChangedEvent("HandPayLevel");
        }
        
        /// <summary>
        /// !!!!! TODO: REFACTOR THESE DIVERT FUNCTIONS !!!!!
        /// </summary>
        public ICommand ChangeLeftDivert { get { return new DelegateCommand(DoChangeDivert); } }
        void DoChangeDivert(object o)
        {
            var actionType = o as string;
            var currentThreshold = BoLib.getHopperDivertLevel(0);
            const uint changeAmount = 50;
            var newValue = currentThreshold;

            if (actionType == "increment" && currentThreshold < 800)
            {
                newValue += changeAmount;
                if (newValue > 800)
                    newValue = 800;
            }
            else if (actionType == "decrement" && currentThreshold > 200)
            {
                newValue -= changeAmount;
                if (newValue < 200)
                    newValue = 0;
            }
            
            BoLib.setHopperDivertLevel(BoLib.getLeftHopper(), newValue);
            NativeWinApi.WritePrivateProfileString("Operator", "LH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            //IniFileUtility.HashFile(Resources.birth_cert);
            
            DivertLeftMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertLeftMessage");
        }
        
        public ICommand ChangeRightDivert { get { return new DelegateCommand(DoChangeDivertRight); } }
        void DoChangeDivertRight(object o)
        {
            var actionType = o as string;
            var currentThreshold = BoLib.getHopperDivertLevel((byte)Hoppers.Right);
            const uint changeAmount = 50;
            var newValue = currentThreshold;

            if (actionType == "increment" && currentThreshold < 600)
            {
                newValue += changeAmount;
                if (newValue > 600)
                    newValue = 600;
            }
            else if (actionType == "decrement" && currentThreshold > 50)
            {
                newValue -= changeAmount;
                if (newValue < 50)
                    newValue = 50;
            }

            BoLib.setHopperDivertLevel(BoLib.getRightHopper(), newValue);
            NativeWinApi.WritePrivateProfileString("Operator", "RH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            //IniFileUtility.HashFile(Resources.birth_cert);

            DivertRightMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertRightMessage");
        }
        
        public ICommand TiToState { get { return new DelegateCommand(ToggleTiToState); } }
        void ToggleTiToState(object o) //TODO: Re-factor this mess of code ffs.
        {
            var state = o as string;
            TiToEnabled = (state == "enabled");

            if (TiToEnabled) // enable
            {
                var sb = new StringBuilder(20);
                NativeWinApi.GetPrivateProfileString("Operator", "AssetNo", "", sb, sb.Capacity, Resources.birth_cert);
                TerminalAssetMsg = sb.ToString();

                BoLib.setFileAction();

               // TerminalAssetMsg = _titoEnabledMsg;
                NativeWinApi.WritePrivateProfileString("Operator", "TiToEnabled", "1", Resources.birth_cert);
                BoLib.setTitoState(1);
                NativeWinApi.WritePrivateProfileString("FactoryOnly", "PayoutType", "1", Resources.birth_cert);
                BoLib.setTerminalType(1); //printer
                
                const string bnvType = "6";

                var printerType = "4";// BoLib.getCabinetType() == 3 ? "3" : "4";

                NativeWinApi.WritePrivateProfileString("FactoryOnly", "PrinterType", printerType, Resources.birth_cert); // 3 = NV200_ST
                BoLib.setPrinterType(Convert.ToByte(printerType));
                NativeWinApi.WritePrivateProfileString("FactoryOnly", "BnvType", bnvType, Resources.birth_cert);
                BoLib.setBnvType(Convert.ToByte(bnvType));
                NativeWinApi.WritePrivateProfileString("Operator", "RecyclerChannel", "0", Resources.birth_cert);
                BoLib.setRecyclerChannel(0);

                BoLib.clearFileAction();
                
                RebootRequired = true;
                GlobalConfig.RebootRequired = true;
            }
            else // disable
            {
                BoLib.setFileAction();
                
                TerminalAssetMsg = _titoDisabledMsg;
                NativeWinApi.WritePrivateProfileString("Operator", "TiToEnabled", "0", Resources.birth_cert);
                BoLib.setTitoState(1);
                NativeWinApi.WritePrivateProfileString("FactoryOnly", "PayoutType", "0", Resources.birth_cert);
                BoLib.setTerminalType(1); //printer

                const string bnvType = "6";

                var printerType = "4";//BoLib.getCabinetType() == 3 ? "3" : "4";

                NativeWinApi.WritePrivateProfileString("FactoryOnly", "PrinterType", printerType, Resources.birth_cert); // 3 = NV200_ST
                BoLib.setPrinterType(Convert.ToByte(printerType));
                NativeWinApi.WritePrivateProfileString("FactoryOnly", "BnvType", bnvType, Resources.birth_cert);
                BoLib.setBnvType(Convert.ToByte(bnvType));
                NativeWinApi.WritePrivateProfileString("Operator", "RecyclerChannel", "0", Resources.birth_cert);
                BoLib.setRecyclerChannel(0);
                
                BoLib.clearFileAction();

                RebootRequired = false;
                GlobalConfig.RebootRequired = false;
            }

            RaisePropertyChangedEvent("RebootRequired");
            RaisePropertyChangedEvent("TerminalAssetMsg");
            RaisePropertyChangedEvent("TiToEnabled");
            RaisePropertyChangedEvent("TerminalAssetMsg");
        }
        
        public ICommand TitoUpdate { get { return new DelegateCommand(DoTitoUpdate); } }
        private void DoTitoUpdate(object o)
        {
            var titoUpdateForm = new IniSettingsWindow();
            titoUpdateForm.BtnComment.Visibility = System.Windows.Visibility.Hidden;
            titoUpdateForm.BtnComment.IsEnabled = false;
            var showDialog = titoUpdateForm.ShowDialog();
            if (showDialog != null && (bool)!showDialog)
            {
                var assetNumber = titoUpdateForm.TxtNewValue.Text;
                //validate value here and update accordingly.
                //create a keyboard with just a numberpad.
                NativeWinApi.WritePrivateProfileString("Operator", "AssetNo", assetNumber, Properties.Resources.birth_cert);
                TerminalAssetMsg = assetNumber;
                RaisePropertyChangedEvent("TerminalAssetMsg");
            }
        }
        
        public ICommand SetHandPayState { get { return new DelegateCommand(DoSetHandPayState); } }
        void DoSetHandPayState(object o)
        {
            var str = o as string;
            var value = "";

            if (str.Equals("hopper"))
            {
                HandPayStateMsg = "Hopper Only";
                value = "0";
            }
            else if (str.Equals("printer"))
            {
                HandPayStateMsg = "Printer Only";
                value = "1";
            }
            else if (str.Equals("combined"))
            {
                HandPayStateMsg = "Combined";
                value = "2";
            }

            if (!string.IsNullOrEmpty(value))
                NativeWinApi.WritePrivateProfileString("FactoryOnly", "PayoutType", value, Resources.birth_cert);
            
            RaisePropertyChangedEvent("HandPayStateMsg");
        }
    }
} 
