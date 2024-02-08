using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class OuterDependenciesInjectorAuthoring : MonoBehaviour
    {
        public bool dependenciesInjected = false;
    }

    public class OuterDependenciesInjectorBaker : Baker<OuterDependenciesInjectorAuthoring>
    {
        public override void Bake(OuterDependenciesInjectorAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(currentEntity, new OuterDependenciesInjector()
            {
                dependenciesInjected = authoring.dependenciesInjected
            });
        }
    }
}