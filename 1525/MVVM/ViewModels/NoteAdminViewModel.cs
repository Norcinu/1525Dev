using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils.MVVM.ViewModels
{
    class NoteAdminViewModel : ObservableObject
    {
        bool _isSpanish = false;
        public bool HasRecycler { get; set; }
        public string RecyclerMessage { get; set; }
        public string NoteOne { get; set; }
        public string NoteTwo { get; set; }
        public string RecyclerValue { get; set; }
       
        public NoteAdminViewModel()
        {
            try
            {
                _isSpanish = false;
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
                
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
        
        public ICommand SetRecycleNote { get { return new DelegateCommand(DoSetRecycleNote); } }
        void DoSetRecycleNote(object o)
        {
            var noteType = o as string;
            
            if (BoLib.getBnvType() != 5) return;
            
            var channel = (noteType == "10") ? "2" : "3";
            BoLib.setUtilRequestBitState((int)UtilBits.RecyclerValue);
            BoLib.setUtilRequestBitState((int)UtilBits.RereadBirthCert);
            DoEmptyRecycler();
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
            else
            {
                var msg = new WpfMessageBoxService();
                msg.ShowMessage("Note Recycler is Empty", "Information");
            }
        }
        
        public void Refresh()
        {
            RecyclerValue = BoLib.getRecyclerFloatValue().ToString();
            RaisePropertyChangedEvent("RecyclerValue");
        }
    }
}

