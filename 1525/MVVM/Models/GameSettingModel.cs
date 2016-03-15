using System.Collections.Generic;
using System.Windows.Input;

namespace PDTUtils.MVVM.Models
{
    class GameSettingModel
    {
        /*public bool? Active
        {
            get { return this._active; }
            set
            {
                if (value == null)
                    this._active = false;
                else
                    this._active = value;
            }
        }*/
        public bool Active { get; set; }       
        public bool Promo { get; set; }
        public bool IsFirstPromo { get; set; }
        public bool IsSecondPromo { get; set; }
        public uint ModelNumber { get; set; }
        public uint StakeMask { get; set; }
        /*public int? StakeOne { get; set; }
        public int? StakeTwo { get; set; }
        public int? StakeThree { get; set; }
        public int? StakeFour { get; set; }
        public int? StakeFive { get; set; }
        public int? StakeSix { get; set; }*/
        public string Title { get; set; }
        public string ModelDirectory { get; set; }
        public string Exe { get; set; }
        public string HashKey { get; set; }


        public GameSettingModel()
        {
            Active = false;
            Promo = false;
            ModelNumber = 0;
            StakeMask = 0;
            /*StakeOne = 0;
            StakeTwo = 0;
            StakeThree = 0;
            StakeFour = 0;
            StakeFive = 0;
            StakeSix = 0;
            StakeSeven = "";
            StakeEight = "";
            StakeNine = "";
            StakeTen = "";*/
            Title = "";
            ModelDirectory = "";
            Exe = "";
            HashKey = "";
            IsFirstPromo = false;
            IsSecondPromo = false;
        }
    }
}
