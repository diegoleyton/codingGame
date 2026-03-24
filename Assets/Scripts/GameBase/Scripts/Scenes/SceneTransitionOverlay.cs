using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Displays a screenshot of the previous scene on top of everything
    /// while the next scene loads in the background.
    /// </summary>
    public class SceneTransitionOverlay : MonoBehaviour, ISceneTransitionStrategy
    {
        [SerializeField]
        private RawImage rawImage_;

        [SerializeField]
        private GameObject visualsContainer_;

        [SerializeField]
        private RectTransform finalImage_;

        [SerializeField]
        private MaskableContainer[] maskableContainers_;

        [SerializeField]
        private float transitionOutDuration_ = 1f;

        [SerializeField]
        private float throwDuration_ = 0.6f;

        [SerializeField]
        private float throwHeight_ = 40f;

        [SerializeField]
        private float throwRotationDegrees_ = 180f;

        [SerializeField]
        private RectTransform throwTarget_;

        private Texture2D screenshotTexture_;
        private Vector2 finalImageInitialAnchoredPosition_;
        private Quaternion finalImageInitialLocalRotation_;
        private bool finalImageInitialStateCached_;

        private Vector3 backgroundPosition_;

        private void Awake()
        {
            CacheInitialFinalImageState();
            visualsContainer_.gameObject.SetActive(false);
            DisableAllMaskableContainers();
        }

        /// <summary>
        /// Play the transition animation and call the next scene action when it is required.
        /// </summary>
        public void RunTransition(Action goToNextSceneAction)
        {
            StartCoroutine(RunTransitionCoroutine(goToNextSceneAction));
        }

        private IEnumerator RunTransitionCoroutine(Action goToNextSceneAction)
        {
            yield return StartCoroutine(PrepareTransition());

            goToNextSceneAction?.Invoke();

            yield return null;
            yield return null;

            yield return StartCoroutine(PlayTransitionOut());
        }

        private IEnumerator PrepareTransition()
        {
            yield return new WaitForEndOfFrame();

            if (screenshotTexture_ != null)
            {
                Destroy(screenshotTexture_);
                screenshotTexture_ = null;
            }

            screenshotTexture_ = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshotTexture_.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshotTexture_.Apply();

            rawImage_.texture = screenshotTexture_;
            rawImage_.color = Color.white;
            rawImage_.rectTransform.SetParent(visualsContainer_.transform, false);

            CacheInitialFinalImageState();
            ResetFinalImageTransform();

            visualsContainer_.gameObject.SetActive(true);
            DisableAllMaskableContainers();
        }

        private IEnumerator PlayTransitionOut()
        {
            DisableAllMaskableContainers();

            if (maskableContainers_ == null || maskableContainers_.Length == 0)
            {
                FinishTransition();
                yield break;
            }

            float waitPerContainer = transitionOutDuration_ / (maskableContainers_.Length + 1);
            MaskableContainer previousContainer = null;

            for (int i = 0; i < maskableContainers_.Length; i++)
            {
                MaskableContainer currentContainer = maskableContainers_[i];

                if (previousContainer != null)
                {
                    previousContainer.Enable(false);
                }

                currentContainer.Enable(true);
                currentContainer.AddBackground(rawImage_.rectTransform);
                rawImage_.transform.position = backgroundPosition_;

                previousContainer = currentContainer;

                yield return new WaitForSeconds(waitPerContainer);
            }

            if (previousContainer != null)
            {
                previousContainer.Enable(false);
            }

            ResetFinalImageTransform();
            finalImage_?.gameObject.SetActive(true);

            yield return new WaitForSeconds(waitPerContainer);
            yield return StartCoroutine(ThrowToTarget(finalImage_));

            DisableAllMaskableContainers();
            rawImage_.rectTransform.SetParent(visualsContainer_.transform, true);
            rawImage_.transform.position = backgroundPosition_;

            FinishTransition();
        }

        private void DisableAllMaskableContainers()
        {
            if (maskableContainers_ != null)
            {
                for (int i = 0; i < maskableContainers_.Length; i++)
                {
                    if (maskableContainers_[i] != null)
                    {
                        maskableContainers_[i].Enable(false);
                    }
                }
            }

            if (finalImage_ != null)
            {
                finalImage_.gameObject.SetActive(false);
            }
        }

        private void FinishTransition()
        {
            ResetFinalImageTransform();

            rawImage_.texture = null;

            if (screenshotTexture_ != null)
            {
                Destroy(screenshotTexture_);
                screenshotTexture_ = null;
            }

            visualsContainer_.gameObject.SetActive(false);
        }

        /// <summary>
        /// Throws a UI element from its current position to the target using a projectile-like arc.
        /// </summary>
        private IEnumerator ThrowToTarget(RectTransform element)
        {
            if (element == null || throwTarget_ == null)
            {
                yield break;
            }

            RectTransform parentRect = element.parent as RectTransform;
            if (parentRect == null)
            {
                yield break;
            }

            Vector2 startPosition = element.anchoredPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                RectTransformUtility.WorldToScreenPoint(null, throwTarget_.position),
                null,
                out Vector2 endPosition);

            float elapsed = 0f;

            while (elapsed < throwDuration_)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / throwDuration_);

                Vector2 position = Vector2.Lerp(startPosition, endPosition, t);

                float arcOffset = 4f * t * (1f - t) * throwHeight_;
                position.y += arcOffset;

                element.anchoredPosition = position;
                element.localRotation = Quaternion.Euler(
                    0f,
                    0f,
                    Mathf.Lerp(0f, throwRotationDegrees_, t));

                yield return null;
            }

            element.anchoredPosition = endPosition;
            element.localRotation = Quaternion.Euler(0f, 0f, throwRotationDegrees_);

            ResetFinalImageTransform();
        }

        /// <summary>
        /// Caches the initial transform state of the final image so it can be reused on subsequent transitions.
        /// </summary>
        private void CacheInitialFinalImageState()
        {
            if (finalImage_ == null || finalImageInitialStateCached_)
            {
                return;
            }

            finalImageInitialAnchoredPosition_ = finalImage_.anchoredPosition;
            finalImageInitialLocalRotation_ = finalImage_.localRotation;
            finalImageInitialStateCached_ = true;

            backgroundPosition_ = rawImage_.transform.position;
        }

        /// <summary>
        /// Restores the final image to its initial anchored position and rotation.
        /// </summary>
        private void ResetFinalImageTransform()
        {
            if (finalImage_ == null)
            {
                return;
            }

            if (!finalImageInitialStateCached_)
            {
                CacheInitialFinalImageState();
            }

            finalImage_.anchoredPosition = finalImageInitialAnchoredPosition_;
            finalImage_.localRotation = finalImageInitialLocalRotation_;
        }
    }

    [Serializable]
    public class MaskableContainer
    {
        [SerializeField]
        private GameObject mainConatiner_;

        [SerializeField]
        private RectTransform maskConatiner_;

        /// <summary>
        /// Enables or disables the full visual container for this mask step.
        /// </summary>
        public void Enable(bool enabled)
        {
            if (mainConatiner_ != null)
            {
                mainConatiner_.SetActive(enabled);
            }
        }

        /// <summary>
        /// Parents the given background under this mask container.
        /// </summary>
        public void AddBackground(RectTransform background)
        {
            if (background != null && maskConatiner_ != null)
            {
                background.SetParent(maskConatiner_, true);
            }
        }
    }
}