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
        public static int[] WindowSize = { 50, 63 };

        public static Player ActivePlayer;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Console.SetWindowSize(WindowSize[0], WindowSize[1]);
            Console.SetBufferSize(WindowSize[0], WindowSize[1]);
            Console.SetWindowPosition(0, 0);
            Console.CursorVisible = false;
            Console.Title = "Card Game";
            
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
    }
}
