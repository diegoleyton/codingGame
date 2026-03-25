using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Utilities.Navigation;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Displays the previous screen as a static background and animates
    /// the next screen on top of it as if it were a sheet of paper being placed.
    /// </summary>
    public class PaperDropTransitionOverlay : GameSceneTransitionBase
    {
        [SerializeField]
        private GameObject visualsContainer_;

        [SerializeField]
        private RawImage backgroundImage_;

        [SerializeField]
        private RawImage foregroundImage_;

        [SerializeField]
        private float animationDuration_ = 0.6f;

        [SerializeField]
        private Vector2 startAnchoredPosition_ = new Vector2(900f, 700f);

        [SerializeField]
        private float startRotationDegrees_ = -18f;

        [SerializeField]
        private Vector3 startScale_ = new Vector3(0.92f, 0.92f, 1f);

        [SerializeField]
        private bool useUnscaledTime_ = true;

        [SerializeField]
        private int framesToWaitBeforeCapturingForeground_ = 2;

        private Texture2D backgroundTexture_;
        private Texture2D foregroundTexture_;

        private RectTransform backgroundRectTransform_;
        private RectTransform foregroundRectTransform_;

        private Vector2 foregroundInitialAnchoredPosition_;
        private Quaternion foregroundInitialLocalRotation_;
        private Vector3 foregroundInitialLocalScale_;
        private bool foregroundInitialStateCached_;

        private int activeTransitionId_;
        private bool hasPreparedTransition_;

        private void Awake()
        {
            backgroundRectTransform_ = backgroundImage_ != null ? backgroundImage_.rectTransform : null;
            foregroundRectTransform_ = foregroundImage_ != null ? foregroundImage_.rectTransform : null;

            CacheForegroundInitialState();
            HideOverlayImmediately();
        }

        /// <summary>
        /// Captures the currently visible screen and keeps it as the background of the transition.
        /// Any previous transition still in progress is interrupted.
        /// </summary>
        public override IEnumerator PrepareTransition(NavigationTransitionContext _)
        {
            int transitionId = BeginNewTransition();

            yield return new WaitForEndOfFrame();

            if (!IsActiveTransition(transitionId))
            {
                yield break;
            }

            ReplaceTexture(ref backgroundTexture_, CaptureScreenTexture());

            if (!IsActiveTransition(transitionId))
            {
                yield break;
            }

            SetupFullScreenImage(backgroundImage_, backgroundTexture_);
            HideImage(foregroundImage_);
            ResetForegroundTransform();

            visualsContainer_.SetActive(true);
            ShowImage(backgroundImage_);

            hasPreparedTransition_ = true;
        }

        /// <summary>
        /// Captures the newly visible screen, restores the previous screenshot behind it,
        /// and animates the new screenshot into place.
        /// </summary>
        public override IEnumerator FinishTransition(NavigationTransitionContext _)
        {
            int transitionId = activeTransitionId_;

            if (!hasPreparedTransition_ || !IsActiveTransition(transitionId))
            {
                yield break;
            }

            HideImage(foregroundImage_);
            ShowImage(backgroundImage_);

            for (int i = 0; i < framesToWaitBeforeCapturingForeground_; i++)
            {
                yield return null;

                if (!IsActiveTransition(transitionId))
                {
                    HideImage(backgroundImage_);
                    yield break;
                }
            }

            HideImage(backgroundImage_);
            yield return new WaitForEndOfFrame();

            if (!IsActiveTransition(transitionId))
            {
                yield break;
            }

            ReplaceTexture(ref foregroundTexture_, CaptureScreenTexture());

            if (!IsActiveTransition(transitionId))
            {
                yield break;
            }

            SetupFullScreenImage(backgroundImage_, backgroundTexture_);
            SetupFullScreenImage(foregroundImage_, foregroundTexture_);

            ShowImage(backgroundImage_);
            ShowImage(foregroundImage_);

            PlaceForegroundAtStartPose();

            yield return AnimateForegroundIntoPlace(transitionId);

            if (!IsActiveTransition(transitionId))
            {
                yield break;
            }

            CompleteTransition(transitionId);
        }

        /// <summary>
        /// Starts a new transition generation and clears the previous one immediately.
        /// </summary>
        private int BeginNewTransition()
        {
            activeTransitionId_++;
            hasPreparedTransition_ = false;
            AbortCurrentVisualState();
            return activeTransitionId_;
        }

        /// <summary>
        /// Completes the transition if it is still the active one.
        /// </summary>
        private void CompleteTransition(int transitionId)
        {
            if (!IsActiveTransition(transitionId))
            {
                return;
            }

            hasPreparedTransition_ = false;
            HideOverlayImmediately();
            DestroyTextures();
        }

        /// <summary>
        /// Returns whether the provided transition id is still current.
        /// </summary>
        private bool IsActiveTransition(int transitionId)
        {
            return transitionId == activeTransitionId_;
        }

        /// <summary>
        /// Captures the currently rendered frame into a new texture.
        /// </summary>
        private static Texture2D CaptureScreenTexture()
        {
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Replaces a stored texture and destroys the previous one if necessary.
        /// </summary>
        private static void ReplaceTexture(ref Texture2D currentTexture, Texture2D newTexture)
        {
            if (currentTexture != null)
            {
                Destroy(currentTexture);
            }

            currentTexture = newTexture;
        }

        /// <summary>
        /// Assigns the texture to the image and stretches it to fill the screen.
        /// </summary>
        private static void SetupFullScreenImage(RawImage image, Texture texture)
        {
            if (image == null)
            {
                return;
            }

            image.texture = texture;
            image.color = Color.white;

            RectTransform rectTransform = image.rectTransform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }

        /// <summary>
        /// Animates the foreground screenshot from its start pose to the centered final pose.
        /// </summary>
        private IEnumerator AnimateForegroundIntoPlace(int transitionId)
        {
            if (foregroundRectTransform_ == null)
            {
                yield break;
            }

            Vector2 endAnchoredPosition = foregroundInitialAnchoredPosition_;
            Quaternion endRotation = foregroundInitialLocalRotation_;
            Vector3 endScale = foregroundInitialLocalScale_;

            float elapsed = 0f;

            while (elapsed < animationDuration_)
            {
                if (!IsActiveTransition(transitionId))
                {
                    yield break;
                }

                float deltaTime = useUnscaledTime_ ? Time.unscaledDeltaTime : Time.deltaTime;
                elapsed += deltaTime;

                float t = Mathf.Clamp01(elapsed / animationDuration_);
                float easedT = EaseOutCubic(t);

                foregroundRectTransform_.anchoredPosition =
                    Vector2.LerpUnclamped(startAnchoredPosition_, endAnchoredPosition, easedT);

                foregroundRectTransform_.localRotation =
                    Quaternion.LerpUnclamped(
                        Quaternion.Euler(0f, 0f, startRotationDegrees_),
                        endRotation,
                        easedT);

                foregroundRectTransform_.localScale =
                    Vector3.LerpUnclamped(startScale_, endScale, easedT);

                yield return null;
            }

            if (!IsActiveTransition(transitionId))
            {
                yield break;
            }

            ResetForegroundTransform();
        }

        /// <summary>
        /// Places the foreground screenshot at the configured off-screen starting pose.
        /// </summary>
        private void PlaceForegroundAtStartPose()
        {
            if (foregroundRectTransform_ == null)
            {
                return;
            }

            foregroundRectTransform_.anchoredPosition = startAnchoredPosition_;
            foregroundRectTransform_.localRotation = Quaternion.Euler(0f, 0f, startRotationDegrees_);
            foregroundRectTransform_.localScale = startScale_;
        }

        /// <summary>
        /// Caches the original foreground transform so the animation can return to it.
        /// </summary>
        private void CacheForegroundInitialState()
        {
            if (foregroundRectTransform_ == null || foregroundInitialStateCached_)
            {
                return;
            }

            foregroundInitialAnchoredPosition_ = foregroundRectTransform_.anchoredPosition;
            foregroundInitialLocalRotation_ = foregroundRectTransform_.localRotation;
            foregroundInitialLocalScale_ = foregroundRectTransform_.localScale;

            foregroundInitialStateCached_ = true;
        }

        /// <summary>
        /// Restores the foreground image transform to its initial state.
        /// </summary>
        private void ResetForegroundTransform()
        {
            if (foregroundRectTransform_ == null)
            {
                return;
            }

            if (!foregroundInitialStateCached_)
            {
                CacheForegroundInitialState();
            }

            foregroundRectTransform_.anchoredPosition = foregroundInitialAnchoredPosition_;
            foregroundRectTransform_.localRotation = foregroundInitialLocalRotation_;
            foregroundRectTransform_.localScale = foregroundInitialLocalScale_;
        }

        /// <summary>
        /// Interrupts any active visual state immediately.
        /// </summary>
        private void AbortCurrentVisualState()
        {
            HideOverlayImmediately();
            DestroyTextures();
            ResetForegroundTransform();
        }

        /// <summary>
        /// Hides the entire overlay and clears the displayed images.
        /// </summary>
        private void HideOverlayImmediately()
        {
            HideImage(backgroundImage_);
            HideImage(foregroundImage_);

            if (backgroundImage_ != null)
            {
                backgroundImage_.texture = null;
            }

            if (foregroundImage_ != null)
            {
                foregroundImage_.texture = null;
            }

            if (visualsContainer_ != null)
            {
                visualsContainer_.SetActive(false);
            }
        }

        /// <summary>
        /// Destroys any stored textures.
        /// </summary>
        private void DestroyTextures()
        {
            if (backgroundTexture_ != null)
            {
                Destroy(backgroundTexture_);
                backgroundTexture_ = null;
            }

            if (foregroundTexture_ != null)
            {
                Destroy(foregroundTexture_);
                foregroundTexture_ = null;
            }
        }

        /// <summary>
        /// Shows the image game object if it exists.
        /// </summary>
        private static void ShowImage(Graphic image)
        {
            if (image != null)
            {
                image.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the image game object if it exists.
        /// </summary>
        private static void HideImage(Graphic image)
        {
            if (image != null)
            {
                image.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Applies a cubic ease-out interpolation.
        /// </summary>
        private static float EaseOutCubic(float t)
        {
            float inverse = 1f - t;
            return 1f - (inverse * inverse * inverse);
        }
    }
}