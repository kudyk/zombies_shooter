using Unity.Collections;
using Unity.Entities;

namespace ZombiesShooter
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct OuterDependenciesInjectorSystem : ISystem
    {
        private EntityQuery entityQuery;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<OuterDependenciesInjector>();
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<RaycastMouseInputHandler>().Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            OuterDependenciesInjector dependenciesInjector = SystemAPI.ManagedAPI.GetSingleton<OuterDependenciesInjector>();
            if (dependenciesInjector.dependenciesInjected)
                return;

            InjectIntoRaycastMouseInput(dependenciesInjector, ref state);

            dependenciesInjector.dependenciesInjected = true;
        }

        private void InjectIntoRaycastMouseInput(OuterDependenciesInjector dependenciesContainer, ref SystemState systemState)
        {
            foreach (RaycastMouseInputHandler raycastMouseInput in entityQuery.ToComponentArray<RaycastMouseInputHandler>())
                raycastMouseInput.cameraForInput = dependenciesContainer.gameCamera;
        }
    }
}