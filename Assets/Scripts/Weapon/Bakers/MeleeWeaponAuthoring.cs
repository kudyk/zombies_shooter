using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MeleeWeaponAuthoring : MonoBehaviour
    {
        public GameObject        weaponView;
        public MeleeWeaponConfig weaponConfig;
    }

    public class MeleeWeaponBaker : Baker<MeleeWeaponAuthoring>
    {
        public override void Bake(MeleeWeaponAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new MeleeWeapon()
            {
                weaponView   = authoring.weaponView ? GetEntity(authoring.weaponView, TransformUsageFlags.Dynamic) : Entity.Null,
                weaponConfig = authoring.weaponConfig,
            });
        }
    }
}