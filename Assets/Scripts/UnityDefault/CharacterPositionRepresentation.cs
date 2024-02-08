using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ZombiesShooter
{
    public class CharacterPositionRepresentation : MonoBehaviour
    {
        [SerializeField] private Transform representationTransform = null;

        private EntityManager entityManager;
        private EntityQuery   characterQuery;

        private void Awake()
        {
            entityManager  = World.DefaultGameObjectInjectionWorld.EntityManager;
            characterQuery = entityManager.CreateEntityQuery(new EntityQueryBuilder(Allocator.Temp).WithAll<CharacterTag, LocalToWorld>());
        }

        private void LateUpdate()
        {
            NativeArray<Entity> queryResult = characterQuery.ToEntityArray(Allocator.Temp);

            if (queryResult.Length == 0)
                return;

            if (queryResult.Length > 1)
            {
                Debug.LogError($"Error! This logic can handle only one {nameof(CharacterTag)} in game!");
                return;
            }

            LocalToWorld targetLocalToWorld = entityManager.GetComponentData<LocalToWorld>(queryResult[0]);
            representationTransform.position = targetLocalToWorld.Position;

            queryResult.Dispose();
        }
    }
}