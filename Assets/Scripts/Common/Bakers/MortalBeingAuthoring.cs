using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MortalBeingAuthoring : MonoBehaviour
    {
        public int currentHealth = 100;
    }

    public class MortalBeingBaker : Baker<MortalBeingAuthoring>
    {
        public override void Bake(MortalBeingAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new MortalBeing()
            {
                currentHealth     = authoring.currentHealth,
                damageImpactQueue = new FixedList128Bytes<int>()
            });
        }
    }
}