using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Native;
using Timer = System.Timers.Timer;

namespace PDTUtils.MVVM.ViewModels
{
    class EmptyMachineViewModel : BaseViewModel
    {
        public bool HasRecycler { get; set; }
        public uint _recyclerFloat;

        public string RecyclerMessage { get; set; }
        public string NoteOne { get; set; }
        public string NoteTwo { get; set; }
        public string RecyclerValue { get { return (_recyclerFloat / 100).ToString("F2"); } }

        Timer _recyclerFloatTimer;

        HopperViewModel _hopperVM;
        public HopperViewModel HopperVM
        {
            get { return _hopperVM; }
            set
            {
                if (_hopperVM == null)
                    _hopperVM = new HopperViewModel();
                _hopperVM = value;
                RaisePropertyChangedEvent("HopperVM");
            }
        }
        public EmptyMachineViewModel()
            : base("Empty")
        {
            InitNoteRecycler();
            HopperVM = new HopperViewModel();
        }
        
        void InitNoteRecycler()
        {
            try
            {
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

                //Thread.Sleep(200);
                _recyclerFloat = BoLib.getRecyclerFloatValue();
                _recyclerFloatTimer = new Timer() { Enabled = false, Interval = 100 };
                _recyclerFloatTimer.Elapsed += new System.Timers.ElapsedEventHandler(_recyclerFloatTimer_Elapsed);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            RaisePropertyChangedEvent("HasRecycler");
            RaisePropertyChangedEvent("RecyclerMessage");
            RaisePropertyChangedEvent("NoteOne");
            RaisePropertyChangedEvent("NoteTwo");
            RaisePropertyChangedEvent("RecyclerValue");
        }

        void _recyclerFloatTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Refresh();
            if (_recyclerFloat == 0)
                _recyclerFloatTimer.Stop();
        }
        
        public ICommand EmptyRecycler { get { return new DelegateCommand(o => DoEmptyRecycler()); } }
        void DoEmptyRecycler()
        {
            if (RecyclerValue != "0")
            {
                BoLib.setUtilRequestBitState((int)UtilBits.EmptyRecycler);
                PDTUtils.Logic.GlobalConfig.ReparseSettings = true;
                _recyclerFloatTimer.Start();
                /*Thread.Sleep(500);
                _recyclerFloat = 0;*/
                RaisePropertyChangedEvent("RecyclerValue");
            }
            else
            {
                var msg = new WpfMessageBoxService();
                msg.ShowMessage("Note Recycler is Empty", "Information");
            }
        }
        
        public void Refresh()
        {
            _recyclerFloat = BoLib.getRecyclerFloatValue();
            RaisePropertyChangedEvent("RecyclerValue");
        }
    }
}
