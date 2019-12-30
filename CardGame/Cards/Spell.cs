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

        //TODO: Move this to Card
        public void ResolveEffects(Battle battle, Battler owner, Card triggeringCard = null) {
            foreach (SpellEffect e in effects) {
                List<Card> possibleTargets = new List<Card>();
                switch (e.TargetType) {
                    case SpellEffectTargetType.MONSTER:
                        if (e.Range != SpellEffectTargetRange.OPPONENT) for (int i = 0; i < owner.Field.GetLength(0); i++) if (owner.Field[0, i] != null) possibleTargets.Add(owner.Field[0, i]);

                        if (e.Range != SpellEffectTargetRange.SELF) {
                            Battler opponent = battle.GetOpponent(owner);
                            for (int i = 0; i < opponent.Field.GetLength(0); i++) if (opponent.Field[0, i] != null) possibleTargets.Add(opponent.Field[0, i]);
                        }
                        break;
                    case SpellEffectTargetType.SPELL:
                        if (e.Range != SpellEffectTargetRange.OPPONENT) for (int i = 0; i < owner.Field.GetLength(1); i++) if (owner.Field[1, i] != null) e.Targets.Add(owner.Field[1, i]);

                        if (e.Range != SpellEffectTargetRange.SELF) {
                            Battler opponent = battle.GetOpponent(owner);
                            for (int i = 0; i < opponent.Field.GetLength(1); i++) if (opponent.Field[1, i] != null) e.Targets.Add(opponent.Field[1, i]);
                        }
                        break;
                }
                switch (e.TargetAssignment) {
                    case SpellEffectTargetAssignment.CHOOSE:
                        e.Targets.Add(owner.ChooseSpellTarget(battle, this, effects.IndexOf(e)));
                        break;
                    case SpellEffectTargetAssignment.PREVIOUS:
                        if (effects.IndexOf(e) > 0) e.Targets.AddRange(effects[effects.IndexOf(e) - 1].Targets);
                        break;
                    case SpellEffectTargetAssignment.ALL:
                        e.Targets.AddRange(possibleTargets);
                        break;
                    case SpellEffectTargetAssignment.FIRST:
                        e.Targets.Add(possibleTargets[0]);
                        break;
                    case SpellEffectTargetAssignment.RANDOM:
                        Random random = new Random();
                        e.Targets.Add(possibleTargets[random.Next(0, possibleTargets.Count)]);
                        break;
                    case SpellEffectTargetAssignment.TRIGGER:
                        break;
                }
                
                //TODO: Actually perform the effect
                switch (e.Action) {
                    case SpellEffectAction.INHIBIT:
                        if (triggeringCard == null) break;
                        
                        break;
                }

            }
        }

        public void TriggerEffects(object sender, BattleEventArgs args) {
            //TODO: Handle triggered effects
            ResolveEffects((Battle)sender, args.TriggeringPlayer);
        }
    }
}