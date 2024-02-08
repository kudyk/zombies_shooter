using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ZombiesShooter
{
    public class EnemyTargetsFinderAuthoring : MonoBehaviour
    {
        public float minDistanceForLink            = 20;
        public float minDistanceDifferenceForReset = 5;
    }

    public class EnemyTargetsFinderBaker : Baker<EnemyTargetsFinderAuthoring>
    {
        public override void Bake(EnemyTargetsFinderAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new EnemyTargetsFinder()
            {
                minDistanceForLinkSQ            = math.pow(authoring.minDistanceForLink, 2),
                minDistanceDifferenceForResetSQ = math.pow(authoring.minDistanceDifferenceForReset, 2)
            });
        }
    }
}