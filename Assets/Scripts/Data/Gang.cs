using System.Collections.Generic;
using System.Linq;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Gang : Entity {
        public static List<Gang> All { get; set; } = new();
                
        public Faction Faction { get; set; }
        public List<Fighter> Fighters { get; set; }
        
        public Gang(GangDto dto) : base(dto) {
            All.Add(this);
        }
        
        public void Create(GangDto dto) {
            Faction = Faction.All.Find(c => c.ID == dto.clanId);
            Fighters = Fighter.All.FindAll(f => dto.fighterIds.ToList().Contains(f.ID));
        }
    }
}