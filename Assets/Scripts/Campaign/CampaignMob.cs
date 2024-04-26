using System.Collections.Generic;
using System.Linq;
using Gangs.Data;
using UnityEngine;

namespace Gangs.Campaign {
    public class CampaignMob : ICampaignEntity {
        public CampaignEntityGameObject GameObject { get; set; }
        public string Name { get; set; }
        
        public List<Monster> Monsters { get; set; } = new();
        
        public CampaignMob(Monster monster) {
            Enumerable.Range(1, Random.Range(3, 5)).ToList().ForEach(_ => Monsters.Add(monster));
            Name = monster.Name + " Mob";
        }
    }
}