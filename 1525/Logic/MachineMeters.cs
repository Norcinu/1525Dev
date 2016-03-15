using System.Collections.ObjectModel;
using System.ComponentModel;
using PDTUtils.Logic;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils
{
    public class MeterDescription
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public MeterDescription() { }
        public MeterDescription(string k, string v)
        {
            this.Key = k;
            this.Value = v;
        }
    }
    
	abstract public class MachineMeters : INotifyPropertyChanged
	{
		public ObservableCollection<MeterDescription> _meterDesc = new ObservableCollection<MeterDescription>();
        public ObservableCollection<MeterDescription> MeterDesc { get { return _meterDesc; } }
        
		public MachineMeters()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
        
		abstract public void ReadMeter();
	}

    public class ShortTermMeters : MachineMeters
    {
        public ShortTermMeters()
        {
        }

        public override void ReadMeter()
        {
            if (_meterDesc.Count > 0)
                _meterDesc.RemoveAll();

            _meterDesc.Add(new MeterDescription("Cash In", BoLib.getCashIn(BoLib.useMoneyInType(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Cash Out", BoLib.getCashOut(BoLib.useMoneyOutType(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Notes In", BoLib.getNotesIn(BoLib.useMoneyInType(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Notes Out", BoLib.getNotesOut(BoLib.useMoneyOutType(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Refill", BoLib.getRefillValue(BoLib.useRefillType(0)).ToString()));

            uint won = 0;
            uint bet = 0;

            bet += (uint)BoLib.getPerformanceMeter((byte)PDTUtils.Native.Performance.WageredSt);
            won += (uint)BoLib.getPerformanceMeter((byte)PDTUtils.Native.Performance.WonSt);

            _meterDesc.Add(new MeterDescription("Bet", bet.ToString()));
            _meterDesc.Add(new MeterDescription("Win", won.ToString()));

            _meterDesc.Add(new MeterDescription("Hand Pay", BoLib.getPerformanceMeter((byte)Performance.HandPaySt).ToString()));
            _meterDesc.Add(new MeterDescription("Ticket In", BoLib.getReconciliationMeter((byte)EShortTermMeters.TitoIn).ToString()));
            _meterDesc.Add(new MeterDescription("Ticket Out", BoLib.getReconciliationMeter((byte)EShortTermMeters.TicketOut).ToString()));

            _meterDesc.Add(new MeterDescription("Reward Points IN", BoLib.getReconciliationMeter((byte)EShortTermMeters.PointsIn).ToString()));
            _meterDesc.Add(new MeterDescription("Cash Match IN", BoLib.getReconciliationMeter((byte)EShortTermMeters.CMatchIn).ToString()));

            this.OnPropertyChanged("ShortTerm");
        }
    }
    
    public class LongTermMeters : MachineMeters
    {
        public LongTermMeters()
        {
        }
        
        public override void ReadMeter()
        {
            if (_meterDesc.Count > 0)
                _meterDesc.RemoveAll();

            _meterDesc.Add(new MeterDescription("Cash In", BoLib.getCashIn(BoLib.useMoneyInType(1)).ToString()));
            _meterDesc.Add(new MeterDescription("Cash Out", BoLib.getCashOut(BoLib.useMoneyOutType(1)).ToString()));
            _meterDesc.Add(new MeterDescription("Notes In", BoLib.getNotesIn(BoLib.useMoneyInType(1)).ToString()));
            _meterDesc.Add(new MeterDescription("Notes Out", BoLib.getNotesOut(BoLib.useMoneyOutType(1)).ToString()));
            _meterDesc.Add(new MeterDescription("Refill", BoLib.getRefillValue(BoLib.useRefillType(1)).ToString()));
            
            uint won = 0;
            uint bet = 0;

            /*for (uint i = 1; i <= BoLib.getNumberOfGames(); i++)
            {*/
            
                bet += (uint)BoLib.getPerformanceMeter((byte)PDTUtils.Native.Performance.WageredLt);//BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWageredLt);
                won += (uint)BoLib.getPerformanceMeter((byte)PDTUtils.Native.Performance.WonLt);//BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWonLt);
            //}
            
            _meterDesc.Add(new MeterDescription("Bet", bet.ToString()));
            _meterDesc.Add(new MeterDescription("Win", won.ToString()));
            
            //_meterDesc.Add(new MeterDescription("Fischas Bet", BoLib.getVtp(BoLib.useVtpMeter(1)).ToString()));
            //_meterDesc.Add(new MeterDescription("Fischas Win", BoLib.getWon(BoLib.useWonMeter(1)).ToString()));
            _meterDesc.Add(new MeterDescription("Hand Pay", BoLib.getPerformanceMeter((byte)Performance.HandPayLt).ToString())); //BoLib.getHandPay(BoLib.useHandPayMeter(1)).ToString()));
            _meterDesc.Add(new MeterDescription("Ticket In", BoLib.getReconciliationMeter((byte)ELongTermMeters.TitoIn).ToString()));
            _meterDesc.Add(new MeterDescription("Ticket Out", BoLib.getReconciliationMeter((byte)ELongTermMeters.TicketOut).ToString())); //BoLib.getTicketsPay(BoLib.useTicketsMeter(1)).ToString()));

            //_meterDesc.Add(new MeterDescription("Cash Match IN", BoLib.getReconciliationMeter((byte)ELongTermMeters.CMatchIn).ToString()));
            _meterDesc.Add(new MeterDescription("Reward Points IN", BoLib.getReconciliationMeter((byte)ELongTermMeters.PointsIn).ToString()));
            _meterDesc.Add(new MeterDescription("Cash Match IN", BoLib.getReconciliationMeter((byte)ELongTermMeters.CMatchIn).ToString()));

/*
            _meterDesc.Add(new MeterDescription("Cash Match £5", BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn1Lt).ToString()));
            _meterDesc.Add(new MeterDescription("Cash Match £10", BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn2Lt).ToString()));*/
            /*_meterDesc.Add(new MeterDescription("Cash Match £20", BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn3Lt).ToString()));

            _meterDesc.Add(new MeterDescription("Reward points IN", BoLib.getRewardPointMeter((byte)RewardMeters.RewardCardInLt).ToString()));
            _meterDesc.Add(new MeterDescription("Reward points OUT", BoLib.getRewardPointMeter((byte)RewardMeters.RewardOutLt).ToString()));
            _meterDesc.Add(new MeterDescription("Reward CARD IN", BoLib.getRewardPointMeter((byte)RewardMeters.RewardCardInLt).ToString()));*/

            this.OnPropertyChanged("LongTerm");
        }
    }

    public class TitoMeters : MachineMeters
    {
        public ObservableCollection<MeterDescription> TitoOut { get; set; }
        public TitoMeters()
        {
            TitoOut = new ObservableCollection<MeterDescription>();
        }
        
        public override void ReadMeter()
        {
            string[] ticketsIn;
            string[] ticketsOut;
            
            if (System.IO.File.Exists(Properties.Resources.tito_log))
            {
                if (_meterDesc.Count > 0)
                    _meterDesc.Clear();
                if (TitoOut.Count > 0)
                    TitoOut.Clear();

                IniFileUtility.GetIniProfileSection(out ticketsIn, "TicketsIn", @Resources.tito_log);
                IniFileUtility.GetIniProfileSection(out ticketsOut, "TicketsOut", @Resources.tito_log);

                var ti = ticketsIn[0].Split("=".ToCharArray());
                var to = ticketsOut[0].Split("=".ToCharArray());

                foreach (var t in ticketsIn)
                {
                    if (!t.StartsWith("TicketsInPtr") && !t.StartsWith("TicketCount"))
                        _meterDesc.Add(new MeterDescription("TicketIn", t.Split("=".ToCharArray())[1]));
                }

                foreach (var tt in ticketsOut)
                {
                    if (!tt.StartsWith("TicketsOutPtr") && !tt.StartsWith("TicketCount"))
                        TitoOut.Add(new MeterDescription("TicketOut", tt.Split("=".ToCharArray())[1]));
                }
                /*_meterDesc.Add(new MeterDescription("TicketIn", ti[1]));
                _meterDesc.Add(new MeterDescription("TicketOut", to[1]));*/
                
                //TitoOut.Add(new MeterDescription("TicketOut", to[1]));
            }
            else
            {
                string def = "0";
                _meterDesc.Add(new MeterDescription("TicketIn", def));
                _meterDesc.Add(new MeterDescription("TicketOut", def));
            }
            
            OnPropertyChanged("TitoMeter");
        }
    }
}

