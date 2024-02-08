using UnityEngine;

namespace ZombiesShooter
{
    public abstract class PanelBase : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup = null;

        public void SetVisible(bool isVisible)
        {
            canvasGroup.alpha          = isVisible ? 1.0f : 0.0f;
            canvasGroup.interactable   = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}