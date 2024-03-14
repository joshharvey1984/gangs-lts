using System.Collections.Generic;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Armour : Equipment {
        public static List<Armour> All { get; set; } = new();
        
        public int DamageReduction { get; set; }
        
        public Armour(ArmourDto dto) : base(dto) {
            All.Add(this);
        }
        
        public void Create(ArmourDto dto) {
            DamageReduction = dto.damageReduction;
        }
    }
}