using Unity.Entities;

namespace ZombiesShooter
{
    public struct MeleeAutoAttacker : IComponentData
    {
        public Entity targetEntity;
        public float  distanceForAttackSQ;
    }
}