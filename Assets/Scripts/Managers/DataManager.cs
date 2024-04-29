using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gangs.Data.DTO;
using UnityEngine;

namespace Gangs.Managers {
    public static class DataManager  {
        public static void CreateData() {
            var path = Application.dataPath + "/Data/";
            
            var rulesets = Directory.GetFiles(path + "Ruleset/", "*.json");
            var rulesetDto = new List<RulesetDto>();
            
            var territories = Directory.GetFiles(path + "Territories/", "*.json");
            var territoryDto = new List<TerritoryDto>();
            
            var factions = Directory.GetFiles(path + "Factions/", "*.json");
            var factionsDto = new List<FactionDto>();
            
            var weapons = Directory.GetFiles(path + "Equipment/Weapons/", "*.json");
            var weaponDto = new List<WeaponDto>();
            
            var armour = Directory.GetFiles(path + "Equipment/Armour/", "*.json");
            var armourDto = new List<ArmourDto>();
            
            var maps = Directory.GetFiles(path + "Maps/", "*.json");
            var mapDto = new List<MapDto>();
            
            var units = Directory.GetFiles(path + "Units/", "*.json");
            var unitDto = new List<UnitDto>();
            
            territories.ToList().ForEach(t => territoryDto.Add(TerritoryDto.CreateFromJson(File.ReadAllText(t))));
            rulesets.ToList().ForEach(r => rulesetDto.Add(RulesetDto.CreateFromJson(File.ReadAllText(r))));
            units.ToList().ForEach(u => unitDto.Add(UnitDto.CreateFromJson(File.ReadAllText(u))));
            factions.ToList().ForEach(c => factionsDto.Add(FactionDto.CreateFromJson(File.ReadAllText(c))));
            weapons.ToList().ForEach(w => weaponDto.Add(WeaponDto.CreateFromJson(File.ReadAllText(w))));
            armour.ToList().ForEach(a => armourDto.Add(ArmourDto.CreateFromJson(File.ReadAllText(a))));
            maps.ToList().ForEach(m => mapDto.Add(MapDto.CreateFromJson(File.ReadAllText(m))));
            
            territoryDto.ForEach(t => _ = new Data.Territory(t));
            rulesetDto.ForEach(r => _ = new Data.Ruleset(r));
            unitDto.ForEach(u => _ = new Data.Unit(u));
            factionsDto.ForEach(c => _ = new Data.Faction(c));
            weaponDto.ForEach(w => _ = new Data.Weapon(w));
            armourDto.ForEach(a => _ = new Data.Armour(a));
            mapDto.ForEach(m => _ = new Data.Map(m));
            
            Data.Territory.All.ForEach(t => t.Create(territoryDto.Find(dto => dto.id == t.ID)));
            Data.Ruleset.All.ForEach(r => r.Create(rulesetDto.Find(dto => dto.id == r.ID)));
            Data.Faction.All.ForEach(c => c.Create(factionsDto.Find(dto => dto.id == c.ID)));
            Data.Unit.All.ForEach(u => u.Create(unitDto.Find(dto => dto.id == u.ID)));
            Data.Weapon.All.ForEach(w => w.Create(weaponDto.Find(dto => dto.id == w.ID)));
            Data.Armour.All.ForEach(a => a.Create(armourDto.Find(dto => dto.id == a.ID)));
            Data.Map.All.ForEach(m => m.Create(mapDto.Find(dto => dto.id == m.ID)));
        }
    }
}