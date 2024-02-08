using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class ECSDependenciesInjector : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera = null;

        private EntityManager entityManager;
        private EntityQuery   injectorQuery;

        private void Awake()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            injectorQuery = entityManager.CreateEntityQuery(new EntityQueryBuilder(Allocator.Temp).WithAll<OuterDependenciesInjector>());
        }

        private void Update()
        {
            int resultCount = injectorQuery.CalculateEntityCount();

            if (resultCount == 0)
                return;

            if (resultCount > 1)
            {
                Debug.LogError($"Error! Only one {nameof(OuterDependenciesInjector)} must be in ECS World!");
                return;
            }

            OuterDependenciesInjector dependenciesInjector = injectorQuery.GetSingleton<OuterDependenciesInjector>();
            dependenciesInjector.gameCamera = gameCamera;

            SetSelfEnabled(false);
        }

        private void SetSelfEnabled(bool isEnabled) => enabled = isEnabled;
    }
}