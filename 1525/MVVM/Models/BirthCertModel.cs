using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.MVVM.Models
{
    class BirthCertModel
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public BirthCertModel(string f, string v)
        {
            this.Field = f;
            this.Value = v;
        }
        /*public KeyValuePair<string, string> PayoutType;
        public KeyValuePair<string, string> NumberOfHoppers;
        public KeyValuePair<string, string> PrinterType;
        public KeyValuePair<string, string> CpuType;
        public KeyValuePair<string, string> CabinetType;
        public KeyValuePair<string, string> BnvType;
        public KeyValuePair<string, string> CoinValidatorType;
        public KeyValuePair<string, string> NvFloatControl;
        public KeyValuePair<string, string> RecyclerChannel;
        public KeyValuePair<string, string> CardReader;
        public KeyValuePair<string, string> ScreenCount;
        public KeyValuePair<string, string> DumpSwitchFitted;
        public KeyValuePair<string, string> HandPayThreshold;
        public KeyValuePair<string, string> HandPayOnly;
        public KeyValuePair<string, string> LhDivertThreshold;
        public KeyValuePair<string, string> VolumeControl;
        public KeyValuePair<string, string> DGS;
        public KeyValuePair<string, string> OverrideRecycler;
        public KeyValuePair<string, string> TitoEnabled;
        public KeyValuePair<string, string> TitoHost;*/
    }
}
