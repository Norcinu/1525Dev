using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    enum HopperPayoutNames
    {
        LeftHandCoinValue       = 0,
        LeftHandCoinCount       = 1,
        RightHandCoinValue      = 2,
        RightHandCoinCount      = 3,
        NoteValue               = 4,
        NoteCount               = 5,
        InitBankValue           = 6,
        InitPartCollectValue    = 7,
        InitCreditValue         = 8,
        HandPayValue            = 9
    }

    enum TicketPayoutNames
    {
        RSTicketModelNo             = 0,
        RSTicketNumber              = 1,
        RSTicketBarcode             = 2, //an array of 32 ints
        RSticketDuplicateNumber     = 3,
        RSPrintProgress             = 4,
        RSPrinterStatus             = 5,
        StartPrinterBankValue       = 6,
        StartPrintPartCollectValue  = 7,
        StartPrintCreditValue       = 8
    }

    public class LastCollectLogViewModel : ObservableObject
    {
        bool _hopperPayout = false;
        bool _ticketPayout = false;
        bool _showListView = false;
        bool _errorMessageActive = false;

        int _notesPaid = 0;
        int _leftHopperCoinsPaid = 0;
        int _rightHopperCoinsPaid = 0;
        int _totalPaidOut = 0;
        int _handPaidOut = 0;

        string _errorMessage = "";
        string _payoutFile = ""; //Properties.Resources.payout;
        string _payoutStatus = "";

        DateTime _payoutDate = DateTime.Now;

        Dictionary<string, Pair<int, int>> _entries = new Dictionary<string, Pair<int, int>>();

        #region PROPERTIES
        public bool ShowListView { get; set; }
        public ObservableCollection<string> LastCollect { get; set; }

        public bool ErrorMessageActive
        {
            get { return _errorMessageActive; }
            set
            {
                _errorMessageActive = value;
                RaisePropertyChangedEvent("ErrorMessageActive");
            }
        }
        
        public bool HopperPayout
        {
            get { return _hopperPayout; }
            set
            {
                _hopperPayout = value;
                RaisePropertyChangedEvent("HopperPayout");
            }
        }

        public bool TicketPayout
        {
            get { return _ticketPayout; }
            set
            {
                _ticketPayout = value;
                RaisePropertyChangedEvent("TicketPayout");
            }
        }

        public string PayoutDate
        {
            get { return _payoutDate.ToShortTimeString() + " " + _payoutDate.ToShortDateString(); }
            set
            {
                //_payoutDate = value;
                RaisePropertyChangedEvent("PayoutDate");
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChangedEvent("ErrorMessage");
            }
        }
        
        public int LeftHandCoinsPaid
        {
            get { return _leftHopperCoinsPaid; }
            set
            {
                _leftHopperCoinsPaid = value;
                RaisePropertyChangedEvent("LeftHandCoinsPaid");
            }
        }

        public int RightHandCoinsPaid
        {
            get { return _rightHopperCoinsPaid; }
            set
            {
                _rightHopperCoinsPaid = value;
                RaisePropertyChangedEvent("RightHandCoinsPaid");
            }
        }

        public int TotalPaidOut
        {
            get { return _totalPaidOut; }
            set
            {
                _totalPaidOut = value;
                RaisePropertyChangedEvent("TotalPaidOut");
            }
        }

        public int NotesPaidOut
        {
            get { return _notesPaid; }
            set
            {
                _notesPaid = value;
                RaisePropertyChangedEvent("NotesPaid");
            }
        }
        
        public int HandPaidOut
        {
            get { return _handPaidOut; }
            set
            {
                _handPaidOut = value;
            }
        }
        
        public Dictionary<string, Pair<int, int>> Entries
        {
            get { return _entries; }
        }

        public string PayoutStatus
        {
            get { return _payoutStatus; }
            set
            {
                _payoutStatus = value;
                RaisePropertyChangedEvent("PayoutStatus");
            }
        }
        #endregion

        public LastCollectLogViewModel()
        {
            /*
            Decide to show ticket or payout info
            DateTime payout;
            DateTime ticket;
            
            if (File.Exists(Properties.Resources.payout))
                payout = File.GetLastWriteTime(Properties.Resources.payout);
                            
            if (File.Exists(Properties.Resources.payout_ticket))
                ticket = File.GetLastWriteTime(Properties.Resources.payout_ticket);
             */


            LastCollect = new ObservableCollection<string>();
            ShowListView = false;
            RaisePropertyChangedEvent("ShowListView");

            DoLoadLog();
        }
        
        bool TestCheckSums(int liveChecksum, int finalChecksum)
        {
            if (liveChecksum != finalChecksum)
            {
                if (LastCollect.Count > 0)
                    LastCollect.Clear();

                LastCollect.Add("ERROR: Checksums do not match.");
                RaisePropertyChangedEvent("LastCollect");
                return false;
            }
            return true;
        }

        void HopperCollectPayout(ref List<int> wagwan, ref int liveChecksum, ref int finalChecksum)
        {
            BoLib.setFileAction();
            try
            {
                using (var b = new BinaryReader(File.Open(@_payoutFile, FileMode.Open)))
                {
                    int position = 0;
                    int length = (int)b.BaseStream.Length;
                    while (position < length)
                    {
                        var value = b.ReadInt32();
                        wagwan.Add(value);

                        if (position != length - sizeof(int))
                            liveChecksum += value;

                        position += sizeof(int);
                    }
                }

                finalChecksum = wagwan[wagwan.Count - 1];
                if (TestCheckSums(liveChecksum, finalChecksum))
                {
                    var attr = File.GetAttributes(_payoutFile);
                    _payoutDate = File.GetLastWriteTime(_payoutFile);

                    int InitBankValue = wagwan[(int)HopperPayoutNames.InitBankValue];
                    int InitPartCollectValue = wagwan[(int)HopperPayoutNames.InitPartCollectValue];
                    int InitCreditValue = wagwan[(int)HopperPayoutNames.InitCreditValue];

                    LeftHandCoinsPaid = wagwan[(int)HopperPayoutNames.LeftHandCoinCount] * wagwan[(int)HopperPayoutNames.LeftHandCoinValue];
                    RightHandCoinsPaid = wagwan[(int)HopperPayoutNames.RightHandCoinCount] * wagwan[(int)HopperPayoutNames.RightHandCoinValue];
                    NotesPaidOut = wagwan[(int)HopperPayoutNames.NoteValue];
                    HandPaidOut = wagwan[(int)HopperPayoutNames.HandPayValue];
                    TotalPaidOut = LeftHandCoinsPaid + RightHandCoinsPaid + NotesPaidOut;

                    Entries.Add("Partial Collect", new Pair<int, int>(InitPartCollectValue, (InitPartCollectValue > 0) ? TotalPaidOut : 0));
                    Entries.Add("Bank", new Pair<int, int>(InitBankValue, (InitBankValue > 0 && InitPartCollectValue == 0) ? TotalPaidOut : 0));
                    Entries.Add("Credits", new Pair<int, int>(InitCreditValue, (InitCreditValue > 0 && InitBankValue == 0 && InitPartCollectValue == 0) ? TotalPaidOut : 0));
                    //Entries.Add("Hand Pay", new Pair<int, int>(HandPaidOut, (HandPaidOut > 0) ? HandPaidOut : 0));

                    byte res = 0;
                    long MaxSingleCollect = (long)BoLib.getMaxStagePayoutValue();
                    long lTotalPaidOut = (TotalPaidOut + NotesPaidOut);

                    if (InitPartCollectValue > 0)
                    {

                        if (MaxSingleCollect > 0 && (MaxSingleCollect == TotalPaidOut))
                        {
                            res = 0;
                        }
                        else if (InitPartCollectValue - TotalPaidOut >= 10)
                        {
                            res = 1; //short pay
                        }
                        else if (TotalPaidOut - InitPartCollectValue >= 10)
                        {
                            res = 2; //over pay
                        }
                        else
                            res = 0; //complete

                    }
                    else if (InitBankValue > 0)
                    {
                        if (MaxSingleCollect > 0 && (MaxSingleCollect == TotalPaidOut))
                        {
                            res = 0;
                        }

                        else if (InitBankValue - TotalPaidOut >= 10)
                        {
                            res = 1;
                        }
                        else if (TotalPaidOut - InitBankValue >= 10)
                        {
                            res = 2;
                        }
                        else
                            res = 0;
                    }
                    else if (InitCreditValue > 0)
                    {
                        if (MaxSingleCollect > 0 && (MaxSingleCollect == TotalPaidOut))
                        {
                            res = 0;
                        }
                        else if ((InitCreditValue - TotalPaidOut) >= 10)
                        {
                            res = 1;
                        }
                        else if ((TotalPaidOut - InitCreditValue) >= 10)
                        {
                            res = 2;
                        }
                        else
                            res = 0;
                    }

                    //print result
                    switch (res)
                    {
                        case 0:
                            PayoutStatus = "Complete";
                            break;
                        case 1:
                            PayoutStatus = "Short Pay";
                            break;
                        case 2:
                            PayoutStatus = "Over Pay";
                            break;
                    }

                    RaisePropertyChangedEvent("PayoutDate");
                    RaisePropertyChangedEvent("Entries");
                }
                else
                {
                    ErrorMessage = "ERROR: CHECKSUM MISMATCH";
                    ErrorMessageActive = true;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                ErrorMessageActive = true;
            }

            BoLib.clearFileAction();
        }
        
        //!!! TODO COMPLETE THIS
        void TicketCollectPayout(ref List<int> wagwan, ref List<int> ticketNumber, ref int liveChecksum, ref int finalChecksum)
        {
            try
            {
                BoLib.setFileAction();

                uint RSTicketFaceValue = 0;
                uint RSTicketModelNo = 0;
                uint RSTicketNumber;
                uint RSTicketDuplicateNumber = 0;
                uint RSPrintProgress;
                uint RSPrinterStatus;
                var RSTicketBarCode = new char[32];
                
                using (var b = new BinaryReader(File.Open(@Properties.Resources.payout_ticket, FileMode.Open, FileAccess.Read, FileShare.None)))
                {
                    int position = 0;
                    int length = (int)b.BaseStream.Length;
                    while (position < length)
                    {
                        if (position != (sizeof(int) * 3)) //3?
                        {
                            var value = b.ReadInt32();
                            wagwan.Add(value);
                            if (position != length - sizeof(int))
                                liveChecksum += value;
                        }
                        else
                        {
                            for (int i = 0; i < 32; i++)
                            {
                                var value = b.ReadInt32();
                                ticketNumber.Add(value);
                                liveChecksum += value;
                            }
                        }
                        position += sizeof(int);
                    }
                }
                BoLib.clearFileAction();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        
        void DoLoadLog()
        {
            var liveChecksum = 0;
            var finalChecksum = 0;
            var wagwan = new List<int>();

            if (BoLib.getLastPayoutType() == (int)CollectType.Hopper) // need an enum of these payout types.
            {
                _payoutFile = Properties.Resources.payout;
                HopperCollectPayout(ref wagwan, ref liveChecksum, ref finalChecksum);
            }
            else
            {
                var ticketNumber = new List<int>();
                _payoutFile = Properties.Resources.payout_ticket;
                TicketCollectPayout(ref wagwan, ref ticketNumber, ref liveChecksum, ref finalChecksum);
            }

            ShowListView = true;
            RaisePropertyChangedEvent("ShowListView");
        }

        public System.Windows.Input.ICommand LoadLog { get { return new DelegateCommand(o => DoLoadLog()); } }
    }
}
