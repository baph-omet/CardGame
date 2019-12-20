using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Cards;
using CardGame.UI;

namespace CardGame.Scenes {
    public class ChestScene : TextScene {
        protected static SortedDictionary<int, int> Chest;
        protected static SortedDictionary<int, int> Deck;

        protected new static string Title = "Chest";

        protected bool SpellMode;

        public ChestScene(bool spellMode) : base(Title, (spellMode ? "Spells:" : "Monsters:")) {
            Chest = new SortedDictionary<int, int>();
            Deck = new SortedDictionary<int, int>();
            SpellMode = spellMode;
            UpdateCards();
        }

        public override void Update() {
            switch (Control.GetKey().Key) {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    SpellMode = !SpellMode;
                    UpdateCards();
                    break;
                case ConsoleKey.DownArrow:
                    if (ChoicesAvailable()) {
                        ChoiceIndex++;
                        if (ChoiceIndex >= Choices.Count) ChoiceIndex = 0;
                    }
                    Rerender = true;
                    break;
                case ConsoleKey.UpArrow:
                    if (ChoicesAvailable()) {
                        ChoiceIndex--;
                        if (ChoiceIndex < 0) ChoiceIndex = Choices.Count - 1;
                    }
                    Rerender = true;
                    break;
                case ConsoleKey.Enter:
                    if (ChoicesAvailable()) {
                        Action choice = Choices.Values.ToList()[ChoiceIndex];
                        choice();
                    } else {
                        TextIndex++;
                        if (TextIndex < Texts.Length) ((TextBox)SpriteHash["TextBox"]).Text = Texts[TextIndex];
                        else if (ChoicesAvailable()) ((TextBox)SpriteHash["TextBox"]).Choices = Choices.Keys.ToArray();
                        else EndScene();
                    }
                    Rerender = true;
                    break;
                case ConsoleKey.Escape:
                    if (ChoicesAvailable()) {
                        Action negativeAction = null;
                        string[] names = new[] { "Cancel", "No", "Back", "Return", "Exit", "Quit" };
                        foreach (string choice in Choices.Keys) {
                            foreach (string name in names) {
                                if (choice.Equals(name)) {
                                    negativeAction = Choices[choice];
                                    break;
                                }
                            }
                            if (negativeAction != null) break;
                        }
                        negativeAction?.Invoke();
                    }
                    break;
            }
            ((TextBox)SpriteHash["TextBox"]).Index = ChoiceIndex;
        }

        protected virtual void UpdateCards() {
            ClearChoices();
            Chest.Clear();
            Deck.Clear();

            Title = SpellMode ? "Spells" : "Monsters";
            foreach (Card c in Program.ActivePlayer.Chest) {
                if ((SpellMode && c is Spell) || (!SpellMode && c is Monster)) {
                    if (Chest.ContainsKey(c.ID)) Chest[c.ID]++;
                    else Chest.Add(c.ID,1);
                }
            }

            if (Chest.Count == 0) {
                SetTexts(new[] { "There are no cards in your chest. Win some battles to get some cards!" });
                return;
            }

            foreach (Card c in Program.ActivePlayer.Deck) {
                if ((SpellMode && c is Spell) || (!SpellMode && c is Monster)) {
                    if (Deck.ContainsKey(c.ID)) Deck[c.ID]++;
                    else Deck.Add(c.ID,1);
                }
            }

            foreach (int id in Chest.Keys) {
                Card c;
                if (SpellMode) c = new Spell(id);
                else c = new Monster(id);
                Dictionary<string, Action> choices = new Dictionary<string, Action>();
                choices.Add("Add to Deck", delegate() { AddToDeck(c); UpdateCards(); });
                choices.Add("Add to Chest", delegate() { AddToChest(c); UpdateCards(); });
                choices.Add("Back", delegate() { Program.Scene.EndSubscene(); });

                int noInDeck = 0;
                if (Deck.ContainsKey(c.ID)) noInDeck = Deck[c.ID];
                int noInChest = 0;
                if (Chest.ContainsKey(c.ID)) noInChest = Chest[c.ID];
                AddChoice(
                    "(" + id.ToString("000") + ") " + c.Name + " [" + noInChest + "-" + noInDeck + "]",
                    delegate() { AddSubscene(new TextScene(new CardSprite(c).Render() + "\nDeck: " + noInDeck + " Chest: " + noInChest, "What do you want to do?", choices)); }
                );
            }

            AddChoice("Back", delegate() { Program.Scene.EndSubscene(); });
        }

        public static void AddToDeck(Card c) {
            if (!Deck.ContainsKey(c.ID)) Deck.Add(c.ID, 0);
            if (Deck[c.ID] >= 3) {
                Program.Scene.AddSubscene(new TextScene("", "You can't add any more of this card to your deck."));
            } else if (!Chest.ContainsKey(c.ID)) {
                Program.Scene.AddSubscene(new TextScene("", "You don't have any more of this card in your chest."));
            } else if (Chest[c.ID] > 0) {
                bool removed = false;
                foreach (Card ca in Program.ActivePlayer.Chest) {
                    if (ca.ID == c.ID) {
                        Program.ActivePlayer.Chest.Remove(ca);
                        removed = true;
                        break;
                    }
                }
                if (!removed) {
                    Program.Scene.AddSubscene(new TextScene("Warning", "Could not remove card from chest."));
                    return;
                }
                Chest[c.ID]--;
                Program.ActivePlayer.Deck.Add(c);
                Deck[c.ID]++;
                Program.Scene.AddSubscene(new TextScene("", "Added to deck."));
                Program.ActivePlayer.Save();
            } else {
                Program.Scene.AddSubscene(new TextScene("", "You don't have any more to add."));
            }
        }

        public static void AddToChest(Card c) {
            if (!Chest.ContainsKey(c.ID)) Chest.Add(c.ID, 0);
            if (Deck[c.ID] > 0) {
                bool removed = false;
                foreach (Card ca in Program.ActivePlayer.Deck) {
                    if (ca.ID == c.ID) {
                        Program.ActivePlayer.Deck.Remove(ca);
                        removed = true;
                        break;
                    }
                }
                if (!removed) {
                    Program.Scene.AddSubscene(new TextScene("Warning", "Could not remove card from deck."));
                    return;
                }
                Deck[c.ID]--;
                Program.ActivePlayer.Chest.Add(c);
                Chest[c.ID]++;
                Program.Scene.AddSubscene(new TextScene("", "Added to chest."));
                Program.ActivePlayer.Save();
            } else {
                Program.Scene.AddSubscene(new TextScene("", "You don't have any more to add."));
            }
        }
    }
}
