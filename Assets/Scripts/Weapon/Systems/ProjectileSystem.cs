using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace ZombiesShooter
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct ProjectileSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<Projectile>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            double elapsedTime = SystemAPI.Time.ElapsedTime;

            foreach (RefRW<Projectile> projectileComponent in SystemAPI.Query<RefRW<Projectile>>())
            {
                if (projectileComponent.ValueRO.timeBeforeAutoreset <= elapsedTime)
                    projectileComponent.ValueRW.stateInPool = StateInPool.NEED_FOR_RESET;
            }

            state.Dependency = new ProjectileCollisionDetectionJob
            {
                projectilesLookup  = SystemAPI.GetComponentLookup<Projectile>(),
                mortalBeingsLookup = SystemAPI.GetComponentLookup<MortalBeing>(),
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }

        [BurstCompile]
        private struct ProjectileCollisionDetectionJob : ICollisionEventsJob
        {
            public ComponentLookup<Projectile>  projectilesLookup;
            public ComponentLookup<MortalBeing> mortalBeingsLookup;

            public void Execute(CollisionEvent collisionEvent)
            {
                Entity entityA = collisionEvent.EntityA;
                Entity entityB = collisionEvent.EntityB;

                bool projectileIsA = projectilesLookup.HasComponent(entityA);
                bool projectileIsB = projectilesLookup.HasComponent(entityB);

                bool mortalIsA = mortalBeingsLookup.HasComponent(entityA);
                bool mortalIsB = mortalBeingsLookup.HasComponent(entityB);

                if (projectileIsA && mortalIsB)
                    SetProjectileImpactOnTarget(in entityA, in entityB);

                if (mortalIsA && projectileIsB)
                    SetProjectileImpactOnTarget(in entityB, in entityA);
            }

            private void SetProjectileImpactOnTarget(in Entity projectileEntity, in Entity mortalEntity)
            {
                MortalBeing mortalBeing = mortalBeingsLookup[mortalEntity];
                Projectile  projectile  = projectilesLookup[projectileEntity];

                if (projectile.stateInPool != StateInPool.IN_USE)
                    return;

                if (mortalBeing.currentHealth > 0)
                {
                    mortalBeing.damageImpactQueue.Add(projectile.damageImpact);
                    mortalBeingsLookup[mortalEntity] = mortalBeing;
                }

                projectile.stateInPool              = StateInPool.NEED_FOR_RESET;
                projectilesLookup[projectileEntity] = projectile;
            }
        }
    }
}