using Unity.Collections;
using Unity.Entities;

namespace ZombiesShooter
{
    // TODO: Don't convert to IJobEntity, remove comment before release!
    [UpdateBefore(typeof(MortalBeingPhysicsDisablerSystem))]
    [RequireMatchingQueriesForUpdate]
    public abstract partial class MortalBeingComponentsDisablerSystem<T> : SystemBase where T : IComponentData
    {
        private EntityQuery entityQuery;

        protected override void OnCreate()
        {
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<T, MortalBeing, MortalBeingDisabler>().Build(this);
        }

        protected override void OnUpdate()
        {
            foreach (Entity entity in entityQuery.ToEntityArray(Allocator.Temp))
            {
                RefRO<MortalBeing>         mortalBeing         = SystemAPI.GetComponentRO<MortalBeing>(entity);
                RefRW<MortalBeingDisabler> mortalBeingDisabler = SystemAPI.GetComponentRW<MortalBeingDisabler>(entity);

                if (mortalBeing.ValueRO.currentHealth > 0 || mortalBeingDisabler.ValueRO.componentsDisabled)
                    continue;

                DisableComponents(entity);

                mortalBeingDisabler.ValueRW.componentsDisabled = true;
            }
        }

        protected abstract void DisableComponents(Entity entity);
    }
}