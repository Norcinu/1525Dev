using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using PDTUtils.Properties;

namespace PDTUtils.Logic
{
	public class ServiceEnabler : INotifyPropertyChanged
	{
		Dictionary<string, bool> _categories = new Dictionary<string, bool>();

		#region Properties
        public bool Audit
        {
            get { return _categories[Categories.Audit]; }
        }

		public bool GameStatistics
		{
			get { return _categories[Categories.GameStatistics]; }
		}
        
		public bool MachineIni
		{
			get { return _categories[Categories.MachineIni]; }
		}

		public bool Volume
		{
			get { return _categories[Categories.Volume]; }
		}

		public bool Meters
		{
			get { return _categories[Categories.Meters]; }
		}

		public bool Performance
		{
			get { return _categories[Categories.Performance]; }
		}

		public bool System
		{
			get { return _categories[Categories.System]; }
		}

		public bool Setup
		{
			get { return _categories[Categories.Setup]; }
		}

		public bool Logfile
		{
			get { return _categories[Categories.Logfile]; }
		}

        public bool MainPage
        {
            get { return _categories[Categories.MainPage]; }
        }

		#endregion

		public ServiceEnabler()
		{
			_categories.Add(Categories.GameStatistics, false);
			this.OnPropertyChanged(Categories.GameStatistics);
			_categories.Add(Categories.MachineIni, false);
			this.OnPropertyChanged(Categories.MachineIni);
			_categories.Add(Categories.Volume, false);
			this.OnPropertyChanged(Categories.Volume);
			_categories.Add(Categories.Meters, false);
			this.OnPropertyChanged(Categories.Meters);
			_categories.Add(Categories.Performance, false);
			this.OnPropertyChanged(Categories.Performance);
			_categories.Add(Categories.System, false);
			this.OnPropertyChanged(Categories.System);
			_categories.Add(Categories.Setup, false);
			this.OnPropertyChanged(Categories.Setup);
			_categories.Add(Categories.Logfile, false);
			this.OnPropertyChanged(Categories.Logfile);
            _categories.Add(Categories.MainPage, true);
            this.OnPropertyChanged(Categories.MainPage);
            _categories.Add(Categories.Audit, false);
            this.OnPropertyChanged(Categories.MainPage);
            OnPropertyChanged("Enabler");
		}
        
		#region Property Changed
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
        #endregion

        /// <summary>
		/// Sets the named category to active, sets everything else to inactive
		/// </summary>
		/// <param name="name">Name of the key to enable</param>
		public void EnableCategory(string name)
		{
			if (_categories.Keys.Count == 0 || name == "")
				return;
			ClearAll();
			_categories[name] = true;
			this.OnPropertyChanged(name);
            OnPropertyChanged("Enabler");
		}
        
		public void ClearAll()
		{
            var keys = new List<string>(_categories.Keys);
			foreach (var key in keys)
			{
				_categories[key] = false;
				this.OnPropertyChanged(key);
			}
		}
	}
}
