using System;
using System.Linq;
using System.Collections.Generic;
using CardGame.Cards;
using System.Xml.Serialization;
using CardGame.Scenes;

namespace CardGame.Characters {
    public abstract class Battler {
        public string Name;
        public List<Card> Deck;

        [XmlIgnore]
        public int Mana;
        [XmlIgnore]
        public int ManaAllotment;
        [XmlIgnore]
        public int MaxManaAllotment;
        [XmlIgnore]
        public BattlerField Field;
        [XmlIgnore]
        public List<Card> PlayDeck;
        [XmlIgnore]
        public List<Card> Hand;
        [XmlIgnore]
        public List<Card> Discard;
        [XmlIgnore]
        public bool HasMoved;

        public Battler() {
            Deck = new List<Card>();
            PlayDeck = new List<Card>();
            Hand = new List<Card>();
            Discard = new List<Card>();
        }

        public void Initialize() {
            Mana = 50;
            MaxManaAllotment = 1;
            ManaAllotment = 0;
            Field = new BattlerField();
            Hand = new List<Card>();
            Discard = new List<Card>();
            PlayDeck = new List<Card>(Deck);
            HasMoved = false;
            for (int i = 0; i < 5; i++) Draw();
        }

        public void Draw() {
            Draw(null);
        }
        public void Draw(Card card) {
            if (card == null) {
                Random random = new Random();
                card = PlayDeck[random.Next(PlayDeck.Count)];
            }
            if (PlayDeck.Count > 0 && Hand.Count < 10 && PlayDeck.Contains(card)) {
                Hand.Add(card);
                PlayDeck.Remove(card);
            }
        }

        public bool Summon(Monster monster, int fieldIndex) {
            if (Hand.Contains(monster) && Field.Monsters[fieldIndex] == null && ManaAllotment >= monster.Level) {
                Field.Monsters[fieldIndex] = monster;
                Hand.Remove(monster);
                ManaAllotment -= monster.Level;
                HasMoved = true;
                return true;
            } else return false;
        }

        public bool Withdraw(Monster monster) {
            for (int i = 0;i<Field.Length;i++) {
                if (Field.Monsters[i].Equals(monster)) {
                    Hand.Add(monster);
                    Field.Monsters[ i] = null;
                    ManaAllotment += monster.Level;
                    HasMoved = true;
                    return true;
                }
            } return false;
        }

        public void RetireMonster(int fieldIndex) {
            if (fieldIndex >Field.Length) return;
            if (Field.Monsters[fieldIndex] != null) {
                Monster target = (Monster) Field.Monsters[ fieldIndex];
                Field.Monsters[ fieldIndex] = null;
                Discard.Add(target);
                //Mana += target.Level;
            }
        }

        public void RemoveSpell(int fieldIndex) {
            if (fieldIndex > Field.Length) return;
            if (Field.Spells[fieldIndex] != null) {
                Discard.Add(Field.Spells[ fieldIndex]);
                Field.Spells[fieldIndex] = null;
            }
        }

        public bool Set(Monster monster, int fieldIndex) {
            if (Summon(monster,fieldIndex)) {
                monster.Flip();
                return true;
            } else return false;
        }

        public void SetSpell(Spell spell, int fieldIndex) {
            if (Hand.Contains(spell) && Field.Spells[ fieldIndex] == null && ManaAllotment >= spell.Level) {
                Field.Spells[ fieldIndex] = spell;
                Field.Spells[ fieldIndex].Facedown = true;
                HasMoved = true;
                Hand.Remove(spell);
                ManaAllotment -= spell.Level;
            }
        }

        public void StealMana(Monster attacker, Battler opponent, bool direct = false) {
            if (!attacker.CanAttack || (!direct && !CanStealMana(opponent))) return;
            StealMana(attacker.Level, opponent, direct);
            attacker.CanAttack = false;
            HasMoved = true;
        }
        public void StealMana(int amount, Battler opponent, bool direct = false) {
            if (!direct && !CanStealMana(opponent)) return;
            opponent.Mana -= amount;
            Mana += amount;
        }

        public bool CanStealMana(Battler opponent) {
            return CanAttack() && !opponent.HasMonstersSummoned();
        }

        public bool CanAttack() {
            for (int i = 0; i <Field.Length; i++) if (Field.Monsters[ i] != null && ((Monster) Field.Monsters[ i]).CanAttack) return true;
            return false;
        }

        public bool CanMove() {
            //TODO: Sacrifice monsters to play better monsters
            if (!AllMonsterZonesFull()) {
                foreach (Card c in Hand) {
                    if (c.Level <= ManaAllotment && c.WillPlay) return true;
                }
            }
            for (int i = 0; i <Field.Length; i++) {
                Monster m = (Monster) Field.Monsters[ i];
                if (m != null && m.CanAttack) return true;
            }
            return false;
        }

        public bool HasMonstersSummoned() {
            for (int i = 0; i <Field.Length; i++) {
                if (Field.Monsters[ i] != null) return true;
            } return false;
        }

        public bool AllMonsterZonesFull() {
            for (int i = 0; i <Field.Length; i++) {
                if (Field.Monsters[ i] == null) return false;
            } return true;
        }

        public bool HasSpellCards() {
            for (int i = 0; i <Field.Length; i++) if (Field.Spells[ i] != null) return true;
            return false;
        }

        public bool HasCounterSpells() {
            for (int i = 0; i <Field.Length; i++)  if (Field.Spells[ i] != null && ((Spell) Field.Spells[ i]).SpellType == SpellType.COUNTER) return true;
            return false;
        }

        public void ShuffleDeck() {
            PlayDeck = Util.Shuffle(PlayDeck, new Random());
        }

        public abstract Card ChooseEffectTarget(Battle battle, Card spell, int spellEffectIndex);

        public new bool Equals(object obj) {
            if (this == null) return false;
            if (obj == null) return false;
            if (obj == this) return true;
            else return false;
        }
    }

    public class BattlerField {
        public Spell[] Spells { get; set; }
        public Monster[] Monsters { get; set; }
        public int Length { get { return 5; } }

        public BattlerField() {
            Spells = new Spell[5];
            Monsters = new Monster[5];
        }
    }
}
