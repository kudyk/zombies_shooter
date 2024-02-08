using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Works only with buffer ordered by ascending distance.
// No per-frame auto ordering logic for better optimization.
namespace ZombiesShooter
{
    public partial struct SpeedByDistanceChangerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpeedByDistanceConfigsBlobReference>();
            state.RequireForUpdate<SpeedByDistanceChanger>();
            state.RequireForUpdate<MovableOnTargetByImpulse>();
            state.RequireForUpdate<MovementParams>();
            state.RequireForUpdate<LocalToWorld>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SpeedByDistanceConfigsBlobReference speedByDistanceConfigsComponent = SystemAPI.GetSingleton<SpeedByDistanceConfigsBlobReference>();
            ComponentLookup<LocalToWorld>       localToWorldLookup              = SystemAPI.GetComponentLookup<LocalToWorld>();

            state.Dependency = new SpeedChangeJob()
            {
                speedByDistanceConfigs = speedByDistanceConfigsComponent.blobReference,
                localToWorldLookup     = localToWorldLookup
            }.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct SpeedChangeJob : IJobEntity
    {
        [ReadOnly] public BlobAssetReference<SpeedByDistanceConfigs> speedByDistanceConfigs;
        [ReadOnly] public ComponentLookup<LocalToWorld>              localToWorldLookup;

        private void Execute(in SpeedByDistanceChanger speedByDistanceChanger, in MovableOnTargetByImpulse movableOnTarget, ref MovementParams movementParams, in LocalToWorld currentLocalToWorld)
        {
            if (movableOnTarget.targetEntity == Entity.Null)
                return;

            int configIndex = TryGetConfigIndex(speedByDistanceChanger.configType);

            if (configIndex < 0)
                return;

            ref SpeedByDistanceConfig speedByDistanceConfig = ref speedByDistanceConfigs.Value.configs[configIndex];

            LocalToWorld targetLocalToWorld = localToWorldLookup[movableOnTarget.targetEntity];
            float        distanceSQ         = math.distancesq(currentLocalToWorld.Position, targetLocalToWorld.Position);

            for (int i = 0; i < speedByDistanceConfig.configParams.Length; i++)
            {
                ref SpeedByDistanceParam speedByDistanceParam = ref speedByDistanceConfig.configParams[i];

                if (distanceSQ > speedByDistanceParam.distanceSQ)
                    continue;

                if (math.abs(movementParams.maxSpeed - speedByDistanceParam.speed) < 0.1f)
                    break;

                movementParams.maxSpeed = speedByDistanceParam.speed;
                break;
            }
        }

        int TryGetConfigIndex(SpeedByDistanceConfigType configType)
        {
            int configIndex = -1;

            for (int i = 0; i < speedByDistanceConfigs.Value.configs.Length; i++)
            {
                if (speedByDistanceConfigs.Value.configs[i].configType == configType)
                {
                    configIndex = i;
                    break;
                }
            }

            return configIndex;
        }
    }
}