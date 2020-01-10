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
        public bool Facedown;
        [XmlIgnore]
        public bool WillPlay = true;

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

        public void ResolveEffects(Battle battle, Card triggeringCard = null) {
            Battler opponent = battle.GetOpponent(Owner);
            foreach (CardEffect e in effects) {
                List<Card> possibleTargets = new List<Card>();
                switch (e.TargetType) {
                    case CardEffectTargetType.MONSTER:
                        if (e.Range != CardEffectTargetRange.OPPONENT) for (int i = 0; i < Owner.Field.Length; i++) 
                                if (Owner.Field.Monsters[i] != null) possibleTargets.Add(Owner.Field.Monsters[i]);

                        if (e.Range != CardEffectTargetRange.SELF)  for (int i = 0; i < opponent.Field.Length; i++) 
                                if (opponent.Field.Monsters[i] != null) possibleTargets.Add(opponent.Field.Monsters[i]);
                        break;
                    case CardEffectTargetType.SPELL:
                        if (e.Range != CardEffectTargetRange.OPPONENT) for (int i = 0; i < Owner.Field.Length; i++) 
                                if (Owner.Field.Spells[i] != null) e.Targets.Add(Owner.Field.Spells[i]);

                        if (e.Range != CardEffectTargetRange.SELF) for (int i = 0; i < opponent.Field.Length; i++)
                                if (opponent.Field.Spells[i] != null) e.Targets.Add(opponent.Field.Spells[i]);
                        break;
                }

                switch (e.TargetAssignment) {
                    case CardEffectTargetAssignment.CHOOSE:
                        e.Targets.Add(Owner.ChooseEffectTarget(battle, this, effects.IndexOf(e)));
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
                if (e.Targets.Count == 0 && e.Action == CardEffectAction.MANA) {
                    if (e.Action == CardEffectAction.MANA) {
                        if (e.Range == CardEffectTargetRange.OPPONENT) Owner.StealMana(e.Amount, opponent, true);
                        else if (e.Range == CardEffectTargetRange.SELF) {
                            switch (e.EffectStat) {
                                case CardEffectStat.ALLOTMENT:
                                    Owner.ManaAllotment += e.Amount;
                                    break;
                                case CardEffectStat.RESERVE:
                                    Owner.Mana += e.Amount;
                                    break;
                            }
                        }
                        effectIndex++;
                        continue;
                    }
                }

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
                                Owner.StealMana((Monster) e.Targets[0], opponent, true);
                                break;
                            }

                            Owner.StealMana(e.Targets[0].Level, opponent, true);
                            break;
                        case CardEffectAction.STAT:
                            if (target is Monster) {
                                Monster m = (Monster)target;
                                m.EquippedBonuses.Add(new MonsterSpellBonus(Owner, -1, new MonsterStats(e.EffectStat == CardEffectStat.ATTACK ? e.Amount : 0, e.EffectStat == CardEffectStat.DEFENSE ? e.Amount : 0, e.EffectStat == CardEffectStat.LEVEL ? e.Amount : 0, m.Type)));
                            }
                            break;
                        case CardEffectAction.VIEW:
                            break;
                    }
                }

                effectIndex++;
            }
        }
    }
}
