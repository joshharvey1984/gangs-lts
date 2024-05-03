namespace Gangs.Battle.AI {
    [System.Serializable]
    public struct BattleAIWeightings {
        public float RemainingActionPointWeight;
        public float FullCoverWeight;
        public float HalfCoverWeight;
        public float HeightAdvantageWeight;
        public float CanFlankWeight;
        public float IsFlankedWeight;
        public float DistanceCheckWeight;
    }
}
