using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class RangedWeaponAuthoring : MonoBehaviour
    {
        public GameObject         weaponView   = null;
        public RangedWeaponConfig weaponConfig = default;
    }

    public class RangedWeaponBaker : Baker<RangedWeaponAuthoring>
    {
        public override void Bake(RangedWeaponAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new RangedWeapon()
            {
                weaponView   = authoring.weaponView ? GetEntity(authoring.weaponView, TransformUsageFlags.Renderable) : Entity.Null,
                weaponConfig = authoring.weaponConfig,
            });
        }
    }
}