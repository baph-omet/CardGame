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

        [XmlIgnore]
        protected List<SpellEffect> effects;
        [XmlIgnore]
        public List<SpellEffect> Effects { get { return effects; } }

        [XmlIgnore]
        protected int effectIndex = 0;
        [XmlIgnore]
        public int EffectIndex { get { return effectIndex; } }
    }
}
