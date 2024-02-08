using Unity.Entities;
using Unity.Transforms;

namespace ZombiesShooter
{
    public struct InstantiationHelper
    {
        public static Entity InstantiateInContainer(Entity entityPrefab, Entity entityContainer, ref SystemState state)
        {
            Entity viewInstance = state.EntityManager.Instantiate(entityPrefab);

            state.EntityManager.AddComponentData(viewInstance, new Parent { Value = entityContainer });
            state.EntityManager.AddComponentData(viewInstance, LocalTransform.Identity);

            return viewInstance;
        }
    }
}