using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.UI;

namespace CardGame.Scenes {
    public class TextEnterScene : Scene {
        private int CursorLocation = 0;
        private const int CharLimit = 8;

        private string Prompt;
        public string TypedText = "";

        private bool Cancellable;

        public TextEnterScene(string prompt) : this(prompt, true) { }
        public TextEnterScene(string prompt,bool cancellable) : base() {
            Prompt = prompt;
            Cancellable = cancellable;

            SpriteHash.Add("TextBox", new TextBox(prompt,"_"));
            SpriteHash.Add("WarningBox", null);
        }

        public override void Update() {
            ConsoleKeyInfo key = Control.getKey();
            if (key.Key == ConsoleKey.Enter) {
                if (ShowingWarning()) {
                    HideWarning();
                    Rerender = true;
                } else {
                    if (!Cancellable && TypedText.Length == 0) {
                        ShowWarning("This text entry is mandatory, kiddo.");
                        Rerender = true;
                    } else Hide();
                }
            } else if (key.Key == ConsoleKey.Escape) {
                if (ShowingWarning()) {
                    HideWarning();
                } else {
                    if (Cancellable) {
                        TypedText = "";
                        Hide();
                    } else {
                        ShowWarning("This text entry is mandatory, kiddo.");
                        Rerender = true;
                    }
                }
            } else if (key.Key == ConsoleKey.Backspace && !ShowingWarning() && CursorLocation > 0) {
                TypedText = TypedText.Remove(CursorLocation - 1, 1);
                CursorLocation--;
            } else if (key.Key == ConsoleKey.Delete && !ShowingWarning() && CursorLocation < TypedText.Length) {
                TypedText = TypedText.Remove(CursorLocation,1);
            } else if (key.Key == ConsoleKey.LeftArrow && !ShowingWarning() && CursorLocation > 0) {
                CursorLocation--;
                Rerender = true;
            } else if (key.Key == ConsoleKey.RightArrow && !ShowingWarning() && CursorLocation < TypedText.Length) {
                CursorLocation++;
                Rerender = true;
            } else if (Control.IsTextCharacter(key) && !ShowingWarning() && TypedText.Length < CharLimit) {
                if (CursorLocation < TypedText.Length) TypedText = TypedText.Insert(CursorLocation, key.KeyChar.ToString());
                else TypedText += key.KeyChar.ToString();
                CursorLocation++;
            }

            TextBox tb = ((TextBox)SpriteHash["TextBox"]);

            if (TypedText != tb.Text.Replace("_", "").Replace("|", "") || Rerender) {
                if (CursorLocation == TypedText.Length) {
                    if (CursorLocation <= CharLimit) tb.Text = TypedText + "_";
                    else tb.Text = TypedText;
                } else tb.Text = TypedText.Insert(CursorLocation, "|");
                Rerender = true;
            }
        }

        private bool ShowingWarning() {
            return SpriteHash["WarningBox"] != null;
        }

        private void HideWarning() {
            SpriteHash["WarningBox"] = null;
        }

        private void ShowWarning(string warning) {
            if (!ShowingWarning()) SpriteHash["WarningBox"] = new TextBox(warning);
            else ((TextBox)SpriteHash["WarningBox"]).Text = warning;
        }
    }
}
