using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

namespace ZombiesShooter
{
    public class RaycastMouseInputHandlerAuthoring : MonoBehaviour
    {
        public PhysicsCategoryTags belongsTo;
        public PhysicsCategoryTags interactsWith;
        public Camera              cameraForInput = null;
    }

    public class RaycastMouseInputHandlerBaker : Baker<RaycastMouseInputHandlerAuthoring>
    {
        public override void Bake(RaycastMouseInputHandlerAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponentObject(currentEntity, new RaycastMouseInputHandler()
            {
                belongsTo      = authoring.belongsTo,
                interactsWith  = authoring.interactsWith,
                cameraForInput = authoring.cameraForInput,
            });
        }
    }
}