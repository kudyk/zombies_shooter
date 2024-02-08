using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class WeaponChangeInputHandlerAuthoring : MonoBehaviour
    {
        public WeaponChangeInputConfigType configType;
    }

    public class WeaponChangeInputHandlerBaker : Baker<WeaponChangeInputHandlerAuthoring>
    {
        public override void Bake(WeaponChangeInputHandlerAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new WeaponChangeInputHandler()
            {
                configType = authoring.configType
            });
        }
    }
}