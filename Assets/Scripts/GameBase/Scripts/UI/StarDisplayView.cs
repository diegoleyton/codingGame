using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Services;
using Flowbit.Utilities.Core.Events;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Displays a row of stars and can animate their reveal.
    /// </summary>
    public sealed class StarDisplayView : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage_;
        [SerializeField] private Image fillImage_;
        [SerializeField] private float revealInitialDelaySeconds_;
        [SerializeField] private float revealDurationPerStarSeconds_;
        [SerializeField] private AnimationCurve revealFillCurve_;

        public void SetStars(int starCount, int maxStars)
        {
            float fillAmount = CalculateFillAmount(starCount, maxStars);

            if (backgroundImage_ != null)
            {
                backgroundImage_.gameObject.SetActive(maxStars > 0);
            }

            if (fillImage_ != null)
            {
                fillImage_.gameObject.SetActive(maxStars > 0);
                fillImage_.fillAmount = fillAmount;
            }
        }

        public IEnumerator PlayRevealAnimation(int starCount, int maxStars, bool dispatchEvents = true)
        {
            if (backgroundImage_ != null)
            {
                backgroundImage_.gameObject.SetActive(maxStars > 0);
            }

            if (fillImage_ == null)
            {
                yield break;
            }

            float targetFillAmount = CalculateFillAmount(starCount, maxStars);
            fillImage_.gameObject.SetActive(maxStars > 0);
            fillImage_.fillAmount = 0f;

            int clampedStarCount = maxStars <= 0
                ? 0
                : Mathf.Clamp(starCount, 0, maxStars);

            EventDispatcher eventDispatcher = dispatchEvents
                ? GlobalServiceContainer.ServiceContainer.Get<EventDispatcher>()
                : null;

            if (revealInitialDelaySeconds_ > 0f)
            {
                yield return new WaitForSecondsRealtime(revealInitialDelaySeconds_);
            }

            eventDispatcher?.Send(new OnStarFillStarted());

            float duration = clampedStarCount * revealDurationPerStarSeconds_;
            if (duration <= 0f)
            {
                fillImage_.fillAmount = targetFillAmount;
                eventDispatcher?.Send(new OnStarFillCompleted());
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float curveValue = revealFillCurve_ != null ? revealFillCurve_.Evaluate(t) : t;
                fillImage_.fillAmount = Mathf.LerpUnclamped(0f, targetFillAmount, curveValue);
                yield return null;
            }

            fillImage_.fillAmount = targetFillAmount;
            eventDispatcher?.Send(new OnStarFillCompleted());
        }

        private static float CalculateFillAmount(int starCount, int maxStars)
        {
            if (maxStars <= 0)
            {
                return 0f;
            }

            int clampedStarCount = Mathf.Clamp(starCount, 0, maxStars);
            return (float)clampedStarCount / maxStars;
        }
    }
}
