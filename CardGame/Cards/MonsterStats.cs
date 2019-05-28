using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGame.Cards {
    public class MonsterStats {
        public int Attack { get; set; }

        public int Defense { get; set; }

        public int Level { get; set; }

        public MonsterType Type { get; set; }

        public MonsterStats() : this(0, 0, 0, MonsterType.NONE) { }
        public MonsterStats(int attack, int defense, int level, MonsterType type) {
            Attack = attack;
            Defense = defense;
            Level = level;
            Type = type;
        }

        /// <summary>
        /// Adds two stat objects together to get a composite stat object. The Type property of the first stat object is overridden by the second.
        /// </summary>
        /// <param name="s1">The original stat object</param>
        /// <param name="s2">The stat modification to add in</param>
        /// <returns></returns>
        public static MonsterStats operator +(MonsterStats s1, MonsterStats s2) {
            return new MonsterStats(s1.Attack + s2.Attack, s1.Defense + s2.Defense, s1.Level + s2.Level, s2.Type);
        }
    }
}
