using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

namespace ZombiesShooter
{
    public partial struct MeleeWeaponSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MortalBeing>();
            state.RequireForUpdate<MeleeWeapon>();
            state.RequireForUpdate<WeaponView>();
            state.RequireForUpdate<LocalToWorld>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeArray<double> elapsedTime = new NativeArray<double>(1, Allocator.TempJob);
            elapsedTime[0] = SystemAPI.Time.ElapsedTime;

            PhysicsWorldSingleton         physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            ComponentLookup<WeaponView>   weaponViewLookup      = SystemAPI.GetComponentLookup<WeaponView>();
            ComponentLookup<MortalBeing>  mortalBeingLookup     = SystemAPI.GetComponentLookup<MortalBeing>();
            ComponentLookup<LocalToWorld> localToWorldLookup    = SystemAPI.GetComponentLookup<LocalToWorld>(true);

            state.Dependency = new MeleeAttackJob()
            {
                physicsWorldSingleton = physicsWorldSingleton,
                mortalBeingLookup     = mortalBeingLookup,
                weaponViewLookup      = weaponViewLookup,
                localToWorldLookup    = localToWorldLookup,
                elapsedTime           = elapsedTime,
            }.Schedule(state.Dependency);

            elapsedTime.Dispose(state.Dependency);
        }

        [BurstCompile]
        private partial struct MeleeAttackJob : IJobEntity
        {
            public ComponentLookup<MortalBeing> mortalBeingLookup;

            [ReadOnly] public NativeArray<double>           elapsedTime;
            [ReadOnly] public ComponentLookup<WeaponView>   weaponViewLookup;
            [ReadOnly] public ComponentLookup<LocalToWorld> localToWorldLookup;
            [ReadOnly] public PhysicsWorldSingleton         physicsWorldSingleton;

            private void Execute(ref MeleeWeapon meleeWeapon)
            {
                if (!meleeWeapon.isAttacking || meleeWeapon.nextHitTime > elapsedTime[0] || meleeWeapon.weaponConfig.weaponID == WeaponID.NONE)
                    return;

                WeaponView   weaponView         = weaponViewLookup[meleeWeapon.weaponView];
                LocalToWorld originLocalToWorld = localToWorldLookup[weaponView.impactPoint];

                float3 startPosition = originLocalToWorld.Position;
                float3 endPosition   = startPosition + originLocalToWorld.Forward * meleeWeapon.weaponConfig.impactDistance;

                CollisionFilter collisionFilter = new CollisionFilter()
                {
                    BelongsTo    = meleeWeapon.weaponConfig.belongsTo.Value,
                    CollidesWith = meleeWeapon.weaponConfig.impactsOn.Value,
                    GroupIndex   = 0
                };

                RaycastInput raycastInput = new RaycastInput()
                {
                    Start  = startPosition,
                    End    = endPosition,
                    Filter = collisionFilter
                };

                if (physicsWorldSingleton.CastRay(raycastInput, out RaycastHit closestHit))
                {
                    if (mortalBeingLookup.TryGetComponent(closestHit.Entity, out MortalBeing mortalBeing))
                    {
                        if (mortalBeing.currentHealth > 0)
                        {
                            mortalBeing.damageImpactQueue.Add(meleeWeapon.weaponConfig.damageImpact);
                            mortalBeingLookup[closestHit.Entity] = mortalBeing;
                        }
                    }
                }

                meleeWeapon.nextHitTime = elapsedTime[0] + meleeWeapon.weaponConfig.hitsFrequency;
            }
        }
    }
}