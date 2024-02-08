using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class WeaponViewAuthoring : MonoBehaviour
    {
        public GameObject impactPoint;
    }

    public class WeaponViewBaker : Baker<WeaponViewAuthoring>
    {
        public override void Bake(WeaponViewAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.Renderable);

            AddComponent(currentEntity, new WeaponView()
            {
                impactPoint = GetEntity(authoring.impactPoint, TransformUsageFlags.Dynamic)
            });
        }
    }
}