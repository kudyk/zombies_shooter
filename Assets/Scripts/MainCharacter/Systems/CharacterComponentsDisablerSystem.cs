using Unity.Entities;

namespace ZombiesShooter
{
    public partial class CharacterComponentsDisablerSystem : MortalBeingComponentsDisablerSystem<CharacterTag>
    {
        protected override void DisableComponents(Entity entity)
        {
            EntityManager.SetComponentEnabled<BaseInputHandler>(entity, false);
            EntityManager.SetComponentEnabled<WeaponChangeInputHandler>(entity, false);
            EntityManager.SetComponentEnabled<MovableByImpulseForce>(entity, false);
            EntityManager.SetComponentEnabled<RotatableOnTarget>(entity, false);
            EntityManager.SetComponentEnabled<RangedWeapon>(entity, false);
        }
    }
}