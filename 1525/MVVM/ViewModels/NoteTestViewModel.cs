using System.Threading;
using PDTUtils.Native;

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
            /*if (!BoLib.getUtilDoorAccess()) 
            {
                MessageBox.Show("Please turn refill key before continuing.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                if (!BoLib.getUtilDoorAccess())
                {
                    _running = false;
                    return;
                }
            */
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
                System.Diagnostics.Debug.WriteLine("NOTE THREAD");
                var value = BoLib.getBankCreditsReservePtr();//BoLib.getCredit() + BoLib.getBank() + (int)BoLib.getReserveCredits();
                if (value <= 0)
                {
                    ValueMessage = "";
                    //return;
                }
                else
                {
                    BoLib.clearBankCreditReserve();
                    ValueMessage = (value / 100).ToString("f2");
                }
                Thread.Sleep(100);
            }
        }
        
        public void Cleanup()
        {
            _running = false;
            BoLib.clearUtilRequestBitState((int)UtilBits.NoteTest);
        }
    }
}
