using System;
using System.Collections.Generic;
using System.Linq;
using PDTUtils.Native;


namespace PDTUtils.MVVM.ViewModels
{
    class GameChartViewModel : ObservableObject
    {
        public class KeepOnGiving
        {
            /*public uint Money { get; set; }*/
            public double Money { get; set; }
            public uint GameCount { get; set; }
        }
        
        /*public List<KeyValuePair<string, uint>> IncomingsSimple { get; set; }
        public List<KeyValuePair<string, uint>> OutgoingsSimple { get; set; }*/

        public List<KeyValuePair<string, double>> IncomingsSimple { get; set; }
        public List<KeyValuePair<string, double>> OutgoingsSimple { get; set; }

        public List<KeyValuePair<string, KeepOnGiving>> Incomings { get; set; }
        public List<KeyValuePair<string, KeepOnGiving>> Outgoings { get; set; }

        string _manifest = Properties.Resources.machine_ini;
        
        public GameChartViewModel()
        {
            try
            {
                Incomings = new List<KeyValuePair<string, KeepOnGiving>>();
                Outgoings = new List<KeyValuePair<string, KeepOnGiving>>();

                /*IncomingsSimple = new List<KeyValuePair<string, uint>>();
                OutgoingsSimple = new List<KeyValuePair<string, uint>>();*/

                IncomingsSimple = new List<KeyValuePair<string, double>>();
                OutgoingsSimple = new List<KeyValuePair<string, double>>();
                
                var buffer = new char[3];
                NativeWinApi.GetPrivateProfileString("Terminal", "NumberGames", "", buffer, buffer.Length, _manifest);
                var gameCount = Convert.ToUInt32(new string(buffer)) + 1;
                for (var i = 1; i < gameCount; i++)
                {
                    var modelNo = BoLib.getGameModel(i);
                    var bet = (uint)BoLib.getGamePerformanceMeter((uint)i, 0);
                    var won = (uint)BoLib.getGamePerformanceMeter((uint)i, 1);

                    var titleBuffer = new char[64];
                    var name = NativeWinApi.GetPrivateProfileString("Game" + i, "Title", "", titleBuffer, titleBuffer.Length, _manifest);

                    var count = (uint)BoLib.getGamePerformanceMeter((uint)i, 2);
                    var title = new string(titleBuffer).Trim("\0".ToCharArray());

                    Incomings.Add(new KeyValuePair<string, KeepOnGiving>(title, new KeepOnGiving() { Money = bet / 100.00, GameCount = count }));
                    Outgoings.Add(new KeyValuePair<string, KeepOnGiving>(title, new KeepOnGiving() { Money = won / 100.00, GameCount = count }));

                    IncomingsSimple.Add(new KeyValuePair<string, double>(title, bet));
                    OutgoingsSimple.Add(new KeyValuePair<string, double>(title, won));
                }
                
                Incomings.Sort(CompareValue); 
                Outgoings.Sort(CompareValue);
            }
            catch (Exception e)
            {
                //new WpfMessageBoxService().ShowMessage(e.Message, "Loading Error");
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            RaisePropertyChangedEvent("Incomings");
            RaisePropertyChangedEvent("Outgoings");

            RaisePropertyChangedEvent("IncomingsSimple");
            RaisePropertyChangedEvent("OutgoingsSimple");
        }
        
        static int CompareTitle(KeyValuePair<string, KeepOnGiving> left, KeyValuePair<string, KeepOnGiving> right)
        {
            return left.Key.CompareTo(right.Key); // for ascending sort.
        }
        
        static int CompareValue(KeyValuePair<string, KeepOnGiving> left, KeyValuePair<string, KeepOnGiving> right)
        {
            return right.Value.Money.CompareTo(left.Value.Money); // for descending sort.
        }
    }   
}
