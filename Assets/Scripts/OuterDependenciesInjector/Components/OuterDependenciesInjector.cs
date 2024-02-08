using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class OuterDependenciesInjector : IComponentData
    {
        public Camera gameCamera;

        public bool dependenciesInjected;
    }
}