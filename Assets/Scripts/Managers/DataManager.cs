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
            
            var clans = Directory.GetFiles(path + "Clans/", "*.json");
            var clansDto = new List<ClanDto>();
            
            var starterGangs = Directory.GetFiles(path + "StarterGangs/", "*.json");
            var gangDto = new List<GangDto>();
            
            var starterFighters = Directory.GetFiles(path + "StarterGangs/Fighters/", "*.json", SearchOption.AllDirectories);
            var fighterDto = new List<FighterDto>();
            
            var monsters = Directory.GetFiles(path + "Monsters/", "*.json");
            var monsterDto = new List<MonsterDto>();
            
            var weapons = Directory.GetFiles(path + "Equipment/Weapons/", "*.json");
            var weaponDto = new List<WeaponDto>();
            
            var armour = Directory.GetFiles(path + "Equipment/Armour/", "*.json");
            var armourDto = new List<ArmourDto>();
            
            var maps = Directory.GetFiles(path + "Maps/", "*.json");
            var mapDto = new List<MapDto>();
            
            territories.ToList().ForEach(t => territoryDto.Add(TerritoryDto.CreateFromJson(File.ReadAllText(t))));
            rulesets.ToList().ForEach(r => rulesetDto.Add(RulesetDto.CreateFromJson(File.ReadAllText(r))));
            clans.ToList().ForEach(c => clansDto.Add(ClanDto.CreateFromJson(File.ReadAllText(c))));
            starterGangs.ToList().ForEach(g => gangDto.Add(GangDto.CreateFromJson(File.ReadAllText(g))));
            starterFighters.ToList().ForEach(f => fighterDto.Add(FighterDto.CreateFromJson(File.ReadAllText(f))));
            monsters.ToList().ForEach(m => monsterDto.Add(MonsterDto.CreateFromJson(File.ReadAllText(m))));
            weapons.ToList().ForEach(w => weaponDto.Add(WeaponDto.CreateFromJson(File.ReadAllText(w))));
            armour.ToList().ForEach(a => armourDto.Add(ArmourDto.CreateFromJson(File.ReadAllText(a))));
            maps.ToList().ForEach(m => mapDto.Add(MapDto.CreateFromJson(File.ReadAllText(m))));
            
            territoryDto.ForEach(t => new Data.Territory(t));
            rulesetDto.ForEach(r => new Data.Ruleset(r));
            clansDto.ForEach(c => new Data.Clan(c));
            gangDto.ForEach(g => new Data.Gang(g));
            fighterDto.ForEach(f => new Data.Fighter(f));
            monsterDto.ForEach(m => new Data.Monster(m));
            weaponDto.ForEach(w => new Data.Weapon(w));
            armourDto.ForEach(a => new Data.Armour(a));
            mapDto.ForEach(m => new Data.Map(m));
            
            Data.Territory.All.ForEach(t => t.Create(territoryDto.Find(dto => dto.id == t.ID)));
            Data.Ruleset.All.ForEach(r => r.Create(rulesetDto.Find(dto => dto.id == r.ID)));
            Data.Clan.All.ForEach(c => c.Create(clansDto.Find(dto => dto.id == c.ID)));
            Data.Gang.All.ForEach(g => g.Create(gangDto.Find(dto => dto.id == g.ID)));
            Data.Fighter.All.ForEach(f => f.Create(fighterDto.Find(dto => dto.id == f.ID)));
            Data.Monster.All.ForEach(m => m.Create(monsterDto.Find(dto => dto.id == m.ID)));
            Data.Weapon.All.ForEach(w => w.Create(weaponDto.Find(dto => dto.id == w.ID)));
            Data.Armour.All.ForEach(a => a.Create(armourDto.Find(dto => dto.id == a.ID)));
            Data.Map.All.ForEach(m => m.Create(mapDto.Find(dto => dto.id == m.ID)));
        }
    }
}