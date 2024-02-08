using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class BaseInputHandlerAuthoring : MonoBehaviour
    {
        public BaseInputConfigType configType;
    }

    public class BaseInputHandlerBaker : Baker<BaseInputHandlerAuthoring>
    {
        public override void Bake(BaseInputHandlerAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new BaseInputHandler()
            {
                configType = authoring.configType
            });
        }
    }
}