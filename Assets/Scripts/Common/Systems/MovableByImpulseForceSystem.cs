using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace ZombiesShooter
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct MovableByImpulseForceSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovableByImpulseForce>();
            state.RequireForUpdate<MovementParams>();
            state.RequireForUpdate<PhysicsVelocity>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeArray<float> fixedDeltaTime = new NativeArray<float>(1, Allocator.TempJob);
            fixedDeltaTime[0] = SystemAPI.Time.fixedDeltaTime;

            state.Dependency = new ImpulseMovementJob()
            {
                fixedDeltaTime = fixedDeltaTime
            }.ScheduleParallel(state.Dependency);

            fixedDeltaTime.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct ImpulseMovementJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float> fixedDeltaTime;

        private void Execute(in MovableByImpulseForce movableByImpulse, in MovementParams movementParams, ref PhysicsVelocity physicsVelocity)
        {
            float3 impulse           = movableByImpulse.impulseForce * movementParams.accelerationSpeed * fixedDeltaTime[0];
            float3 unclampedVelocity = physicsVelocity.Linear + impulse;

            physicsVelocity.Linear = Float3Ext.ClampMagnitude(unclampedVelocity, movementParams.maxSpeed);
        }
    }
}