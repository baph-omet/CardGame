using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.UI;

namespace CardGame.Scenes {
    public class TextScene : Scene {
        public string Title;
        public string[] Texts;
        protected Dictionary<string, Action> Choices;
        
        protected int TextIndex;
        protected int ChoiceIndex {
            get {
                return ((TextBox)SpriteHash["TextBox"]).Index;
            } set {
                if (value >= Choices.Count) value = 0;
                if (value < 0) value = Choices.Count - 1;
                ((TextBox)SpriteHash["TextBox"]).Index = value;
            }
        }

        public TextScene(string title) : this(title, "") { Texts = null; }
        public TextScene(string title, string text) : this(title, text, new Dictionary<string, Action>()) { }
        public TextScene(string title, string[] texts) : this(title, texts, new Dictionary<string, Action>()) { }
        public TextScene(string title, string text, Dictionary<string,Action> choices) : this(title, new string[] {text},choices) {}
        public TextScene(string title, string[] texts, Dictionary<string, Action> choices) : base() {
            Title = title;
            Texts = texts;
            Choices = choices;
            TextIndex = 0;

            SpriteHash.Add("TextBox", new TextBox(Title, Texts[0], (ChoicesAvailable() ? Choices.Keys.ToArray() : null)));
        }

        public override void Update() {
            switch (Control.getKey().Key) {
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
                        if (TextIndex < Texts.Length) ((TextBox) SpriteHash["TextBox"]).Text = Texts[TextIndex];
                        else if (ChoicesAvailable()) ((TextBox) SpriteHash["TextBox"]).Choices = Choices.Keys.ToArray();
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
                        } negativeAction?.Invoke();
                    } break;
            }
            ((TextBox) SpriteHash["TextBox"]).Index = ChoiceIndex;
        }

        private bool ChoicesAvailable() {
            return Choices != null && Choices.Keys.Count > 0 /*&& TextIndex == Texts.Length - 1*/;
        }

        public void AddChoice(string key, Action action) {
            if (Choices == null) Choices = new Dictionary<string, Action>();
            int i = 1;
            bool iteratorFound = !Choices.ContainsKey(key);
            while (!iteratorFound) {
                if (Choices.ContainsKey(key + " [" + i + "]")) i++;
                else {
                    key += " [" + i + "]";
                    iteratorFound = true;
                }
            }
            Choices.Add(key, action);
            ((TextBox) SpriteHash["TextBox"]).Choices = Choices.Keys.ToArray();
        }

        public void ClearChoices() {
            Choices = new Dictionary<string, Action>();
        }

        public void SetTexts(string[] newTexts) {
            Texts = newTexts;
            TextIndex = 0;
            ((TextBox) SpriteHash["TextBox"]).Text = Texts[TextIndex];
            Rerender = true;
        }
    }
}
