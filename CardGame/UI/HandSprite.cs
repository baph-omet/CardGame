using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Cards;
using CardGame.Characters;
using ConsoleUI;

namespace CardGame.UI {
    public class HandSprite : Sprite {
        private Battler Battler;
        private int[] cursorLocation;
        
        public HandSprite(Battler battler, int[] cursorlocation) {
            Battler = battler;
            cursorLocation = cursorlocation;
        }

        public string Render() {
            StringBuilder display = new StringBuilder();
            if (Battler is Player) {
                string[, ,] handCards = new string[3, 3, 6];

                for (int k = 0; k < handCards.GetLength(0); k++) {
                    for (int i = 0; i < handCards.GetLength(1); i++) {
                        int cardIndex = i + (k * 3);
                        if (cardIndex < Battler.Hand.Count) {
                            Card card = Battler.Hand[cardIndex];
                            handCards[k, i, 0] = " __________  ";
                            handCards[k, i, 1] = "| " + string.Format("{0:000}", card.ID)
                                + "-" + card.Name.Substring(0, 4) + " | ";
                            StringBuilder lvl = new StringBuilder();
                            if (card is Spell) lvl.Append("| SP-");
                            else lvl.Append("| " + ((Monster)card).Stats.Type.ToString().Substring(0, 2).ToUpper() + "-");
                            if (card.Level < 5) {
                                for (int l = 0; l < 5; l++) {
                                    if (l < card.Level) lvl.Append('*');
                                    else lvl.Append(" ");
                                }
                            } else lvl.Append("*" + card.Level + "   ");
                            lvl.Append(" | ");
                            handCards[k, i, 2] = lvl.ToString();
                            if (card is Monster) {
                                Monster mon = (Monster) card;
                                handCards[k, i, 3] = "| ATK-" + string.Format("{0:0000}", mon.Attack) + " | ";
                                handCards[k, i, 4] = "| DEF-" + string.Format("{0:0000}", mon.Defense) + " | ";
                            } else {
                                string spellType;
                                Spell spell = (Spell) card;
                                if (spell.SpellType.ToString().Length < 8) spellType = spell.SpellType.ToString();
                                else spellType = spell.SpellType.ToString().Substring(0, 8);
                                handCards[k, i, 3] = "| " + string.Format("{0,-8}", spellType.ToUpper()) + " | ";
                                handCards[k, i, 4] = "|          | ";
                            }
                            handCards[k, i, 5] = " ----------  ";
                        }
                    }
                }

                for (int row = 0; row < handCards.GetLength(0); row++) {
                    if (cursorLocation[0] == (4 + row)) {
                        for (int i = 0; i <= cursorLocation[1] && i < 3; i++) {
                            if (i == cursorLocation[1] && (cursorLocation[0] - 4) * 3 + cursorLocation[1] < Battler.Hand.Count) {
                                display.Append("     v      ");
                            } else display.Append("             ");
                        }
                        display.AppendLine();
                    } else display.AppendLine();
                    int numNonNullLines = 0;
                    for (int line = 0; line < handCards.GetLength(2); line++) {
                        int numNonNullCrds = 0;
                        for (int crd = 0; crd < handCards.GetLength(1); crd++) {
                            if (handCards[row, crd, line] != null) {
                                display.Append(handCards[row, crd, line]);
                                numNonNullCrds += 1;
                            }
                        }
                        if (numNonNullCrds > 0) display.AppendLine();
                    }
                    if (numNonNullLines > 0) display.AppendLine();
                }
            }
            return display.ToString();
        }
    }
}
