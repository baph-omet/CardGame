using CardGame.Characters;
using CardGame.Scenes;
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
        public string Name;
        [XmlIgnore]
        public string Description;
        [XmlIgnore]
        public virtual int Level { get; set; }
        [XmlIgnore]
        public Boolean Facedown;
        [XmlIgnore]
        public bool WillPlay = true;

        //TODO: Set Owner and Controller properties
        [XmlIgnore]
        public Battler Owner { get; set; }
        [XmlIgnore]
        public Battler Controller { get; set; }

        [XmlIgnore]
        protected List<CardEffect> effects;
        [XmlIgnore]
        public List<CardEffect> Effects { get { return effects; } }

        [XmlIgnore]
        protected int effectIndex = 0;
        [XmlIgnore]
        public int EffectIndex { get { return effectIndex; } }

        public void ResolveEffects(Battle battle, Battler owner, Card triggeringCard = null) {
            Battler opponent = battle.GetOpponent(owner);
            foreach (CardEffect e in effects) {
                List<Card> possibleTargets = new List<Card>();
                switch (e.TargetType) {
                    case CardEffectTargetType.MONSTER:
                        if (e.Range != CardEffectTargetRange.OPPONENT) for (int i = 0; i < owner.Field.Length; i++) 
                                if (owner.Field.Monsters[i] != null) possibleTargets.Add(owner.Field.Monsters[i]);

                        if (e.Range != CardEffectTargetRange.SELF)  for (int i = 0; i < opponent.Field.Length; i++) 
                                if (opponent.Field.Monsters[i] != null) possibleTargets.Add(opponent.Field.Monsters[i]);
                        break;
                    case CardEffectTargetType.SPELL:
                        if (e.Range != CardEffectTargetRange.OPPONENT) for (int i = 0; i < owner.Field.Length; i++) 
                                if (owner.Field.Spells[i] != null) e.Targets.Add(owner.Field.Spells[i]);

                        if (e.Range != CardEffectTargetRange.SELF) for (int i = 0; i < opponent.Field.Length; i++)
                                if (opponent.Field.Spells[i] != null) e.Targets.Add(opponent.Field.Spells[i]);
                        break;
                }

                switch (e.TargetAssignment) {
                    case CardEffectTargetAssignment.CHOOSE:
                        e.Targets.Add(owner.ChooseEffectTarget(battle, this, effects.IndexOf(e)));
                        break;
                    case CardEffectTargetAssignment.PREVIOUS:
                        if (effects.IndexOf(e) > 0) e.Targets.AddRange(effects[effects.IndexOf(e) - 1].Targets);
                        break;
                    case CardEffectTargetAssignment.ALL:
                        e.Targets.AddRange(possibleTargets);
                        break;
                    case CardEffectTargetAssignment.FIRST:
                        e.Targets.Add(possibleTargets[0]);
                        break;
                    case CardEffectTargetAssignment.RANDOM:
                        Random random = new Random();
                        e.Targets.Add(possibleTargets[random.Next(0, possibleTargets.Count)]);
                        break;
                    case CardEffectTargetAssignment.TRIGGER:
                        if (triggeringCard != null) e.Targets.Add(triggeringCard);
                        break;
                }

                if (e.Targets.Count == 0) {
                    if (e.Action == CardEffectAction.MANA) {
                        owner.StealMana(e.Amount, opponent, true);
                        break;
                    }
                }

                //TODO: Actually perform the effect
                foreach (Card target in e.Targets) {
                    switch (e.Action) {
                        case CardEffectAction.INHIBIT:
                            target.Effects[target.EffectIndex].Negated = true;
                            break;
                        case CardEffectAction.KILL:
                            if (target is Monster) battle.DestroyingMonster(Owner, this, (Monster)target, -1, -1);
                            else battle.DestroyingSpell(Owner, this, (Spell)target, -1, -1);
                            break;
                        case CardEffectAction.MANA:
                            if (e.Targets[0] is Monster) {
                                owner.StealMana((Monster) e.Targets[0], opponent, true);
                                break;
                            }

                            owner.StealMana(e.Targets[0].Level, opponent, true);
                            break;
                        case CardEffectAction.STAT:
                            if (target is Monster) {
                                Monster m = (Monster)target;
                                m.EquippedSpells.Add(new MonsterSpellBonus(Owner, -1, new MonsterStats(e.EffectStat == CardEffectStat.ATTACK ? e.Amount : 0, e.EffectStat == CardEffectStat.DEFENSE ? e.Amount : 0, e.EffectStat == CardEffectStat.LEVEL ? e.Amount : 0, m.Type)));
                            }
                            break;
                        case CardEffectAction.VIEW:
                            break;
                    }
                }
            }
        }
    }
}
