using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PDTUtils.Native
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	static class Games
	{
		public class GameInfo
		{
			string name = "";
			string _hashCode = "";
			string path = "";

			public string Name { get { return name; } }
			public string HashCode { get { return _hashCode; } }
			public string Path { get { return path; } }
		}
	}

#pragma warning disable 0169, 0649
    [System.Security.SuppressUnmanagedCodeSecurity]
	static class NativeWinApi
	{
		public enum ModeNum : int
		{
			EnuCurrentSettings = -1,
			EnuRegistrySettings = -2
		}
        
		public struct Memorystatus
		{
			public uint DwLength;
			public uint DwMemoryLoad;
			public uint DwTotalPhys;
			public uint DwAvailPhys;
			public uint DwTotalPageFile;
			public uint DwAvailPageFile;
			public uint DwTotalVirtual;
			public uint DwAvailVirtual;
		}

		[DllImport("kernel32.dll")]
		public static extern void GlobalMemoryStatus(ref Memorystatus lpBuffer);

		[StructLayout(LayoutKind.Sequential)]
		public struct Pointl
		{
			public int x;
			public int y;
		}

		internal enum Dmcolor : short
		{
			DmcolorUnknown = 0,
			DmcolorMonochrome = 1,
			DmcolorColor = 2
		}

		[Flags()]
		public enum Dm : int
		{
			Orientation = 0x1,
			PaperSize = 0x2,
			PaperLength = 0x4,
			PaperWidth = 0x8,
			Scale = 0x10,
			Position = 0x20,
			Nup = 0x40,
			DisplayOrientation = 0x80,
			Copies = 0x100,
			DefaultSource = 0x200,
			PrintQuality = 0x400,
			Color = 0x800,
			Duplex = 0x1000,
			YResolution = 0x2000,
			TtOption = 0x4000,
			Collate = 0x8000,
			FormName = 0x10000,
			LogPixels = 0x20000,
			BitsPerPixel = 0x40000,
			PelsWidth = 0x80000,
			PelsHeight = 0x100000,
			DisplayFlags = 0x200000,
			DisplayFrequency = 0x400000,
			IcmMethod = 0x800000,
			IcmIntent = 0x1000000,
			MediaType = 0x2000000,
			DitherType = 0x4000000,
			PanningWidth = 0x8000000,
			PanningHeight = 0x10000000,
			DisplayFixedOutput = 0x20000000
		}

        public enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified,
            ComputerNameMax
        }

		/// <summary>
		/// Specifies whether collation should be used when printing multiple copies.
		/// </summary>
		internal enum Dmcollate : short
		{
			/// <summary>
			/// Do not collate when printing multiple copies.
			/// </summary>
			DmcollateFalse = 0,

			/// <summary>
			/// Collate when printing multiple copies.
			/// </summary>
			DmcollateTrue = 1
		}

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
		public struct Devmode
		{
			public const int Cchdevicename = 32;
			public const int Cchformname = 32;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Cchdevicename)]
			[FieldOffset(0)]
			public string dmDeviceName;
			[FieldOffset(32)]
			public Int16 dmSpecVersion;
			[FieldOffset(34)]
			public Int16 dmDriverVersion;
			[FieldOffset(36)]
			public Int16 dmSize;
			[FieldOffset(38)]
			public Int16 dmDriverExtra;
			[FieldOffset(40)]
			public Dm dmFields;

			[FieldOffset(44)]
			Int16 dmOrientation;
			[FieldOffset(46)]
			Int16 dmPaperSize;
			[FieldOffset(48)]
			Int16 dmPaperLength;
			[FieldOffset(50)]
			Int16 dmPaperWidth;
			[FieldOffset(52)]
			Int16 dmScale;
			[FieldOffset(54)]
			Int16 dmCopies;
			[FieldOffset(56)]
			Int16 dmDefaultSource;
			[FieldOffset(58)]
			Int16 dmPrintQuality;
            
			[FieldOffset(44)]
			public Pointl dmPosition;
			[FieldOffset(52)]
			public Int32 dmDisplayOrientation;
			[FieldOffset(56)]
			public Int32 dmDisplayFixedOutput;

			[FieldOffset(60)]
			public short dmColor; // See note below!
			[FieldOffset(62)]
			public short dmDuplex; // See note below!
			[FieldOffset(64)]
			public short dmYResolution;
			[FieldOffset(66)]
			public short dmTTOption;
			[FieldOffset(68)]
			public short dmCollate; // See note below!
			[FieldOffset(72)]
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Cchformname)]
			public string dmFormName;
			[FieldOffset(102)]
			public Int16 dmLogPixels;
			[FieldOffset(104)]
			public Int32 dmBitsPerPel;
			[FieldOffset(108)]
			public Int32 dmPelsWidth;
			[FieldOffset(112)]
			public Int32 dmPelsHeight;
			[FieldOffset(116)]
			public Int32 dmDisplayFlags;
			[FieldOffset(116)]
			public Int32 dmNup;
			[FieldOffset(120)]
			public Int32 dmDisplayFrequency;
		}
        
		[DllImport("user32.dll")]
		public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref Devmode devMode);

		[DllImport("user32.dll")]
		public static extern int ChangeDisplaySettings(ref Devmode devMode, int flags);
        
		public const int EnuCurrentSettings = -1;
		public const int CdsUpdateregistry = 0x01;
		public const int CdsTest = 0x02;
		public const int DispChangeSuccessful = 0;
		public const int DispChangeRestart = 1;
		public const int DispChangeFailed = -1;
        
		public struct Osversioninfo
		{
			public uint DwOsVersionInfoSize;
			public uint DwMajorVersion;
			public uint DwMinorVersion;
			public uint DwBuildNumber;
			public uint DwPlatformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string SzCsdVersion;
			public Int16 WServicePackMajor;
			public Int16 WServicePackMinor;
			public Int16 WSuiteMask;
			public Byte WProductType;
			public Byte WReserved;
		}
        
        [DllImport("kernel32")]
        public static extern bool GetVersionEx(ref Osversioninfo osvi);
        

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        //public static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer,
        //                                                       uint nSize,
        //                                                       string lpFileName);

        public static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer,
                                                               int nSize,
                                                               string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetPrivateProfileString(string lpAppName,
                                                          string lpKeyName,
                                                          string lpDefault,
                                                          StringBuilder lpReturnedString,
                                                          int nSize,
                                                          string lpFileName);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetPrivateProfileString(string lpAppName,
                                                          string lpKeyName,
                                                          string lpDefault,
                                                          [In, Out] char[] lpReturnedString,
                                                          int nSize,
                                                          string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileString(string lpAppName,
                                                         string lpKeyName,
                                                         string lpDefault,
                                                         IntPtr lpReturnedString,
                                                         uint nSize,
                                                         string lpFileName);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileInt(string lpAppName,
                                                      string lpKeyName,
                                                      int lpDefault,
                                                      string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileSection(string lpAppName,
                                                          IntPtr lpReturnedString,
                                                          uint nSize,
                                                          string lpFileName);

        // We explicitly enable the SetLastError attribute here because
        // WritePrivateProfileString returns errors via SetLastError.
        // Failure to set this can result in errors being lost during 
        // the marshal back to managed code.
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WritePrivateProfileSection(string lpAppName,
                                                             string lpString,
                                                             string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WritePrivateProfileString(string lpAppName,
                                                            string lpKeyName,
                                                            string lpString,
                                                            string lpFileName);
        
        [DllImport("kernel32.dll")]
        public static extern bool SetFileAttributes(string lpFileName,
                                                    uint dwFileAttributes);

        [DllImport("kernel32.dll")]
        public static extern bool WritePrivateProfileStruct(string lpszSection, 
                                                            string lpszKey, 
                                                            IntPtr lpStruct, 
                                                            uint uSizeStruct, 
                                                            string szFile);
        
		public const int FileAttributeNormal = 0x80;
        
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern bool GetComputerName(StringBuilder lpBuffer, ref uint lpnSize);
        
        [DllImport("Kernel32", CharSet = CharSet.Ansi)]
        public static extern unsafe bool GetComputerName(byte* lpBuffer, long* nSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern bool GetComputerNameEx(COMPUTER_NAME_FORMAT NameType, StringBuilder lpBuffer, ref uint lpnSize);

        [DllImport("kernel32.dll")]
        public static extern bool SetComputerName(string lpComputerName);

        [DllImport("kernel32.dll")]
        public static extern bool SetComputerNameEx(COMPUTER_NAME_FORMAT NameType, string lpBuffer);
        
        public struct Systemtime
        {
            public short Year;
            public short Month;
            public short DayOfWeek;
            public short Day;
            public short Hour;
            public short Minute;
            public short Second;
            public short Milliseconds;
        }
        
        [DllImport("coredll.dll")]
        private extern static void GetSystemTime(ref Systemtime lpSystemTime);

        [DllImport("coredll.dll")]
        private extern static uint SetSystemTime(ref Systemtime lpSystemTime);

        [DllImport("kernel32.dll")]
        public static extern bool MoveFile(string lpExistingFileName, string lpNewFileName);

        [DllImport("kernel32.dll")]
        public static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);
	}
}
