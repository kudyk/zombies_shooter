using Unity.Entities;

namespace ZombiesShooter
{
    public struct WeaponView : IComponentData
    {
        public Entity impactPoint;
    }
}