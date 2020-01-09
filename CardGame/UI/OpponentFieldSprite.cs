using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Characters;
using CardGame.Cards;

namespace CardGame.UI {
    public class OpponentFieldSprite : Sprite {
        private Battler Battler;
        public int[] cursorLocation;
        
        public OpponentFieldSprite(Battler battler, int[] cursorlocation) {
            Battler = battler;
            cursorLocation = cursorlocation;
        }

        public string Render() {
            StringBuilder display = new StringBuilder();
            display.AppendLine(Battler.Name + " - Mana: (" + Battler.ManaAllotment + ") " + Battler.Mana);
            display.AppendLine("---------------------------------------------");

            String[,] spells = new String[2, 6];
            spells[0, 0] = "|DEC|";
            spells[1, 0] = "|" + String.Format("{0:000}", Battler.PlayDeck.Count) + "|";
            for (int i = 1; i < Battler.Field.Length + 1; i++) {
                Card card = Battler.Field.Spells[i - 1];
                if (card != null) {
                    if (card.Facedown) {
                        spells[0, i] = "   | ? |";
                        spells[1, i] = "   | ? |";
                    } else {
                        spells[0, i] = "   |" + card.Name.Substring(0, 3) + "|";
                        spells[1, i] = "   |" + ((Spell)card).SpellType.ToString().Substring(0, 3) + "|";
                    }
                } else {
                    spells[0, i] = "    [ ] ";
                    spells[1, i] = "    [ ] ";
                }
            }
            for (int i = 0; i < spells.GetLength(0); i++) {
                for (int j = 0; j < spells.GetLength(1); j++) {
                    display.Append(spells[i, j]);
                }
                display.Append('\n');
            }
            if (cursorLocation[0] == 0) {
                display.Append("        ");
                for (int i = 0; i <= cursorLocation[1]; i++) {
                    if (i == cursorLocation[1]) {
                        display.Append("  ^  ");
                        //selected = Battler.Field.Spells[i];
                    } else display.Append("        ");
                }
                display.Append('\n');
            } else display.AppendLine();

            String[,] monsters = new String[2, 6];
            monsters[0, 0] = "|DIS|";
            monsters[1, 0] = "|" + String.Format("{0:000}", Battler.Discard.Count) + "|";
            for (int i = 1; i < Battler.Field.Length + 1; i++) {
                Monster card = (Monster) Battler.Field.Monsters[i - 1];
                if (card != null) {
                    if (card.Facedown) {
                        monsters[0, i] = "   | ? |";
                        monsters[1, i] = "   | ? |";
                    } else {
                        monsters[0, i] = "   |" + String.Format("{0:000}", card.Attack) + "|";
                        monsters[1, i] = "   |" + String.Format("{0:000}", card.Defense) + "|";
                    }
                } else {
                    monsters[0, i] = "    [ ] ";
                    monsters[1, i] = "    [ ] ";
                }
            }
            for (int i = 0; i < monsters.GetLength(0); i++) {
                for (int j = 0; j < monsters.GetLength(1); j++) {
                    display.Append(monsters[i, j]);
                }
                display.AppendLine();
            }
            if (cursorLocation[0] == 1) {
                display.Append("        ");
                for (int i = 0; i <= cursorLocation[1]; i++) {
                    if (i == cursorLocation[1]) {
                        display.Append("  ^  ");
                        //selected = Battler.Field.Monsters[i];
                    } else display.Append("        ");
                }
                display.Append('\n');
            } else display.AppendLine();

            return display.ToString();
        }
    }
}
