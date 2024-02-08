using Unity.Entities;

namespace ZombiesShooter
{
    public struct MovableOnTargetByImpulse : IComponentData, IEnableableComponent
    {
        public Entity targetEntity;
    }
}