using System;
using System.Collections;
using UnityEngine;
using Flowbit.MovingGame.Core.Levels;
using Flowbit.Utilities.Core.Events;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Services;
using Flowbit.GameBase.Scenes;
using Flowbit.Utilities.Unity.UI;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Loads the requested moving game level into the current scene and exposes
    /// retry / next / back actions for UI buttons.
    /// </summary>
    public sealed class MovingGameSceneController : SceneBase
    {
        [Header("References")]
        [SerializeField] private MovingGameLevelsLibrary levelsLibrary_;
        [SerializeField] private MovingGameController movingGameController_;

        [Header("Defaults")]
        [SerializeField] private int defaultLevelIndex_ = 0;

        [Header("Completion Popup")]
        [SerializeField] private float completedPopupDelaySeconds_ = 1.5f;

        private EventDispatcher eventDispatcher_;
        private IGameNavigationService navigationService_;
        private int currentLevelIndex_;
        private bool currentLevelCompleted_;
        private bool currentLevelFailed_;
        private Coroutine showCompletedPopupCoroutine_;
        private IDisposable navigationBlock_;
        private ScreenBlocker screenBlocker_;

        protected override bool HasBackButton => true;

        private void RegisterEvents()
        {
            if (movingGameController_ != null && eventDispatcher_ != null)
            {
                eventDispatcher_.Subscribe<LevelCompletedEvent>(movingGameController_, OnLevelCompleted);
                eventDispatcher_.Subscribe<LevelFailedEvent>(movingGameController_, OnLevelFailed);
            }
        }

        private void UnRegisterEvents()
        {
            if (movingGameController_ != null && eventDispatcher_ != null)
            {
                eventDispatcher_.Unsubscribe<LevelCompletedEvent>(movingGameController_, OnLevelCompleted);
                eventDispatcher_.Unsubscribe<LevelFailedEvent>(movingGameController_, OnLevelFailed);
            }

            StopCompletedPopupCoroutine();
        }

        private void OnEnable()
        {
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnRegisterEvents();
        }

        protected override void Initialize()
        {
            var movingGameParams = GetSceneParameters<MovingGameNavigationParams>();
            Initialize(movingGameParams.LevelIndex);
        }

        protected override void DefaultInitialize()
        {
            Initialize(defaultLevelIndex_);
        }

        private void Initialize(int levelIndex)
        {
            var serviceContainer = GlobalServiceContainer.ServiceContainer;
            eventDispatcher_ = serviceContainer.Get<EventDispatcher>();
            navigationService_ = serviceContainer.Get<IGameNavigationService>();
            screenBlocker_ = serviceContainer.Get<ScreenBlocker>();

            RegisterEvents();

            if (levelsLibrary_ == null)
            {
                throw new InvalidOperationException("Levels library is not assigned.");
            }

            if (movingGameController_ == null)
            {
                throw new InvalidOperationException("Moving game controller is not assigned.");
            }

            levelsLibrary_.Load();
            LoadLevel(levelIndex);
        }

        /// <summary>
        /// Loads the level at the given index.
        /// </summary>
        public void LoadLevel(int levelIndex)
        {
            int levelCount = levelsLibrary_.GetLevelCount();
            if (levelIndex < 0 || levelIndex >= levelCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(levelIndex),
                    $"Level index '{levelIndex}' is out of range.");
            }

            StopCompletedPopupCoroutine();

            MovingGameLevelData levelData = levelsLibrary_.GetLevelAt(levelIndex);

            currentLevelCompleted_ = false;
            currentLevelFailed_ = false;
            currentLevelIndex_ = levelIndex;

            movingGameController_.LoadLevel(levelData);
            eventDispatcher_.Send(new LevelLoadedEvent(levelData.id, levelIndex));
        }

        /// <summary>
        /// Reloads the current level from scratch.
        /// </summary>
        public void RetryLevel()
        {
            LoadLevel(currentLevelIndex_);
        }

        /// <summary>
        /// Loads the next level if one exists.
        /// </summary>
        public void LoadNextLevel()
        {
            int nextLevelIndex = currentLevelIndex_ + 1;
            if (nextLevelIndex >= levelsLibrary_.GetLevelCount())
            {
                Debug.Log("There is no next level to load.");
                return;
            }

            LoadLevel(nextLevelIndex);
        }

        /// <summary>
        /// Navigates back to the level selector.
        /// </summary>
        public void BackToLevelSelector()
        {
            NavigationService.Back();
        }

        /// <summary>
        /// Returns the current level index.
        /// </summary>
        public int GetCurrentLevelIndex()
        {
            return currentLevelIndex_;
        }

        /// <summary>
        /// Returns whether the current level has been completed.
        /// </summary>
        public bool IsCurrentLevelCompleted()
        {
            return currentLevelCompleted_;
        }

        /// <summary>
        /// Returns whether the current level has failed.
        /// </summary>
        public bool IsCurrentLevelFailed()
        {
            return currentLevelFailed_;
        }

        private void OnLevelCompleted(LevelCompletedEvent levelCompletedEvent)
        {
            currentLevelCompleted_ = true;
            currentLevelFailed_ = false;

            int nextLevelIndex = currentLevelIndex_ + 1;
            if (nextLevelIndex < levelsLibrary_.GetLevelCount())
            {
                MovingGameLevelData nextLevelData = levelsLibrary_.GetLevelAt(nextLevelIndex);
                eventDispatcher_.Send(new LevelUnlockedEvent(nextLevelData.id, nextLevelIndex));
            }

            StopCompletedPopupCoroutine();
            showCompletedPopupCoroutine_ = StartCoroutine(ShowCompletedPopupRoutine());
        }

        private void OnLevelFailed(LevelFailedEvent levelFailedEvent)
        {
            currentLevelCompleted_ = false;
            currentLevelFailed_ = true;
            StopCompletedPopupCoroutine();
        }

        private IEnumerator ShowCompletedPopupRoutine()
        {

            BlockScreen();
            yield return new WaitForSeconds(completedPopupDelaySeconds_);
            UnblockScreen();

            int nextLevelIndex = currentLevelIndex_ + 1;
            bool hasNextLevel = nextLevelIndex < levelsLibrary_.GetLevelCount();

            string nextLevelTitle = hasNextLevel
                ? BuildLevelTitle(nextLevelIndex, levelsLibrary_.GetLevelAt(nextLevelIndex))
                : "No more levels";

            navigationService_.Navigate(
                SceneType.MovingGameLevelCompletedPopup,
                new MovingGameCompletedPopupParams(
                    nextLevelTitle: nextLevelTitle,
                    hasNextLevel: hasNextLevel,
                    onContinue: hasNextLevel ? LoadNextLevel : BackToLevelSelector,
                    onRetry: RetryLevel,
                    onClose: BackToLevelSelector));

            showCompletedPopupCoroutine_ = null;
        }

        private void UnblockScreen()
        {
            if (navigationBlock_ != null)
            {
                navigationBlock_.Dispose();
                navigationBlock_ = null;
            }
        }

        private void BlockScreen()
        {
            UnblockScreen();
            navigationBlock_ = screenBlocker_.BlockScope();
        }

        private void StopCompletedPopupCoroutine()
        {
            UnblockScreen();

            if (showCompletedPopupCoroutine_ != null)
            {
                StopCoroutine(showCompletedPopupCoroutine_);
                showCompletedPopupCoroutine_ = null;
            }
        }

        private static string BuildLevelTitle(int levelIndex, MovingGameLevelData levelData)
        {
            string levelName = string.IsNullOrWhiteSpace(levelData.name)
                ? $"Level {levelIndex + 1}"
                : levelData.name;

            return $"{levelIndex + 1}. {levelName}";
        }
    }
}
