using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MovableByImpulseForceAuthoring : MonoBehaviour
    {
        public Vector3 impulseForce = Vector3.zero;
    }

    public class MovableByImpulseForceBaker : Baker<MovableByImpulseForceAuthoring>
    {
        public override void Bake(MovableByImpulseForceAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(currentEntity, new MovableByImpulseForce()
            {
                impulseForce = authoring.impulseForce
            });
        }
    }
}