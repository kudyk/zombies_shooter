using Unity.Entities;

namespace ZombiesShooter
{
    public struct ItemsByCircleSpawner : IBufferElementData, IEnableableComponent
    {
        public Entity itemPrefab;
        public int    spawnCount;
        public float  positionY;
        public float  startAngle;
        public float  startRadius;
        public float  angleStep;
        public float  radiusStep;
    }
}