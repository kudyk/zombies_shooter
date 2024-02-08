using Unity.Burst;
using Unity.Entities;

namespace ZombiesShooter
{
    [UpdateInGroup(typeof(WeaponAssignerSystemGroup), OrderLast = true)]
    public partial struct WeaponAssignerQueriesCleanerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponAssigner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new QueriesCleanJob().Schedule(state.Dependency);
        }

        [BurstCompile]
        public partial struct QueriesCleanJob : IJobEntity
        {
            private void Execute(ref WeaponAssigner weaponAssigner)
            {
                if (weaponAssigner.weaponChangeTo == WeaponID.NONE)
                    return;

                // Can be added some additional logic or logging of unhandled case
                weaponAssigner.weaponChangeTo = WeaponID.NONE;
            }
        }
    }
}