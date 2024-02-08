using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MortalBeingDisablerAuthoring : MonoBehaviour
    {
        public bool physicsDisabled    = false;
        public bool componentsDisabled = false;
    }

    public class MortalBeingDisablerBaker : Baker<MortalBeingDisablerAuthoring>
    {
        public override void Bake(MortalBeingDisablerAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(currentEntity, new MortalBeingDisabler()
            {
                physicsDisabled    = authoring.physicsDisabled,
                componentsDisabled = authoring.componentsDisabled,
            });
        }
    }
}