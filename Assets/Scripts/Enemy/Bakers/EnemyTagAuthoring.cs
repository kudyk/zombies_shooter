using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class EnemyTagAuthoring : MonoBehaviour { }

    public class EnemyTagBaker : Baker<EnemyTagAuthoring>
    {
        public override void Bake(EnemyTagAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new EnemyTag());
        }
    }
}