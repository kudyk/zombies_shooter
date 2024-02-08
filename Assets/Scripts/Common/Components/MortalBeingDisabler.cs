using Unity.Entities;

namespace ZombiesShooter
{
    public struct MortalBeingDisabler : IComponentData
    {
        public bool physicsDisabled;
        public bool componentsDisabled;
    }
}