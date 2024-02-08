using Unity.Entities;

namespace ZombiesShooter
{
    public struct EnemyTargetsFinder : IComponentData
    {
        public float minDistanceForLinkSQ;
        public float minDistanceDifferenceForResetSQ;
    }
}