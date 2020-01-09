using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Cards;
using CardGame.Characters;
using CardGame.Scenes;

namespace CardGame.AI {
    public class BattleAI {

        public int difficulty = 0;
        
        public Battler User = null;
        public Battler Opponent = null;

        public BattleAI(ref Battler user, ref Battler opponent) {
            this.User = user;
            this.Opponent = opponent;
            if (user is NPC) difficulty = ((NPC) user).Difficulty;
            else if (user is Player) difficulty = ((Player) user).Level / 255;
        }

        public int GetHighestOpponentAttack() {
            int highest = 0;
            for (int i = 0; i < Opponent.Field.Length; i++) {
                if (Opponent.Field.Monsters[i] != null) {
                    Monster mon = (Monster) Opponent.Field.Monsters[i];
                    if (mon.Attack > highest) highest = mon.Attack;
                }
            } return highest;
        }

        public int GetHighestOpponentDefense() {
            int highest = 0;
            for (int i = 0; i < Opponent.Field.Length; i++) {
                if (Opponent.Field.Monsters[i] != null) {
                    Monster mon = (Monster) Opponent.Field.Monsters[i];
                    if (mon.Defense > highest) highest = mon.Defense;
                }
            } return highest;
        }

        public List<Monster> GetSummonableMonstersFromHand() {
            List<Monster> moves = new List<Monster>();
            if (User.AllMonsterZonesFull()) return moves;
            foreach (Card card in User.Hand) {
                if (card.Level <= User.ManaAllotment && card is Monster) moves.Add((Monster)card);
            }
            return moves;
        }

        public Monster GetBestSummon() {
            Monster best = null;
            //TODO: Take opponent's field into consideration
            foreach (Monster m in GetSummonableMonstersFromHand()) {
                if (best == null || m.Attack > best.Attack) best = m;
            }
            return best;
        }

        public List<int> GetUntappedSummons() {
            List<int> moves = new List<int>();
            for (int i = 0; i < User.Field.Length; i++) {
                if (User.Field.Monsters[i] != null) moves.Add(i);
            } return moves;
        }

        public List<int> GetViableTargets(int monsterIndex) {
            List<int> targets = new List<int>();
            Monster mon = (Monster)User.Field.Monsters[monsterIndex];
            if (mon != null) {
                for (int i = 0; i < Opponent.Field.Length; i++) {
                    if (Opponent.Field.Monsters[i] != null) {
                        Monster target = (Monster) Opponent.Field.Monsters[i];
                        if (target.Defense < mon.Attack || MonsterTypes.GetWeakness(target.Type) == mon.Type) targets.Add(i);
                    }
                }
            } return targets;
        }

        public int GetBestTarget(int monsterIndex) {
            Monster mon = (Monster)User.Field.Monsters[monsterIndex];
            int targetIndex = -1;
            int strongest = 0;
            foreach (int i in GetViableTargets(monsterIndex)) {
                Monster target = (Monster) Opponent.Field.Monsters[i];
                if (target.Attack > strongest || MonsterTypes.GetWeakness(target.Type) == mon.Type) {
                    strongest = target.Attack;
                    targetIndex = i;
                }
            } return targetIndex;
        }

        public int GetFirstOpenMonsterZone() {
            for (int i = 0; i < User.Field.Length; i++) {
                if (User.Field.Monsters[i] == null) return i;
            } return -1;
        }

        public bool KeepMoving() {
            return User.CanMove();
        }
    }
}
