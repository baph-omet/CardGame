using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using CardGame.Cards;
using CardGame.Characters;
using CardGame.Scenes;

namespace CardGame {
    static class Program {

        public static Scene Scene;
        public static bool Running;
        public static int[] WindowSize = { 50, 65 };

        public static Player ActivePlayer;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Console.SetWindowSize(WindowSize[0],WindowSize[1]);
            Console.SetWindowPosition(0, 0);
            Console.CursorVisible = false;
            
            Initialize();
            GameLoop();
        }

        private static void Initialize() {
            Running = true;
            Scene = new TitleScene();
        }

        private static void GameLoop() {
            while (Running) {
                if (Scene != null) Scene.Main();
                else Running = false;
            }
        }

        static void testBattleRender() {
            Player player = Player.GetPlayer(0);
            NPC opponent = NPC.GetNPC(1);
            Battle b = new Battle(player, opponent);
            b.Update();
        }

        static void playerWriteTest() {
            List<Player> players = Player.GetAllPlayers();

            //List<Player> players = new List<Player>();
            //players.Add(new Player(0));
            //players.Add(new Player(1));

            Player.WriteAllPlayers(players);
        }

        /*static void randomCardMatchups() {
            Random random = new Random();
            string thingy = "";
            while (thingy != "quit") {
                Console.Clear();
                Monster test = new Monster(random.Next(8) + 1);
                Console.WriteLine();
                Monster test2 = new Monster(random.Next(8) + 1);
                Console.WriteLine("Attacker:");
                Console.WriteLine(test.ToString());
                Console.WriteLine("Defender:");
                Console.WriteLine(test2.ToString());
                Monster winner = test.battle(test2);
                if (winner == null) Console.WriteLine("It's a draw!");
                else Console.WriteLine("The winner is: " + winner.name);
                Console.WriteLine("Type 'quit' to quit or hit ENTER to continue.");
                thingy = Console.ReadLine();
            }
        }*/
    }
}
