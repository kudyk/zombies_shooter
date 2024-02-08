using Unity.Entities;

namespace ZombiesShooter
{
    public struct RangedWeapon : IComponentData, IEnableableComponent
    {
        public Entity             weaponView;
        public RangedWeaponConfig weaponConfig;

        public bool   isReloaded;
        public bool   isTriggerDown;
        public bool   wasTriggerReleased;
        public double nextShotTime;
    }
}