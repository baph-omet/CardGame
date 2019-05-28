using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Characters;

namespace CardGame.Scenes {
    class TitleScene : TextScene {
        public TitleScene() : base("Card Game","Test text",InitializeChoices()) {}

        private static Dictionary<string, Action> InitializeChoices() {
            Dictionary<string, Action> d = new Dictionary<string, Action>();
            d.Add("New Game", NewGame);
            d.Add("Load Game", LoadGame);
            d.Add("Test Battle", TestBattle);
            //d.Add("Format Player File", FormatPlayerFile);
            d.Add("Quit", Quit);
            return d;
        }

        public static void Quit() {
            Program.Scene = null;
        }

        public static void TestBattle() {
            Player player = Player.GetPlayer(1);
            NPC opponent = NPC.GetNPC(1);
            Battle b = new Battle(player, opponent);
            b.Update();
        }

        public static void NewGame() {
            Program.Scene = new NewGameScene();
        }

        public static void LoadGame() {
            TextScene loadscreen = new TextScene("Choose your player", "", new Dictionary<string, Action>());
            List<Player> players = Player.GetAllPlayers();
            foreach (Player p in players) {
                if (p.ID > 0) loadscreen.AddChoice(p.Name + " (" + p.Level + ")", delegate() { LoadPlayer(p.ID); });
            }
            loadscreen.AddChoice("Cancel", delegate() { Program.Scene = new TitleScene(); });
            Program.Scene = loadscreen;
        }

        public static void LoadPlayer(int ID) {
            Player p = Player.GetPlayer(ID);
            Program.ActivePlayer = p;
            Program.Scene.NextScene(new MainMenuScene());
        }

        public static void FormatPlayerFile() {
            List<Player> asdf = new List<Player>();
            asdf.Add(new Player());
            Player.WriteAllPlayers(asdf);
        }
    }
}
