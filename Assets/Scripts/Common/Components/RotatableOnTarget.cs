using Unity.Entities;
using Unity.Mathematics;

namespace ZombiesShooter
{
    public struct RotatableOnTarget : IComponentData, IEnableableComponent
    {
        public Entity targetEntity;
        public Entity rotatingEntity;
        public float3 upDirection;
    }
}