using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Characters;
using CardGame.Cards;

namespace CardGame.Scenes {
    public class MainMenuScene : TextScene {
        public MainMenuScene() : base("Choose your option.") {
            AddChoice("Campaign", StartCampaign);
            AddChoice("Deck", OpenDeck);
            AddChoice("Chest", OpenChest);
            AddChoice("Player", ShowPlayerInfo);
            AddChoice("Quit", QuitToMenu);
        }
        
        public static void StartCampaign() {
            //TODO: Go to campaign screen
        }

        public static void OpenDeck() {
            Program.Scene.AddSubscene(new DeckScene(false));
        }

        public static void OpenChest() {
            Program.Scene.AddSubscene(new ChestScene(false));
        }

        public static void ShowPlayerInfo() {
            Player p = Program.ActivePlayer;
            StringBuilder pText = new StringBuilder("Name: " + p.Name +
                "\nLevel: " + p.Level +
                "\nWins: " + p.Wins +
                "\nLosses: " + p.Losses +
                "\nTies: " + p.Ties +
                "\nGames Played: " + (p.Wins + p.Losses + p.Ties) +
                "\nWin/Loss Ratio: " + String.Format("{0:0.00}", (p.Losses !=0 ? (double) p.Wins / (double) p.Losses : 0))
            );
            pText.AppendLine();
            foreach (NPC n in NPC.GetAllNPCs()) pText.AppendLine(
                n.Name + ": Wins: " + p.GetWins(n.ID)
                + " Losses: " + p.GetLosses(n.ID) 
                + " Ties: " + p.GetTies(n.ID) 
                + " Total: " + p.GetTotalGames(n.ID)
            );

            Program.Scene.AddSubscene(new TextScene("Player Stats", pText.ToString()));
        }

        public static void QuitToMenu() {
            TextScene confirm = new TextScene("Warning", "Are you sure you want to quit?");
            confirm.AddChoice("Yes", delegate() { Program.Scene = new TitleScene(); });
            confirm.AddChoice("No", delegate() { confirm.EndScene(); });
            Program.Scene.AddSubscene(confirm);
        }
    }
}
