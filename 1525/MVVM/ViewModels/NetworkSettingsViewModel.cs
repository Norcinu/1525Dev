using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    internal class NetworkSettingsViewModel : BaseViewModel
    {
        public NetworkSettingsViewModel(string name)
            : base(name)
        {
            ChangesMade = false;

            IpAddressActive = false;
            SubnetActive = false;
            DefaultActive = false;
            SubnetActive = false;
            DefaultComputerName = false;

            IpAddress = "";
            SubnetAddress = "";
            DefaultGateway = "";
            ComputerName = "";

            PingOne = "";
            PingTwo = "* Sending PING (Google DNS) #1 *";
            PingTestRunning = false;

            PopulateInfo();
        }

        public bool ChangesMade { get; set; }
        public bool IpAddressActive { get; set; }
        public bool SubnetActive { get; set; }
        public bool DefaultActive { get; set; }
        public bool DefaultComputerName { get; set; }
        public bool PingTestRunning { get; set; }

        public string ConnectionName { get; set; }
        public string IpAddress { get; set; }
        public string SubnetAddress { get; set; }
        public string DefaultGateway { get; set; }
        public string ComputerName { get; set; }
        public string MacAddress { get; set; }
        public string PingOne { get; set; }
        public string PingTwo { get; set; }

        public ICommand PingSites
        {
            get { return new DelegateCommand(DoPingSites); }
        }

        public ICommand ToggleIp
        {
            get { return new DelegateCommand(o => DoToggleIp()); }
        }

        public ICommand ToggleSubnet
        {
            get { return new DelegateCommand(o => DoToggleSubnet()); }
        }

        public ICommand ToggleDefault
        {
            get { return new DelegateCommand(o => DoToggleDefault()); }
        }

        public ICommand SaveNetworkInfo
        {
            get { return new DelegateCommand(o => DoSaveNetworkInfo()); }
        }

        public ICommand ToggleName
        {
            get { return new DelegateCommand(o => DoToggleName()); }
        }
        
        void PopulateInfo()
        {
            try
            {
                //IP Address
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    //TODO: Handle 2 network cards.
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                            IpAddress = ip.Address.ToString();
                            SubnetAddress = ip.IPv4Mask.ToString();
                            DefaultGateway = ni.GetIPProperties().GatewayAddresses[0].Address.ToString();
                            ConnectionName = ni.Name;
                        }
                    }

                    if (ni.OperationalStatus == OperationalStatus.Up && MacAddress == null)
                    {
                        MacAddress += ni.GetPhysicalAddress().ToString();
                    }
                }
                ComputerName = Environment.MachineName;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
            RaisePropertyChangedEvent("IPAddressActive");
            RaisePropertyChangedEvent("SubnetActive");
            RaisePropertyChangedEvent("DefaultActive");
            RaisePropertyChangedEvent("DefaultComputerName");

            RaisePropertyChangedEvent("IPAddress");
            RaisePropertyChangedEvent("ComputerName");
            RaisePropertyChangedEvent("SubnetAddress");
            RaisePropertyChangedEvent("DefaultGateway");
            RaisePropertyChangedEvent("MacAddress");
            RaisePropertyChangedEvent("PingTestRunning");
        }

        public void DoPingSites(object o)
        {
            var t = new Thread(() => _DoPingSite(o));
            t.Start();
        }

        void _DoPingSite(object o)
        {
            try
            {
                var indexer = o as int?;
                var index = indexer ?? 0;
                // ping google dns. Add more - non google sources?
                var addies = new IPAddress[3]
                {
                    IPAddress.Parse("8.8.8.8"), // Google 1
                    IPAddress.Parse("8.8.4.4"), // Google 2
                    IPAddress.Parse("169.254.1.1") // Internal Back Office
                };

                if (!BoLib.isBackOfficeAvilable())
                {
                    if (index == 0 && PingOne.Length > 0)
                    {
                        PingTwo = "* Sending PING (Google DNS) #1 *";
                        PingOne = "";
                        RaisePropertyChangedEvent("PingOne");
                        RaisePropertyChangedEvent("PingTwo");
                    }
                    else if (index == 1 && PingOne.Length > 0)
                    {
                        PingTwo = "* Sending PING (Google DNS) #2 *";
                        RaisePropertyChangedEvent("PingTwo");
                    }
                }
                else
                {
                    PingTwo = "* Sending PING to  Back Office *";
                    PingOne = "";
                    RaisePropertyChangedEvent("PingOne");
                    RaisePropertyChangedEvent("PingTwo");
                    index = 2;
                }
                
                PingTestRunning = true;
                RaisePropertyChangedEvent("PingTestRunning");

                var pinger = new Ping();
                var reply = pinger.Send(addies[index]);

                if (reply.Status == IPStatus.Success)
                {
                    PingOne += "Ping to " + addies[index] + " OK - " + reply.Status + "\n\n";
                    RaisePropertyChangedEvent("PingOne");
                }
                else
                {
                    switch (index)
                    {
                        case 1:
                            PingOne += "\n\n";
                            PingTwo = "*** Internet Ping Test Completed ***";
                            RaisePropertyChangedEvent("PingTwo");
                            break;
                        case 2:
                            PingOne += "\n\n";
                            PingTwo = "*** Back Office Ping Test Completed ***";
                            RaisePropertyChangedEvent("PingTwo");
                            break;
                    }

                    Debug.WriteLine("Host Not Reached {0}", "[" + addies[index] + "]");
                    PingOne += "Ping to " + addies[index] + " FAILED - " + reply.Status;
                    RaisePropertyChangedEvent("PingOne");

                    if (index == 0)
                        DoPingSites(index + 1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                PingOne = ex.Message;
                RaisePropertyChangedEvent("PingOne");
            }

            RaisePropertyChangedEvent("PingTestRunning");
        }

        void DoToggleIp()
        {
            ChangesMade = true;
            IpAddressActive = !IpAddressActive;
            RaisePropertyChangedEvent("IPAddressActive");
            RaisePropertyChangedEvent("ChangesMade");
        }
        
        void DoToggleSubnet()
        {
            ChangesMade = true;
            SubnetActive = !SubnetActive;
            RaisePropertyChangedEvent("SubnetActive");
            RaisePropertyChangedEvent("ChangesMade");
        }
        
        void DoToggleDefault()
        {
            ChangesMade = true;
            DefaultActive = !DefaultActive;
            RaisePropertyChangedEvent("DefaultActive");
            RaisePropertyChangedEvent("ChangesMade");
        }
        
        void DoToggleName()
        {
            ChangesMade = true;
            DefaultComputerName = !DefaultComputerName;
            RaisePropertyChangedEvent("DefaultComputerName");
            RaisePropertyChangedEvent("ChangesMade");
        }
        
        void DoSaveNetworkInfo()
        {
            var objMc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var objMoc = objMc.GetInstances();
            
            if (!ChangesMade) return;

            NativeWinApi.SetComputerNameEx(NativeWinApi.COMPUTER_NAME_FORMAT.ComputerNamePhysicalDnsHostname, ComputerName);

            foreach (var o in objMoc)
            {
                var objMo = (ManagementObject)o;
                if (!(bool)objMo["IPEnabled"]) continue;

                try
                {
                    /*using (var newIp = objMo.GetMethodParameters("EnableStatic"))
                    {
                        // ReSharper disable once UnusedVariable
                        // var newGateway = objMo.GetMethodParameters("SetGateways");
                        newIp["IPAddress"] = new[] { IpAddress };
                        newIp["SubnetMask"] = new[] { SubnetAddress };
                    }*/

                    var parameters = new System.Text.StringBuilder();
                    parameters.Append(" interface ip set address local static");
                    parameters.Append(" addr=").Append(IpAddress);
                    parameters.Append(" mask=").Append(SubnetAddress);
                    parameters.Append(" gateway=").Append(DefaultGateway);
                    parameters.Append(" gwmetric=").Append("1");

                    Process p = new Process();
                    p.StartInfo.FileName = "netsh";
                    p.StartInfo.Arguments = parameters.ToString();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    
                    try
                    {
                        p.Start();
                        p.WaitForExit();
                        //p.WaitForExit(30000);

                        var infoString = p.StandardOutput.ReadToEnd();
                        System.Diagnostics.Debug.WriteLine(infoString);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            ChangesMade = false;
            PDTUtils.Logic.GlobalConfig.RebootRequired = true;
            DiskCommit.Save();
        }
    }
}

