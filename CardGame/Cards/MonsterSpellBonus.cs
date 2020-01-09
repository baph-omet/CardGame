using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Characters;

namespace CardGame.Cards {
    public class MonsterSpellBonus {
        private Battler cardowner;
        public Battler CardOwner { get { return cardowner; } }

        private int spellzone;
        public int SpellZone { get { return spellzone; } }

        private MonsterStats statchanges;
        public MonsterStats StatChanges { get { return statchanges; } }

        public MonsterSpellBonus(Battler cardowner, int spellzone, MonsterStats statchanges) {
            cardowner = cardowner;
            spellzone = spellzone;
            statchanges = statchanges;
        }
    }
}
