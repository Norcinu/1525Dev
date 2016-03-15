using System.IO;
using System.Security.Cryptography;

namespace PDTUtils
{
	static class FileHashing
	{
		public static string GetFileHash(string filename)
		{
			var result = "";
			
			try
			{
				var stream = File.Open(filename, FileMode.Open);
				var md5 = new MD5CryptoServiceProvider();
				var byteHashValue = md5.ComputeHash(stream);
				stream.Close();
				
				var hashData = System.BitConverter.ToString(byteHashValue);
				hashData = hashData.Replace("-", "");
				result = hashData;
			}
			catch (System.Exception ex)
			{
				result = "Could not find hash of filename: " + filename + @"\n" + ex.Message;
			}

			return (result);
		}
	}
}

