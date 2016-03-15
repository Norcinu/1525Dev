using System.Collections.ObjectModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils.Logic
{
	public class SystemInfo
	{
		public string Field { get; set; }
		public bool IsEditable { get; set; }
		
		public SystemInfo(string name)
		{
			Field = name;
			IsEditable = false;
		}
	}

	/// <summary>
	/// Class to represent the hardware and operating system settings.
	/// </summary>
	public class MachineInfo : ObservableCollection<SystemInfo>
	{
		public MachineInfo()
		{
			//QueryMachine();
		}
		
/*
		public void ProbeUsb()
		{
			
		}
*/

		private string GetMachineIp()
		{
			var address = "IP Address: ";
			foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
			{
			    if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
			        ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
			    
                foreach (var ip in ni.GetIPProperties().UnicastAddresses)
			    {
			        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			        {
			            address += ip.Address.ToString();
			        }
			    }
			}
		    return address;
		}

		private string GetComputerName()
		{
			return "Computer Name: " + System.Environment.MachineName;
		}

		public string GetMemoryInfo()
		{
			var ms = new NativeWinApi.Memorystatus();
			NativeWinApi.GlobalMemoryStatus(ref ms);

			var str = new StringBuilder("Total Physical Memory: " + (ms.DwTotalPhys / 1024) / 1024 + " MB");
			str.Append("\nFree Physical Memory: " + (ms.DwAvailPhys / 1024) / 1024 + " MB");
			str.Append("\nTotal Virtual Memory: " + (ms.DwTotalVirtual / 1024) / 1024 + " MB");
			str.Append("\nFree Virtual Memory: " + (ms.DwAvailVirtual / 1024) / 1024 + " MB");

			return str.ToString();
		}
		
		public string GetScreenResolution()
		{
			const string errorString = "Screen Not Active/Fitted.\n";

			var str = new StringBuilder("Top Screen:\t "); 
			var dm = new NativeWinApi.Devmode();
            
			var result = NativeWinApi.EnumDisplaySettings("\\\\.\\Display2", 
				(int)NativeWinApi.ModeNum.EnuCurrentSettings, ref dm);
			
			if (result)
			{
				str.Append("Resolution: " + dm.dmPelsWidth + "x" + dm.dmPelsHeight + ". ");
				str.Append("BPP: " + dm.dmBitsPerPel + ".\n");
			}
			else
				str.Append(errorString);
			
			str.Append("Bottom Screen:\t "); 
			var dm2 = new NativeWinApi.Devmode();
			result = NativeWinApi.EnumDisplaySettings("\\\\.\\Display1", 
				(int)NativeWinApi.ModeNum.EnuCurrentSettings, ref dm2);
			
			if (result == true)
			{
				str.Append("Resolution: " + dm2.dmPelsWidth + "x" + dm2.dmPelsHeight + ". ");//\n
				str.Append("BPP: " + dm2.dmBitsPerPel);
			}
			else
				str.Append(errorString);
			
			return str.ToString();
		}

		public string GetMachineSerial()
		{
			return BoLib.getSerialNumber();
		}

		public string GetCpuId()
		{
			return "CPU-ID: " + BoLib.GetUniquePcbID(0);
		}

		public string GetEdc()
		{
			return BoLib.getEDCTypeStr();
		}

		public string GetCountryCode()
		{
			try
			{
				return BoLib.getCountryCodeStr();
			}
			catch (System.Exception ex)
			{
				return ex.Message;
			}
		}

		public string GetOsVersion()
		{
			var os = new NativeWinApi.Osversioninfo();
			os.DwOsVersionInfoSize = (uint)Marshal.SizeOf(os);
			NativeWinApi.GetVersionEx(ref os);
			return "OS Version:\tWindows XPe - " + os.SzCsdVersion.ToString();
		}

		public string GetLastMd5Check()
		{
			return ReadFileLine(Resources.security_log);
		}
        
		public string GetUpdateKey()
		{
            if (!File.Exists(Resources.update_log))
                return "Update Key Does Not Exist. - Please contact PROJECT.";

			var strs = ReadFileLine(Resources.update_log,1).Split("=".ToCharArray());
			var final = new StringBuilder(strs[1]);
			
			for (var i = 0; i < 68; i++)
			{
				if ((i % 15 == 0) && (i > 0))
					final.Insert(i, "-");
			}

			return "Update Key: " + final;
		}

        string ReadFileLine(string filename, int index = 0)
        {
            var line = "";

            try
            {
                using (var stream = new StreamReader(filename))
                {
                    for (var i = 0; i < index; i++)
                        stream.ReadLine();
                    line = stream.ReadLine();
                }
            }
            catch (System.Exception ex)
            {
                line = ex.Message;
            }
            
            return line;
        }
	}
}
