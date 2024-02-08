using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        public StateInPool stateInPool  = StateInPool.READY_FOR_USE;
        public int         damageImpact = 12;
    }

    public class ProjectileBaker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(currentEntity, new Projectile()
            {
                stateInPool         = authoring.stateInPool,
                damageImpact        = authoring.damageImpact,
                timeBeforeAutoreset = double.MinValue
            });
        }
    }
}