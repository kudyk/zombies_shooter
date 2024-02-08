using Unity.Entities;
using Unity.Mathematics;

namespace ZombiesShooter
{
    public struct MovableByImpulseForce : IComponentData, IEnableableComponent
    {
        public float3 impulseForce;
    }
}