using Unity.Entities;

namespace ZombiesShooter
{
    public struct WeaponChangeInputConfigsBlobReference : IComponentData
    {
        public BlobAssetReference<WeaponChangeInputConfigs> blobReference;
    }

    public struct WeaponChangeInputConfigs
    {
        public BlobArray<WeaponChangeInputConfig> configs;
    }

    public struct WeaponChangeInputConfig
    {
        public WeaponChangeInputConfigType       configType;
        public BlobArray<WeaponChangeInputParam> configParams;
    }

    public struct WeaponChangeInputParam
    {
        public BlobString axisKey;
        public WeaponID   weaponID;
    }
}