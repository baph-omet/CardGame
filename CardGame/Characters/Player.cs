using System;
using System.Collections.Generic;
using System.Xml;
using CardGame.Cards;
using System.IO;
using System.Xml.Serialization;

namespace CardGame.Characters{
    [Serializable]
    public class Player : Battler {
        [XmlAttribute("id")]
        public int ID;

        public List<BattleRecord> Record;
        
        [XmlIgnore]
        public int Wins {
            get {
                int w = 0;
                foreach (BattleRecord b in Record) w += b.Wins;
                return w;
            }
        }
        [XmlIgnore]
        public int Losses {
            get {
                int l = 0;
                foreach (BattleRecord b in Record) l += b.Losses;
                return l;
            }
        }
        [XmlIgnore]
        public int Ties {
            get {
                int t = 0;
                foreach (BattleRecord b in Record) t += b.Ties;
                return t;
            }
        }
        public int Level;

        public List<Card> Chest;

        public Player() {
            Level = 0;
            Deck = new List<Card>();
            Chest = new List<Card>();
            Name = "Anonymous";
        }

        public void Save() {
            List<Player> players = GetAllPlayers();
            int found = -1;
            for (int i = 0; i < players.Count; i++) {
                if (players[i].ID == ID) {
                    found = i;
                    break;
                }
            }

            if (found < 0) players.Add(this);
            else players[found] = this;

            WriteAllPlayers(players);
        }

        public void AddWin(int OpponentID) {
            foreach (BattleRecord br in Record) {
                if (br.OpponentID == OpponentID) {
                    br.Wins++;
                    Level += NPC.GetNPC(OpponentID).Bounty;
                    break;
                }
            }
        }

        public int GetWins(int OpponentID) {
            foreach (BattleRecord br in Record) if (br.OpponentID == OpponentID) return br.Wins;
            return 0;
        }

        public void AddLoss(int OpponentID) {
            foreach (BattleRecord br in Record) {
                if (br.OpponentID == OpponentID) {
                    br.Losses++;
                    Level -= NPC.GetNPC(OpponentID).Bounty;
                    break;
                }
            }
        }

        public int GetLosses(int OpponentID) {
            foreach (BattleRecord br in Record) if (br.OpponentID == OpponentID) return br.Losses;
            return 0;
        }

        public void AddTie(int OpponentID) {
            foreach (BattleRecord br in Record) {
                if (br.OpponentID == OpponentID) br.Ties++;
                break;
            }
        }

        public int GetTies(int OpponentID) {
            foreach (BattleRecord br in Record) if (br.OpponentID == OpponentID) return br.Ties;
            return 0;
        }

        public int GetTotalGames(int OpponentID) {
            foreach (BattleRecord br in Record) if (br.OpponentID == OpponentID) return br.Wins + br.Losses + br.Ties;
            return -1;
        }

        private static int GetNextID() {
            XmlDocument doc = new XmlDocument();
            
            doc.Load(Directory.GetCurrentDirectory() + "/Data/Player.xml");

            int highest = 0;

            foreach (XmlNode node in doc.LastChild.ChildNodes) {
                int nodeId = Convert.ToInt32(node.Attributes["id"].Value);
                if (nodeId > highest) highest = nodeId;
            }

            return highest + 1;
        }

        public static void WriteAllPlayers(List<Player> players) {
            XmlSerializer serializer = new XmlSerializer(players.GetType());
            using(FileStream stream = new FileStream(Directory.GetCurrentDirectory() + "/Data/Player.xml", FileMode.Create)) serializer.Serialize(stream, players);
        }

        public static List<Player> GetAllPlayers() {
            XmlReader reader = new XmlTextReader(Directory.GetCurrentDirectory() + "\\Data\\Player.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
            List<Player> players = (List<Player>)serializer.Deserialize(reader);

            foreach (Player p in players) {
                for (int i = 0; i < p.Chest.Count; i++) {
                    Card c = p.Chest[i];
                    if (c is Monster) p.Chest[i] = new Monster(c.ID);
                    else if (c is Spell) p.Chest[i] = new Spell(c.ID);
                }
                for (int i = 0; i < p.Deck.Count; i++) {
                    Card c = p.Deck[i];
                    if (c is Monster) p.Deck[i] = new Monster(c.ID);
                    else if (c is Spell) p.Deck[i] = new Spell(c.ID);
                }
            }

            reader.Close();
            return players;
        }

        public static Player GetPlayer(int ID) {
            foreach (Player p in GetAllPlayers()) if (p.ID == ID) return p;
            return null;
        }

        public static Player CreatePlayer(string name) {
            Player defaultPlayer = GetPlayer(0);
            defaultPlayer.ID = GetNextID();
            defaultPlayer.Name = name;
            defaultPlayer.Save();
            return defaultPlayer;
        }

        public override Card ChooseSpellTarget(Scenes.Battle battle, Spell spell, int spellEffectIndex) {
            //TODO: ChooseSpellTarget(...)
            throw new NotImplementedException();
        }
    }
}
