using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace PDTUtils
{
    /// <summary>
    /// This class commits any changes that are made to shared memory.
    /// </summary>
	public static class DiskCommit
	{
        /// <summary>
        /// Runs the EWFMGR command which commits changes to shared memory.
        /// This must always be called when changes are made and the machine must
        /// be rebooted.
        /// </summary>
        public static void Save()
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C EWFMGR C: -COMMIT";
            process.StartInfo = startInfo;
            process.Start();
        }
        
        /// <summary>
        /// Forces the machine to reboot.
        /// </summary>
		public static void RebootMachine()
		{
			var W32_OS = new ManagementClass("Win32_OperatingSystem");
            W32_OS.Scope.Options.EnablePrivileges = true;
            
			foreach(var o in W32_OS.GetInstances())
			{
			    var obj = (ManagementObject) o;
			    var inParams = obj.GetMethodParameters("Win32Shutdown");
                inParams["Flags"] = 6; // ForcedReboot;
				inParams["Reserved"] = 0;
                
				var outParams = obj.InvokeMethod("Win32Shutdown", inParams, null);
				var result = Convert.ToInt32(outParams["returnValue"]);
				if (result != 0)
					throw new Win32Exception(result);
			}
		}
        
        public static void SaveAndReboot()
        {
            var t = new Thread(() => Save());
            t.Start();
            Thread.Sleep(2000);
            RebootMachine();
        }
	}
}
