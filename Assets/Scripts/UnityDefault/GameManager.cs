using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MainScreenUI mainScreenUI = null;
        [SerializeField] private GameState    currentState = GameState.ACTIVE;

        private EntityManager entityManager;
        private EntityQuery   characterQuery;

        private void Awake()
        {
            entityManager  = World.DefaultGameObjectInjectionWorld.EntityManager;
            characterQuery = entityManager.CreateEntityQuery(new EntityQueryBuilder(Allocator.Temp).WithAll<CharacterTag, MortalBeing>());
        }

        private void Update()
        {
            if (currentState == GameState.GAME_OVER)
                return;

            NativeArray<Entity> queryResult = characterQuery.ToEntityArray(Allocator.Temp);

            if (queryResult.Length == 0)
                return;

            if (queryResult.Length > 1)
            {
                Debug.LogError($"Error! This logic can handle only one {nameof(CharacterTag)} in game!");
                return;
            }

            MortalBeing             mortalBeing  = entityManager.GetComponentData<MortalBeing>(queryResult[0]);
            MortalBeingHealthParams healthParams = entityManager.GetSharedComponent<MortalBeingHealthParams>(queryResult[0]);

            float healthPercent = Mathf.InverseLerp(healthParams.minHealth, healthParams.maxHealth, mortalBeing.currentHealth);
            mainScreenUI.UpdateHUDState(healthPercent);

            if (mortalBeing.currentHealth > 0)
                return;

            SetGameOverState();
            mainScreenUI.SetGameOverState();
        }

        private void SetGameOverState()
        {
            SetGameOnPause(true);
            currentState = GameState.GAME_OVER;
        }

        private void SetGameOnPause(bool isPaused)
        {
            Time.timeScale = isPaused ? 0.0f : 1.0f;
        }
    }
}