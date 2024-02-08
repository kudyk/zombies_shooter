using UnityEngine;

namespace ZombiesShooter
{
    public class HUDPanel : PanelBase
    {
        [SerializeField] private UIProgressBar progressBar = null;

        public void UpdateState(float healthPercent)
        {
            progressBar.UpdateProgressbar(healthPercent);
        }
    }
}