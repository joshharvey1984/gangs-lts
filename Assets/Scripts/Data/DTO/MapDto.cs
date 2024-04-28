using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class MapDto : EntityDto {
        public TileDto[] tiles;
        public WallDto[] walls;
        public LadderDto[] ladders;
        
        public static MapDto CreateFromJson(string json) => JsonUtility.FromJson<MapDto>(json);
    }
    
    [Serializable]
    public class TileDto : EntityDto {
        public int x;
        public int y;
        public int z;
    }
    
    [Serializable]
    public class WallDto : EntityDto {
        public float x;
        public int y;
        public float z;
    }
    
    [Serializable]
    public class LadderDto : EntityDto {
        public float x;
        public int y;
        public float z;
        public int rotation;
    }
}