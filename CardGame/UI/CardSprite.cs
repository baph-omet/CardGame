using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Cards;

namespace CardGame.UI {
    public class CardSprite : Sprite {
        private Card Card;

        public CardSprite(Card card) {
            Card = card;
        }

        public string Render() {
            StringBuilder cardDetail = new StringBuilder(" ____________________________________________\n");

            cardDetail.Append("| (" + string.Format("{0:000}", Card.ID) + ") " +
                (Card.Name.Length > 35 ? Card.Name.Substring(0, 35) : Card.Name) + ":");
            for (int i = 0; i < 35 - Card.Name.Length; i++) cardDetail.Append(" ");
            cardDetail.AppendLine(" |");

            StringBuilder lvl = new StringBuilder();
            for (int i = 0; i < 9; i++) {
                if (i < Card.Level) lvl.Append('*');
                else lvl.Append(" ");
            }
            cardDetail.AppendLine("| " + string.Format("{0,-42}", (Card is Monster ? ((Monster)Card).OriginalStats.Type.ToString() : "SPELL")
                + " - " + lvl.ToString()) + " |");
            if (Card is Monster) {
                Monster mon = (Monster) Card;
                cardDetail.AppendLine("| " + string.Format("{0,-42}", "Attack:  " + string.Format("{0:0000}", mon.Attack)) + " |");
                cardDetail.AppendLine("| " + string.Format("{0,-42}", "Defense: " + string.Format("{0:0000}", mon.Defense)) + " |");
            } else {
                Spell spell = (Spell) Card;
                cardDetail.AppendLine("| " + string.Format("{0,-42}", spell.SpellType.ToString()) + " |");
                cardDetail.AppendLine();
            }

            if (Card.Description.Length > 42) {
                string[] words = Card.Description.Split(' ');
                StringBuilder message = new StringBuilder();
                for (int i = 0; i < words.Length; i++) {
                    if (message.Length > 0) message.Append(' ');
                    message.Append(words[i]);
                    if (i == words.Length - 1 || message.Length + words[i + 1].Length + 1 > 42) {
                        cardDetail.AppendLine("| " + string.Format("{0,-42}", message.ToString()) + " |");
                        message.Clear();
                    }
                }
            }
            cardDetail.AppendLine(" --------------------------------------------");
            return cardDetail.ToString();
        }
    }
}
