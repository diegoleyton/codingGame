using System;
using UnityEngine;
using Flowbit.MovingGame.Core.Levels;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Unity.Navigation;
using Flowbit.GameBase.Definitions;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Loads the requested moving game level into the current scene and exposes
    /// retry / next / back actions for UI buttons.
    /// </summary>
    public sealed class MovingGameSceneController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MovingGameLevelsLibrary levelsLibrary_;
        [SerializeField] private MovingGameController movingGameController_;
        [SerializeField] private NavigationDestinationAsset levelSelectorDestination_;

        [Header("Defaults")]
        [SerializeField] private int defaultLevelIndex_ = 0;

        private readonly EventDispatcher eventDispatcher_ =
            GlobalEventDispatcher.EventDispatcher;

        private int currentLevelIndex_;
        private bool currentLevelCompleted_;
        private bool currentLevelFailed_;

        private void OnEnable()
        {
            if (movingGameController_ != null)
            {
                eventDispatcher_.Subscribe<LevelCompletedEvent>(
                    movingGameController_,
                    OnLevelCompleted);

                eventDispatcher_.Subscribe<LevelFailedEvent>(
                    movingGameController_,
                    OnLevelFailed);
            }
        }

        private void OnDisable()
        {
            if (movingGameController_ != null)
            {
                eventDispatcher_.Unsubscribe<LevelCompletedEvent>(
                    movingGameController_,
                    OnLevelCompleted);

                eventDispatcher_.Unsubscribe<LevelFailedEvent>(
                    movingGameController_,
                    OnLevelFailed);
            }
        }

        /// <summary>
        /// Loads the initial level for this scene.
        /// </summary>
        private void Start()
        {
            if (levelsLibrary_ == null)
            {
                throw new InvalidOperationException("Levels library is not assigned.");
            }

            if (movingGameController_ == null)
            {
                throw new InvalidOperationException("Moving game controller is not assigned.");
            }

            levelsLibrary_.Load();

            int levelIndex = ResolveInitialLevelIndex();
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

            MovingGameLevelData levelData = levelsLibrary_.GetLevelAt(levelIndex);

            currentLevelCompleted_ = false;
            currentLevelFailed_ = false;
            currentLevelIndex_ = levelIndex;

            movingGameController_.LoadLevel(levelData);

            eventDispatcher_.Send(
                new LevelLoadedEvent(levelData.id, levelIndex));
        }

        /// <summary>
        /// Reloads the current level.
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
            if (levelSelectorDestination_ == null)
            {
                throw new InvalidOperationException(
                    "Level selector destination is not assigned.");
            }

            if (UnityNavigationLocator.Service == null)
            {
                throw new InvalidOperationException(
                    "UnityNavigationLocator.Service is not initialized.");
            }

            UnityNavigationLocator.Service.Navigate(levelSelectorDestination_);
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
        }

        private void OnLevelFailed(LevelFailedEvent levelFailedEvent)
        {
            currentLevelCompleted_ = false;
            currentLevelFailed_ = true;
        }

        private int ResolveInitialLevelIndex()
        {
            if (SceneNavigationState.LastSceneNavigationParams is MovingGameNavigationParams movingGameParams)
            {
                int levelIndex = movingGameParams.LevelIndex;
                SceneNavigationState.Clear();
                return levelIndex;
            }

            return defaultLevelIndex_;
        }
    }
}