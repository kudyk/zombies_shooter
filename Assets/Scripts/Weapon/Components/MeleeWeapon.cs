using Unity.Entities;

namespace ZombiesShooter
{
    public struct MeleeWeapon : IComponentData, IEnableableComponent
    {
        public Entity            weaponView;
        public MeleeWeaponConfig weaponConfig;

        public double nextHitTime;
        public bool   isAttacking;
    }
}