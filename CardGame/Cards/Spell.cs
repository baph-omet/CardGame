using System;
using System.Collections.Generic;
using CardGame.Scenes;
using CardGame.Characters;
using Vergil.XML;
using System.Xml.Serialization;
using System.Linq;

namespace CardGame.Cards {

    public class Spell : Card {
        [XmlIgnore]
        private CardEffectType spelltype;
        [XmlIgnore]
        public CardEffectType SpellType { get { return spelltype; } }

        [XmlIgnore]
        private CardEffectTrigger spelltrigger;
        [XmlIgnore]
        public CardEffectTrigger Trigger { get { return spelltrigger; } }

        [XmlIgnore]
        private bool voluntary;
        [XmlIgnore]
        public bool TriggerVoluntary { get { return voluntary; } }

        [XmlIgnore]
        private int triggeramount;
        [XmlIgnore]
        public int TriggerAmount { get { return triggeramount; } }

        [XmlIgnore]
        public bool Activated = false;

        public Spell() : this(0) {}
        public Spell(int id) {
            ID = id;
            XMLFile file = new XMLFile("Data/Spell.xml");
            foreach (XMLSection spell in file.GetSections()[0].GetSections("spell")) {
                if (Convert.ToInt32(spell.GetAttribute("id")) == id) {
                    Name = spell.Get("name", "Unknown");
                    Description = spell.Get("description", "Description not found.");
                    voluntary = spell.Get("voluntary",true);
                    Level = spell.Get("level",0);
                    spelltype = spell.GetEnum("type",CardEffectType.INSTANT);
                    spelltrigger = spell.GetEnum("trigger",CardEffectTrigger.IMMEDIATE);
                    triggeramount = spell.Get("triggeramount",0);

                    effects = new List<CardEffect>();
                    foreach (XMLSection e in spell.GetSections("effect")) {
                        CardEffect effect = new CardEffect();
                        
                        effect.Range = e.GetEnum("range",CardEffectTargetRange.ANY);
                        effect.Action = e.GetEnum<CardEffectAction>("action");
                        effect.TargetAssignment = e.GetEnum("assignment",CardEffectTargetAssignment.NONE);
                        if (effect.TargetAssignment == CardEffectTargetAssignment.PREVIOUS && effects.Count > 0) effect.TargetType = effects.Last().TargetType;
                        else effect.TargetType = e.GetEnum<CardEffectTargetType>("target");
                        effect.EffectStat = e.GetEnum("stat", CardEffectStat.NULL);
                        effect.Amount = e.Get("amount", 0);

                        effect.Requirements = new List<CardEffectTargetRequirement>();
                        foreach (XMLSection r in e.GetSections("requirement")) {
                            CardEffectTargetRequirement requirement = new CardEffectTargetRequirement();
                            requirement.Stat = r.GetEnum<CardEffectStat>("stat");
                            requirement.Maximum = r.Get("maximum", 9999);
                            requirement.Minimum = r.Get("minimum",0);
                            effect.Requirements.Add(requirement);
                        }

                        effects.Add(effect);
                    }

                    break;
                }
            }
        }

        public void TriggerEffects(object sender, BattleEventArgs args) {
            ResolveEffects((Battle)sender);
        }
    }
}