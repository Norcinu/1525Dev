using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using PDTUtils.Native;

//make the machine ini editable only for the factory only card.
namespace PDTUtils.Logic
{
	public class IniElement
	{
		private string _category;
		private string _field;
		private string _value;

		#region Properties
		public string Category
		{
			get { return _category; }
			set { _category = value; }
		}

		public string Field
		{
			get { return _field; }
			set { _field = value; }
		}
		
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}
		#endregion
		
		public IniElement(string category, string field, string value)
		{
			_category = category;
			_field = field;
			_value = value;
		}
	}
    
	//  need to check for duplicates, remove etc...
	public class UniqueIniCategory : ObservableCollection<IniElement>
	{ 
		private List<string> _uniqueEntries = new List<string>();
		public UniqueIniCategory()
		{
		}
	}
    
    /// <summary>
	/// Represents the machine ini of the cabinet.
	/// </summary>
	public class MachineIni : ObservableCollection<IniElement>
	{
        const string IniPath = "D:\\machine\\machine.ini";
        const string EndOfIni = "[END]";
        const string BackUpFile = IniPath + "_backup";

        public bool ChangesPending { get; set; }

        readonly List<IniElement> _models = new List<IniElement>();
        string _firstLine = "";

		public MachineIni()
		{
			ParseIni();
		}
        
        void RemoveBackupFile()
        {
            if (File.Exists(BackUpFile))
            {
                try
                {
                	File.Delete(BackUpFile);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
        
        /// <summary>
        /// Read Machine and parse accordingly.
        /// *** Refactor this to use the native INI functions ***
        /// </summary>
        /// <returns></returns>
        void ParseIni()
        {
            try
            {
                using (var fs = File.Open(IniPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                    string line;
                    try
                    {
                        if (File.Exists(BackUpFile))
                            RemoveBackupFile();
                        File.Copy(IniPath, BackUpFile);

                        var first = new char[10];
                        sr.Read(first, 0, 7);
                        _firstLine = new string(first).Trim('\0');
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Equals(EndOfIni))
                            break;

                        if (!line.StartsWith("[") || LineContainsCategories(line) || line.Equals("")) continue;
                        var category = line.Trim("[]".ToCharArray());

                        string[] str;
                        IniFileUtility.GetIniProfileSection(out str, category, IniPath);
                        if (str == null) continue;

                        foreach (var val in str)
                        {
                            if (!val.Contains("=")) continue;
                            var options = val.Split("=".ToCharArray());
                            Add(new IniElement(category, options[0], options[1]));
                        }
                    }
                }

                /* NOT SURE THIS IS USED ANYMORE!?
                string[] models;
                IniFileUtility.GetIniProfileSection(out models, "Models", IniPath);
                foreach (var m in models)
                {
                    _models.Add(new IniElement("Models", m, ""));
                }
                 */
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        
        static bool LineContainsCategories(string line)
        {
            return (line.Contains("Game")   == true || line.Contains("Select")  == true ||
                    line.Contains("Models") == true || line.Contains("Standby") == true);
        }
        
        //I think in categories that are linked, I should just unset all of them at the same time.
        //I mean the ones that are commented out.
        public void WriteMachineIni(string category, string field)
        {
            if (category == null)
                WriteMachineIni();
            else
            {
                var found = false;
                for (var i = 0; i < Items.Count && !found; i++)
                {
                    if (Items[i].Category == category && Items[i].Field == field)
                    {
                        var text = File.ReadAllText(IniPath);
                        text = Regex.Replace(text, "#" + Items[i].Field, Items[i].Field);
                        File.WriteAllText(IniPath, text);
                        NativeWinApi.WritePrivateProfileString(category, Items[i].Field, 
                                                               Items[i].Value, IniPath);
                        found = !found;
                    }
                }
                HashMachineIni();
            }
        }
        
        public void WriteMachineIni()
        {
            var divider = "Models";
            using (var w = File.CreateText(IniPath))
            {
                w.WriteLine(_firstLine);
                w.WriteLine("[" + divider + "]");
                foreach (var m in _models)
                    w.WriteLine(m.Field);
                w.Flush();
            }
            
            foreach (var item in Items)
            {
                if (item.Category != divider)
                {
                    divider = item.Category;
                    using (var w = File.AppendText(IniPath))
                    {
                        w.WriteLine("\r\n");
                        w.Flush();
                    }
                }
                NativeWinApi.WritePrivateProfileString(item.Category, item.Field, item.Value, IniPath);
            }
            
            HashMachineIni();
            GlobalConfig.RebootRequired = true;
        }
        
		public void HashMachineIni()
		{
			var retries = 10;
			if (NativeMD5.CheckFileType(IniPath) == true)
			{
				if (NativeMD5.CheckHash(IniPath) != true)
				{
					if (NativeWinApi.SetFileAttributes(IniPath, NativeWinApi.FileAttributeNormal))
					{   
						NativeWinApi.WritePrivateProfileSection("End", null, IniPath);
						NativeWinApi.WritePrivateProfileSection("End", "", IniPath);
                        
						do
						{
                            NativeMD5.AddHashToFile(IniPath);
						} while (!NativeMD5.CheckHash(IniPath) && retries-- > 0);
					}
				}
			}
		}
	}
}
