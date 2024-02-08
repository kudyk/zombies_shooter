using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ZombiesShooter
{
    public class MeleeAutoAttackerAuthoring : MonoBehaviour
    {
        public GameObject targetEntity      = null;
        public float      distanceForAttack = 4.0f;
    }

    public class MeleeAutoAttackerBaker : Baker<MeleeAutoAttackerAuthoring>
    {
        public override void Bake(MeleeAutoAttackerAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new MeleeAutoAttacker()
            {
                targetEntity        = authoring.targetEntity ? GetEntity(authoring.targetEntity, TransformUsageFlags.WorldSpace) : Entity.Null,
                distanceForAttackSQ = math.pow(authoring.distanceForAttack, 2)
            });
        }
    }
}