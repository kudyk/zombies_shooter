using UnityEngine;

namespace ZombiesShooter
{
    public class MainScreenUI : MonoBehaviour
    {
        [SerializeField] private HUDPanel      hudPanel      = null;
        [SerializeField] private GameOverPanel gameOverPanel = null;

        private void Awake()
        {
            hudPanel.SetVisible(true);
            gameOverPanel.SetVisible(false);
        }

        public void UpdateHUDState(float healthPercent)
        {
            hudPanel.UpdateState(healthPercent);
        }

        public void SetGameOverState()
        {
            gameOverPanel.SetVisible(true);
        }
    }
}