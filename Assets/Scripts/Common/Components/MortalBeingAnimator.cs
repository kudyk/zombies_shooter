using Unity.Entities;
using Unity.Mathematics;

namespace ZombiesShooter
{
    public struct MortalBeingAnimator : IComponentData
    {
        public float3 viewDeadPosition;
        public float  viewDeadRotation;
        public bool   animationPlayed;
    }
}