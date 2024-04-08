namespace Gangs.AI {
    [System.Serializable]
    public struct EnemyAIWeightings {
        public float RemainingActionPointWeight;
        public float FullCoverWeight;
        public float HalfCoverWeight;
        public float HeightAdvantageWeight;
        public float CanFlankWeight;
        public float IsFlankedWeight;
        public float DistanceCheckWeight;
    }
}
