namespace PDTUTils.Logic
{
    static class SmartCardStrings
    {
        static string[] _strings = new string[8] {"Player", "Cashier", "Collector", "Engineer", "Administrator", 
                                           "Distributor", "Manufacturer", "No Card Present"};

        public static string[] Strings { get { return _strings; } }
    }
}
