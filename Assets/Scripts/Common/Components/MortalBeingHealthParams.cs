using Unity.Entities;

namespace ZombiesShooter
{
    public struct MortalBeingHealthParams : ISharedComponentData
    {
        public int minHealth;
        public int maxHealth;
    }
}