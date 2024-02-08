using Unity.Entities;
using Unity.Mathematics;

namespace ZombiesShooter
{
    public struct ProjectilesPool : IComponentData
    {
        public Entity projectilePrefab;
        public float3 defaultPosition;
        public int    batchSize;
    }
}