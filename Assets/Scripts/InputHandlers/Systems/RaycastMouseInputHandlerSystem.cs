using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Ray = UnityEngine.Ray;

namespace ZombiesShooter
{
    // TODO: Don't convert to IJobEntity, remove comment before release!
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct RaycastMouseInputHandlerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<RaycastMouseInputHandler>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var (raycastInput, localTransform) in SystemAPI.Query<RaycastMouseInputHandler, RefRW<LocalTransform>>())
            {
                if (raycastInput.cameraForInput == null)
                    continue;

                Ray inputRay = raycastInput.cameraForInput.ScreenPointToRay(Input.mousePosition);

                CollisionFilter collisionFilter = new CollisionFilter()
                {
                    BelongsTo    = raycastInput.belongsTo.Value,
                    CollidesWith = raycastInput.interactsWith.Value,
                    GroupIndex   = 0
                };

                RaycastInput input = new RaycastInput()
                {
                    Start  = inputRay.origin,
                    End    = inputRay.GetPoint(raycastInput.cameraForInput.farClipPlane),
                    Filter = collisionFilter
                };

                if (physicsWorld.CastRay(input, out var hit))
                {
                    localTransform.ValueRW.Position = hit.Position;
                }
            }
        }
    }
}