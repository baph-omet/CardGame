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
        public bool CanAttack;
        
        [XmlIgnore]
        private MonsterStats originalStats;
        [XmlIgnore]
        public MonsterStats OriginalStats { get { return originalStats; } }
        
        [XmlIgnore]
        private List<MonsterSpellBonus> equippedBonuses;
        [XmlIgnore]
        public List<MonsterSpellBonus> EquippedBonuses { get { return equippedBonuses; } }
        
        [XmlIgnore]
        private List<MonsterSpellBonus> nonequippedbonuses;
        [XmlIgnore]
        public List<MonsterSpellBonus> NonEquippedBonuses { get { return nonequippedbonuses; } }

        [XmlIgnore]
        public MonsterStats Stats {
            get {
                MonsterStats stats = originalStats;
                foreach (MonsterSpellBonus bonus in equippedBonuses) stats += bonus.StatChanges;
                foreach (MonsterSpellBonus bonus in nonequippedbonuses) stats += bonus.StatChanges;
                return stats;
            }
        }

        public Monster() {
            Facedown = false;
            CanAttack = true;
            equippedBonuses = new List<MonsterSpellBonus>();
            nonequippedbonuses = new List<MonsterSpellBonus>();

            using (XmlReader reader = XmlReader.Create("Data/Monster.xml")) {
                originalStats = new MonsterStats();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.Name) {
                            case "card":
                                if (reader.GetAttribute(0) != string.Format("{0:000}", ID)) reader.Skip();
                                break;
                            case "name":
                                reader.Read();
                                Name = reader.Value;
                                break;
                            case "description":
                                reader.Read();
                                Description = reader.Value;
                                break;
                            case "attack":
                                reader.Read();
                                originalStats.Attack = Convert.ToInt32(reader.Value);
                                break;
                            case "defense":
                                reader.Read();
                                originalStats.Defense = Convert.ToInt32(reader.Value);
                                break;
                            case "level":
                                reader.Read();
                                originalStats.Level = Convert.ToInt32(reader.Value);
                                break;
                            case "type":
                                reader.Read();
                                originalStats.Type = (MonsterType)Enum.Parse(typeof(MonsterType), reader.Value.ToUpper());
                                break;
                        }
                    }
                }
            }
        }
        public Monster(int id) {
            ID = id;
            Facedown = false;
            CanAttack = true;
            equippedBonuses = new List<MonsterSpellBonus>();
            nonequippedbonuses = new List<MonsterSpellBonus>();

            using (XmlReader reader = XmlReader.Create("Data/Monster.xml")) {
                originalStats = new MonsterStats();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.Name) {
                            case "card":
                                if (reader.GetAttribute(0) != string.Format("{0:000}", ID)) reader.Skip();
                                break;
                            case "name":
                                reader.Read();
                                Name = reader.Value;
                                break;
                            case "description":
                                reader.Read();
                                Description = reader.Value;
                                break;
                            case "attack":
                                reader.Read();
                                originalStats.Attack = Convert.ToInt32(reader.Value);
                                break;
                            case "defense":
                                reader.Read();
                                originalStats.Defense = Convert.ToInt32(reader.Value);
                                break;
                            case "level":
                                reader.Read();
                                originalStats.Level = Convert.ToInt32(reader.Value);
                                break;
                            case "type":
                                reader.Read();
                                originalStats.Type = (MonsterType) Enum.Parse(typeof(MonsterType), reader.Value.ToUpper());
                                break;
                        }
                    }
                }
            }
        }

        public MonsterAttackOutcome Battle(Monster target) {
            Facedown = false;
            CanAttack = false;
            if (target.Facedown) target.Flip();
            if (Stats.Type == MonsterTypes.GetWeakness(target.Stats.Type)) return MonsterAttackOutcome.WIN;
            else if (target.Stats.Type == MonsterTypes.GetWeakness(Stats.Type)) return MonsterAttackOutcome.LOSS;
            else if (Attack > target.Defense) return MonsterAttackOutcome.WIN;
            else return MonsterAttackOutcome.TIE;
        }

        public void Flip() {
            Facedown = !Facedown;
            //TODO: activate effect if one exists
            if (Effects.Count > 0) { }
        }

        public override string ToString() {
            StringBuilder str = new StringBuilder();
            StringBuilder lvl = new StringBuilder();
            str.AppendLine("(" + string.Format("{0:000}", ID) + ") " + Name + ":");
            for (int i = 0; i < Level; i++) lvl.Append("*");
            str.Append('\n');
            str.AppendLine("| " + Stats.Type + " - " + lvl.ToString());
            str.AppendLine("| Attack:  " + string.Format("{0:0000}", Attack));
            str.AppendLine("| Defense: " + string.Format("{0:0000}", Defense));
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
