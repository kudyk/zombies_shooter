using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MovementParamsAuthoring : MonoBehaviour
    {
        public float maxSpeed          = 20.0f;
        public float accelerationSpeed = 200.0f;
    }

    public class MovementParamsBaker : Baker<MovementParamsAuthoring>
    {
        public override void Bake(MovementParamsAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new MovementParams()
            {
                maxSpeed          = authoring.maxSpeed,
                accelerationSpeed = authoring.accelerationSpeed
            });
        }
    }
}