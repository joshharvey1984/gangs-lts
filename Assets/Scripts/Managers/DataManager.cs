using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gangs.Data.DTO;
using UnityEngine;

namespace Gangs.Managers {
    public static class DataManager  {
        public static void CreateData() {
            var path = Application.dataPath + "/Data/";
            
            var clans = Directory.GetFiles(path + "Clans/", "*.json");
            var clansDto = new List<ClanDto>();
            
            var starterGangs = Directory.GetFiles(path + "StarterGangs/", "*.json");
            var gangDto = new List<GangDto>();
            
            var starterFighters = Directory.GetFiles(path + "StarterGangs/Fighters/", "*.json", SearchOption.AllDirectories);
            var fighterDto = new List<FighterDto>();
            
            var weapons = Directory.GetFiles(path + "Equipment/Weapons/", "*.json");
            var weaponDto = new List<WeaponDto>();
            
            var armour = Directory.GetFiles(path + "Equipment/Armour/", "*.json");
            var armourDto = new List<ArmourDto>();
            
            clans.ToList().ForEach(c => clansDto.Add(ClanDto.CreateFromJson(File.ReadAllText(c))));
            starterGangs.ToList().ForEach(g => gangDto.Add(GangDto.CreateFromJson(File.ReadAllText(g))));
            starterFighters.ToList().ForEach(f => fighterDto.Add(FighterDto.CreateFromJson(File.ReadAllText(f))));
            weapons.ToList().ForEach(w => weaponDto.Add(WeaponDto.CreateFromJson(File.ReadAllText(w))));
            armour.ToList().ForEach(a => armourDto.Add(ArmourDto.CreateFromJson(File.ReadAllText(a))));
            
            clansDto.ForEach(c => new Data.Clan(c));
            gangDto.ForEach(g => new Data.Gang(g));
            fighterDto.ForEach(f => new Data.Fighter(f));
            weaponDto.ForEach(w => new Data.Weapon(w));
            armourDto.ForEach(a => new Data.Armour(a));
            
            Data.Clan.All.ForEach(c => c.Create(clansDto.Find(dto => dto.id == c.ID)));
            Data.Gang.All.ForEach(g => g.Create(gangDto.Find(dto => dto.id == g.ID)));
            Data.Fighter.All.ForEach(f => f.Create(fighterDto.Find(dto => dto.id == f.ID)));
            Data.Weapon.All.ForEach(w => w.Create(weaponDto.Find(dto => dto.id == w.ID)));
            Data.Armour.All.ForEach(a => a.Create(armourDto.Find(dto => dto.id == a.ID)));
        }
    }
}