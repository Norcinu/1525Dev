using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.Impls
{
	public class Impl
	{
		bool _isRunning;
		string _name = "";

		#region Properties
		public bool IsRunning { get; set; }
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		#endregion

		public Impl()
		{
			this._isRunning = false;
			this._name = "";
		}
	}
    
	public class ButtonTestImpl : Impl
    {
        #region Properties
        public bool _doSpecials;
		public bool DoSpecials
		{
			get { return _doSpecials; }
			set { _doSpecials = value; }
		}
		public bool[] _toggled;
		public bool[] Toggled
		{
			get { return _toggled; }
			set { _toggled = value; }
		}
		public int _currentButton;
		public int CurrentButton
		{
			get { return _currentButton; }
			set { _currentButton = value; }
		}
		public int _numberOfButtons;
		public int NumberOfButtons
		{
			get { return _numberOfButtons; }
			set { _numberOfButtons = value; }
		}
		public int _specials;
		public int Specials
		{
			get { return _specials; }
			set { _specials = value; }
		}
		public int _currentSpecial;
		public int CurrentSpecial
		{
			get { return _currentSpecial; }
			set { _currentSpecial = value; }
		}
        #endregion
        
        public ButtonTestImpl()
		{
			_doSpecials = true;
            _toggled = new bool[2] { false, false };
			_currentButton = 0;
			_numberOfButtons = 8;
			_specials = 2;
			_currentSpecial = 0;
		}
	}
    
	public class CoinNoteValImpl : Impl
	{
		public bool IsCoinTest;

		public CoinNoteValImpl()
		{
			IsCoinTest = false;
		}

		public CoinNoteValImpl(bool coinTest) 
		{
			IsCoinTest = coinTest;
		}
	}
    
	public class HopperImpl : Impl
	{
		public bool _dumpSwitchPressed = false;
		public bool DumpSwitchPressed
		{
			get { return _dumpSwitchPressed; }
			set { _dumpSwitchPressed = value; }
		}
		public HopperImpl() : base() { }
	}

	public class LampTestImpl
	{
		public LampTestImpl()
		{

		}
	}
}
