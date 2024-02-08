using Unity.Entities;

namespace ZombiesShooter
{
    public struct BaseInputHandler : IComponentData, IEnableableComponent
    {
        public BaseInputConfigType configType;

        public bool prevFiringState;
    }
}