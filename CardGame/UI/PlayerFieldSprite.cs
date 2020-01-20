using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Characters;
using CardGame.Cards;
using ConsoleUI;

namespace CardGame.UI {
    public class PlayerFieldSprite : Sprite {
        private Battler Battler;
        public int[] cursorLocation;
        
        public PlayerFieldSprite(Battler battler, int[] cursorlocation) {
            Battler = battler;
            cursorLocation = cursorlocation;
        }

        public string Render() {
            StringBuilder display = new StringBuilder();
            display.AppendLine();

            if (cursorLocation[0] == 2) {
                display.Append("        ");
                for (int i = 0; i <= cursorLocation[1]; i++) {
                    if (i == cursorLocation[1]) {
                        display.Append("  v  ");
                        // selected = Battler.Field.Monsters[i];
                    } else display.Append("        ");
                }
                display.Append('\n');
            } else display.AppendLine();

            string[,] monsters2 = new string[2, 6];
            monsters2[0, 0] = "|DIS|";
            monsters2[1, 0] = "|" + string.Format("{0:000}", Battler.Discard.Count) + "|";
            for (int i = 1; i < Battler.Field.Length + 1; i++) {
                Monster card = (Monster) Battler.Field.Monsters[i - 1];
                if (card != null) {
                    if (card.Facedown) {
                        monsters2[0, i] = "   | ? |";
                        monsters2[1, i] = "   | ? |";
                    } else {
                        monsters2[0, i] = "   |" + string.Format("{0:000}", card.Attack) + "|";
                        monsters2[1, i] = "   |" + string.Format("{0:000}", card.Defense) + "|";
                    }
                } else {
                    monsters2[0, i] = "    [ ] ";
                    monsters2[1, i] = "    [ ] ";
                }
            }

            for (int i = 0; i < monsters2.GetLength(0); i++) {
                for (int j = 0; j < monsters2.GetLength(1); j++) {
                    display.Append(monsters2[i, j]);
                }
                display.AppendLine();
            }

            if (cursorLocation[0] == 3) {
                display.Append("        ");
                for (int i = 0; i <= cursorLocation[1]; i++) {
                    if (i == cursorLocation[1]) {
                        display.Append("  v  ");
                        // selected = Battler.Field.Spells[i];
                    } else display.Append("        ");
                }
                display.Append('\n');
            } else display.AppendLine();

            string[,] spells2 = new string[2, 6];
            spells2[0, 0] = "|DEC|";
            spells2[1, 0] = "|" + string.Format("{0:000}", Battler.PlayDeck.Count) + "|";
            for (int i = 1; i < Battler.Field.Length + 1; i++) {
                Card card = Battler.Field.Spells[i - 1];
                if (card != null) {
                    if (card.Facedown) {
                        spells2[0, i] = "   | ? |";
                        spells2[1, i] = "   | ? |";
                    } else {
                        spells2[0, i] = "   |" + card.Name.Substring(0, 3) + "|";
                        spells2[1, i] = "   |" + ((Spell)card).SpellType.ToString().Substring(0, 3) + "|";
                    }
                } else {
                    spells2[0, i] = "    [ ] ";
                    spells2[1, i] = "    [ ] ";
                }
            }
            for (int i = 0; i < spells2.GetLength(0); i++) {
                for (int j = 0; j < spells2.GetLength(1); j++) {
                    display.Append(spells2[i, j]);
                }
                display.Append('\n');
            }

            display.AppendLine("---------------------------------------------");
            display.AppendLine(Battler.Name + " - Mana: (" + Battler.ManaAllotment + ") " + Battler.Mana);

            display.AppendLine();

            return display.ToString();
        }
    }
}
