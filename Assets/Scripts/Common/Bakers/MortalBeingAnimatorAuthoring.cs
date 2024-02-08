using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MortalBeingAnimatorAuthoring : MonoBehaviour
    {
        public Vector3 viewDeadPosition = Vector3.zero;
        public float   viewDeadRotation = -1.55f;
        public bool    animationPlayed  = false;
    }

    public class MortalBeingAnimatorBaker : Baker<MortalBeingAnimatorAuthoring>
    {
        public override void Bake(MortalBeingAnimatorAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(currentEntity, new MortalBeingAnimator()
            {
                viewDeadPosition = authoring.viewDeadPosition,
                viewDeadRotation = authoring.viewDeadRotation,
                animationPlayed  = authoring.animationPlayed,
            });
        }
    }
}