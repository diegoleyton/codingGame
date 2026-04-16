using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.GameBase.Character;
using Flowbit.GameBase.Scenes;
using Flowbit.GameBase.Services;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.UI;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Unity.UI;

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
        [SerializeField] private Text elapsedTimeText_;
        [SerializeField] private StarDisplayView starsView_;

        [SerializeField] private CharacterAnimation characterAnimation_;
        [SerializeField] private UIComponentAnimatorController UIAnimationController_;

        private Action onContinueAction_;
        private Action onRetryAction_;
        private Action onCloseAction_;

        /// <summary>
        /// Initializes the popup with navigation parameters.
        /// </summary>
        protected override void Initialize()
        {
            var popupParams = GetSceneParameters<MovingGameCompletedPopupParams>();
            onContinueAction_ = popupParams.OnContinue;
            onRetryAction_ = popupParams.OnRetry;
            onCloseAction_ = popupParams.OnClose;

            GlobalServiceContainer.ServiceContainer.Get<EventDispatcher>().Send(
                        this,
                        new OnLevelCompletedPopupEvent());

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

            if (elapsedTimeText_ != null)
            {
                elapsedTimeText_.text = popupParams.RankingResult.SummaryText;
            }

            if (characterAnimation_ != null)
            {
                characterAnimation_.SetState(CharacterAnimationStateType.Look);
            }

            if (starsView_ != null)
            {
                starsView_.gameObject.SetActive(true);
                starsView_.StopAllCoroutines();
                starsView_.SetStars(0, popupParams.RankingResult.MaxStars);
                StartCoroutine(PlayStarsSequence(
                    popupParams.RankingResult.StarCount,
                    popupParams.RankingResult.MaxStars));
            }
        }

        private IEnumerator PlayStarsSequence(int starCount, int maxStars)
        {
            yield return StartCoroutine(starsView_.PlayRevealAnimation(starCount, maxStars));
            UIAnimationController_.GoToFinalState();

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
            onCloseAction_?.Invoke();
        }

    }
}
