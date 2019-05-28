using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CardGame.Cards {
    [Serializable]
    [XmlInclude(typeof(Monster))]
    [XmlInclude(typeof(Spell))]
    public abstract class Card {
        [XmlAttribute("id")]
        public int ID;
        [XmlIgnore]
        public String Name;
        [XmlIgnore]
        public String Description;
        [XmlIgnore]
        public virtual int Level { get; set; }
        [XmlIgnore]
        public Boolean Facedown;
        [XmlIgnore]
        public bool WillPlay = true;
    }
}
