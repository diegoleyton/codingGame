using System;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Utilities.Navigation;
using Flowbit.GameBase.Character;
using Flowbit.GameBase.Scenes;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Popup shown after completing a level.
    /// </summary>
    public sealed class MovingGameCompletedPopupView : PopupBase
    {
        [Header("UI")]
        [SerializeField] private Text titleText_;
        [SerializeField] private Button continueButton_;
        [SerializeField] private Button retryButton_;
        [SerializeField] private Button closeButton_;

        [SerializeField] private CharacterAnimation characterAnimation_;

        private Action onContinueAction_;
        private Action onRetryAction_;
        private Action onCloseAction_;

        /// <summary>
        /// Initializes the popup with navigation parameters.
        /// </summary>
        public override void Initialize(NavigationParams navigationParams)
        {
            if (navigationParams is not MovingGameCompletedPopupParams popupParams)
            {
                throw new ArgumentException(
                    $"Expected {nameof(MovingGameCompletedPopupParams)} but got {navigationParams?.GetType().Name ?? "null"}.");
            }

            onContinueAction_ = popupParams.OnContinue;
            onRetryAction_ = popupParams.OnRetry;
            onCloseAction_ = popupParams.OnClose;

            if (titleText_ != null)
            {
                titleText_.text = popupParams.HasNextLevel
                    ? popupParams.NextLevelTitle
                    : "No more levels";
            }

            if (continueButton_ != null)
            {
                continueButton_.interactable = popupParams.HasNextLevel;
                continueButton_.onClick.RemoveAllListeners();
                continueButton_.onClick.AddListener(OnContinuePressed);
            }

            if (retryButton_ != null)
            {
                retryButton_.onClick.RemoveAllListeners();
                retryButton_.onClick.AddListener(OnRetryPressed);
            }

            if (closeButton_ != null)
            {
                closeButton_.onClick.RemoveAllListeners();
                closeButton_.onClick.AddListener(OnClosePressed);
            }

            if (characterAnimation_ != null)
            {
                characterAnimation_.SetState(CharacterAnimationStateType.Celebrate);
            }
        }

        private void OnContinuePressed()
        {
            Close();
            onContinueAction_?.Invoke();
        }

        private void OnRetryPressed()
        {
            Close();
            onRetryAction_?.Invoke();
        }

        private void OnClosePressed()
        {
            Close();
            onCloseAction_?.Invoke();
        }
    }
}