using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class CharacterTagAuthoring : MonoBehaviour { }

    public class CharacterTagBaker : Baker<CharacterTagAuthoring>
    {
        public override void Bake(CharacterTagAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(currentEntity, new CharacterTag());
        }
    }
}