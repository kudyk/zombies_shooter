using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class SpeedByDistanceChangerAuthoring : MonoBehaviour
    {
        public SpeedByDistanceConfigType configType;
    }

    public class SpeedByDistanceChangerBaker : Baker<SpeedByDistanceChangerAuthoring>
    {
        public override void Bake(SpeedByDistanceChangerAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.WorldSpace);

            AddSharedComponent(currentEntity, new SpeedByDistanceChanger()
            {
                configType = authoring.configType
            });
        }
    }
}