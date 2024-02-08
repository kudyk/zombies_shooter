using Unity.Entities;

namespace ZombiesShooter
{
    public struct WeaponAssigner : IComponentData
    {
        public Entity   weaponContainer;
        public WeaponID weaponChangeTo;
    }
}