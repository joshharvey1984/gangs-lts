using System;
using System.Collections.Generic;

namespace Gangs.Campaign {
    public class CampaignSquad  {
        public string Name { get; set; }
        public List<CampaignUnit> Units { get; set; } = new();
        public event Action<bool> OnSelect;
        
        public void AddUnit(CampaignUnit campaignUnit) {
            Units.Add(campaignUnit);
            Units.Sort((a, b) => b.Level.CompareTo(a.Level));
        }
        
        public void Select(bool select) {
            OnSelect?.Invoke(select);
        }

        private void EndTurn() {
        
        }
    }
}