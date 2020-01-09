using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Scenes;
using CardGame.Characters;

namespace CardGame.Cards {
    public enum CardEffectAction {
        MANA,
        STAT,
        VIEW,
        KILL,
        INHIBIT
    }

    public enum CardEffectStat {
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

    public enum CardEffectTargetRange {
        SELF,
        OPPONENT,
        ANY
    }

    public enum CardEffectTargetAssignment {
        CHOOSE,
        RANDOM,
        FIRST,
        ALL,
        PREVIOUS,
        TRIGGER
    }

    public enum CardEffectTargetType {
        MONSTER,
        SPELL,
        MANA,
        HAND,
        DECK,
        DISCARD
    }

    public class CardEffect {
        public CardEffectAction Action { get; set; }

        public CardEffectTargetType TargetType { get; set; }

        public CardEffectTargetRange Range { get; set; }

        public CardEffectTargetAssignment TargetAssignment { get; set; }

        public List<CardEffectTargetRequirement> Requirements { get; set; }

        public CardEffectStat EffectStat { get; set; }

        public int Amount { get; set; }

        public List<Card> Targets { get; set; }

        public bool Negated { get; set; }
    }

    public struct CardEffectTargetRequirement {
        public CardEffectStat Stat { get; set; }

        public int Minimum { get; set; }

        public int Maximum { get; set; }
    }
}
