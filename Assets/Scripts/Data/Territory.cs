using System.Collections.Generic;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Territory : Entity {
        public static List<Territory> All { get; set; } = new();
        
        public Territory(EntityDto dto) : base(dto) {
            All.Add(this);
        }

        public void Create(TerritoryDto dto) {
        }
    }
}