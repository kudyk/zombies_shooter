using Unity.Entities;

namespace ZombiesShooter
{
    public struct SpeedByDistanceConfigsBlobReference : IComponentData
    {
        public BlobAssetReference<SpeedByDistanceConfigs> blobReference;
    }

    public struct SpeedByDistanceConfigs
    {
        public BlobArray<SpeedByDistanceConfig> configs;
    }

    public struct SpeedByDistanceConfig
    {
        public SpeedByDistanceConfigType       configType;
        public BlobArray<SpeedByDistanceParam> configParams;
    }

    public struct SpeedByDistanceParam
    {
        public float distanceSQ;
        public float speed;
    }
}