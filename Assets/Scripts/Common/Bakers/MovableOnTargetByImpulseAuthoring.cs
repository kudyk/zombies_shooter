using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MovableOnTargetByImpulseAuthoring : MonoBehaviour
    {
        public GameObject targetObj = null;
    }

    public class MovableOnTargetByImpulseBaker : Baker<MovableOnTargetByImpulseAuthoring>
    {
        public override void Bake(MovableOnTargetByImpulseAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(currentEntity, new MovableOnTargetByImpulse
            {
                targetEntity = authoring.targetObj ? GetEntity(authoring.targetObj, TransformUsageFlags.WorldSpace) : Entity.Null,
            });
        }
    }
}