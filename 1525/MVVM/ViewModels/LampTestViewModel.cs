
using System.Threading;
using System.Windows;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class LampTestViewModel : BaseViewModel
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

        Visibility _buttonVisibility;
        public Visibility ButtonVisibility
        {
            get { return _buttonVisibility; }
            set
            {
                _buttonVisibility = value;
                RaisePropertyChangedEvent("ButtonVisibility");
            }
        }

        public LampTestViewModel(string name) : base(name)
        {
            BannerMessage = "Press Start Button";
        }

        void LampLoop()
        {
            for (short i = 128; i > 0; i /= 2)
            {
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
            }
            
            BannerMessage = "Press Start Button";
            ButtonVisibility = Visibility.Visible;
        }

        public System.Windows.Input.ICommand StartLampTest { get { return new DelegateCommand(o => DoLampTest()); } }
        void DoLampTest()
        {
            BannerMessage = "Check button lamps are flashing";
            ButtonVisibility = Visibility.Hidden;
            
            BannerMessage = "Press Start Button";
            ButtonVisibility = Visibility.Visible;
            
            Thread lampThread = new Thread(new ThreadStart(() => LampLoop()));
            lampThread.Start();
        }
    }
}
