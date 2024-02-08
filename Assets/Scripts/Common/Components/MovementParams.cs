using Unity.Entities;

namespace ZombiesShooter
{
    public struct MovementParams : IComponentData
    {
        public float maxSpeed;
        public float accelerationSpeed;
    }
}