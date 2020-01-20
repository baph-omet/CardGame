using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Characters;
using CardGame.UI;
using ConsoleUI;

namespace CardGame.Scenes {
    public class NewGameScene : TextScene {

        private int index = 0;

        public NewGameScene() : base("New Game", Message.GetMessage("NewGame").Lines) { }

        public override void Update() {
            base.Update();
            if (TextIndex >= Texts.Length) {
                index++;
                switch (index) {
                    case 1:
                        AddSubscene(new TextEnterScene("What is your name?", false));
                        Program.SceneManager.AddSubscene(this);
                        break;
                    case 2:
                        string typedText = ((TextEnterScene) Subscene).TypedText;
                        Program.ActivePlayer = Player.CreatePlayer(typedText);
                        SetTexts(new string[] {
                            "Welcome, " + Program.ActivePlayer.Name + "!",
                            "Your journey to be the best starts here. Good luck!"
                        });
                        NextScene(new MainMenuScene());
                        break;
                }
            }
        }
    }
}
