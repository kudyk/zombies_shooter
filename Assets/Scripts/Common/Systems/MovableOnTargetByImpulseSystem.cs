using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ZombiesShooter
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(MovableByImpulseForceSystem))]
    public partial struct MovableOnTargetByImpulseSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovableOnTargetByImpulse>();
            state.RequireForUpdate<MovableByImpulseForce>();
            state.RequireForUpdate<LocalToWorld>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ComponentLookup<LocalToWorld> localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();

            state.Dependency = new MovableOnTargetByImpulseJob()
            {
                localToWorldLookup = localToWorldLookup
            }.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct MovableOnTargetByImpulseJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalToWorld> localToWorldLookup;

        private void Execute(in MovableOnTargetByImpulse movableOnTarget, ref MovableByImpulseForce movableByImpulse, in LocalToWorld localToWorld)
        {
            if (movableOnTarget.targetEntity == Entity.Null)
            {
                if (!movableByImpulse.impulseForce.Equals(float3.zero))
                    movableByImpulse.impulseForce = float3.zero;

                return;
            }

            LocalToWorld targetLocalToWorld = localToWorldLookup[movableOnTarget.targetEntity];
            movableByImpulse.impulseForce = math.normalize(Float3Ext.StraightRelativePositionTo(localToWorld.Position, targetLocalToWorld.Position));
        }
    }
}