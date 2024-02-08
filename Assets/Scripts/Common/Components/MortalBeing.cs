using Unity.Collections;
using Unity.Entities;

namespace ZombiesShooter
{
    public struct MortalBeing : IComponentData
    {
        public int                    currentHealth;
        public FixedList128Bytes<int> damageImpactQueue;
    }
}