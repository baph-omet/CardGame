using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGame {
    public class Util {
        public static List<T> Shuffle<T>(List<T> list, Random rnd) {
            for (int t = 0; t < 5; t++) {
                for (int i = 0; i < list.Count; i++) {
                    int j = rnd.Next(i, list.Count);
                    T item = list[i];
                    list[i] = list[j];
                    list[j] = item;
                }
            } return list;
        }
    }
}
