using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ComingSoonPopup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text messageText;
        [SerializeField] private float visibleDuration = 1.4f;
        [SerializeField] private float fadeDuration = 0.2f;

        private Coroutine activeRoutine;

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        public void Show()
        {
            Show("Sắp ra mắt");
        }

        public void Show(string message)
        {
            if (messageText != null) messageText.text = message;

            if (activeRoutine != null) StopCoroutine(activeRoutine);
            activeRoutine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            yield return Fade(1f, fadeDuration);
            yield return new WaitForSecondsRealtime(visibleDuration);
            yield return Fade(0f, fadeDuration);
            activeRoutine = null;
        }

        private IEnumerator Fade(float target, float duration)
        {
            float start = canvasGroup.alpha;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, target, t / duration);
                yield return null;
            }
            canvasGroup.alpha = target;
        }
    }
}
