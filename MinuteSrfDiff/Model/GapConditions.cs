using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GapCondition.Util
{
    public class GapConditions
    {
        [XmlElement("Condition")]
        public List<Condition> HouseNo { get; set; }

        public GapConditions()
        {
            HouseNo = new List<Condition>();
        }
    }

    public class Condition
    {
        [XmlElement("Large")]
        public float Large { get; set; }

        [XmlElement("Minor")]
        public double Minor { get; set; }

        [XmlElement("ColorString")]
        public string ColorString { get; set; }

        [XmlElement("Visible")]
        public bool Visible { get; set; }

        [XmlElement("Comment")]
        public string Comment { get; set; }

        [XmlElement("Lock")]
        public bool Lock { get; set; }
    }
}
