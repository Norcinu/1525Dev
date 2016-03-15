using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDTUtils.MVVM.Models
{
    class DemoEvent
    {
        public string CardNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }

    class CashMatch
    {
        public string CardNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Value { get; set; }
    }

    class CashMatchAudit
    {
        public string Description { get; set; }
        public string LongTerm { get; set; }
        public string ShortTerm { get; set; }
    }

    class LoyaltyAudit
    {
        public string Description { get; set; }
        public string LongTerm { get; set; }
        public string ShortTerm { get; set; }
    }

    class LoyaltyLogField
    {
        public string CardNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string PointsAwarded { get; set; }
        public string PointsRedeemed { get; set; }
    }
}
