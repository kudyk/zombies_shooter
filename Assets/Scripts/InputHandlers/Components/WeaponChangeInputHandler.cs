using Unity.Entities;

namespace ZombiesShooter
{
    public struct WeaponChangeInputHandler : IComponentData, IEnableableComponent
    {
        public WeaponChangeInputConfigType configType;
    }
}