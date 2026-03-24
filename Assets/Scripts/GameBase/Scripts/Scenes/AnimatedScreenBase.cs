using Flowbit.Utilities.Unity.Navigation;
using Flowbit.Utilities.Core.Navigation;
using System.Collections;
using UnityEngine;
using System;

namespace Flowbit.GameBase.Scenes
{
    public abstract class AnimatedScreenBase : MonoBehaviour, IAnimatedScreen
    {
        [SerializeField]
        private RectTransform container_;

        [SerializeField]
        private float enterDuration_ = 0.25f;

        [SerializeField]
        private float exitDuration_ = 0.2f;

        [SerializeField]
        private float enterOffsetY_ = 1200f;

        private Vector2 initialAnchoredPosition_;

        private Action<IScreen> navigatorCloseAction_;

        protected virtual void Awake()
        {
            if (container_ == null)
            {
                container_ = transform as RectTransform;
            }

            initialAnchoredPosition_ = container_.anchoredPosition;
        }

        public abstract void Initialize(NavigationParams navigationParams);

        public virtual IEnumerator PlayEnter()
        {
            Vector2 end = initialAnchoredPosition_;
            Vector2 start = end + Vector2.up * enterOffsetY_;

            container_.anchoredPosition = start;

            float elapsed = 0f;
            while (elapsed < enterDuration_)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / enterDuration_);
                t = EaseOutCubic(t);
                container_.anchoredPosition = Vector2.LerpUnclamped(start, end, t);
                yield return null;
            }

            container_.anchoredPosition = end;
        }

        public virtual IEnumerator PlayExit()
        {
            Vector2 start = container_.anchoredPosition;
            Vector2 end = initialAnchoredPosition_ + Vector2.up * enterOffsetY_;

            float elapsed = 0f;
            while (elapsed < exitDuration_)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / exitDuration_);
                t = EaseInCubic(t);
                container_.anchoredPosition = Vector2.LerpUnclamped(start, end, t);
                yield return null;
            }

            container_.anchoredPosition = end;
        }

        public void SetCloseAction(Action<IScreen> closeAction)
        {
            navigatorCloseAction_ = closeAction;
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        protected void RequestClose()
        {
            navigatorCloseAction_?.Invoke(this);
        }

        private float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        private float EaseInCubic(float t)
        {
            return t * t * t;
        }
    }
}