using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MortalBeingViewAuthoring : MonoBehaviour
    {
        public GameObject viewObj = null;
    }

    public class MortalBeingViewBaker : Baker<MortalBeingViewAuthoring>
    {
        public override void Bake(MortalBeingViewAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.Renderable);

            AddComponent(currentEntity, new MortalBeingView()
            {
                viewEntity = GetEntity(authoring.viewObj, TransformUsageFlags.Dynamic),
            });
        }
    }
}