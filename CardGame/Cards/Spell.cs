using System;
using System.Collections.Generic;
using CardGame.Scenes;
using CardGame.Characters;
using SOPAPI.XML;
using System.Xml.Serialization;

namespace CardGame.Cards {
    public enum SpellType {
        INSTANT,
        CONTINUOUS,
        EQUIP,
        COUNTER
    }

    public enum SpellTargetFacedownMode {
        FACEUP,
        FACEDOWN,
        ANY
    }

    public enum SpellTrigger {
        IMMEDIATE,
        DRAW,
        SUMMON,
        SPELL,
        SPELLDESTROYED,
        SETMONSTER,
        FLIPMONSTER,
        SETSPELL,
        TURNSTART,
        TURNEND,
        MONSTERDESTROYED,
        MANASTEAL,
        MONSTERATTACK,
        MANAALLOTMENT,
        MANA,
        MANACHANGE
    }

    public class Spell : Card {
        [XmlIgnore]
        private SpellType spelltype;
        [XmlIgnore]
        public SpellType SpellType { get { return spelltype; } }

        [XmlIgnore]
        private SpellTrigger spelltrigger;
        [XmlIgnore]
        public SpellTrigger Trigger { get { return spelltrigger; } }

        [XmlIgnore]
        private List<SpellEffect> effects;
        [XmlIgnore]
        public List<SpellEffect> Effects { get { return effects; } }

        [XmlIgnore]
        private bool voluntary;
        [XmlIgnore]
        public bool TriggerVoluntary { get { return voluntary; } }

        [XmlIgnore]
        private int triggeramount;
        [XmlIgnore]
        public int TriggerAmount { get { return triggeramount; } }

        public bool Activated = false;

        public Spell() : this(0) {}
        public Spell(int id) {
            this.ID = id;
            XMLFile file = new XMLFile("Data/Spell.xml");
            foreach (XMLSection spell in file.GetSections()[0].GetSections("spell")) {
                if (Convert.ToInt32(spell.GetAttribute("id")) == id) {
                    Name = spell.Get("name", "Unknown");
                    Description = spell.Get("description", "Description not found.");
                    voluntary = spell.Get("voluntary",true);
                    Level = spell.Get("level",0);
                    spelltype = (SpellType) spell.GetEnum("type",SpellType.INSTANT);
                    spelltrigger = (SpellTrigger) spell.GetEnum("trigger",SpellTrigger.IMMEDIATE);
                    triggeramount = spell.Get("triggeramount",0);

                    this.effects = new List<SpellEffect>();
                    foreach (XMLSection e in spell.GetSections("effect")) {
                        SpellEffect effect = new SpellEffect();
                        effect.TargetType = e.GetEnum<SpellEffectTargetType>("target");
                        effect.Range = e.GetEnum("range",SpellEffectTargetRange.ANY);
                        effect.Action = e.GetEnum<SpellEffectAction>("action");
                        effect.TargetAssignment = e.GetEnum("assignment",SpellEffectTargetAssignment.CHOOSE);
                        effect.EffectStat = e.GetEnum("stat", SpellEffectStat.NULL);
                        effect.Amount = e.Get("amount", 0);

                        effect.Requirements = new List<SpellEffectTargetRequirement>();
                        foreach (XMLSection r in e.GetSections("requirement")) {
                            SpellEffectTargetRequirement requirement = new SpellEffectTargetRequirement();
                            requirement.Stat = r.GetEnum<SpellEffectStat>("stat");
                            requirement.Maximum = r.Get("maximum", 9999);
                            r.Get("minimum",0);
                            effect.Requirements.Add(requirement);
                        }

                        effects.Add(effect);
                    }

                    break;
                }
            }
        }

        public void ResolveEffects(Battle battle, Battler owner, int fieldIndex) {
            foreach (SpellEffect e in effects) {
                Card target = owner.ChooseSpellTarget(battle, this, effects.IndexOf(e));
            }
        }

        public void TriggerEffects(object sender, BattleEventArgs args) {
            //TODO: How to resolve field effects
            //ResolveEffects((Battle)sender,args.TriggeringPlayer,args)
        }
    }
}
