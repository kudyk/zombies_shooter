using UnityEngine;
using UnityEngine.UI;

namespace ZombiesShooter
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private Image progressBar = null;

        public void UpdateProgressbar(float fillAmount)
        {
            progressBar.fillAmount = fillAmount;
        }
    }
}