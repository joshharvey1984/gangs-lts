using Gangs.Data.DTO;

namespace Gangs.Data {
    public abstract class Entity {
        public string ID { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }

        protected Entity(EntityDto dto) {
            ID = dto.id;
            Name = dto.name;
            Description = dto.description;
        }
    }
}