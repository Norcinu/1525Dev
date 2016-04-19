using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class MetersViewModel : BaseViewModel
    {
        readonly NumberFormatInfo _nfi;
        readonly LongTermMeters _longTerm = new LongTermMeters();
        readonly ShortTermMeters _shortTerm = new ShortTermMeters();
        readonly TitoMeters _titoMeters = new TitoMeters();

        readonly ObservableCollection<HelloImJohnnyCashMeters> _cashRecon = new ObservableCollection<HelloImJohnnyCashMeters>();
        readonly ObservableCollection<HelloImJohnnyCashMeters> _performance = new ObservableCollection<HelloImJohnnyCashMeters>();
        readonly ObservableCollection<HelloImJohnnyCashMeters> _refill = new ObservableCollection<HelloImJohnnyCashMeters>();

        readonly ObservableCollection<GameStatMeter> _gameStats = new ObservableCollection<GameStatMeter>();
        
        public int NumberOfGamesLt { get; set; }
        public int NumberOfGamesSt { get; set; }
        
        #region Properties
        public LongTermMeters LongTerm { get { return _longTerm; } }
        public ShortTermMeters ShortTerm { get { return _shortTerm; } }
        public TitoMeters TitoMeter { get { return _titoMeters; } }
        public ObservableCollection<HelloImJohnnyCashMeters> CashRecon { get { return _cashRecon; } }
        public ObservableCollection<HelloImJohnnyCashMeters> Performance { get { return _performance; } }
        public ObservableCollection<HelloImJohnnyCashMeters> Refill { get { return _refill; } }
        public ObservableCollection<GameStatMeter> GameStats { get { return _gameStats; } }
        #endregion

        public MetersViewModel()
            : base("Performance")
        {
            _nfi = new CultureInfo("en-GB").NumberFormat;

            Initialise();
        }

        void Initialise()
        {
            try
            {
                //LCD
                _longTerm.ReadMeter();
                _shortTerm.ReadMeter();
                _titoMeters.ReadMeter();

                NumberOfGamesLt = 0;

                ReadPerformance();
                ReadCashRecon();

                RaisePropertyChangedEvent("LongTerm");
                RaisePropertyChangedEvent("ShortTerm");
                RaisePropertyChangedEvent("TitoMeters");
                RaisePropertyChangedEvent("NumberOfGames");
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        
        public void Refresh()
        {
            _cashRecon.RemoveAll();
            _gameStats.RemoveAll();
            _performance.RemoveAll();
            _refill.RemoveAll();

            Initialise();
        }
        
        public ICommand ClearShortTerms
        {
            get { return new DelegateCommand(o => ClearShortTermMeters()); }
        }
        
        void ClearShortTermMeters()
        {           
            if (BoLib.getCredit() > 0 || BoLib.getBank() > 0 || (int)BoLib.getReserveCredits() > 0)
            {
                var msg = new WpfMessageBoxService();
                msg.ShowMessage("Please Clear Bank, Credits and Reserve Credits before clearing short term meters.", "ERROR");
            }
            else
            {
                Performance.Clear();
                CashRecon.Clear();
                GameStats.Clear();
                Refill.Clear();
       
                BoLib.clearShortTermMeters();

                if (System.IO.File.Exists(Properties.Resources.tito_log))
                {
                    NativeWinApi.WritePrivateProfileString("TicketsIn", "TicketCount", "0", @Properties.Resources.tito_log);
                    NativeWinApi.WritePrivateProfileString("TicketsOut", "TicketCount", "0", @Properties.Resources.tito_log);
                }

                ReadPerformance();
                ReadCashRecon();

                _shortTerm.ReadMeter();

                RaisePropertyChangedEvent("ShortTerm");
                RaisePropertyChangedEvent("LongTerm");
            }
        }
        
        public void ReadCashRecon()
        {
            var noteHeaders = new[] { "£50 NOTES", "£20 NOTES", "£10 NOTES", "£5 NOTES" };
            // ReSharper disable once UnusedVariable
            //var cashHeaders = new[] { "TOTAL CASH IN", "TOTAL CASH OUT", "TOTAL" };
            var coinHeaders = new[] { "£2 COINS", "£1 COINS", "50p COINS", "20p COINS", "10p COINS", "5p COINS" };

            _cashRecon.Add(new HelloImJohnnyCashMeters("STAKE IN",
                                                       ((int)BoLib.useStakeInMeter(0)).ToString(),
                                                       ((int)BoLib.useStakeInMeter(1)).ToString()));

            var perfLt = new byte[] { 2, 3, 4, 5 };
            var perfSt = new byte[] { 34, 35, 36, 37 };
            //var perfSt = new byte[] { 30, 31, 32, 33 };
            var ctr = 0;
            
            for (var i = 2; i <= 5; i++)
            {
                _cashRecon.Add(new HelloImJohnnyCashMeters(noteHeaders[i - 2],
                                                           BoLib.getReconciliationMeter(perfLt[ctr]).ToString(),
                                                           BoLib.getReconciliationMeter(perfSt[ctr]).ToString()));
                ctr++;
            }
            
            // corresponding to recon meter #defines in the bo lib. - TODO!!! Refactor this in some way so they are not hard coded.
            var perfCoinLt = new byte[] { 8, 9, 10, 11, 12 };
            var perfCoinSt = new byte[] {40, 41, 42, 43, 44};
            ctr = 0;
            for (var i = 8; i <= 12; i++)
            {
                _cashRecon.Add(new HelloImJohnnyCashMeters(coinHeaders[i - 8],
                                                           BoLib.getReconciliationMeter(perfCoinLt[ctr]).ToString(),
                                                           BoLib.getReconciliationMeter(perfCoinSt[ctr]).ToString()));
                ctr++;
            }
            _cashRecon.Add(new HelloImJohnnyCashMeters("TiTo", BoLib.getReconciliationMeter((byte)ELongTermMeters.TitoIn).ToString(),
                                                        BoLib.getReconciliationMeter((byte)EShortTermMeters.TitoIn).ToString()));

            _cashRecon.Add(new HelloImJohnnyCashMeters("Cash Match", BoLib.getReconciliationMeter((byte)ELongTermMeters.CMatchIn).ToString(),
                                                        BoLib.getReconciliationMeter((byte)EShortTermMeters.CMatchIn).ToString()));

            _cashRecon.Add(new HelloImJohnnyCashMeters("Points", BoLib.getReconciliationMeter((byte)ELongTermMeters.PointsIn).ToString(),
                                                        BoLib.getReconciliationMeter((byte)EShortTermMeters.PointsIn).ToString()));
          
            
            _refill.Add(new HelloImJohnnyCashMeters("£1 Coins",
                                                    BoLib.getReconciliationMeter((byte)ELongTermMeters.RefillL).ToString(),
                                                    BoLib.getReconciliationMeter((byte)EShortTermMeters.RefillL).ToString()));

            _refill.Add(new HelloImJohnnyCashMeters("10p Coins",
                                                    BoLib.getReconciliationMeter((byte)ELongTermMeters.RefillR).ToString(),
                                                    BoLib.getReconciliationMeter((byte)EShortTermMeters.RefillR).ToString()));


            RaisePropertyChangedEvent("CashRecon");
            RaisePropertyChangedEvent("Refill");
        }

        // TODO: Tidy this up. There is multiple calls to the same function/enum type here. Just get the value once.
        void ReadPerformance()
        {
            var longTermCashIn = (int)BoLib.getPerformanceMeter((byte)Native.Performance.MoneyInLt) / 100.0M;
            var longTermCashOut = (int)BoLib.getPerformanceMeter((byte)Native.Performance.MoneyOutLt) / 100.0M;
            var longTermTotal = longTermCashIn - longTermCashOut;
            
            var shortTermCashIn = (int)BoLib.getPerformanceMeter((byte)Native.Performance.MoneyInSt) / 100.0M;
            var shortTermCashOut = (int)BoLib.getPerformanceMeter((byte)Native.Performance.MoneyOutSt) / 100.0M;
            var shortTermTotal = shortTermCashIn - shortTermCashOut;
            
            var handPayLt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.HandPayLt) / 100.0M;
            var handPaySt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.HandPaySt) / 100.0M;
            
            Performance.Add(new HelloImJohnnyCashMeters("Total Money in:", 
                                                        longTermCashIn.ToString("C", _nfi), 
                                                        shortTermCashIn.ToString("C", _nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Total Money Out:", 
                                                        longTermCashOut.ToString("C", _nfi), 
                                                        shortTermCashOut.ToString("C", _nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Gross Income:", 
                                                        longTermTotal.ToString("C", _nfi), 
                                                        shortTermTotal.ToString("C", _nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Hand Pay:", 
                                                        handPayLt.ToString("C", _nfi), 
                                                        handPaySt.ToString("C", _nfi)));

            var incomeLt = longTermTotal - handPayLt; 
            var incomeSt = shortTermTotal - handPaySt;
            Performance.Add(new HelloImJohnnyCashMeters("Net Income:", incomeLt.ToString("C", _nfi), incomeSt.ToString("C", _nfi)));

            decimal refillSt = BoLib.getReconciliationMeter((byte)EShortTermMeters.RefillL) + 
                               BoLib.getReconciliationMeter((byte)EShortTermMeters.RefillR);
            
            decimal refillLt = BoLib.getReconciliationMeter((byte)ELongTermMeters.RefillL) + 
                               BoLib.getReconciliationMeter((byte)ELongTermMeters.RefillR);
            
            Performance.Add(new HelloImJohnnyCashMeters("Refill:", refillLt.ToString("C", _nfi), refillSt.ToString("C", _nfi)));
            
            double totalBetsLt = 0;
            double totalBetsSt = 0;
            double totalWonLt = 0;
            double totalWonSt = 0;

            totalBetsLt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.WageredLt);
            totalBetsSt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.WageredSt);
            totalWonLt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.WonLt);
            totalWonSt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.WonSt);
            
            totalBetsLt /= 100;
            totalBetsSt /= 100;
            totalWonLt  /= 100;
            totalWonSt  /= 100;
            
            var percentageLt = (totalWonLt > 0 && totalBetsLt > 0) ? (totalWonLt / totalBetsLt) : 0;
            var percentageSt = (totalWonSt > 0 && totalBetsSt > 0) ? (totalWonSt / totalBetsSt) : 0;

            decimal retainedPercLt = (int)BoLib.getPerformanceMeter(0) / 100.00M;
            decimal retainedPercSt = (int)BoLib.getPerformanceMeter(7) / 100.00M;

            var min_lt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.MoneyInLt);
            var mout_lt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.MoneyOutLt);
            var hpa_lt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.HandPayLt);
            if (retainedPercLt > 0)
                retainedPercLt = (((retainedPercLt * 100) - (mout_lt + hpa_lt)) / min_lt);

            var min_st = (int)BoLib.getPerformanceMeter((byte)Native.Performance.MoneyInSt);
            var mout_st = (int)BoLib.getPerformanceMeter((byte)Native.Performance.MoneyOutSt);
            var hpa_st = (int)BoLib.getPerformanceMeter((byte)Native.Performance.HandPaySt);
            
            if (retainedPercSt > 0)
                retainedPercSt = (((retainedPercSt * 100) - (mout_st + hpa_st)) / min_st);
            
            Performance.Add(new HelloImJohnnyCashMeters("Total Bet:", 
                                                        totalBetsLt.ToString("C", _nfi),
                                                        totalBetsSt.ToString("C", _nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Total Wins:",
                                                        totalWonLt.ToString("C", _nfi),
                                                        totalWonSt.ToString("C", _nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Payout Percentage:",
                                                        percentageLt.ToString("P"),
                                                        percentageSt.ToString("P")));
            Performance.Add(new HelloImJohnnyCashMeters("Retained Percentage:",
                                                        retainedPercLt.ToString("P"),
                                                        retainedPercSt.ToString("P")));

            NumberOfGamesLt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.NoGamesLt);
            NumberOfGamesSt = (int)BoLib.getPerformanceMeter((byte)Native.Performance.NoGamesSt);
            Performance.Add(new HelloImJohnnyCashMeters("Number of Games: ", NumberOfGamesLt.ToString(), NumberOfGamesSt.ToString()));
                        
            //Read Game Stats meters
            for (uint i = 0; i <= BoLib.getNumberOfGames(); i++) //shell as well as games
            {
                var model = BoLib.getGameModel((int)i);
                var bets = (uint)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWageredLt) / 100.0M;
                var won = (int)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWonLt) / 100.0M;
                var percentage = (won > 0 || bets > 0) ? (won / bets) : 0;
                GameStats.Add(new GameStatMeter(model.ToString(), bets.ToString("C", _nfi), won.ToString("C", _nfi),
                                                System.Math.Round(percentage, 2).ToString("P", _nfi)));
            }
            
            RaisePropertyChangedEvent("CashRecon");
            RaisePropertyChangedEvent("NumberOfGamesLt");
            RaisePropertyChangedEvent("NumberOfGamesSt");
            RaisePropertyChangedEvent("GameStats");
        }
    }
}
