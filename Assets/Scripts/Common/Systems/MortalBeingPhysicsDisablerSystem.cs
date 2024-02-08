using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;

namespace ZombiesShooter
{
    [RequireMatchingQueriesForUpdate]
    public partial struct MortalBeingPhysicsDisablerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MortalBeing>();
            state.RequireForUpdate<MortalBeingDisabler>();
            state.RequireForUpdate<PhysicsMassOverride>();
            state.RequireForUpdate<PhysicsCollider>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new PhysicsDisablingJob().ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct PhysicsDisablingJob : IJobEntity
    {
        private void Execute(in MortalBeing mortalBeing, ref MortalBeingDisabler mortalBeingDisabler, ref PhysicsMassOverride massOverride, ref PhysicsCollider physicsCollider)
        {
            if (mortalBeing.currentHealth > 0 || mortalBeingDisabler.physicsDisabled)
                return;

            massOverride.IsKinematic       = 1;
            massOverride.SetVelocityToZero = 1;

            // ForceUnique in PhysicsShape doesn't work, so that is a temporary solution
            BlobAssetReference<Collider> clonedCollider = physicsCollider.Value.Value.Clone();
            clonedCollider.As<Collider>().SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
            physicsCollider = clonedCollider.AsComponent();

            mortalBeingDisabler.physicsDisabled = true;
        }
    }
}