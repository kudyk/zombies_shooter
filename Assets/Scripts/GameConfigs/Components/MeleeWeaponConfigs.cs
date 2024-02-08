using System;
using Unity.Entities;
using Unity.Physics.Authoring;

namespace ZombiesShooter
{
    public struct MeleeWeaponConfigsBlobReference : IComponentData
    {
        public BlobAssetReference<MeleeWeaponConfigs> blobReference;
    }

    public struct MeleeWeaponConfigs
    {
        public BlobArray<MeleeWeaponConfig> configs;
    }

    [Serializable]
    public struct MeleeWeaponConfig
    {
        public WeaponID            weaponID;
        public PhysicsCategoryTags belongsTo;
        public PhysicsCategoryTags impactsOn;
        public int                 damageImpact;
        public float               impactDistance;
        public float               hitsFrequency;
    }
}