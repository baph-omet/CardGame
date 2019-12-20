using CardGame.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGame.Scenes {
    public class CampaignScene : TextScene {
        private int Level = 0;
        private int MaxLevel = 0;
        private List<NPC> Opponents;

        public CampaignScene() : base("Choose your opponent") {
            Opponents = new List<NPC>();
            SetUpChoices();
        }

        private void SetUpChoices() {
            Title = "Level " + Level.ToString() + ". Choose your opponent";
            GetOpponents();
            foreach (NPC npc in Opponents) AddChoice(npc.Name, StartBattle);
            if (Level > 0) AddChoice("Previous Level", PreviousLevel);
            if (Level < MaxLevel) AddChoice("Next Level", NextLevel);
            AddChoice("Back to Menu", EndScene);
        }

        private void GetOpponents() {
            Opponents.Clear();
            int max = 0;
            foreach (NPC npc in NPC.GetAllNPCs()) {
                if (npc.Level == Level) Opponents.Add(npc);
                if (npc.Level > max) max = npc.Level;
            }
            MaxLevel = max;
        }

        private void StartBattle() {
            AddSubscene(new Battle(Program.ActivePlayer, Opponents[ChoiceIndex]));
        }

        private void NextLevel() {
            Level++;
            SetUpChoices();
        }

        private void PreviousLevel() {
            Level--;
            SetUpChoices();
        }
    }
}
