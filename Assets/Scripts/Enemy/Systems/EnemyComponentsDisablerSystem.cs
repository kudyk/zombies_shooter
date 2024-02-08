using Unity.Entities;

namespace ZombiesShooter
{
    public partial class EnemyComponentsDisablerSystem : MortalBeingComponentsDisablerSystem<EnemyTag>
    {
        protected override void DisableComponents(Entity entity)
        {
            EntityManager.SetComponentEnabled<RotatableOnTarget>(entity, false);
            EntityManager.SetComponentEnabled<MovableOnTargetByImpulse>(entity, false);
            EntityManager.SetComponentEnabled<MovableByImpulseForce>(entity, false);
            EntityManager.SetComponentEnabled<MeleeWeapon>(entity, false);
        }
    }
}