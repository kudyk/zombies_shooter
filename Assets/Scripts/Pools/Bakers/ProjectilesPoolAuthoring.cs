using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class ProjectilesPoolAuthoring : MonoBehaviour
    {
        public GameObject projectilePrefab = null;
        public Vector3    defaultPosition  = Vector3.zero;
        public int        batchSize        = 3;
    }

    public class ProjectilesPoolBaker : Baker<ProjectilesPoolAuthoring>
    {
        public override void Bake(ProjectilesPoolAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new ProjectilesPool
            {
                projectilePrefab = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                defaultPosition  = authoring.defaultPosition,
                batchSize        = authoring.batchSize,
            });
        }
    }
}