using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class WeaponChangeInputConfigsAuthoring : MonoBehaviour
    {
        public ConfigAuthoring[] configs;

        [Serializable]
        public struct ConfigAuthoring
        {
            public WeaponChangeInputConfigType configType;
            public ParamAuthoring[]            configParams;
        }

        [Serializable]
        public struct ParamAuthoring
        {
            public string   axisKey;
            public WeaponID weaponID;
        }
    }

    public class WeaponChangeInputConfigsBaker : Baker<WeaponChangeInputConfigsAuthoring>
    {
        public override void Bake(WeaponChangeInputConfigsAuthoring authoring)
        {
            BlobBuilder builder = new BlobBuilder(Allocator.Temp);

            ref WeaponChangeInputConfigs configs = ref builder.ConstructRoot<WeaponChangeInputConfigs>();

            BlobBuilderArray<WeaponChangeInputConfig> arrayBuilder = builder.Allocate(
                ref configs.configs,
                authoring.configs.Length
            );

            for (int i = 0; i < authoring.configs.Length; i++)
            {
                arrayBuilder[i] = new WeaponChangeInputConfig()
                {
                    configType = authoring.configs[i].configType
                };

                BlobBuilderArray<WeaponChangeInputParam> paramsBaked = builder.Allocate(
                    ref arrayBuilder[i].configParams,
                    authoring.configs[i].configParams.Length
                );

                WeaponChangeInputConfigsAuthoring.ParamAuthoring[] paramsAuthoring = authoring.configs[i].configParams;

                for (var j = 0; j < paramsAuthoring.Length; j++)
                {
                    WeaponChangeInputConfigsAuthoring.ParamAuthoring configParam = paramsAuthoring[j];

                    paramsBaked[j] = new WeaponChangeInputParam()
                    {
                        weaponID = configParam.weaponID
                    };

                    builder.AllocateString(ref paramsBaked[j].axisKey, configParam.axisKey);
                }
            }

            BlobAssetReference<WeaponChangeInputConfigs> blobReference
                = builder.CreateBlobAssetReference<WeaponChangeInputConfigs>(Allocator.Persistent);

            builder.Dispose();

            AddBlobAsset(ref blobReference, out _);
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new WeaponChangeInputConfigsBlobReference { blobReference = blobReference });
        }
    }
}