using System;
using System.Collections.Generic;
using System.Xml;
using CardGame.Cards;
using CardGame.Scenes;

namespace CardGame.Characters {
    class NPC : Battler {
        public int ID;
        public string Description;
        public int Bounty;
        public int Level;
        public int Difficulty;
        public string[] Text;

        public List<Card> Chest;

        public NPC(int id, string name, string description, int bounty, int level, int difficulty, string[] texts, List<Card> chest, List<Card> deck) : base() {
            ID = id;
            Name = name;
            Description = description;
            Bounty = bounty;
            Level = level;
            Difficulty = difficulty;
            Text = texts;
            Chest = chest;
            Deck = deck;
        }

        public static List<NPC> GetAllNPCs() {
            List<NPC> npcs = new List<NPC>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Data/NPC.xml");
            foreach (XmlNode node in doc.GetElementsByTagName("npc")) {
                int id = Convert.ToInt32(node.Attributes["id"].Value);
                string name = "", description = "";
                int bounty = -1, level = -1, difficulty = -1;
                List<Card> chest = new List<Card>();
                List<Card> deck = new List<Card>();
                List<string> texts = new List<string>();
                foreach (XmlNode child in node.ChildNodes) {
                    switch (child.Name.ToLower()) {
                        case "name":
                            name = child.InnerText;
                            break;
                        case "description":
                            description = child.InnerText;
                            break;
                        case "bounty":
                            bounty = Convert.ToInt32(child.InnerText);
                            break;
                        case "level":
                            level = Convert.ToInt32(child.InnerText);
                            break;
                        case "difficulty":
                            difficulty = Convert.ToInt32(child.InnerText);
                            break;
                        case "text":
                            foreach (XmlNode t in child.ChildNodes) texts.Add(t.InnerText);
                            break;
                        case "chest":
                            foreach (XmlNode c in child.ChildNodes) {
                                switch (c.Attributes["type"].Value) {
                                    case "MONSTER":
                                        chest.Add(new Monster(Convert.ToInt32(c.InnerText)));
                                        break;
                                    case "SPELL":
                                        chest.Add(new Spell(Convert.ToInt32(c.InnerText)));
                                        break;
                                }
                            }
                            break;
                        case "deck":
                            foreach (XmlNode c in child.ChildNodes) {
                                switch (c.Attributes["type"].Value) {
                                    case "MONSTER":
                                        deck.Add(new Monster(Convert.ToInt32(c.InnerText)));
                                        break;
                                    case "SPELL":
                                        deck.Add(new Spell(Convert.ToInt32(c.InnerText)));
                                        break;
                                }
                            }
                            break;
                    }
                }
                npcs.Add(new NPC(id, name, description, bounty, level, difficulty, texts.ToArray(), chest, deck));
            }
            return npcs;
        }

        public static NPC GetNPC(int id) {
            foreach (NPC n in GetAllNPCs()) if (n.ID == id) return n;
            return null;
        }

        public override Card ChooseEffectTarget(Battle battle, Card spell, int spellEffectIndex) {
            throw new NotImplementedException();
        }
    }
}
