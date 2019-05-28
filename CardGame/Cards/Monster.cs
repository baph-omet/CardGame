using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace CardGame.Cards {

    public enum MonsterAttackOutcome {
        WIN,
        LOSS,
        TIE
    }

    [Serializable]
    public class Monster : Card {
        [XmlIgnore]
        public int Attack { get { return Stats.Attack; } }
        [XmlIgnore]
        public int Defense { get { return Stats.Defense; } }
        [XmlIgnore]
        public override int Level { get { return Stats.Level; } }
        [XmlIgnore]
        public MonsterType Type { get { return Stats.Type; } }

        [XmlIgnore]
        public Boolean CanAttack;
        
        [XmlIgnore]
        private MonsterStats originalStats;
        [XmlIgnore]
        public MonsterStats OriginalStats { get { return originalStats; } }
        
        [XmlIgnore]
        private List<MonsterSpellBonus> equippedspells;
        [XmlIgnore]
        public List<MonsterSpellBonus> EquippedSpells { get { return equippedspells; } }
        
        [XmlIgnore]
        private List<MonsterSpellBonus> nonequippedbonuses;
        [XmlIgnore]
        public List<MonsterSpellBonus> NonEquippedBonuses { get { return nonequippedbonuses; } }

        [XmlIgnore]
        public MonsterStats Stats {
            get {
                MonsterStats stats = originalStats;
                foreach (MonsterSpellBonus bonus in equippedspells) stats += bonus.StatChanges;
                foreach (MonsterSpellBonus bonus in nonequippedbonuses) stats += bonus.StatChanges;
                return stats;
            }
        }

        public Monster() : this(0) { }
        public Monster(int id) {
            this.ID = id;
            this.Facedown = false;
            this.CanAttack = true;
            this.equippedspells = new List<MonsterSpellBonus>();
            this.nonequippedbonuses = new List<MonsterSpellBonus>();

            using (XmlReader reader = XmlReader.Create("Data/Monster.xml")) {
                this.originalStats = new MonsterStats();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.Name) {
                            case "card":
                                if (reader.GetAttribute(0) != String.Format("{0:000}", id)) reader.Skip();
                                break;
                            case "name":
                                reader.Read();
                                this.Name = reader.Value;
                                break;
                            case "description":
                                reader.Read();
                                this.Description = reader.Value;
                                break;
                            case "attack":
                                reader.Read();
                                this.originalStats.Attack = Convert.ToInt32(reader.Value);
                                break;
                            case "defense":
                                reader.Read();
                                this.originalStats.Defense = Convert.ToInt32(reader.Value);
                                break;
                            case "level":
                                reader.Read();
                                this.originalStats.Level = Convert.ToInt32(reader.Value);
                                break;
                            case "type":
                                reader.Read();
                                this.originalStats.Type = (MonsterType) Enum.Parse(typeof(MonsterType), reader.Value.ToUpper());
                                break;
                        }
                    }
                }
            }
        }

        public MonsterAttackOutcome Battle(Monster target) {
            this.Facedown = false;
            this.CanAttack = false;
            if (target.Facedown) target.Flip();
            if (this.Stats.Type == MonsterTypes.GetWeakness(target.Stats.Type)) return MonsterAttackOutcome.WIN;
            else if (target.Stats.Type == MonsterTypes.GetWeakness(this.Stats.Type)) return MonsterAttackOutcome.LOSS;
            else if (this.Attack > target.Defense) return MonsterAttackOutcome.WIN;
            else return MonsterAttackOutcome.TIE;
        }

        public void Flip() {
            this.Facedown = !this.Facedown;
            //TODO: activate effect if one exists
        }

        public override String ToString() {
            StringBuilder str = new StringBuilder();
            StringBuilder lvl = new StringBuilder();
            str.AppendLine("(" + String.Format("{0:000}", ID) + ") " + Name + ":");
            for (int i = 0; i < Level; i++) lvl.Append("*");
            str.Append('\n');
            str.AppendLine("| " + Stats.Type + " - " + lvl.ToString());
            str.AppendLine("| Attack:  " + String.Format("{0:0000}", Attack));
            str.AppendLine("| Defense: " + String.Format("{0:0000}", Defense));
            str.AppendLine("| " + Description);

            return str.ToString();
        }

        public static Monster getHighestAttack(List<Monster> monsters) {
            Monster highestAttack = null;
            foreach (Monster mon in monsters) if (highestAttack == null || mon.Attack > highestAttack.Attack) highestAttack = mon;
            return highestAttack;
        }

        public static Monster getHighestDefense(List<Monster> monsters) {
            Monster highestDefense = null;
            foreach (Monster mon in monsters) if (highestDefense == null || mon.Defense > highestDefense.Defense) highestDefense = mon;
            return highestDefense;
        }
    }
}
