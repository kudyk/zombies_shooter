using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ZombiesShooter
{
    // TODO: Don't convert to IJobEntity, remove comment before release!
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct ItemsByCircleSpawnerSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<ItemsByCircleSpawner>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (Entity spawnerEntity in entityQuery.ToEntityArray(Allocator.Temp))
            {
                DynamicBuffer<ItemsByCircleSpawner> spawnersBuffer = SystemAPI.GetBuffer<ItemsByCircleSpawner>(spawnerEntity);

                for (int i = spawnersBuffer.Length - 1; i >= 0; i--)
                {
                    ItemsByCircleSpawner itemsByCircleSpawner = spawnersBuffer[i];
                    RefRO<LocalToWorld>  localToWorld         = SystemAPI.GetComponentRO<LocalToWorld>(spawnerEntity);

                    NativeArray<float3> positions = GetSpawnPositions(in itemsByCircleSpawner, in localToWorld.ValueRO);
                    SpawnItems(in positions, in itemsByCircleSpawner, ref state);
                }

                SystemAPI.SetBufferEnabled<ItemsByCircleSpawner>(spawnerEntity, false);
            }
        }

        private void SpawnItems(in NativeArray<float3> positions, in ItemsByCircleSpawner itemsByCircleSpawner, ref SystemState state)
        {
            for (int j = 0; j < itemsByCircleSpawner.spawnCount; j++)
            {
                Entity                entityInstance = state.EntityManager.Instantiate(itemsByCircleSpawner.itemPrefab);
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(entityInstance);
                localTransform.ValueRW.Position = positions[j];
            }
        }

        private NativeArray<float3> GetSpawnPositions(in ItemsByCircleSpawner itemsByCircleSpawner, in LocalToWorld localToWorld)
        {
            NativeArray<float3> positions = new NativeArray<float3>(itemsByCircleSpawner.spawnCount, Allocator.Temp);

            for (int j = 0; j < itemsByCircleSpawner.spawnCount; j++)
            {
                float currentAngle  = itemsByCircleSpawner.startAngle + (itemsByCircleSpawner.angleStep * j);
                float currentRadius = itemsByCircleSpawner.startRadius + (itemsByCircleSpawner.radiusStep * j);

                float posX = currentRadius * math.sin(currentAngle);
                float posZ = currentRadius * math.cos(currentAngle);

                float3 worldPosition = localToWorld.Position;
                positions[j] = new float3(worldPosition.x + posX, itemsByCircleSpawner.positionY, worldPosition.z + posZ);
            }

            return positions;
        }
    }
}