using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDTUtils.Native;
using System.Threading;

namespace PDTUtils.MVVM.ViewModels
{
    class PrinterTestViewModel : BaseViewModel
    {
        string _bannerMessage;
        public string BannerMessage
        {
            get { return _bannerMessage; }
            set
            {
                _bannerMessage = value;
                RaisePropertyChangedEvent("BannerMessage");
            }
        }
        
        public PrinterTestViewModel(string name)
            : base(name)
        {
            BannerMessage = "Printing Ticket";
            DoPrinterTest();
        }

        //public ICommand StartTest { get { new DelegateCommand(o => DoPrinterTest); } }
        void DoPrinterTest()
        {
            char[] ret = new char[3];
            NativeWinApi.GetPrivateProfileString("FactoryOnly", "PayoutType", "", ret, 3, @Properties.Resources.birth_cert);
            if (ret[0] != '0')
            {
                var _testPrintThread = new Thread(new ThreadStart(BoLib.printTestTicket));
                _testPrintThread.Start();
            }
            else
            {
                BannerMessage = "Printer Not Installed or Not Configured";
            }
        }


    }
}
