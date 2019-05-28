using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGame.Cards {
    public enum MonsterType {
        FIRE,
        WATER,
        ELECTRIC,
        EARTH,
        WIND,
        FOREST,
        DARK,
        LIGHT,
        NONE
    }

    public static class MonsterTypes {
        private static MonsterType[] lowTypes = {
            MonsterType.FIRE,
            MonsterType.WATER,
            MonsterType.ELECTRIC,
            MonsterType.EARTH,
            MonsterType.WIND,
            MonsterType.FOREST
        };

        private static MonsterType[] highTypes = {
            MonsterType.DARK,
            MonsterType.LIGHT
        };

        private static MonsterType[] internalTypes = {
            MonsterType.NONE
        };

        public static MonsterType GetWeakness(MonsterType type) {
            foreach (MonsterType[] typegroup in new MonsterType[][] { lowTypes, highTypes, internalTypes }) {
                for (int i = 0; i < typegroup.Length; i++) {
                    if (type == typegroup[i]) return typegroup[(i + 1) % typegroup.Length];
                }
            }
            return MonsterType.NONE;
        }
    }
}
