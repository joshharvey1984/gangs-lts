using System;

namespace Gangs.Data.DTO {
    [Serializable]
    public abstract class EntityDto {
        public string id;
        public string name;
        public string description;
    }
}