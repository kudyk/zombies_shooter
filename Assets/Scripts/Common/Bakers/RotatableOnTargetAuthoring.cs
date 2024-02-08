using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class RotatableOnTargetAuthoring : MonoBehaviour
    {
        public GameObject targetObj   = null;
        public GameObject rotatingObj = null;
        public Vector3    upDirection = Vector3.up;
    }

    public class RotatableOnTargetBaker : Baker<RotatableOnTargetAuthoring>
    {
        public override void Bake(RotatableOnTargetAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new RotatableOnTarget()
            {
                targetEntity   = authoring.targetObj ? GetEntity(authoring.targetObj, TransformUsageFlags.WorldSpace) : Entity.Null,
                rotatingEntity = GetEntity(authoring.rotatingObj, TransformUsageFlags.Dynamic),
                upDirection    = authoring.upDirection,
            });
        }
    }
}