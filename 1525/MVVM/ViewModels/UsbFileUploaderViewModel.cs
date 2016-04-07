using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using PDTUtils.Native;
using System.Diagnostics;

namespace PDTUtils.MVVM.ViewModels
{
    class UsbFileUploaderViewModel : BaseViewModel
    {
        System.Windows.Visibility _isVisible = System.Windows.Visibility.Hidden;
        int _selectedIndex = -1;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChangedEvent("SelectedIndex");
            }
        }
               
        public ObservableCollection<string> FilePath { get; set; }
        public System.Windows.Visibility IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChangedEvent("IsVisible");
            }
        }
        
        public UsbFileUploaderViewModel()
        {
            FilePath = new ObservableCollection<string>();
        }

        public ICommand Select
        {
            get { return new DelegateCommand(o => DoSelectItem()); }
        }

        void DoSelectItem()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "DAT Files (*.dat)|*.dat|Ini Files (*.ini)|*.ini|PDT Files (*.pdt)|*.pdt",
                InitialDirectory = @"d:\",
                Multiselect = true        
            };
            
            var result = ofd.ShowDialog();
            if (result != null && result != false)
            {
                if (ofd.FileNames.Length > 0)
                {
                    foreach (var str in ofd.FileNames)
                    {
                        FilePath.Add(str);
                    }

                    IsVisible = System.Windows.Visibility.Visible;
                }
            }
        }
        
        public ICommand RemoveSelection { get { return new DelegateCommand(o => DoRemoveSelection()); } }
        void DoRemoveSelection()
        {
            if (SelectedIndex > -1)
            {
                FilePath.RemoveAt(_selectedIndex);
                SelectedIndex--;
            }
        }

        
        public ICommand Copy { get { return new DelegateCommand(o => DoCopy()); } }
        void DoCopy()
        {
            bool result = false;
            var drives = DriveInfo.GetDrives();
            foreach (var d in drives)
            {
                if (d.Name[0] > 'D' && d.DriveType == DriveType.Removable)
                {
                    result = true;
                    break;
                }
            }

            if (result)
            {
                try
                {
                    var machineSerial = BoLib.getSerialNumber();
                    var fullPath = @"E:\" + machineSerial;
                    var tempToday = DateTime.Now.ToShortDateString();
                    var today = tempToday.Replace("/", "_");
                    var copyDir = fullPath + @"\" + today;

                    if (!Directory.Exists(copyDir))
                        Directory.CreateDirectory(copyDir);

                    var machine = copyDir + @"\machine";
                    if (!Directory.Exists(machine))
                        Directory.CreateDirectory(machine);

                    var gameData = /*copyDir +*/ @"\machine\GAME_DATA\";
                    if (!Directory.Exists(@"E:\" + gameData))
                        Directory.CreateDirectory(gameData);

                    foreach (var str in FilePath)
                    {
                        if (str.EndsWith(".dat"))
                        {
                            var nameNoPath = str.Substring(str.LastIndexOf("\\"));
                            File.Copy(str, copyDir + gameData + nameNoPath, true);
                        }
                        else if (str.EndsWith(".ini"))
                        {
                            var nameNoPath = str.Substring(str.LastIndexOf("\\"));
                            File.Copy(str, machine + nameNoPath, true);
                        }
                        else if (str.EndsWith(".pdt"))
                        {
                            var lastIndex = str.LastIndexOf("\\");
                            var firstIndex = str.IndexOf("\\", 0) + 1;
                            var gameFolder = str.Replace("D:", "E:").
                                                                    Remove(lastIndex + 1).
                                                                    Insert(firstIndex, machineSerial + @"\" + today + @"\");

                            if (!Directory.Exists(gameFolder))
                                Directory.CreateDirectory(gameFolder);

                            var nameNoPath = str.Substring(lastIndex);
                            File.Copy(str, gameFolder + nameNoPath, true);
                        }
                    }

                    /* Create 7za archive and hash with MD5 code.
                     ProcessStartInfo startInfo = new ProcessStartInfo(@"D:\1525\extras\7za.exe");
                     startInfo.WindowStyle = ProcessWindowStyle.Normal; //ProcessWindowStyle.Hidden;
                     startInfo.WorkingDirectory=@
                     Process p = Process.Start(startInfo, @"a archive2.zip .\subdir\*");
                     p.WaitForExit();

                     var exc = p.ExitCode;*/

                    var msg = new WpfMessageBoxService().ShowMessage("Files Copied Successfully. Please remove USB stick.", "Files Copied");
                }
                catch (Exception e)
                {
                    var msg = new WpfMessageBoxService().ShowMessage(e.Message, "Error");
                }
                
                if (FilePath.Count > 0)
                    FilePath.RemoveAll();
            }
            else
            {
                var msg = new WpfMessageBoxService().ShowMessage("No USB stick found, please insert one and try again.", "Error");
            }
        }
    }
}
//Hi, I am Steven from the article. I wasnt paid a penny. I was offered substantial sums of money, infact more than I've ever had 
//in my life but I requested that any fees were paid intro