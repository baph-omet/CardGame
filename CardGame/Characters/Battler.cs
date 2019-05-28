using System;
using System.Linq;
using System.Collections.Generic;
using CardGame.Cards;
using System.Xml.Serialization;
using CardGame.Scenes;

namespace CardGame.Characters {
    public abstract class Battler {
        public String Name;
        public List<Card> Deck;

        [XmlIgnore]
        public int Mana;
        [XmlIgnore]
        public int ManaAllotment;
        [XmlIgnore]
        public int MaxManaAllotment;
        [XmlIgnore]
        public Card[,] Field;
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
            this.Mana = 50;
            this.MaxManaAllotment = 1;
            this.ManaAllotment = 0;
            this.Field = new Card[2, 5];
            this.Hand = new List<Card>();
            this.Discard = new List<Card>();
            this.PlayDeck = new List<Card>(this.Deck);
            this.HasMoved = false;
            for (int i = 0; i < 5; i++) this.Draw();
        }

        public void Draw() {
            Draw(null);
        }
        public void Draw(Card card) {
            if (PlayDeck.Count > 0 && Hand.Count < 10 && PlayDeck.Contains(card)) {
                if (card == null) {
                    Random random = new Random();
                    card = PlayDeck[random.Next(PlayDeck.Count)];
                }
                Hand.Add(card);
                PlayDeck.Remove(card);
            }
        }

        public bool Summon(Monster monster, int fieldIndex) {
            if (this.Hand.Contains(monster) && Field[0,fieldIndex] == null && ManaAllotment >= monster.Level) {
                Field[0,fieldIndex] = monster;
                Hand.Remove(monster);
                ManaAllotment -= monster.Level;
                this.HasMoved = true;
                return true;
            } else return false;
        }

        public bool Withdraw(Monster monster) {
            for (int i = 0;i< this.Field.GetLength(0);i++) {
                if (this.Field[0,i].Equals(monster)) {
                    this.Hand.Add(monster);
                    this.Field[0, i] = null;
                    this.ManaAllotment += monster.Level;
                    this.HasMoved = true;
                    return true;
                }
            } return false;
        }

        public void RetireMonster(int fieldIndex) {
            if (fieldIndex > Field.GetLength(1)) return;
            if (Field[0,fieldIndex] != null) {
                Monster target = (Monster) Field[0, fieldIndex];
                Field[0, fieldIndex] = null;
                Discard.Add(target);
                //Mana += target.Level;
            }
        }

        public void RemoveSpell(int fieldIndex) {
            if (fieldIndex > Field.GetLength(1)) return;
            if (Field[1, fieldIndex] != null) {
                Discard.Add(Field[1, fieldIndex]);
                Field[1, fieldIndex] = null;
            }
        }

        public bool Set(Monster monster, int fieldIndex) {
            if (Summon(monster,fieldIndex)) {
                monster.Flip();
                return true;
            } else return false;
        }

        public void SetSpell(Spell spell, int fieldIndex) {
            Field[1, fieldIndex] = spell;
            Field[1, fieldIndex].Facedown = true;
        }

        public void StealMana(Monster attacker, Battler opponent) {
            if (CanStealMana(opponent) && attacker.CanAttack) {
                opponent.Mana -= attacker.Level;
                this.Mana += attacker.Level;
                attacker.CanAttack = false;
                this.HasMoved = true;
            }
        }

        public bool CanStealMana(Battler opponent) {
            return CanAttack() && !opponent.HasMonstersSummoned();
        }

        public bool CanAttack() {
            for (int i = 0; i < Field.GetLength(1); i++) if (Field[0, i] != null && ((Monster) Field[0, i]).CanAttack) return true;
            return false;
        }

        public bool CanMove() {
            //TODO: Sacrifice monsters to play better monsters
            if (!AllMonsterZonesFull()) {
                foreach (Card c in Hand) {
                    if (c.Level <= this.ManaAllotment && c.WillPlay) return true;
                }
            }
            for (int i = 0; i < Field.GetLength(1); i++) {
                Monster m = (Monster) Field[0, i];
                if (m != null && m.CanAttack) return true;
            }
            return false;
        }

        public bool HasMonstersSummoned() {
            for (int i = 0; i < Field.GetLength(1); i++) {
                if (Field[0, i] != null) return true;
            } return false;
        }

        public bool AllMonsterZonesFull() {
            for (int i = 0; i < Field.GetLength(1); i++) {
                if (Field[0, i] == null) return false;
            } return true;
        }

        public bool HasSpellCards() {
            for (int i = 0; i < Field.GetLength(1); i++) if (Field[1, i] != null) return true;
            return false;
        }

        public bool HasCounterSpells() {
            for (int i = 0; i < Field.GetLength(1); i++)  if (Field[1, i] != null && ((Spell) Field[1, i]).SpellType == SpellType.COUNTER) return true;
            return false;
        }

        public void ShuffleDeck() {
            PlayDeck = Util.Shuffle(PlayDeck, new Random());
        }

        public abstract Card ChooseSpellTarget(Battle battle, Spell spell, int spellEffectIndex);

        public new bool Equals(object obj) {
            if (this == null) return false;
            if (obj == null) return false;
            if (obj == this) return true;
            else return false;
        }
    }
}
