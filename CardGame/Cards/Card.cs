using CardGame.Characters;
using CardGame.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
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
        protected List<CardEffect> effects = new List<CardEffect>();
        [XmlIgnore]
        public List<CardEffect> Effects { get { return effects; } }

        [XmlIgnore]
        private CardEffectType effectType;
        [XmlIgnore]
        public CardEffectType EffectType { get => effectType; }

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

                        if (e.Range != CardEffectTargetRange.SELF) for (int i = 0; i < opponent.Field.Length; i++)
                                if (opponent.Field.Monsters[i] != null) possibleTargets.Add(opponent.Field.Monsters[i]);
                        break;
                    case CardEffectTargetType.SPELL:
                        if (e.Range != CardEffectTargetRange.OPPONENT) for (int i = 0; i < Owner.Field.Length; i++)
                                if (Owner.Field.Spells[i] != null) e.Targets.Add(Owner.Field.Spells[i]);

                        if (e.Range != CardEffectTargetRange.SELF) for (int i = 0; i < opponent.Field.Length; i++)
                                if (opponent.Field.Spells[i] != null) e.Targets.Add(opponent.Field.Spells[i]);
                        break;
                }

                if (e.Targets == null) e.Targets = new List<Card>();
                switch (e.TargetAssignment) {
                    case CardEffectTargetAssignment.CHOOSE:
                        e.Targets.Add(Owner.ChooseEffectTarget(battle, this, effects.IndexOf(e), possibleTargets));
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
                                    battle.ShowText(Owner.Name + " loaded " + e.Amount + " mana!");
                                    break;
                                case CardEffectStat.RESERVE:
                                    Owner.Mana += e.Amount;
                                    battle.ShowText(Owner.Name + " restored " + e.Amount + " mana!");
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
                            battle.ShowText(target.Name + "'s effect was negated!");
                            break;
                        case CardEffectAction.KILL:
                            if (target is Monster) battle.DestroyingMonster(Owner, this, (Monster)target, -1, target.Owner.Field.FindIndex(target));
                            else battle.DestroyingSpell(Owner, this, (Spell)target, -1, -1);
                            battle.ShowText(Owner.Name + "'s " + target.Name + " was destroyed!");
                            break;
                        case CardEffectAction.MANA:
                            if (e.Targets[0] is Monster) Owner.StealMana((Monster)e.Targets[0], opponent, true);
                            else Owner.StealMana(e.Targets[0].Level, opponent, true);
                            battle.ShowText(Owner.Name + "'s " + target.Name + " stole " + e.Targets[0].Level + "mana from " + opponent.Name + "!");
                            break;
                        case CardEffectAction.STAT:
                            if (target is Monster) {
                                Monster m = (Monster)target;
                                MonsterStats ms = new MonsterStats();
                                ms.Type = m.Type;
                                string direction = "increased";
                                if (e.Amount < 0) direction = "decreased";
                                switch (e.EffectStat) {
                                    case CardEffectStat.ATTACK:
                                        ms.Attack = e.Amount;
                                        battle.ShowText(m.Name + "'s Attack " + direction + " by " + e.Amount + "!");
                                        break;
                                    case CardEffectStat.DEFENSE:
                                        ms.Defense = e.Amount;
                                        battle.ShowText(m.Name + "'s Defense " + direction + " by " + e.Amount + "!");
                                        break;
                                    case CardEffectStat.LEVEL:
                                        ms.Level = e.Amount;
                                        battle.ShowText(m.Name + "'s Level " + direction + " by " + e.Amount + "!");
                                        break;
                                }
                                m.EquippedBonuses.Add(new MonsterSpellBonus(Owner, -1, ms));
                            }
                            break;
                        case CardEffectAction.VIEW:
                            break;
                    }
                }

                effectIndex++;
            }

            if (this is Spell) {
                Owner.ManaAllotment -= Level;
                if (!(new[] { CardEffectType.CONTINUOUS, CardEffectType.EQUIP }.Contains(EffectType))) {
                    int fieldIndex = Owner.Field.FindIndex(this);
                    if (fieldIndex >= 0) Owner.DestroyCard(fieldIndex, false);
                    else {
                        foreach (Card c in Owner.Hand) {
                            if (c == this) {
                                Owner.Discard.Add(c);
                                Owner.Hand.Remove(c);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static List<Card> GetAllCards() {
            List<Card> cards = new List<Card>();
            for (int i = 1; i < 100; i++) {
                try {
                    Monster m = new Monster(i);
                    if (string.IsNullOrEmpty(m.Name)) break;
                    cards.Add(m);
                } catch (Exception) {
                    break;
                }
            }
            for (int i = 1; i < 100; i++) {
                try {
                    Spell s = new Spell(i);
                    if (string.IsNullOrEmpty(s.Name)) break;
                    cards.Add(s);
                } catch (Exception) {
                    break;
                }
            }
            return cards;
        }
    }
}
