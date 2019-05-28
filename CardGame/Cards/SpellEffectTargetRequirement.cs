using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGame.Cards {


    public class SpellEffectTargetRequirement {
        public SpellEffectStat Stat { get; set; }

        public int Minimum { get; set; }

        public int Maximum { get; set; }
    }
}
