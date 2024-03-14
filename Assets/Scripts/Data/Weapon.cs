using System.Collections.Generic;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Weapon : Equipment {
        public static List<Weapon> All { get; set; } = new();
        
        public int Damage { get; set; }
        public int ShortRange { get; set; }
        public int ShortRangeModifier { get; set; }
        public int LongRangeModifier { get; set; }

        public Weapon(WeaponDto dto) : base(dto) {
            All.Add(this);
        }
        
        public void Create(WeaponDto dto) {
            Damage = dto.damage;
            ShortRange = dto.shortRange;
            ShortRangeModifier = dto.shortRangeModifier;
            LongRangeModifier = dto.longRangeModifier;
        }
    }
}