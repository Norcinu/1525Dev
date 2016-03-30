using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDTUtils.Native;
using System.Threading;

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
                var value = BoLib.getBankCreditsReservePtr();
                if (value > 0)
                {
                    BoLib.clearBankCreditReserve();
                    ValueMessage = (value / 100).ToString("f2");
                }
                Thread.Sleep(100);
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
            BoLib.clearUtilRequestBitState((int)UtilBits.CoinTest);
        }
    }
}
