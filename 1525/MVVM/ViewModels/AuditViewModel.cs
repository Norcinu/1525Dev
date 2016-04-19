using System;
using System.Collections.ObjectModel;
using System.Text;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class AuditViewModel : BaseViewModel //ObservableObject
    {
        int _demoCountLt = 0;
        int _demoCountSt = 0;

        enum CashMatchValues { Five = 0, Ten, Twenty, TOTAL };
        
        int[] _totalCashMatchSt = new int[(int)CashMatchValues.TOTAL];
        int[] _totalCashMatchLt = new int[(int)CashMatchValues.TOTAL];

        ObservableCollection<DemoEvent> _demoCardLog = new ObservableCollection<DemoEvent>();
        ObservableCollection<CashMatchAudit> _cashMatchAudit = new ObservableCollection<CashMatchAudit>();
        ObservableCollection<CashMatch> _cashMatchLog = new ObservableCollection<CashMatch>();
        ObservableCollection<LoyaltyAudit> _loyaltyAudit = new ObservableCollection<LoyaltyAudit>();
        ObservableCollection<LoyaltyLogField> _loyaltyLog = new ObservableCollection<LoyaltyLogField>();
        
        #region PROPERTIES
        public int DemoCountLt
        {
            get { return _demoCountLt; }
            set
            {
                _demoCountLt = value;
                RaisePropertyChangedEvent("DemoCountLt");
            }
        }
        
        public int DemoCountSt
        {
            get { return _demoCountSt; }
            set
            {
                _demoCountSt = value;
                RaisePropertyChangedEvent("DemoCountSt");
            }
        }

        public int FivePoundSakeMatchCountLt
        {
            get { return _totalCashMatchLt[(int)CashMatchValues.Five]; }
            set
            {
                _totalCashMatchLt[(int)CashMatchValues.Five] = value;
                RaisePropertyChangedEvent("FivePoundSakeMatchCountLt");
            }
        }

        public int FivePoundSakeMatchCountSt
        {
            get { return _totalCashMatchSt[(int)CashMatchValues.Five]; }
            set
            {
                _totalCashMatchSt[(int)CashMatchValues.Five] = value;
                RaisePropertyChangedEvent("FivePoundSakeMatchCountSt");
            }
        }
        
        public int TenPoundSakeMatchCountSt
        {
            get { return _totalCashMatchSt[(int)CashMatchValues.Ten]; }
            set
            {
                _totalCashMatchSt[(int)CashMatchValues.Ten] = value;
                RaisePropertyChangedEvent("TenPoundSakeMatchCountSt");
            }
        }

        public int TenPoundSakeMatchCountLt
        {
            get { return _totalCashMatchLt[(int)CashMatchValues.Ten]; }
            set
            {
                _totalCashMatchLt[(int)CashMatchValues.Ten] = value;
                RaisePropertyChangedEvent("TenPoundSakeMatchCountLt");
            }
        }


        public int TwentyPoundSakeMatchCountLt
        {
            get { return _totalCashMatchLt[(int)CashMatchValues.Twenty]; }
            set
            {
                _totalCashMatchLt[(int)CashMatchValues.Ten] = value;
                RaisePropertyChangedEvent("TwentyPoundSakeMatchCountLt");
            }
        }

        public int TwentyPoundSakeMatchCountSt
        {
            get { return _totalCashMatchSt[(int)CashMatchValues.Twenty]; }
            set
            {
                _totalCashMatchSt[(int)CashMatchValues.Twenty] = value;
                RaisePropertyChangedEvent("TwentyPoundSakeMatchCountSt");
            }
        }



        public ObservableCollection<DemoEvent> DemoCardLog
        {
            get { return _demoCardLog; }
            set { _demoCardLog = value; }
        }

        public ObservableCollection<CashMatch> CashMatchLog
        {
            get { return _cashMatchLog; }
            set { _cashMatchLog = value; }
        }

        public ObservableCollection<CashMatchAudit> CashMatchAudit
        {
            get { return _cashMatchAudit; }
            set { _cashMatchAudit = value; }
        }
        
        public ObservableCollection<LoyaltyAudit> LoyaltyAudit
        {
            get { return _loyaltyAudit; }
            set { _loyaltyAudit = value; }
        }

        public ObservableCollection<LoyaltyLogField> LoyaltyLog
        {
            get { return _loyaltyLog; }
            set { _loyaltyLog = value; }
        }
        #endregion

        public AuditViewModel()
            : base("Audit")
        {
            Populate();
        }


        public AuditViewModel(string name)
        {
            Name = name;
            Populate();
        }
        
        void Populate()
        {
            try
            {
                DemoCountLt = (int)BoLib.getDemoPlayMeter((byte)DemoPlayMeters.DemoPlayNoGamesLt);
                DemoCountSt = (int)BoLib.getDemoPlayMeter((byte)DemoPlayMeters.DemoplayNoGamesSt);

                var sb = new StringBuilder(1024);
                NativeWinApi.GetPrivateProfileString("SmartCardIndex", "Index", "", sb, sb.Capacity, Properties.Resources.smart_card_log);
                var limit = Convert.ToUInt32(sb.ToString());

                for (int i = 0; i < limit - 1; i++)
                {
                    var temp = new StringBuilder(256);
                    //    PDTUtils.Logic.IniFileUtility.GetIniProfileSection(out andting, "SmartCardLog", Properties.Resources.smart_card_log);
                    NativeWinApi.GetPrivateProfileString("SmartCardLog", (i + 1).ToString(), "", temp, temp.Capacity, Properties.Resources.smart_card_log);
                    var tokens = temp.ToString().Split(new char[1] { ',' });
                    if (tokens[4].Equals("0") || tokens[4].Equals("1"))
                    {
                        //cash match 
                        PopulateCashMatchLog(tokens);
                    }
                    else if (tokens[4].Equals("2"))
                    {
                        PopulateDemoLog(tokens);
                    }
                    /*else if (tokens[4].Equals("3"))
                    {
                        //none
                    }*/
                }
                
                //PopulateDemoLog();
                PopulateLoyalty();
                PopulateCashMatchAudit();
                //PopulateLoyalty();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        
        void PopulateDemoLog(string[] tokens)
        {
            var date = tokens[0].Substring(0, 8).Insert(4, "/").Insert(7, "/");
            var time = tokens[0].Substring(8).Insert(2, ":").Insert(5, ":");

            _demoCardLog.Add(new DemoEvent() { CardNumber = tokens[1], Date = date, Time = time });
        }

        void PopulateCashMatchAudit()
        {
            var cmValues = new int[2,2]//ulong[2, 2] 
            { 
                {(int)BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn1St), (int)BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn1Lt)}, //£5
                {(int)BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn2St), (int)BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn2Lt)}, //£10
                //{BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn3St), BoLib.getCashMatchMeter((byte)CashMatchMeters.CashMatchIn3Lt)}  //£20
            };

            _cashMatchAudit.Add(new CashMatchAudit()
            {
                Description = "Total Quantity £5",
                ShortTerm = cmValues[0, 0].ToString(),
                LongTerm = cmValues[0, 1].ToString()
            });
            
            _cashMatchAudit.Add(new CashMatchAudit()
            {
                Description = "Total Quantity £10",
                ShortTerm = cmValues[1, 0].ToString(),
                LongTerm = cmValues[1, 1].ToString()
            });

            /*_cashMatchAudit.Add(new CashMatchAudit()
            {
                Description = "Total Quantity £20",
                ShortTerm = cmValues[2, 0].ToString(),
                LongTerm = cmValues[2, 1].ToString()
            });*/

            _cashMatchAudit.Add(new CashMatchAudit()
            {
                Description = "Total Value £5",
                ShortTerm = "£" + (cmValues[0, 0] * 5).ToString(),
                LongTerm = "£" + (cmValues[0, 1] * 5).ToString()
            });

            _cashMatchAudit.Add(new CashMatchAudit()
            {
                Description = "Total Value £10",
                ShortTerm = "£" + (cmValues[1, 0] * 10).ToString(),
                LongTerm = "£" + (cmValues[1, 1] * 10).ToString()
            });

            /*_cashMatchAudit.Add(new CashMatchAudit()
            {
                Description = "Total Value £20",
                ShortTerm = "£" + (cmValues[2, 0] * 20).ToString(),
                LongTerm = "£" + (cmValues[2, 1] * 20).ToString()
            });*/
            
            var sumShortTerm = ((cmValues[0, 0] * 5) + (cmValues[1, 0] * 10)).ToString();// + (cmValues[2, 0] * 20)).ToString();
            var sumLongTerm = ((cmValues[0, 1] * 5) + (cmValues[1, 1] * 10)).ToString();// + (cmValues[2, 1] * 20)).ToString();
            _cashMatchAudit.Add(new CashMatchAudit()
            {
                Description = "Total Value £",
                ShortTerm = "£" + sumShortTerm,
                LongTerm = "£" + sumLongTerm
            });
        }
        
        void PopulateCashMatchLog(string[] tokens)
        {
            var date = tokens[0].Substring(0, 8).Insert(4, "/").Insert(7, "/");
            var time = tokens[0].Substring(8).Insert(2, ":").Insert(5, ":");
            var cmValue = (tokens[4].Equals("0")) ? "5" : "10";

            _cashMatchLog.Add(new CashMatch() { CardNumber = tokens[1], Date = date, Time = time, Value = cmValue });
        }
        
        void PopulateLoyalty()
        {
            _loyaltyAudit.Add(new LoyaltyAudit()
            {
                Description = "Points Awarded",
                ShortTerm = ((int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardInSt)).ToString(),
                LongTerm = ((int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardInLt)).ToString()
            });

            _loyaltyAudit.Add(new LoyaltyAudit()
            {
                Description = "Points Redeemed",
                ShortTerm = ((int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardOutSt)).ToString(),
                LongTerm = ((int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardOutLt)).ToString()
            });
            
            _loyaltyAudit.Add(new LoyaltyAudit()
            {
                Description = "Card Inserts",
                ShortTerm = ((int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardCardInSt)).ToString(),
                LongTerm =((int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardCardInLt)).ToString()
            });

            _loyaltyAudit.Add(new LoyaltyAudit()
            {
                Description = "Average Transaction",
                ShortTerm = "£" + ((decimal)(int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardInSt) / (int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardCardInSt) / 100).ToString(),
                LongTerm = "£" + ((decimal)(int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardInLt) / (int)BoLib.getRewardPointMeter((byte)RewardMeters.RewardCardInLt) / 100).ToString()
            });

            
            /*for (int i = 0; i < 100; i++)
            {
                _loyaltyLog.Add(new LoyaltyLogField()
                {
                    CardNumber = "13615817",
                    Date = DateTime.Now.ToShortDateString(),
                    Time = DateTime.Now.ToShortTimeString(),
                    PointsAwarded = "562",
                    PointsRedeemed = "0"
                });
            }*/
        }
    }
}

//alt credits
//[cashmatch]
//[demoplay]
//last 25 of each
