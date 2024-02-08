using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

namespace ZombiesShooter
{
    [RequireMatchingQueriesForUpdate]
    public partial struct RangedWeaponSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Projectile, PhysicsVelocity, PhysicsMass, PhysicsMassOverride>().Build(ref state);

            state.RequireForUpdate<WeaponView>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<LocalToWorld>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeArray<double> elapsedTime = new NativeArray<double>(1, Allocator.TempJob);
            elapsedTime[0] = SystemAPI.Time.ElapsedTime;

            NativeArray<Entity> projectilesArray = entityQuery.ToEntityArray(Allocator.TempJob);

            ComponentLookup<Projectile>          projectilesLookup     = SystemAPI.GetComponentLookup<Projectile>();
            ComponentLookup<WeaponView>          weaponViewLookup      = SystemAPI.GetComponentLookup<WeaponView>();
            ComponentLookup<LocalTransform>      localTransformLookup  = SystemAPI.GetComponentLookup<LocalTransform>();
            ComponentLookup<LocalToWorld>        localToWorldLookup    = SystemAPI.GetComponentLookup<LocalToWorld>();
            ComponentLookup<PhysicsMass>         physicsMassLookup     = SystemAPI.GetComponentLookup<PhysicsMass>();
            ComponentLookup<PhysicsVelocity>     physicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
            ComponentLookup<PhysicsMassOverride> massOverrideLookup    = SystemAPI.GetComponentLookup<PhysicsMassOverride>();

            state.Dependency = new RangedWeaponJob()
            {
                elapsedTime           = elapsedTime,
                projectilesArray      = projectilesArray,
                projectilesLookup     = projectilesLookup,
                weaponViewLookup      = weaponViewLookup,
                localTransformLookup  = localTransformLookup,
                localToWorldLookup    = localToWorldLookup,
                physicsMassLookup     = physicsMassLookup,
                physicsVelocityLookup = physicsVelocityLookup,
                massOverrideLookup    = massOverrideLookup,
            }.Schedule(state.Dependency);

            elapsedTime.Dispose(state.Dependency);
            projectilesArray.Dispose(state.Dependency);
        }

        [BurstCompile]
        public partial struct RangedWeaponJob : IJobEntity
        {
            public ComponentLookup<Projectile>      projectilesLookup;
            public ComponentLookup<LocalTransform>  localTransformLookup;
            public ComponentLookup<PhysicsVelocity> physicsVelocityLookup;

            [ReadOnly] public NativeArray<double>           elapsedTime;
            [ReadOnly] public NativeArray<Entity>           projectilesArray;
            [ReadOnly] public ComponentLookup<WeaponView>   weaponViewLookup;
            [ReadOnly] public ComponentLookup<LocalToWorld> localToWorldLookup;
            [ReadOnly] public ComponentLookup<PhysicsMass>  physicsMassLookup;

            [WriteOnly] public ComponentLookup<PhysicsMassOverride> massOverrideLookup;

            private void Execute(ref RangedWeapon rangedWeapon)
            {
                if (rangedWeapon.weaponConfig.weaponID == WeaponID.NONE)
                    return;

                if (rangedWeapon.weaponConfig.isAutomatic)
                    FireAsAutomatic(ref rangedWeapon);
                else
                    FireAsNonAutomatic(ref rangedWeapon);
            }

            private void FireAsAutomatic(ref RangedWeapon rangedWeapon)
            {
                if (!rangedWeapon.isTriggerDown || rangedWeapon.nextShotTime > elapsedTime[0])
                    return;

                InstantiateProjectileAndFire(in rangedWeapon);

                rangedWeapon.nextShotTime = elapsedTime[0] + rangedWeapon.weaponConfig.firingFrequency;
            }

            private void FireAsNonAutomatic(ref RangedWeapon rangedWeapon)
            {
                if (rangedWeapon.wasTriggerReleased)
                {
                    rangedWeapon.isReloaded = true;
                    return;
                }

                if (!rangedWeapon.isTriggerDown || !rangedWeapon.isReloaded || rangedWeapon.nextShotTime > elapsedTime[0])
                    return;

                InstantiateProjectileAndFire(in rangedWeapon);

                rangedWeapon.isReloaded   = false;
                rangedWeapon.nextShotTime = elapsedTime[0] + rangedWeapon.weaponConfig.firingFrequency;
            }

            private void InstantiateProjectileAndFire(in RangedWeapon rangedWeapon)
            {
                Entity projectileInstance = GetFreeProjectileFromPool();
                if (projectileInstance == Entity.Null)
                    return;

                FireInstantiatedProjectile(ref projectileInstance, in rangedWeapon);
            }

            private Entity GetFreeProjectileFromPool()
            {
                foreach (Entity projectileEntity in projectilesArray)
                {
                    Projectile projectileComponent = projectilesLookup[projectileEntity];

                    if (projectileComponent.stateInPool != StateInPool.READY_FOR_USE)
                        continue;

                    massOverrideLookup[projectileEntity] = new PhysicsMassOverride { IsKinematic = 0, SetVelocityToZero = 0 };

                    projectileComponent.stateInPool     = StateInPool.IN_USE;
                    projectilesLookup[projectileEntity] = projectileComponent;

                    return projectileEntity;
                }

                return Entity.Null;
            }

            private void FireInstantiatedProjectile(ref Entity projectileInstance, in RangedWeapon rangedWeapon)
            {
                WeaponView     weaponView               = weaponViewLookup[rangedWeapon.weaponView];
                LocalToWorld   firingPointLocalToWorld  = localToWorldLookup[weaponView.impactPoint];
                LocalTransform projectileLocalTransform = localTransformLookup[projectileInstance];

                localTransformLookup[projectileInstance] = projectileLocalTransform.WithPosition(firingPointLocalToWorld.Position);

                Projectile projectileComponent = projectilesLookup[projectileInstance];
                projectileComponent.timeBeforeAutoreset = elapsedTime[0] + rangedWeapon.weaponConfig.projectileLifetime;
                projectileComponent.damageImpact        = rangedWeapon.weaponConfig.projectileDamage;
                projectilesLookup[projectileInstance]   = projectileComponent;

                PhysicsMass     projectileMass     = physicsMassLookup[projectileInstance];
                PhysicsVelocity projectileVelocity = physicsVelocityLookup[projectileInstance];

                float3 newVelocity = Float3Ext.LocalToWorld(firingPointLocalToWorld.Value, Vector3.forward) * rangedWeapon.weaponConfig.projectileSpeed;
                projectileVelocity.ApplyLinearImpulse(in projectileMass, newVelocity);
                physicsVelocityLookup[projectileInstance] = projectileVelocity;
            }
        }
    }
}