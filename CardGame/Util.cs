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

        /*public static void Swap<T>(this IList<T> list, int i, int j) {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static void Shuffle<T>(this IList<T> list, Random rnd) {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }*/
    }
}
