using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.Native;
using PDTUtils.Properties;
 
namespace PDTUtils.MVVM.ViewModels
{
    //View model to configure hoppers and note recycler
    class ConfigureViewModel : BaseViewModel
    {
        bool _hasTwoHoppers;
        bool _needToSync = false;
        bool _syncLeft = false;
        bool _syncRight = false;
        string _refloatLeft;
        string _refloatRight;

        public string DivertLeftMessage { get; set; }
        public string DivertRightMessage { get; set; }

        public bool HasRecycler { get; set; }
        public string RecyclerMessage { get; set; }
        public string NoteOne { get; set; }
        public string NoteTwo { get; set; }
        public string RecyclerValue { get; set; }        
        
        WpfMessageBoxService _msg = new WpfMessageBoxService();

        Visibility _isLeftVisible;
        public Visibility IsLeftVisible
        {
            get { return _isLeftVisible; }
            set
            {
                _isLeftVisible = value;
                RaisePropertyChangedEvent("IsLeftVisible");
            }
        }

        Visibility _isRightVisible;
        public Visibility IsRightVisible
        {
            get { return _isRightVisible; }
            set
            {
                _isRightVisible = value;
                RaisePropertyChangedEvent("IsRightVisible");
            }
        }

        public string RefloatRight
        {
            get { return _refloatRight; }
            set
            {
                _refloatRight = value;
                RaisePropertyChangedEvent("RefloatRight");
            }
        }

        public string RefloatLeft
        {
            get { return _refloatLeft; }
            set
            {
                _refloatLeft = value;
                RaisePropertyChangedEvent("RefloatLeft");
            }
        }

        public bool NeedToSync
        {
            get { return _needToSync; }
            set
            {
                _needToSync = value;
                RaisePropertyChangedEvent("NeedToSync");
            }
        }
        
        public ConfigureViewModel(string name)
            : base(name)
        {
            DivertLeftMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Left).ToString();
            DivertRightMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Right).ToString();

            char[] buffer = new char[3];
            NativeWinApi.GetPrivateProfileString("FactoryOnly", "NumberOfHoppers", "", buffer, 3, Properties.Resources.birth_cert);
            if (buffer[0] == '0')
                return;

            _hasTwoHoppers = (buffer[0] == '1') ? false : true;
            var hopperCount = Convert.ToUInt32(new string(buffer, 0, buffer.Length));

            RefloatLeft = "";
            RefloatRight = "";

            IsLeftVisible = Visibility.Visible;
            IsRightVisible = _hasTwoHoppers ? Visibility.Visible : Visibility.Collapsed;

            //Note recycler
            if (BoLib.getBnvType() == 5)
            {
                HasRecycler = true;
                if (BoLib.getRecyclerChannel() == 3)
                    RecyclerMessage = "£20 NOTE TO BE RECYCLED";
                else
                    RecyclerMessage = "£10 NOTE TO BE  RECYCLED";
            }
            else
            {
                HasRecycler = false;
                RecyclerMessage = "NO RECYCLER";
            }

            NoteOne = "£10";
            NoteTwo = "£20";

            Thread.Sleep(2000);
            RecyclerValue = BoLib.getRecyclerFloatValue().ToString();
        }
        
        public ICommand ChangeLeftDivert { get { return new DelegateCommand(DoChangeDivert); } }
        void DoChangeDivert(object o)
        {
            const uint changeAmount = 10;
            var actionType = o as string;
            char[] divert = new char[10];
            NativeWinApi.GetPrivateProfileString("Operator", "LH Divert Threshold", "", divert, 10, Resources.birth_cert);
            var currentThreshold = Convert.ToUInt32(new string(divert));//BoLib.getHopperDivertLevel(0);
            var newValue = currentThreshold;

            if (actionType == "increase")
            {
                newValue += changeAmount;
            }
            else if (actionType == "decrease" && currentThreshold > 200)
            {
                newValue -= changeAmount;
                if (newValue < 200)
                    newValue = 0;
            }
            
            GlobalConfig.ReparseSettings = true;
            NativeWinApi.WritePrivateProfileString("Operator", "LH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            BoLib.setHopperDivertLevel((byte)Hoppers.Left, newValue);

            DivertLeftMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertLeftMessage");
        }
        
        public ICommand ChangeRightDivert { get { return new DelegateCommand(DoChangeDivertRight); } }
        void DoChangeDivertRight(object o)
        {
            var actionType = o as string;
            //var currentThreshold = BoLib.getHopperDivertLevel((byte)Hoppers.Right);
            char[] divert = new char[10];
            NativeWinApi.GetPrivateProfileString("Operator", "RH Divert Threshold", "", divert, 10, Resources.birth_cert);
            var currentThreshold = Convert.ToUInt32(new string(divert));
            const uint changeAmount = 10;
            var newValue = currentThreshold;

            if (actionType == "increase")
            {
                newValue += changeAmount;
            }
            else if (actionType == "decrease" && currentThreshold > 50)
            {
                newValue -= changeAmount;
                if (newValue < 50)
                    newValue = 50;
            }


            //BoLib.setHopperDivertLevel(BoLib.getRightHopper(), newValue);
            GlobalConfig.ReparseSettings = true;
            NativeWinApi.WritePrivateProfileString("Operator", "RH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            //PDTUtils.Logic.IniFileUtility.HashFile(Resources.birth_cert);
            BoLib.setHopperDivertLevel((byte)Hoppers.Right, newValue);

            DivertRightMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertRightMessage");
        }
        
        public ICommand LoadDefaults { get { return new DelegateCommand(o => DoLoadDefaults()); } }
        void DoLoadDefaults()
        {
            uint[] refloatDefaults = new uint[2] { 600, 50 };
            uint[] divertDefaults = new uint[2] { 800, 250 };

            BoLib.setHopperFloatLevel((byte)Hoppers.Left, refloatDefaults[0]);
            BoLib.setHopperFloatLevel((byte)Hoppers.Right, refloatDefaults[1]);

            RefloatLeft = refloatDefaults[0].ToString();
            RefloatRight = refloatDefaults[1].ToString();

            BoLib.setHopperDivertLevel((byte)Hoppers.Left, divertDefaults[0]);
            BoLib.setHopperDivertLevel((byte)Hoppers.Right, divertDefaults[1]);

            DivertLeftMessage = divertDefaults[0].ToString();
            DivertRightMessage = divertDefaults[1].ToString();

            NeedToSync = false;
            _syncLeft = false;
            _syncRight = false;

            NativeWinApi.WritePrivateProfileString("Operator", "RefloatLH", RefloatLeft, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "RefloatRH", RefloatRight, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "LH Divert Threshold", DivertLeftMessage, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Operator", "RH Divert Threshold", DivertRightMessage, Resources.birth_cert);

            RaisePropertyChangedEvent("DivertLeftMessage");
            RaisePropertyChangedEvent("DivertRightMessage");
        }

        public ICommand SetRecycleNote { get { return new DelegateCommand(DoSetRecycleNote); } }
        void DoSetRecycleNote(object o)
        {
            var noteType = o as string;

            if (BoLib.getBnvType() != 5) return;

            var channel = (noteType == "10") ? "2" : "3";
            BoLib.setUtilRequestBitState((int)UtilBits.ChangeRnv);
            //BoLib.setUtilRequestBitState((int)UtilBits.RereadBirthCert);
            GlobalConfig.ReparseSettings = true;
            //DoEmptyRecycler();
            NativeWinApi.WritePrivateProfileString("Operator", "RecyclerChannel", channel, Resources.birth_cert);
            RecyclerMessage = (noteType == "10") ? NoteOne + " NOTE TO BE RECYCLED" : NoteTwo + " NOTE TO BE RECYCLED";
            RaisePropertyChangedEvent("RecyclerMessage");
        }
        
        public ICommand EmptyRecycler { get { return new DelegateCommand(o => DoEmptyRecycler()); } }
        void DoEmptyRecycler()
        {
            if (RecyclerValue != "0")
            {
                BoLib.setUtilRequestBitState((int)UtilBits.EmptyRecycler);
                Thread.Sleep(500);
                RecyclerValue = "0";
                RaisePropertyChangedEvent("RecyclerValue");
            }
        }
        
        public void Refresh()
        {
            RecyclerValue = BoLib.getRecyclerFloatValue().ToString();
            RaisePropertyChangedEvent("RecyclerValue");
        }
        
        public void Cleanup()
        {
            
        }
    }
}
