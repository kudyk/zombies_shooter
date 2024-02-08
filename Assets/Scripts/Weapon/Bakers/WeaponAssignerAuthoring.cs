using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class WeaponAssignerAuthoring : MonoBehaviour
    {
        public GameObject weaponContainer = null;
        public WeaponID   weaponChangeTo  = WeaponID.NONE;
    }

    public class WeaponAssignerBaker : Baker<WeaponAssignerAuthoring>
    {
        public override void Bake(WeaponAssignerAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new WeaponAssigner()
            {
                weaponContainer = GetEntity(authoring.weaponContainer, TransformUsageFlags.Dynamic),
                weaponChangeTo  = authoring.weaponChangeTo
            });
        }
    }
}