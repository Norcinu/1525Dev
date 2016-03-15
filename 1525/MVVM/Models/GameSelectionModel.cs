using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.MVVM.Models
{
    class GameSelectionModel
    {
        public bool Active { get; set; }
        public bool Promo { get; set; }
        public string Name { get; set; }
        List<int> Pops { get; set; }
    }
}
