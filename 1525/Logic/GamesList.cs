using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using PDTUtils.Native;

namespace PDTUtils.Logic
{
	public class GamesList : INotifyPropertyChanged
	{
		public GamesList()
		{
		}

		ObservableCollection<GamesInfo> _gamesInfo = new ObservableCollection<GamesInfo>();
		public ObservableCollection<GamesInfo> GamesInfo
		{
			get { return _gamesInfo; }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		
        
		public void GetGamesList()
		{
			if (_gamesInfo.Count > 0)
				_gamesInfo.RemoveAll();
            //
			var numGames = BoLib.getNumberOfGames();
			for (var i = 1; i < numGames; i++)
			{
				var g = new GamesInfo();
			    
				var sb = new StringBuilder(500);
				NativeWinApi.GetPrivateProfileString("Game" + (i + 1).ToString(), "Exe", "", sb, sb.Capacity, @"D:\machine\machine.ini");
				g.path = sb.ToString();
				var modelNo = sb.ToString().Substring(0, 4);
				g.name = @"D:\" + modelNo + @"\" + modelNo + ".png";
                
				if (NativeMD5.CheckHash(@"d:\" + modelNo + @"\" + sb))
				{
					var hash = NativeMD5.CalcHashFromFile(@"d:\" + modelNo + @"\" + sb);
					var hex = NativeMD5.HashToHex(hash);
					g.hash_code = hex;
				}
				else
					g.hash_code = "ERROR: NOT AUTHORISED";
			
				g.Name = g.name;
				g.Path = g.path;
				g.HashCode = g.hash_code; 

				_gamesInfo.Add(g);
                
				OnPropertyChanged("Name");
				OnPropertyChanged("Path");
				OnPropertyChanged("Hash_code");
			}
		}
	}
}
