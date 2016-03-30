using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class EmptyMachineViewModel : BaseViewModel
    {
        public bool HasRecycler { get; set; }
        public string RecyclerMessage { get; set; }
        public string NoteOne { get; set; }
        public string NoteTwo { get; set; }
        public string RecyclerValue { get; set; }
        
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
                RecyclerValue = BoLib.getRecyclerFloatValue().ToString();
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
            else
            {
                var msg = new WpfMessageBoxService();
                msg.ShowMessage("Note Recycler is Empty", "Information");
            }
        }

    }
}
