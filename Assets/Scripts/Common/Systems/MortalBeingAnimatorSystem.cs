using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ZombiesShooter
{
    public partial struct MortalBeingAnimatorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MortalBeing>();
            state.RequireForUpdate<MortalBeingView>();
            state.RequireForUpdate<MortalBeingAnimator>();
            state.RequireForUpdate<LocalTransform>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ComponentLookup<LocalTransform> localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();

            state.Dependency = new MortalBeingAnimationJob()
            {
                localTransformLookup = localTransformLookup
            }.Schedule(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct MortalBeingAnimationJob : IJobEntity
    {
        public ComponentLookup<LocalTransform> localTransformLookup;

        private void Execute(in MortalBeing mortalBeing, in MortalBeingView mortalBeingView, ref MortalBeingAnimator mortalBeingAnimator)
        {
            if (mortalBeing.currentHealth > 0 || mortalBeingAnimator.animationPlayed)
                return;

            LocalTransform viewLocalTransform = localTransformLookup[mortalBeingView.viewEntity];
            viewLocalTransform.Position = mortalBeingAnimator.viewDeadPosition;
            LocalTransform newLocalTransform = viewLocalTransform.RotateX(mortalBeingAnimator.viewDeadRotation); //Some buggy behaviour with angle value
            localTransformLookup[mortalBeingView.viewEntity] = newLocalTransform;

            mortalBeingAnimator.animationPlayed = true;
        }
    }
}