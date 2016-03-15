using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using PDTUtils.Native;

namespace PDTUtils.Logic
{
    static class IniFileUtility
    {
        public static bool GetIniProfileSection(out string[] section, string field, string file, bool removeField=false)
        {
            const uint bufferSize = 4096; //4048
            var retStringPtr = Marshal.AllocCoTaskMem((int)bufferSize * sizeof(char));
            var bytesReturned = NativeWinApi.GetPrivateProfileSection(field, retStringPtr, bufferSize, @file);
            if ((bytesReturned == bufferSize - 2) || (bytesReturned == 0))
            {
                section = null;
                Marshal.FreeCoTaskMem(retStringPtr);
                return false;
            }
            
            var retString = Marshal.PtrToStringAuto(retStringPtr, bytesReturned - 1);
            if (!removeField)
                section = retString.Split('\0');
            else
            {
                section = retString.Split('\0');
                for (var i = 0; i < section.Length; i++)
                {
                    if (section[i].Length > 4 )
                        section[i] = section[i].Substring(section[i].IndexOf("=")+1);
                }
            }
            
            Marshal.FreeCoTaskMem(retStringPtr);
            return true;
        }
        
        public static bool WriteIniProfileSection(string[] section, string field, string file)
        {
            return true;
        }
        
        public static void HashFile(string filename)
        {
            try
            {
                // delete garbage after [End] section.
                var lines = new List<string>(File.ReadAllLines(filename));
                var afterEnd = false;
                
                for (var i = 0; i < lines.Count; i++)
                {
                    if (lines[i] == "[End]")
                        afterEnd = true;
                    if (lines[i] != "[End]" && afterEnd)
                        lines.RemoveAt(i);
                }
 
                File.WriteAllLines(filename, lines.ToArray());
                var retries = 10;
                if (NativeMD5.CheckFileType(filename) && !NativeMD5.CheckHash(filename))
                {
                    //make sure file in not read-only
                    if (NativeWinApi.SetFileAttributes(filename, NativeWinApi.FileAttributeNormal))
                    {
                        do
                        {
                            NativeMD5.AddHashToFile(filename);
                        } while (!NativeMD5.CheckHash(filename) && retries-- > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
