using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ZombiesShooter
{
    // TODO: Don't convert to IJobEntity, remove comment before release!
    public partial struct BaseInputHandlerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BaseInputConfigsBlobReference>();
            state.RequireForUpdate<BaseInputHandler>();
            state.RequireForUpdate<RangedWeapon>();
            state.RequireForUpdate<MovableByImpulseForce>();
        }

        public void OnUpdate(ref SystemState state)
        {
            BaseInputConfigsBlobReference baseInputConfigs = SystemAPI.GetSingleton<BaseInputConfigsBlobReference>();

            foreach (var (playerInputHandler, rangedWeapon, movableByImpulse) in SystemAPI.Query<RefRW<BaseInputHandler>, RefRW<RangedWeapon>, RefRW<MovableByImpulseForce>>())
            {
                int configIndex = TryGetCurrentConfigIndex(ref baseInputConfigs.blobReference.Value.configs, playerInputHandler.ValueRO.configType);
                if (configIndex < 0)
                    continue;

                ref BaseInputParam configParams = ref baseInputConfigs.blobReference.Value.configs[configIndex].configParam;

                movableByImpulse.ValueRW.impulseForce = new float3(Input.GetAxis(configParams.movementHorizontal.ToString()), 0, Input.GetAxis(configParams.movementVertical.ToString()));

                bool isFiring = Input.GetAxis(configParams.firingInput.ToString()) > 0.9f;

                rangedWeapon.ValueRW.isTriggerDown      = isFiring;
                rangedWeapon.ValueRW.wasTriggerReleased = !isFiring && playerInputHandler.ValueRO.prevFiringState;

                playerInputHandler.ValueRW.prevFiringState = isFiring;
            }
        }

        private int TryGetCurrentConfigIndex(ref BlobArray<BaseInputConfig> inputConfigs, BaseInputConfigType configType)
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