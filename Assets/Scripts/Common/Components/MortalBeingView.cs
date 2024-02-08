using Unity.Entities;

namespace ZombiesShooter
{
    public struct MortalBeingView : IComponentData
    {
        public Entity viewEntity;
    }
}