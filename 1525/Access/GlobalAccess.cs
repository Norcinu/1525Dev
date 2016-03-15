using System.Collections;

namespace PDTUtils.Access
{
    enum SmartCardLevels
    {
        Player = 0x0, Cashier = 0x01, Collector = 0x02, Engineer = 0x03,
        Administrator = 0x04, Distributor = 0x05, Manufacturer = 0x06, None = 0x7
    }
    
    static class GlobalAccess
    {
        static int _level = (int)SmartCardLevels.None;
        static object _spinLock = new object();
        static BitArray _accessBits = new BitArray(8);
        
        public static int Level
        {
            get { return _level; }
            set
            {
                lock (_spinLock)
                {
                    _level = value;
                }
            }
        }
        
        static bool _hasSmartCard;
        public static bool HasSmartCard
        {
            get { return _hasSmartCard; }
            set { _hasSmartCard = value; }
        }
        
        static bool _openOrManufacturer;
        public static bool OpenOrManufacturer
        {
            get
            {
                return _openOrManufacturer;
            }
        }

    //    static bool _openOrDistributor
       /* public static BitArray AccessBits
        {
            get { return _accessBits; }
            set
            {
                _accessBits[value] = !_accessBits[value];
            }
        }*/
    }
}
