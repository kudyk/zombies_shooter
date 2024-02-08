using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    // TODO: Don't convert to IJobEntity, remove comment before release!
    public partial struct WeaponChangeInputHandlerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponChangeInputConfigsBlobReference>();
            state.RequireForUpdate<WeaponChangeInputHandler>();
            state.RequireForUpdate<WeaponAssigner>();
        }

        public void OnUpdate(ref SystemState state)
        {
            WeaponChangeInputConfigsBlobReference  configsComponent = SystemAPI.GetSingleton<WeaponChangeInputConfigsBlobReference>();
            ref BlobArray<WeaponChangeInputConfig> inputConfigs     = ref configsComponent.blobReference.Value.configs;

            foreach (var (inputHandler, weaponAssigner) in SystemAPI.Query<RefRO<WeaponChangeInputHandler>, RefRW<WeaponAssigner>>())
            {
                int configIndex = TryGetCurrentConfigIndex(ref inputConfigs, inputHandler.ValueRO.configType);
                if (configIndex < 0)
                    continue;

                ref BlobArray<WeaponChangeInputParam> currentConfig = ref inputConfigs[configIndex].configParams;

                for (int i = 0; i < currentConfig.Length; i++)
                {
                    if (Input.GetAxis(currentConfig[i].axisKey.ToString()) > 0)
                    {
                        weaponAssigner.ValueRW.weaponChangeTo = currentConfig[i].weaponID;
                        break;
                    }
                }
            }
        }

        private int TryGetCurrentConfigIndex(ref BlobArray<WeaponChangeInputConfig> inputConfigs, WeaponChangeInputConfigType configType)
        {
            int configIndex = -1;
            for (int i = 0; i < inputConfigs.Length; i++)
            {
                if (inputConfigs[i].configType == configType)
                {
                    configIndex = i;
                    break;
                }
            }

            return configIndex;
        }
    }
}