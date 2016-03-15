using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Xml;
//using PDTUtils.Helpers;

namespace PDTUtils.MVVM.ViewModels
{
    public class Pair<T, U>
    {
        public Pair()
        {
        }
        
        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    }

    public class RouletteSetting
    {
        public RouletteSetting()
        {

        }

        public RouletteSetting(string header, int min, int max)
        {
            Header = header;
            Max = max;
            Min = min;
        }

        public string Header { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
    }

    class RouletteBettingViewModel : ObservableObject
    {
        int _selectedIndex = -1;
        
        //TODO: Make the model number configurable instead of fixed to 2001.
        readonly string _betValues = @Properties.Resources.roulette_bet_values; //@"D:\2001\BetValues.xml";
        Dictionary<string, Pair<int, int>> _betInfo = new Dictionary<string, Pair<int, int>>();
        List<string> _names = new List<string>();
        
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChangedEvent("SelectedIndex");
                RaisePropertyChangedEvent("SelectedName");
                RaisePropertyChangedEvent("SelectedMin");
                RaisePropertyChangedEvent("SelectedMax");
                RaisePropertyChangedEvent("BetInfo");
            }
        }

        public string SelectedName
        {
            get
            {
                if (_selectedIndex >= 0)
                    return _names[_selectedIndex];
                else
                    return "";
            }
        }
        
        public int SelectedMin
        {
            get
            {
                if (_selectedIndex >= 0)
                    return BetInfo[_names[_selectedIndex]].First;
                else
                    return -1;
            }
            set
            {
                BetInfo[_names[_selectedIndex]].First = value;
                _items.Items.Refresh();
                RaisePropertyChangedEvent("SelectedMin");
                RaisePropertyChangedEvent("BetInfo");
            }
        }
        
        public int SelectedMax
        {
            get 
            {
                if (_selectedIndex >= 0)
                    return BetInfo[_names[_selectedIndex]].Second;
                else
                    return -1;
            }
            set
            {
                if (_selectedIndex >= 0)
                {
                    BetInfo[_names[_selectedIndex]].Second = value;
                    _items.Items.Refresh();
                    RaisePropertyChangedEvent("SelectedMax");
                    RaisePropertyChangedEvent("BetInfo");
                }
            }
        }
        
        public List<string> Names { get { return _names; } }

        public Dictionary<string, Pair<int, int>> BetInfo
        {
            get { return _betInfo; }
            set { _betInfo = value; }
        }
        
        public RouletteBettingViewModel()
        {
            ParseFile();
        }

        System.Windows.Controls.ListView _items = new System.Windows.Controls.ListView();
        public System.Windows.Controls.ListView Items
        {
            get { return _items; }
            set { _items = value; }
        }
        
        void ParseFile()
        {
            try
            {
                using (var xml = XmlReader.Create(_betValues))
                {
                    string name = "";
                    string[] attribute = new string[2];
                    int count = 0;
                    int length = 2;
                    
                    while (xml.Read())
                    {
                        if (xml.HasAttributes)
                        {
                            string attr = "";
                            if (xml.Name == "betvalue") 
                                attr = "update";
                            else if (xml.Name == "betvalues") 
                                attr = "update";
                            else if (xml.Name == "bet")
                            {
                                attr = "name";
                                name = xml.GetAttribute(attr);
                            }
                            else if (xml.Name == "min")
                            {
                                attr = "value";
                                attribute[0] = xml.GetAttribute(attr);
                                count++;
                            }
                            else if (xml.Name == "max")
                            {
                                attr = "value";
                                attribute[1] = xml.GetAttribute(attr);
                                count++;
                            }
                        }
                        
                        if (count == length)
                        {
                            if (!name.Equals("rose"))
                            {
                                try
                                {
                                    if (Convert.ToInt32(name) > 0)
                                        name += ":1";

                                    _names.Add(name);
                                }
                                catch (Exception e)
                                {
                                    _names.Add(name);
                                }
                                
                                _betInfo.Add(name, new Pair<int, int>(Convert.ToInt32(attribute[0]), Convert.ToInt32(attribute[1])));
                                //_betInfo.Add(new RouletteSetting(name, Convert.ToInt32(attribute[0]), Convert.ToInt32(attribute[1])));
                            }
                            
                            count = 0;
                            attribute[0] = "";
                            attribute[1] = "";
                            name = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not load Roulette bets");
            }

            RaisePropertyChangedEvent("BetInfo");
            RaisePropertyChangedEvent("Names");
        }
        
        public ICommand SaveSettings
        {
            get { return new DelegateCommand(o => Write()); }
        }
        
        void Write()
        {
            Encoding encoding = new ASCIIEncoding();
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, Encoding = encoding, NewLineChars = "\n" };
            using (var xml = XmlWriter.Create(_betValues, settings))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("betvalues");
                xml.WriteAttributeString("update", "true");
                
                foreach (var k in _betInfo)
                {
                    xml.WriteStartElement("bet");
                    xml.WriteAttributeString("name", k.Key.Split(":".ToCharArray())[0]);
                    xml.WriteStartElement("min");
                    xml.WriteAttributeString("value", k.Value.First.ToString());
                    xml.WriteEndElement();
                    xml.WriteStartElement("max");
                    xml.WriteAttributeString("value", k.Value.Second.ToString());
                    xml.WriteEndElement();
                    xml.WriteEndElement();
                }
                
                xml.WriteEndElement();
            }
        }
    }
}
