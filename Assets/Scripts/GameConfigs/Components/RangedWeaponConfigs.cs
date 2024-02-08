using System;
using Unity.Entities;

namespace ZombiesShooter
{
    public struct RangedWeaponConfigsBlobReference : IComponentData
    {
        public BlobAssetReference<RangedWeaponConfigs> blobReference;
    }

    public struct RangedWeaponConfigs
    {
        public BlobArray<RangedWeaponConfig> configs;
    }

    [Serializable]
    public struct RangedWeaponConfig
    {
        public WeaponID weaponID;
        public bool     isAutomatic;
        public float    firingFrequency;
        public int      projectileDamage;
        public float    projectileSpeed;
        public float    projectileLifetime;
    }
}