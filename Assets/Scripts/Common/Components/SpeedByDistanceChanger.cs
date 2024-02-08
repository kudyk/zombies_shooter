using Unity.Entities;

namespace ZombiesShooter
{
    public struct SpeedByDistanceChanger : ISharedComponentData
    {
        public SpeedByDistanceConfigType configType;
    }
}