using System;
using System.Collections.Generic;
using CardGame.Characters;
using CardGame.Cards;
using CardGame.AI;
using CardGame.UI;

namespace CardGame.Scenes {
    public class BattleEventArgs : EventArgs {
        public Battler TriggeringPlayer;
        public Battler NonTriggeringPlayer;
        public bool Cancel = false;

        public BattleEventArgs(Battler triggeringplayer,Battler nontriggeringplayer) {
            TriggeringPlayer = triggeringplayer;
            NonTriggeringPlayer = nontriggeringplayer;
        }
    }
    public class BattleCardEventArgs : BattleEventArgs {
        public Card TriggeringCard;
        public int TriggeringCardIndex;
        public Card TargetedCard;
        public int TargetedCardIndex;

        public bool DestroyTriggerer = false;
        public Card DestroyingCard = null;
        public int DestroyingCardIndex = -1;

        public BattleCardEventArgs(Battler turnplayer, Battler offturnplayer, Card triggeringcard) : this(turnplayer, offturnplayer, triggeringcard, null, -1, -1) { }
        public BattleCardEventArgs(Battler turnplayer, Battler offturnplayer, Card triggeringcard, int triggeringcardindex) : this(turnplayer, offturnplayer, triggeringcard, null, triggeringcardindex, -1) { }
        public BattleCardEventArgs(Battler turnplayer, Battler offturnplayer, Card triggeringcard, Card targetedcard,int triggeringcardindex, int targetedcardindex) : base(turnplayer,offturnplayer) {
            TriggeringCard = triggeringcard;
            TargetedCard = targetedcard;
            TriggeringCardIndex = triggeringcardindex;
            TargetedCardIndex = targetedcardindex;
        }
    }
    public class BattleManaEventArgs : BattleEventArgs {
        public int Amount;

        public BattleManaEventArgs(Battler triggeringplayer, Battler nontriggeringplayer, int amount)
            : base(triggeringplayer, nontriggeringplayer) {
                Amount = amount;
        }
    }

    public class Battle : Scene {
        private Battler[] battlers;
        private int[] cursorLocation;

        private int turn;

        private Card heldCard;
        private int[] heldCardLocation;

        private Battler winner = null;

        private bool ShowingChoices = false;

        public event EventHandler<BattleEventArgs> TurnStart;
        public event EventHandler<BattleEventArgs> TurnEnd;
        
        public event EventHandler<BattleManaEventArgs> ManaChange;

        public event EventHandler<BattleCardEventArgs> Draw;
        public event EventHandler<BattleCardEventArgs> Summon;
        public event EventHandler<BattleCardEventArgs> SpellActivate;
        public event EventHandler<BattleCardEventArgs> SpellDestroyed;
        public event EventHandler<BattleCardEventArgs> SetMonster;
        public event EventHandler<BattleCardEventArgs> FlipMonster;
        public event EventHandler<BattleCardEventArgs> SetSpell;
        public event EventHandler<BattleCardEventArgs> MonsterDestroyed;
        public event EventHandler<BattleCardEventArgs> ManaSteal;
        public event EventHandler<BattleCardEventArgs> MonsterAttack;

        public Battle(Battler battler1, Battler battler2) {
            battlers = new Battler[] {battler1, battler2};
            cursorLocation = new int[]{-1,0};
            heldCard = null;
            turn = 0;


            battlers[0].Initialize();
            battlers[1].Initialize();
        }

        public override void Update() {
            // Pre-battle

            ShowText("Beginning battle between " + battlers[0].Name + " and " + battlers[1].Name + ". ");
            foreach (Battler b in battlers) {
                if (!(b is Player)) {
                    NPC n = (NPC) b;
                    ShowText(n.Name + ": " + n.Text[0]);
                }
            }

            // Main battle loop
            while (battlers[0].Mana > 0 && battlers[1].Mana > 0) {
                // Main turn loop
                foreach (Battler b in battlers) {
                    // Pre-turn
                    cursorLocation[0] = 2;
                    cursorLocation[1] = 0;
                    if (b.Mana <= 0) break;
                    if (b.Mana > b.MaxManaAllotment) {
                        b.Mana -= b.MaxManaAllotment;
                        b.ManaAllotment = b.MaxManaAllotment;
                    } else {
                        b.ManaAllotment = b.Mana - 1;
                        b.Mana = 1;
                    }
                    ShowText(b.Name + "'s turn.",!(b is NPC));
                    b.HasMoved = false;
                    if (b.PlayDeck.Count > 0) {
                        if ((turn > 0 || Array.IndexOf(battlers, b) > 0) && b.Hand.Count < 9) b.Draw();
                    } else {
                        b.Mana -= b.MaxManaAllotment;
                        ShowText(b.Name + "'s deck is empty! " + b.Name + " takes " + b.MaxManaAllotment + " damage as punishment!");
                        if (b.Mana <= 0) break;
                    }

                    for (int i = 0; i < b.Field.GetLength(1); i++) if (b.Field[0, i] != null) ((Monster) b.Field[0, i]).CanAttack = true;

                    TurnStart?.Invoke(this, new BattleEventArgs(b, GetOpponent(b)));

                    // Turn processing
                    if (b is NPC) {
                        AITurn(b);
                    } else if (b is Player) {
                        UpdateSprites();
                        PlayerTurn(b);
                    }
                    
                    // End of turn
                    UpdateSprites("End of " + b.Name + "'s turn.");
                    
                    if (!b.HasMoved && b.MaxManaAllotment < 10) {
                        b.MaxManaAllotment += 2;
                        if (b.MaxManaAllotment > 10) b.MaxManaAllotment = 10;
                        ShowText(b.Name + " forfeited their turn to boost Mana consumption. Max Mana allotment for next turn is " + b.MaxManaAllotment + ".");
                    }

                    TurnEnd?.Invoke(this, new BattleEventArgs(b, GetOpponent(b)));
                    
                    if (b.ManaAllotment > 0) b.Mana += b.ManaAllotment;
                    else b.MaxManaAllotment++;
                    b.ManaAllotment = 0;
                    turn++;
                }
            }

            //End battle
            //TODO: Tie handling
            if (battlers[0].Mana <= 0) winner = battlers[1];
            else if (battlers[1].Mana <= 0) winner = battlers[0];
            ShowText(winner.Name + " wins!");

            if (winner is Player) PlayerVictory();
            else PlayerLoss();
            UpdateSprites();
            EndScene();
        }

        public Battler GetWinner() {
            return winner;
        }

        public Battler GetLoser() {
            return GetOpponent(winner);
        }

        private void UpdateSprites() { UpdateSprites(""); }
        private void UpdateSprites(string battleText) { UpdateSprites(battleText, new string[] { }); }
        private void UpdateSprites(string battleText, string[] choices) {
            SpriteHash.Clear();
            SpriteHash.Add("OpponentField", new OpponentFieldSprite(battlers[1], cursorLocation));
            SpriteHash.Add("PlayerField", new PlayerFieldSprite(battlers[0], cursorLocation));
            if (battlers[0] is Player) SpriteHash.Add("Hand", new HandSprite(battlers[0], cursorLocation));
            if (heldCard != null) SpriteHash.Add("HeldCard", new CardSprite(heldCard));
            Card highlighted = GetHighlightedCard();
            if (highlighted != null) SpriteHash.Add("HighlightedCard", new CardSprite(highlighted));
            if (battleText.Length > 0 || choices.Length > 0) SpriteHash.Add("TextBox", new TextBox("", battleText, choices));
            Render();
        }

        private Card GetHighlightedCard() {
            if (cursorLocation[0] < 0 || cursorLocation[1] < 0) return null;
            if (cursorLocation[0] < 2) return battlers[1].Field[1 - cursorLocation[0], cursorLocation[1]];
            if (cursorLocation[0] < 4) return battlers[0].Field[cursorLocation[0] - 2, cursorLocation[1]];
            if (cursorLocation[0] < 7 && !ShowingChoices && (cursorLocation[0] - 4) * 3 + cursorLocation[1] < battlers[0].Hand.Count) return battlers[0].Hand[(cursorLocation[0] - 4) * 3 + cursorLocation[1]];
            return null;
        }

        public void ShowText(String text, bool anykey = false) {
            ShowText(new string[] { text }, anykey);
        }
        public void ShowText(String[] text, bool anykey = false) {
            foreach (String t in text) {
                UpdateSprites(t);
                if (anykey) Control.WaitForKey();
                else Control.WaitForKey(ConsoleKey.Enter);
            }
        }

        private int ShowChoices(String prompt, String[] choices) {
            int[] previousCursorLocation = new int[2];
            cursorLocation.CopyTo(previousCursorLocation,0);
            int choice = -1;

            int[] cursorBounds = { 4, 4 + choices.Length };
            if (battlers[0] is Player) {
                cursorBounds = new int[] {
                    5 + (battlers[0].Hand.Count - 1) / 3,
                    5 + choices.Length + (battlers[0].Hand.Count - 1) / 3
                };
            }

            cursorLocation[0] = cursorBounds[0];
            ShowingChoices = true;
            UpdateSprites(prompt, choices);

            while (choice < 0) {

                ConsoleKey[] acceptedKeys = {
                    ConsoleKey.DownArrow,
                    ConsoleKey.UpArrow,
                    ConsoleKey.Enter
                };

                ConsoleKey pressedKey = Control.WaitForKey(acceptedKeys);
                switch (pressedKey) {
                    case ConsoleKey.UpArrow:
                        cursorLocation[0] -= 1;
                        break;
                    case ConsoleKey.DownArrow:
                        cursorLocation[0] += 1;
                        break;
                    case ConsoleKey.Enter:
                        choice = cursorLocation[0] - cursorBounds[0];
                        break;
                }

                if (cursorLocation[0] < cursorBounds[0]) cursorLocation[0] = cursorBounds[1] - 1;
                else if (cursorLocation[0] >= cursorBounds[1]) cursorLocation[0] = cursorBounds[0];
                ((TextBox) SpriteHash["TextBox"]).Index = cursorLocation[0] - cursorBounds[0];
                Render();
            }
            cursorLocation = previousCursorLocation;
            ShowingChoices = false;
            return choice;
        }

        private void MoveCursor(int rowDirection, int colDirection) {
            if (rowDirection > 1) rowDirection = 1;
            else if (rowDirection < -1) rowDirection = -1;
            if (colDirection > 1) colDirection = 1;
            else if (colDirection < -1) colDirection = -1;

            cursorLocation[0] += rowDirection;
            cursorLocation[1] += colDirection;

            if (cursorLocation[0] < 0) cursorLocation[0] = 0;
            if (cursorLocation[1] < 0) cursorLocation[1] = 0;
            else if (cursorLocation[0] < 4 && cursorLocation[1] > 4) cursorLocation[1] = 4;
            else if (cursorLocation[0] >= 4 && cursorLocation[1] > 2) cursorLocation[1] = 2;
            if (battlers[0] is Player) {
                if (cursorLocation[0] > 4 + (battlers[0].Hand.Count - 1) / 3) cursorLocation[0] = 4 + (battlers[0].Hand.Count - 1) / 3;
                if (cursorLocation[0] >= 4 && cursorLocation[1] > battlers[0].Hand.Count - (cursorLocation[0] - 4) * 3 - 1) cursorLocation[1] = battlers[0].Hand.Count - (cursorLocation[0] - 4) * 3 - 1;
            }
            UpdateSprites();
        }

        public Battler GetOpponent(Battler battler) {
            foreach (Battler bt in battlers) {
                if (!bt.Equals(battler)) return bt;
            } return null;
        }

        private Monster SelectAttackTarget(Battler b) {
            if (!GetOpponent(b).HasMonstersSummoned()) return null;

            ConsoleKey[] acceptedKeys = {
                ConsoleKey.DownArrow,
                ConsoleKey.UpArrow,
                ConsoleKey.LeftArrow,
                ConsoleKey.RightArrow,
                ConsoleKey.Enter
            };

            cursorLocation[0] = 1;
            UpdateSprites();
            while (true) {
                ConsoleKey pressedKey = Control.WaitForKey(acceptedKeys);
                switch (pressedKey) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                        return null;
                    case ConsoleKey.LeftArrow:
                        MoveCursor(0, -1);
                        break;
                    case ConsoleKey.RightArrow:
                        MoveCursor(0, 1);
                        break;
                    case ConsoleKey.Enter:
                        if (GetOpponent(b).Field[0, cursorLocation[1]] != null) {
                            return (Monster) GetOpponent(b).Field[0, cursorLocation[1]];
                        }
                        break;
                }
            }
        }

        private void AITurn(Battler user) {
            Battler opponent = GetOpponent(user);
            BattleAI ai = new BattleAI(ref user, ref opponent);
            while (ai.KeepMoving()) {
                if (ai.KeepMoving()) {
                    Monster Summon = ai.GetBestSummon();
                    int openslot = ai.GetFirstOpenMonsterZone();
                    if (Summon != null && openslot >= 0) {
                        user.Summon(Summon, ai.GetFirstOpenMonsterZone());
                        ShowText(user.Name + " summoned " + Summon.Name + "!");
                    }

                    //TODO: check for monster effects

                    for (int i = 0; i < user.Field.GetLength(1); i++) {
                        if (user.Field[0,i] != null) {
                            Monster mon = (Monster) user.Field[0, i];
                            if (mon.CanAttack) {
                                if (ai.Opponent.HasMonstersSummoned()) {
                                    int targetIndex = ai.GetBestTarget(i);
                                    if (targetIndex > -1) {
                                        Monster target = (Monster)opponent.Field[0, targetIndex];
                                        MonsterAttacking(user, mon, target, i, targetIndex);
                                    
                                    } else {
                                        mon.CanAttack = false;
                                        return;
                                    }
                                } else {
                                    StealingMana(user, mon, i);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PlayerTurn(Battler b) {
            bool turnEnded = false;
            ConsoleKey[] controls = {
                ConsoleKey.DownArrow,
                ConsoleKey.RightArrow,
                ConsoleKey.UpArrow,
                ConsoleKey.LeftArrow,
                ConsoleKey.Enter,
                ConsoleKey.PageDown,
                ConsoleKey.Home,
                ConsoleKey.End,
                ConsoleKey.Escape
            };
            while (!turnEnded) {
                ConsoleKey pressedKey = Control.WaitForKey(controls);
                switch (pressedKey) {
                    case ConsoleKey.DownArrow:
                        MoveCursor(1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        MoveCursor(0, 1);
                        break;
                    case ConsoleKey.UpArrow:
                        MoveCursor(-1, 0);
                        break;
                    case ConsoleKey.LeftArrow:
                        MoveCursor(0, -1);
                        break;
                    case ConsoleKey.Home:
                        AITurn(b);
                        break;
                    case ConsoleKey.End:
                        heldCard = null;
                        turnEnded = true;
                        break;
                    case ConsoleKey.PageDown:
                        if (heldCard != null) {
                            if (heldCard is Monster && cursorLocation[0] == 2) {
                                Monster mon = (Monster) heldCard;
                                if (b.Field[0, cursorLocation[1]] == null) {
                                    if (b.ManaAllotment >= mon.Level) {
                                        SettingMonster(b, mon, cursorLocation[1]);
                                        heldCard = null;
                                    } else ShowText("You don't have enough Mana to summon this card!");
                                } else ShowText("That space is occupied!");
                            } else if (heldCard is Spell && cursorLocation[0] == 3) {
                                Spell spl = (Spell) heldCard;
                                if (b.Field[0, cursorLocation[1]] == null) {

                                }
                            }
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (heldCard == null) {
                            if (cursorLocation[0] > 3) {
                                SelectCard(b.Hand[(cursorLocation[0] - 4) * 3 + cursorLocation[1]]);
                            } else if (cursorLocation[0] == 2) {
                                MonsterMenu(b,(Monster) b.Field[0, cursorLocation[1]]);
                            }
                        } else {
                            if (heldCardLocation == cursorLocation) {
                                heldCard = null;
                                heldCardLocation = null;
                            } else {
                                if (heldCardLocation[0] > 3) {
                                    if (cursorLocation[0] > 3) {
                                        Card swapCard = b.Hand[(cursorLocation[0] - 4) * 3 + cursorLocation[1]];
                                        b.Hand[(cursorLocation[0] - 4) * 3 + cursorLocation[1]] = heldCard;
                                        b.Hand[(heldCardLocation[0] - 4) * 3 + heldCardLocation[1]] = swapCard;
                                        heldCard = null;
                                        heldCardLocation = null;
                                    } else {
                                        if (cursorLocation[0] == 1 && heldCard is Spell) {
                                            //TODO: play spell
                                        } else if (cursorLocation[0] == (b.Equals(battlers[0]) ? 2 : 1) && heldCard is Monster) {
                                            if (b.Field[0, cursorLocation[1]] == null) {
                                                if (b.ManaAllotment >= heldCard.Level) {
                                                    b.Summon((Monster) heldCard, cursorLocation[1]);
                                                    heldCard = null;
                                                    heldCardLocation = null;
                                                } else ShowText("You don't have enough Mana to summon this card!");
                                            } else ShowText("That space is occupied!");
                                        }
                                    }
                                } else {

                                }
                            }
                        }
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
                UpdateSprites();
                if (b.Mana <= 0 || GetOpponent(b).Mana <= 0) return;
            }
        }

        private void SelectCard(Card target) {
            if (target != null) {
                heldCard = target;
                heldCardLocation = new int[2];
                cursorLocation.CopyTo(heldCardLocation, 0);
            }
        }

        private void DeselectCard() {
            heldCard = null;
            heldCardLocation = null;
        }

        private void MonsterMenu(Battler b, Monster mon) {
            Battler opponent = GetOpponent(b);
            if (mon == null) return;
            SelectCard(mon);

            List<String> choices = new List<string>();
            if (mon.CanAttack && !mon.Facedown && turn > 0) {
                if (b.CanStealMana(opponent)) choices.Add("Steal Mana");
                else choices.Add("Attack");
            }

            //TODO: Check for monster effect

            if (mon.Facedown) choices.Add("Flip");
            if (mon.CanAttack) choices.Add("Retire");
            choices.Add("Cancel");

            string choice = choices[ShowChoices("What should " + mon.Name + " do?", choices.ToArray())];

            //BattleCardEventArgs args;
            switch (choice.ToLower()) {
                case "attack":
                    if (!opponent.HasMonstersSummoned()) ShowText(opponent.Name + " has no monsters summoned.");
                    Monster target = SelectAttackTarget(b);
                    if (target != null) {
                        MonsterAttacking(b, mon, target, heldCardLocation[0], cursorLocation[1]);

                        heldCard = null;
                        heldCardLocation = null;
                    }
                    break;
                case "steal mana":
                    StealingMana(b,mon,heldCardLocation[0]);
                    break;
                case "flip":
                    FlippingMonster(b,mon,heldCardLocation[0]);
                    break;
                case "retire":
                    b.Field[0, heldCardLocation[1]] = null;
                    b.Discard.Add(mon);
                    b.ManaAllotment += mon.Level;
                    ShowText("Retired " + mon.Name + ". Restored " + mon.Level + " Mana.");
                    break;
                case "cancel":
                    break;
                    
            }

            DeselectCard();
        }

        //TODO: Spell effect handling

        public bool MonsterAttacking(Battler user, Monster attacker, Monster defender, int attackerIndex, int defenderIndex) {
            user.HasMoved = true;
            BattleCardEventArgs args = new BattleCardEventArgs(user,GetOpponent(user),attacker,defender,attackerIndex,defenderIndex);
            ShowText(user.Name + "'s " + attacker.Name + " attacked " + GetOpponent(user).Name + "'s " + defender.Name + "!");
            
            MonsterAttack?.Invoke(this, args);

            if (args.DestroyTriggerer) DestroyingMonster(args.TriggeringPlayer, (Monster) args.TriggeringCard, null, args.TriggeringCardIndex, -1);
            if (args.Cancel || args.DestroyTriggerer) return false;

            switch (((Monster)args.TriggeringCard).Battle((Monster)args.TargetedCard)) {
                case MonsterAttackOutcome.WIN:
                    if (DestroyingMonster(args)) ChangingMana(args.TriggeringPlayer, args.TargetedCard.Level);
                    break;
                case MonsterAttackOutcome.LOSS:
                    if (DestroyingMonster(args.NonTriggeringPlayer, args.TargetedCard,(Monster) args.TriggeringCard, args.TargetedCardIndex, args.TriggeringCardIndex)) ChangingMana(args.NonTriggeringPlayer, args.TriggeringCard.Level);
                    break;
                case MonsterAttackOutcome.TIE:
                    ShowText("...but neither claimed victory.");
                    break;
            }

            return true;
        }

        public bool FlippingMonster(Battler user, Monster mon, int monIndex) {
            BattleCardEventArgs args = new BattleCardEventArgs(user, GetOpponent(user), mon, monIndex);

            FlipMonster?.Invoke(this, args);

            if (args.DestroyTriggerer) DestroyingMonster(args.TriggeringPlayer, (Monster) args.TriggeringCard, null, args.TriggeringCardIndex, -1);
            if (args.Cancel || args.DestroyTriggerer) return false;

            if (args.TriggeringCard is Monster) {
                ((Monster) args.TriggeringCard).Flip();
                ShowText("Flipped " + args.TriggeringCard.Name + " face-up.");
            }

            return true;
        }

        public bool StealingMana(Battler user, Monster mon, int monIndex) {
            BattleCardEventArgs args = new BattleCardEventArgs(user, GetOpponent(user), mon, monIndex);

            ManaSteal?.Invoke(this,args);

            if (args.DestroyTriggerer) DestroyingMonster(args.TriggeringPlayer,(Monster)args.TriggeringCard,null,args.TriggeringCardIndex,-1);
            if (args.Cancel || args.DestroyTriggerer) return false;
            
            args.TriggeringPlayer.Mana += mon.Level;
            args.NonTriggeringPlayer.Mana -= mon.Level;
            if (args.TriggeringCard is Monster) ((Monster)args.TriggeringCard).CanAttack = false;
            ShowText(args.TriggeringPlayer.Name + "'s " + args.TriggeringCard.Name + " stole " +
                args.TriggeringCard.Level + " Mana from " + args.NonTriggeringPlayer.Name);
            args.TriggeringPlayer.HasMoved = true;

            return true;
        }

        public bool DestroyingMonster(Battler destroyerOwner, Card destroyer, Monster target, int destroyingMonIndex, int destroyedMonIndex) {
            BattleCardEventArgs args = new BattleCardEventArgs(destroyerOwner, GetOpponent(destroyerOwner), destroyer, target, destroyingMonIndex, destroyedMonIndex);
            return DestroyingMonster(args);
        }
        public bool DestroyingMonster(BattleCardEventArgs args) {
            MonsterDestroyed?.Invoke(this,args);

            if (args.DestroyTriggerer) DestroyingMonster(args.TriggeringPlayer, (Monster)args.TriggeringCard, null, args.TriggeringCardIndex, -1);
            if (args.Cancel || args.DestroyTriggerer) return false;

            ShowText(args.NonTriggeringPlayer.Name + "'s " + args.TargetedCard.Name + " was destroyed!");
            args.NonTriggeringPlayer.RetireMonster(args.TargetedCardIndex);

            return true;
        }

        public bool SettingMonster(Battler user, Monster setMon, int setMonIndex) {
            BattleCardEventArgs args = new BattleCardEventArgs(user, GetOpponent(user), setMon,setMonIndex);

            SetMonster?.Invoke(this, args);

            if (args.DestroyTriggerer) args.TriggeringPlayer.RetireMonster(args.TriggeringCardIndex);
            if (args.Cancel || args.DestroyTriggerer) return false;

            args.TriggeringPlayer.Set((Monster)args.TriggeringCard, args.TriggeringCardIndex);

            return true;
        }

        public bool SettingSpell(Battler user, Spell setSpl, int setSplIndex) {
            BattleCardEventArgs args = new BattleCardEventArgs(user, GetOpponent(user), setSpl);

            SetSpell?.Invoke(this, args);

            if (args.DestroyTriggerer) args.TriggeringPlayer.RemoveSpell(args.TriggeringCardIndex);
            if (args.Cancel || args.DestroyTriggerer) return false;

            Spell spell = (Spell)args.TriggeringCard;
            switch (spell.Trigger) {
                case SpellTrigger.DRAW:
                    Draw += spell.TriggerEffects;
                    break;
                case SpellTrigger.SUMMON:
                    Summon += spell.TriggerEffects;
                    break;
                case SpellTrigger.SPELL:
                    SpellActivate += spell.TriggerEffects;
                    break;
                case SpellTrigger.SETMONSTER:
                    SetMonster += spell.TriggerEffects;
                    break;
                case SpellTrigger.FLIPMONSTER:
                    FlipMonster += spell.TriggerEffects;
                    break;
                case SpellTrigger.SETSPELL:
                    SetSpell += spell.TriggerEffects;
                    break;
                case SpellTrigger.TURNSTART:
                    TurnStart += spell.TriggerEffects;
                    break;
                case SpellTrigger.TURNEND:
                    TurnEnd += spell.TriggerEffects;
                    break;
                case SpellTrigger.MONSTERDESTROYED:
                    MonsterDestroyed += spell.TriggerEffects;
                    break;
                case SpellTrigger.MANASTEAL:
                    ManaSteal += spell.TriggerEffects;
                    break;
                case SpellTrigger.MONSTERATTACK:
                    MonsterAttack += spell.TriggerEffects;
                    break;
                case SpellTrigger.MANAALLOTMENT:
                case SpellTrigger.MANA:
                case SpellTrigger.MANACHANGE:
                    ManaChange += spell.TriggerEffects;
                    break;
            }

            args.TriggeringPlayer.SetSpell(spell, args.TriggeringCardIndex);

            return true;
        }

        public bool ActivatingSpell(Battler user, Spell spl, int splIndex) {
            BattleCardEventArgs args = new BattleCardEventArgs(user, GetOpponent(user), spl, splIndex);

            SpellActivate?.Invoke(this, args);

            if (args.DestroyTriggerer || (args.Cancel && spl.SpellType != SpellType.CONTINUOUS)) DestroyingSpell(args.NonTriggeringPlayer, args.TargetedCard, args.TriggeringCard, args.TargetedCardIndex, args.TriggeringCardIndex);
            if (args.Cancel || args.DestroyTriggerer) return false;

            
            Spell spell = (Spell)args.TriggeringCard;

            ShowText(args.TriggeringPlayer + " activated " + args.TriggeringCard.Name + "!");
            spell.ResolveEffects(this, args.TriggeringPlayer, args.TriggeringCardIndex);
            if (spell.SpellType == SpellType.INSTANT || spell.SpellType == SpellType.COUNTER) RemovingSpell(args.TriggeringPlayer, spell, args.TriggeringCardIndex);
            return true;
        }

        public bool DestroyingSpell(Battler user, Card triggerer, Card target, int triggererindex, int targetindex) {
            BattleCardEventArgs args = new BattleCardEventArgs(user, GetOpponent(user), triggerer, target, triggererindex, targetindex);

            SpellDestroyed?.Invoke(this,args);

            if (args.DestroyTriggerer) DestroyingSpell(args.NonTriggeringPlayer, args.DestroyingCard, args.TriggeringCard, args.DestroyingCardIndex, args.TriggeringCardIndex);
            if (args.DestroyTriggerer || args.Cancel) return false;

            ShowText(args.NonTriggeringPlayer + "'s " + args.TargetedCard.Name + " was destroyed!");
            RemovingSpell(args.NonTriggeringPlayer, (Spell) args.TargetedCard, args.TargetedCardIndex);

            return true;
        }

        private void RemovingSpell(Battler user, Spell spell, int splIndex) {
            switch (spell.Trigger) {
                case SpellTrigger.DRAW:
                    Draw -= spell.TriggerEffects;
                    break;
                case SpellTrigger.SUMMON:
                    Summon -= spell.TriggerEffects;
                    break;
                case SpellTrigger.SPELL:
                    SpellActivate -= spell.TriggerEffects;
                    break;
                case SpellTrigger.SETMONSTER:
                    SetMonster -= spell.TriggerEffects;
                    break;
                case SpellTrigger.FLIPMONSTER:
                    FlipMonster -= spell.TriggerEffects;
                    break;
                case SpellTrigger.SETSPELL:
                    SetSpell -= spell.TriggerEffects;
                    break;
                case SpellTrigger.TURNSTART:
                    TurnStart -= spell.TriggerEffects;
                    break;
                case SpellTrigger.TURNEND:
                    TurnEnd -= spell.TriggerEffects;
                    break;
                case SpellTrigger.MONSTERDESTROYED:
                    MonsterDestroyed -= spell.TriggerEffects;
                    break;
                case SpellTrigger.MANASTEAL:
                    ManaSteal -= spell.TriggerEffects;
                    break;
                case SpellTrigger.MONSTERATTACK:
                    MonsterAttack -= spell.TriggerEffects;
                    break;
                case SpellTrigger.MANAALLOTMENT:
                case SpellTrigger.MANA:
                case SpellTrigger.MANACHANGE:
                    ManaChange -= spell.TriggerEffects;
                    break;
            }
            user.Field[1, splIndex] = null;
            user.Discard.Add(spell);
        }

        public bool ChangingMana(Battler target, int amount) {
            BattleManaEventArgs args = new BattleManaEventArgs(target, GetOpponent(target),amount);

            ManaChange?.Invoke(this, args);

            if (args.Cancel) return false;

            args.TriggeringPlayer.Mana += amount;

            return true;
        }

        public bool DrawingCard(Battler user, Card card) {
            if (card == null) {
                Random random = new Random();
                card = user.PlayDeck[random.Next(user.PlayDeck.Count)];
            }

            BattleCardEventArgs args = new BattleCardEventArgs(user, GetOpponent(user),card);
            Draw?.Invoke(this, args);
            if (args.Cancel) return false;
            user.Draw(card);
            return true;
        }

        private void UpdateRecord(bool win) {
            Player p = null;
            foreach (Battler b in battlers) {
                if (b is Player) {
                    p = (Player)b;
                    break;
                }
            } if (p == null) return;

            if (win) p.AddWin(((NPC)GetOpponent(p)).ID);
            else p.AddLoss(((NPC)GetOpponent(p)).ID);
            p.Save();
        }

        private void PlayerVictory() {
            NPC opp = (NPC)GetOpponent(winner);
            ShowText(opp.Name + ": " + opp.Text[1]);

            Random random = new Random();
            List<Card> prizePool = new List<Card>();
            prizePool.AddRange(opp.Deck);
            prizePool.AddRange(opp.Chest);
            Card prize = prizePool[random.Next(prizePool.Count)];

            ShowText(new[] { "As a prize, you gained a new card: [" + prize.Name + "]!", "It was sent to your chest." });
            Program.ActivePlayer.Chest.Add(prize);

            UpdateRecord(true);
        }

        private void PlayerLoss() {
            UpdateRecord(false);
            ShowText(winner.Name + ": " + ((NPC)winner).Text[2]);
        }
    }
}
