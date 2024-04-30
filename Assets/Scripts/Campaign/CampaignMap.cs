using System.Collections.Generic;
using System.Linq;
using Gangs.Campaign.CampaignGenerators;
using Gangs.Data;
using Gangs.MainMenu;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.Campaign {
    public class CampaignMap {
        public CampaignMapGameObject GameObject { get; set; }
        public List<CampaignTerritory> Territories { get; set; }
        
        public CampaignMap(CampaignData campaignData, GameObject gameObject) {
            GameObject = gameObject.GetComponent<CampaignMapGameObject>();
            Territories = new List<CampaignTerritory>();
            GetActiveTerritories(campaignData.MapSize.GetGridSize());
            FindTerritoryNeighbours();
            GenerateMap(campaignData);
            Territories.ForEach(t => t.SpawnEntities());
        }
        
        public CampaignTerritory GetTerritoryByGameObject(CampaignTerritoryGameObject territoryGameObject) {
            return Territories.Find(t => t.GameObject.Equals(territoryGameObject));
        }
        
        private void GenerateMap(CampaignData campaignData) {
            var ruleset = GameManager.Instance.CurrentRuleset;
            SpawnGangs(campaignData, ruleset.StartingTerritories);
            SpawnMonsters();
            foreach (var territory in Territories.Where(t => t.Territory == null)) {
                territory.SetTerritory(ruleset.ValidTerritories[Random.Range(0, ruleset.ValidTerritories.Count)]);
                if (Random.Range(0, 100) < 10) CreateNeutralSquad(territory);
            }
        }
        
        private void CreateNeutralSquad(CampaignTerritory territory) {
            var faction = Faction.All.First(f => f.Playable == false);
            var squad = CampaignNeutralSquadGenerator.GenerateNeutralSquad(faction, 10);
            territory.Squads.Add(squad);
        }
        
        private void FindTerritoryNeighbours() {
            foreach (var territory in Territories) {
                var neighbours = territory.GameObject.FindNeighbours();
                foreach (var neighbourTerritory in neighbours.Where(n => n.Active).Select(GetTerritoryByGameObject)) {
                    territory.Neighbours.Add(neighbourTerritory);
                }
            }
        }
        
        private void SpawnGangs(CampaignData campaignData, List<StartingTerritory> startingTerritories) {
            var validTerritories = Territories.Where(t => t.Neighbours.Count >= startingTerritories.Count).Select(t => t.GameObject).ToList();
            var spawnPoints = GameObject.FindSpawnPoints(campaignData.CampaignGangManagers.Count, validTerritories);
            spawnPoints = spawnPoints.OrderBy(_ => Random.value).ToList();
            
            for (var i = 0; i < campaignData.CampaignGangManagers.Count; i++) {
                var gang = campaignData.CampaignGangManagers[i];
                var territoryGameObject = spawnPoints[i].GetComponent<CampaignTerritoryGameObject>();
                var territory = GetTerritoryByGameObject(territoryGameObject);
                territory.SetClaimedBy(gang.Gang, true);
                territory.SetTerritory(startingTerritories.First(s => s.Headquarters).Territory);
                var squad = new CampaignGangSquad(gang.Gang);
                gang.Gang.Units.ForEach(u => squad.AddUnit(u));
                campaignData.CampaignGangManagers[i].AddSquad(squad);
                territory.Squads.Add(squad);
                
                if (startingTerritories.Count <= 1) continue;
                for (var j = 1; j < startingTerritories.Count; j++) {
                    var neighbour = territory.Neighbours[j - 1];
                    neighbour?.SetClaimedBy(gang.Gang);
                    neighbour?.SetTerritory(startingTerritories[j].Territory);
                }
            }
        }
        
        private void SpawnMonsters() {
            var monsterTerritories = Territories.Where(t => t.ClaimedBy == null && t.GameObject.Active).ToList();
            var monsterCount = monsterTerritories.Count / 3;
            for (var i = 0; i < monsterCount; i++) {
                var territory = monsterTerritories[Random.Range(0, monsterTerritories.Count)];
                CreateNeutralSquad(territory);
            }
        }

        private void GetActiveTerritories(int getGridSize) {
            var centralTerritories = GameObject.GetActiveTerritories(getGridSize);
            
            foreach (var territory in centralTerritories) {
                var script = territory.GetComponent<CampaignTerritoryGameObject>();
                script.Active = true;
                var campaignTerritory = new CampaignTerritory(territory);
                script.OnMouseEnterEvent += campaignTerritory.MouseEnter;
                Territories.Add(campaignTerritory);
            }
        }
    }
}