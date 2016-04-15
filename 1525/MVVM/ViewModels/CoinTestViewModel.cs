using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDTUtils.Native;
using System.Threading;
using System.Diagnostics;

namespace PDTUtils.MVVM.ViewModels
{
    class CoinTestViewModel : BaseViewModel
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

        Thread _coinThread;

        public CoinTestViewModel(string name)
            : base(name)
        {
            BannerMessage = "Please insert coin";
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
                catch (Exception e)
                {
                    _running = false;
                    Debug.WriteLine(e.Message);
                }
            }
        }
        
        public void StartThread()
        {
            _running = true;

            BoLib.clearBankCreditReserve();
            BoLib.setUtilRequestBitState((int)UtilBits.CoinTest);
            _coinThread = new Thread(new ThreadStart(DoThreadAction));
            _coinThread.Start();
        }

        public override void Cleanup()
        {
            _running = false;
            try
            {
                if (_coinThread != null) _coinThread.Abort();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            Thread.Sleep(50);
            BoLib.clearUtilRequestBitState((int)UtilBits.CoinTest);
        }
    }
}
