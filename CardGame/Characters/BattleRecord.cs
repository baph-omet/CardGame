using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CardGame.Characters {
    [Serializable]
    public class BattleRecord {
        [XmlAttribute("id")]
        public int OpponentID;
        public int Wins;
        public int Losses;
        public int Ties;

        public BattleRecord() : this(0) { }
        public BattleRecord(int id) : this(id,0,0,0) { }
        public BattleRecord(int id, int wins, int losses, int ties) {
            OpponentID = id;
            Wins = wins;
            Losses = losses;
            Ties = ties;
        }

        public int GetTotalGames() {
            return Wins + Losses + Ties;
        }

        public static List<BattleRecord> GetBlankRecord() {
            List<BattleRecord> br = new List<BattleRecord>();
            foreach (NPC n in NPC.GetAllNPCs()) br.Add(new BattleRecord(n.ID));
            return br;
        }
    }
}
