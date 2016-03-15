using System.Runtime.InteropServices;

namespace PDTUtils.Native
{
	static class NativeMD5
	{
#if DEBUG
		const string dllName = "BoLibDllD.dll";
#else
        const string dllName = "BoLibDll.dll";
#endif
		
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string AddHashToFile(string filename);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string CalcHashFromFile(string filename);
		
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern bool CheckHash(string filename);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern bool CheckFileType(string filename);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string HashToHex(string hash);
	}
}
