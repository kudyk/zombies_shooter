using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ZombiesShooter
{
    [UpdateAfter(typeof(EnemyTargetsFinderSystem))]
    [UpdateBefore(typeof(MeleeWeaponSystem))]
    public partial struct MeleeAutoAttackerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MeleeWeapon>();
            state.RequireForUpdate<MeleeAutoAttacker>();
            state.RequireForUpdate<LocalToWorld>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ComponentLookup<LocalToWorld> localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();

            state.Dependency = new AutoAttackerJob()
            {
                localToWorldLookup = localToWorldLookup
            }.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct AutoAttackerJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalToWorld> localToWorldLookup;

        private void Execute(ref MeleeWeapon meleeWeapon, in MeleeAutoAttacker meleeAutoAttacker, in LocalToWorld localToWorld)
        {
            if (meleeAutoAttacker.targetEntity == Entity.Null)
            {
                if (meleeWeapon.isAttacking)
                    meleeWeapon.isAttacking = false;

                return;
            }

            LocalToWorld targetLocalToWorld = localToWorldLookup[meleeAutoAttacker.targetEntity];
            float        distanceSQ         = math.distancesq(localToWorld.Position, targetLocalToWorld.Position);

            meleeWeapon.isAttacking = distanceSQ < meleeAutoAttacker.distanceForAttackSQ;
        }
    }
}