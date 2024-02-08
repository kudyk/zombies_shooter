using System;
using Unity.Entities;

namespace ZombiesShooter
{
    [Serializable]
    public struct WeaponViewConfig : IBufferElementData
    {
        public WeaponID weaponID;
        public Entity   entityPrefab;
    }
}