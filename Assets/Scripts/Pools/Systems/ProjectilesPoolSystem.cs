using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace ZombiesShooter
{
    // TODO: Don't convert to IJobEntity, remove comment before release!
    // Despite the DOTS documentation recommendation about no need for pools in ECS, created projectiles pool for real optimization.
    // Tried to create managed generic version of PoolBase on SystemBase, but nowadays burst compiler crashes with such solution.
    public partial struct ProjectilesPoolSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectilesPool>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Projectile>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ProjectilesPool     projectilesPool = SystemAPI.GetSingleton<ProjectilesPool>();
            EntityCommandBuffer commandBuffer   = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            if (entityQuery.IsEmpty)
            {
                GenerateBatchOfInstances(in projectilesPool, ref commandBuffer, ref state);
                return;
            }

            TryToResetUnusedProjectiles(in projectilesPool, ref state);

            NativeArray<Projectile> projectileComponents = entityQuery.ToComponentDataArray<Projectile>(Allocator.Temp);
            int                     freeProjectilesCount = GetFreeProjectilesCount(in projectileComponents);

            if (freeProjectilesCount < projectilesPool.batchSize)
                GenerateBatchOfInstances(in projectilesPool, ref commandBuffer, ref state);
        }

        private void TryToResetUnusedProjectiles(in ProjectilesPool projectilesPool, ref SystemState state)
        {
            NativeArray<Entity> projectilesArray = entityQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity projectile in projectilesArray)
            {
                RefRW<Projectile> projectileComponent = SystemAPI.GetComponentRW<Projectile>(projectile);

                if (projectileComponent.ValueRO.stateInPool != StateInPool.NEED_FOR_RESET)
                    continue;

                SystemAPI.SetComponent(projectile, GetDefaultLocalTransform(projectilesPool.defaultPosition));
                SystemAPI.SetComponent(projectile, GetDefaultPhysicsMassOverride());

                RefRW<PhysicsVelocity> projectileVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(projectile);
                projectileVelocity.ValueRW.Angular = float3.zero;
                projectileVelocity.ValueRW.Linear  = float3.zero;

                projectileComponent.ValueRW.stateInPool = StateInPool.READY_FOR_USE;
            }
        }

        private void GenerateBatchOfInstances(in ProjectilesPool projectilesPool, ref EntityCommandBuffer commandBuffer, ref SystemState state)
        {
            for (int i = 0; i < projectilesPool.batchSize; i++)
            {
                Entity projectileInstance = commandBuffer.Instantiate(projectilesPool.projectilePrefab);
                commandBuffer.SetComponent(projectileInstance, GetDefaultLocalTransform(projectilesPool.defaultPosition));
                commandBuffer.AddComponent(projectileInstance, GetDefaultPhysicsMassOverride());
            }
        }

        private int GetFreeProjectilesCount(in NativeArray<Projectile> projectiles)
        {
            int result = 0;

            for (int i = 0; i < projectiles.Length; i++)
            {
                if (projectiles[i].stateInPool == StateInPool.READY_FOR_USE)
                    result++;
            }

            return result;
        }

        private PhysicsMassOverride GetDefaultPhysicsMassOverride()
        {
            return new PhysicsMassOverride { IsKinematic = 1, SetVelocityToZero = 1 };
        }

        private LocalTransform GetDefaultLocalTransform(float3 defaultPosition)
        {
            return new LocalTransform { Position = defaultPosition, Rotation = quaternion.identity, Scale = 1.0f };
        }
    }
}