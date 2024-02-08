using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ZombiesShooter
{
    public partial struct RotatableOnTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RotatableOnTarget>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<LocalToWorld>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ComponentLookup<LocalTransform> localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
            ComponentLookup<LocalToWorld>   localToWorldLookup   = SystemAPI.GetComponentLookup<LocalToWorld>();

            state.Dependency = new RotatableJob()
            {
                localTransformLookup = localTransformLookup,
                localToWorldLookup   = localToWorldLookup
            }.Schedule(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct RotatableJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalToWorld>   localToWorldLookup;
        public ComponentLookup<LocalTransform> localTransformLookup;

        private void Execute(in RotatableOnTarget rotatable)
        {
            if (rotatable.targetEntity == Entity.Null)
                return;

            LocalTransform rotatableLocalTransform = localTransformLookup[rotatable.rotatingEntity];
            LocalToWorld   rotatableLocalToWorld   = localToWorldLookup[rotatable.rotatingEntity];
            LocalToWorld   targetLocalToWorld      = localToWorldLookup[rotatable.targetEntity];

            float3         relativeWorldPosition = Float3Ext.StraightRelativePositionTo(rotatableLocalToWorld.Position, targetLocalToWorld.Position);
            LocalTransform updatedState          = rotatableLocalTransform.WithRotation(quaternion.LookRotationSafe(relativeWorldPosition, rotatable.upDirection));

            localTransformLookup[rotatable.rotatingEntity] = updatedState;
        }
    }
}