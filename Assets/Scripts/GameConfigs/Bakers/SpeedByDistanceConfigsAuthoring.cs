using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ZombiesShooter
{
    public class SpeedByDistanceConfigsAuthoring : MonoBehaviour
    {
        public ConfigAuthoring[] configs;

        [Serializable]
        public struct ConfigAuthoring
        {
            public SpeedByDistanceConfigType configType;
            public ParamAuthoring[]          configParams;
        }

        [Serializable]
        public struct ParamAuthoring
        {
            public float distance;
            public float speed;
        }
    }


    public class SpeedByDistanceConfigsBaker : Baker<SpeedByDistanceConfigsAuthoring>
    {
        public override void Bake(SpeedByDistanceConfigsAuthoring authoring)
        {
            BlobBuilder builder = new BlobBuilder(Allocator.Temp);

            ref SpeedByDistanceConfigs configs = ref builder.ConstructRoot<SpeedByDistanceConfigs>();

            BlobBuilderArray<SpeedByDistanceConfig> arrayBuilder = builder.Allocate(
                ref configs.configs,
                authoring.configs.Length
            );

            for (int i = 0; i < authoring.configs.Length; i++)
            {
                arrayBuilder[i] = new SpeedByDistanceConfig()
                {
                    configType = authoring.configs[i].configType
                };

                SpeedByDistanceConfigsAuthoring.ParamAuthoring[] paramsAuthoring = authoring.configs[i].configParams;
                SpeedByDistanceParam[]                           paramsBaked     = new SpeedByDistanceParam[paramsAuthoring.Length];

                for (var j = 0; j < paramsAuthoring.Length; j++)
                {
                    SpeedByDistanceConfigsAuthoring.ParamAuthoring distanceParam = paramsAuthoring[j];

                    paramsBaked[j] = new SpeedByDistanceParam()
                    {
                        distanceSQ = math.pow(distanceParam.distance, 2),
                        speed      = distanceParam.speed
                    };
                }

                builder.Construct(ref arrayBuilder[i].configParams, paramsBaked);
            }

            BlobAssetReference<SpeedByDistanceConfigs> blobReference
                = builder.CreateBlobAssetReference<SpeedByDistanceConfigs>(Allocator.Persistent);

            builder.Dispose();

            AddBlobAsset(ref blobReference, out _);
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpeedByDistanceConfigsBlobReference { blobReference = blobReference });
        }
    }
}