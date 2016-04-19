using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PDTUtils.Logic;

namespace PDTUtils.MVVM.ViewModels
{
    class CardIdentity
    {
        public string DateAndTime { get; set; }
        public string Identifier { get; set; }
        public string Venue { get; set; }
        public string Group { get; set; }
        public string Subgroup { get; set; }
        public string Points { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
    }
    
    class SmartCardLogViewModel : ObservableObject
    {
        string _logfile = Properties.Resources.smart_card_log;
        ObservableCollection<CardIdentity> _cardLog = new ObservableCollection<CardIdentity>();
        //readonly BackgroundWorker _background = new BackgroundWorker();
        
        public ObservableCollection<CardIdentity> CardLog { get { return _cardLog; } }

        string[] _groups = { "PLAYER", "TECHNICIAN", "CASHIER", "ADMIN", "OPERATOR", "DISTRIBUTOR", "MANUFACTURER", "NONE" };
        string[] _subGroups = { "CM 5", "CM 10", "DEMO PLAY", "NONE" };

        public SmartCardLogViewModel()
        {
            Parse();
        }
        
        void _background_DoWork(object sender, DoWorkEventArgs e)
        {
            Parse();
        }
        
        void Parse()
        {
            try
            {
                var ini = new IniFile(_logfile);
                var dic = ini.GetSectionValues("SmartCardLog");
                var tempLogCard = new List<CardIdentity>();
                var tempLog = new List<CardIdentity>();
                var index = ini.GetInt32("SmartCardIndex", "Index", 0);
                var tempIndex = 0;
                
                foreach (var k in dic)
                {
                    var tokens = k.Value.Split(new char[1] { ',' });
                    var dateTime = tokens[0].Insert(8, " ").Insert(4, "/").Insert(7, "/").Split(new char[1] { ' ' });
                    dateTime[1] = dateTime[1].Insert(2, ":");
                    dateTime[1] = dateTime[1].Insert(5, ":");
                    
                    var temparr = dateTime[0].Split(new char[1] { '/' });
                    var temp = temparr[2];
                    temparr[2] = temparr[0];
                    temparr[0] = temp;

                    dateTime[0] = temparr[0] + "/" + temparr[1] + "/" + temparr[2];

                    if (tempIndex < index)
                    {
                        tempLogCard.Add(new CardIdentity()
                        {
                            DateAndTime = dateTime[0] + " " + dateTime[1],
                            Identifier = tokens[1],
                            Venue = tokens[2],
                            Group = _groups[Convert.ToUInt32(tokens[3])],
                            Subgroup = _subGroups[Convert.ToUInt32(tokens[4])],
                            Points = tokens[5],
                            Name = tokens[6] + " " + tokens[7],
                            Contact = tokens[8]
                        });
                        tempIndex++;
                    }
                    else
                    {
                        tempLog.Add(new CardIdentity()
                        {
                            DateAndTime = dateTime[0] + " " + dateTime[1],
                            Identifier = tokens[1],
                            Venue = tokens[2],
                            Group = _groups[Convert.ToUInt32(tokens[3])],
                            Subgroup = _subGroups[Convert.ToUInt32(tokens[4])],
                            Points = tokens[5],
                            Name = tokens[6] + " " + tokens[7],
                            Contact = tokens[8]
                        });
                    }
                }

                if (tempLog.Count > 0)
                {
                    tempLogCard.Reverse();
                    tempLog.Reverse();

                    foreach (var t in tempLogCard)
                        CardLog.Add(t);

                    foreach(var t in tempLog)
                        CardLog.Add(t);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}

