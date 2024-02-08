using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MortalBeingHealthParamsAuthoring : MonoBehaviour
    {
        public int minHealth = 0;
        public int maxHealth = 100;
    }

    public class MortalBeingHealthParamsBaker : Baker<MortalBeingHealthParamsAuthoring>
    {
        public override void Bake(MortalBeingHealthParamsAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddSharedComponent(currentEntity, new MortalBeingHealthParams()
            {
                minHealth = authoring.minHealth,
                maxHealth = authoring.maxHealth
            });
        }
    }
}