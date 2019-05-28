using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGame {
    class Control {

        public static ConsoleKeyInfo getKey() {
            if (Console.KeyAvailable) return Console.ReadKey(true);
            return new ConsoleKeyInfo();
        }

        public static void waitForKey() {
            getKey();
        }
        public static void waitForKey(ConsoleKey key) {
            while (getKey().Key != key) { }
        }

        public static ConsoleKey waitForKey(ConsoleKey[] keys) {
            while (true) {
                ConsoleKey key = getKey().Key;
                foreach (ConsoleKey k in keys) {
                    if (k == key) return key;
                }
            }
        }

        public static bool IsTextCharacter(ConsoleKeyInfo key) {
            return Char.IsLetterOrDigit(key.KeyChar) || key.Key == ConsoleKey.Spacebar;
        }
    }
}
