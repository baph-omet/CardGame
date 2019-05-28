using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Scenes;
using CardGame.Characters;

namespace CardGame.Cards {
    public enum SpellEffectAction {
        MANA,
        STAT,
        VIEW,
        KILL,
        INHIBIT
    }

    public enum SpellEffectStat {
        DEFENSE,
        ATTACK,
        CATEGORY,
        MONSTERTYPE,
        LEVEL,
        RESERVE,
        ALLOTMENT,
        SPELLTYPE,
        SET,
        NULL
    }

    public enum SpellEffectTargetRange {
        SELF,
        OPPONENT,
        ANY
    }

    public enum SpellEffectTargetAssignment {
        CHOOSE,
        RANDOM,
        FIRST,
        ALL,
        PREVIOUS,
        TRIGGER
    }

    public enum SpellEffectTargetType {
        MONSTER,
        SPELL,
        MANA,
        HAND,
        DECK,
        DISCARD
    }

    public class SpellEffect {
        public SpellEffectAction Action { get; set; }

        public SpellEffectTargetType TargetType { get; set; }

        public SpellEffectTargetRange Range { get; set; }

        public SpellEffectTargetAssignment TargetAssignment { get; set; }

        public List<SpellEffectTargetRequirement> Requirements { get; set; }

        public SpellEffectStat EffectStat { get; set; }

        public int Amount { get; set; }
    }
}
