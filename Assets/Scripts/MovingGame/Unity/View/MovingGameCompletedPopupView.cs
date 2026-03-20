using System;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Utilities.Core.Navigation;
using Flowbit.Utilities.Unity.Navigation;
using Flowbit.GameBase.Character;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Popup shown after completing a level.
    /// </summary>
    public sealed class MovingGameCompletedPopupView : MonoBehaviour, IScreen
    {
        [Header("UI")]
        [SerializeField] private Text titleText_;
        [SerializeField] private Button continueButton_;
        [SerializeField] private Button retryButton_;

        [SerializeField] private CharacterAnimation characterAnimation_;

        private Action continueAction_;
        private Action retryAction_;

        /// <summary>
        /// Initializes the popup with navigation parameters.
        /// </summary>
        public void Initialize(NavigationParams navigationParams)
        {
            if (navigationParams is not MovingGameCompletedPopupParams popupParams)
            {
                throw new ArgumentException(
                    $"Expected {nameof(MovingGameCompletedPopupParams)} but got {navigationParams?.GetType().Name ?? "null"}.");
            }

            continueAction_ = popupParams.OnContinue;
            retryAction_ = popupParams.OnRetry;

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

            if (characterAnimation_ != null)
            {
                characterAnimation_.SetState(CharacterAnimationStateType.Celebrate);
            }
        }

        private void OnContinuePressed()
        {
            continueAction_?.Invoke();
            Close();
        }

        private void OnRetryPressed()
        {
            retryAction_?.Invoke();
            Close();
        }

        private void Close()
        {
            Destroy(gameObject);
        }
    }
}