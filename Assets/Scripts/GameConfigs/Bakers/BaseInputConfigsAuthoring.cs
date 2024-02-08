using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class BaseInputConfigsAuthoring : MonoBehaviour
    {
        public ConfigAuthoring[] configs;

        [Serializable]
        public struct ConfigAuthoring
        {
            public BaseInputConfigType configType;
            public ParamAuthoring      configParam;
        }

        [Serializable]
        public struct ParamAuthoring
        {
            public string movementHorizontal;
            public string movementVertical;
            public string firingInput;
        }
    }

    public class BaseInputConfigsBaker : Baker<BaseInputConfigsAuthoring>
    {
        public override void Bake(BaseInputConfigsAuthoring authoring)
        {
            BlobBuilder builder = new BlobBuilder(Allocator.Temp);

            ref BaseInputConfigs configs = ref builder.ConstructRoot<BaseInputConfigs>();

            BlobBuilderArray<BaseInputConfig> arrayBuilder = builder.Allocate(
                ref configs.configs,
                authoring.configs.Length
            );

            for (int i = 0; i < authoring.configs.Length; i++)
            {
                arrayBuilder[i] = new BaseInputConfig()
                {
                    configType  = authoring.configs[i].configType,
                    configParam = new BaseInputParam()
                };

                BaseInputConfigsAuthoring.ParamAuthoring configParam = authoring.configs[i].configParam;

                builder.AllocateString(ref arrayBuilder[i].configParam.movementHorizontal, configParam.movementHorizontal);
                builder.AllocateString(ref arrayBuilder[i].configParam.movementVertical, configParam.movementVertical);
                builder.AllocateString(ref arrayBuilder[i].configParam.firingInput, configParam.firingInput);
            }

            BlobAssetReference<BaseInputConfigs> blobReference
                = builder.CreateBlobAssetReference<BaseInputConfigs>(Allocator.Persistent);

            builder.Dispose();

            AddBlobAsset(ref blobReference, out _);
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BaseInputConfigsBlobReference { blobReference = blobReference });
        }
    }
}