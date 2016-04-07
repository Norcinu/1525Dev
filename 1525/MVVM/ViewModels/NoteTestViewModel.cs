using System.Threading;
using PDTUtils.Native;
using System;
using System.Diagnostics;

namespace PDTUtils.MVVM.ViewModels
{
    class NoteTestViewModel : BaseViewModel
    {
        bool _running;
        string _bannerMessage;
        string _valueMessage;

        public string ValueMessage
        {
            get { return _valueMessage; }
            set
            {
                _valueMessage = value;
                RaisePropertyChangedEvent("ValueMessage");
            }
        }

        public string BannerMessage
        {
            get { return _bannerMessage; }
            set
            {
                _bannerMessage = value;
                RaisePropertyChangedEvent("BannerMessage");
            }
        }
        
        Thread _noteThread;

        public NoteTestViewModel(string name)
            : base(name)
        {
            _running = false;
            BannerMessage = "Enter Note";
        }

        public void StartThread()
        {
            _running = true;

            BoLib.clearBankCreditReserve();
            BoLib.setUtilRequestBitState((int)UtilBits.NoteTest);
            _noteThread = new Thread(new ThreadStart(DoThreadAction));
            _noteThread.Start();
        }

        void DoThreadAction()
        {
            while (_running)
            {
                try
                {
                    var value = BoLib.getBankCreditsReservePtr();
                    if (value > 0)
                    {
                        BoLib.clearBankCreditReserve();
                        ValueMessage = (value / 100).ToString("f2");
                    }
                    Thread.Sleep(100);
                }
                catch(Exception e)
                {
                    _running = false;
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public void Cleanup()
        {
            _running = false;
            try
            {
                if (_noteThread != null) _noteThread.Abort();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            Thread.Sleep(100);
            BoLib.clearUtilRequestBitState((int)UtilBits.NoteTest);
        }
    }
}
