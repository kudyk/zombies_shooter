using Unity.Entities;

namespace ZombiesShooter
{
    public struct BaseInputConfigsBlobReference : IComponentData
    {
        public BlobAssetReference<BaseInputConfigs> blobReference;
    }

    public struct BaseInputConfigs
    {
        public BlobArray<BaseInputConfig> configs;
    }

    public struct BaseInputConfig
    {
        public BaseInputConfigType configType;
        public BaseInputParam      configParam;
    }

    public struct BaseInputParam
    {
        public BlobString movementHorizontal;
        public BlobString movementVertical;
        public BlobString firingInput;
    }
}