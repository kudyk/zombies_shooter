using Unity.Entities;

namespace ZombiesShooter
{
    public struct Projectile : IComponentData
    {
        public StateInPool stateInPool;
        public int         damageImpact;

        public double timeBeforeAutoreset;
    }
}