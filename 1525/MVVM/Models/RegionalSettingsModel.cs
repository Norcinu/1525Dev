using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PDTUtils.Native;

namespace PDTUtils.MVVM.Models
{
    public enum ESiteType { StreetMarket = 1, Arcade }

    class SpainRegionSelection
    {
        public int Id { get; set; }
        public string Community { get; set; }
        public string VenueType { get; set; }
    }
    
    class SpanishRegionalModel
    {	
        public uint MaxStakeCredits { get; set; }
        public uint MaxStakeBank { get; set; }
        public uint StakeMask { get; set; }
        public uint MaxWinPerStake { get; set; }
        public uint MaxCredits { get; set; }
        public uint MaxReserveCredits { get; set; }
        public uint MaxBank { get; set; }
        public uint MaxPlayerPoints { get; set; }
        public uint EscrowState { get; set; }
        public uint Rtp { get; set; }
        public uint GameTime { get; set; }
        public uint GiveChangeThreshold { get; set; }
        public uint MaxBankNote { get; set; }
        public uint AllowBank2Credit { get; set; }
        public uint ConvertToPlay { get; set; }
        public uint CycleSize { get; set; }
        public uint FastTransfer { get; set; }
        //public uint Allow
        public uint GamesPerPeriod { get; set; }
        public string Community { get; set; }
        
        
        public SpanishRegionalModel(string community, SpanishRegional region)
        {
            this.Community = community;
            this.GiveChangeThreshold = region.ChangeValue;
            this.CycleSize = region.CycleSize;
            this.FastTransfer = region.FastTransfer;
            this.GameTime = region.Gtime;
            this.MaxBank = region.MaxBank;
            this.MaxPlayerPoints = region.MaxPlayerPoints;
            this.MaxCredits = region.MaxCredit;
            this.MaxReserveCredits = region.MaxReserve;
            this.MaxStakeBank = region.MaxStakeFromBank;
            this.MaxStakeCredits = region.MaxStake;
            this.EscrowState = region.NoteEscrow;
            this.Rtp = region.Rtp;
            this.StakeMask = region.StakeMask;
            this.MaxWinPerStake = region.MaxWinPerStake;
            this.AllowBank2Credit = region.BankToCredits;
            this.ConvertToPlay = region.ChargeConvertPoints;
            this.MaxBankNote = region.MaxNote;
            this.GamesPerPeriod = region.GamesPerPeriod;
        }
    }
}
