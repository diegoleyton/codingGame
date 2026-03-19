using System;
using UnityEngine;
using Flowbit.MovingGame.Core.Levels;
using Flowbit.Utilities.Unity.Navigation;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Builds and handles the moving game level selector UI.
    /// </summary>
    public sealed class MovingGameLevelSelectorController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MovingGameLevelsLibrary levelsLibrary_;
        [SerializeField] private NavigationDestinationAsset movingGameSceneDestination_;
        [SerializeField] private Transform contentRoot_;
        [SerializeField] private LevelSelectionButtonView levelButtonPrefab_;

        private void Start()
        {
            if (levelsLibrary_ == null)
            {
                throw new InvalidOperationException("Levels library is not assigned.");
            }

            if (movingGameSceneDestination_ == null)
            {
                throw new InvalidOperationException("Moving game scene destination is not assigned.");
            }

            if (contentRoot_ == null)
            {
                throw new InvalidOperationException("Content root is not assigned.");
            }

            if (levelButtonPrefab_ == null)
            {
                throw new InvalidOperationException("Level button prefab is not assigned.");
            }

            if (UnityNavigationLocator.Service == null)
            {
                throw new InvalidOperationException(
                    "UnityNavigationLocator.Service is not initialized. Make sure a UnityNavigationInstaller exists in the scene.");
            }

            levelsLibrary_.Load();
            BuildLevelButtons();
        }

        private void BuildLevelButtons()
        {
            ClearButtons();

            int levelCount = levelsLibrary_.GetLevelCount();

            for (int i = 0; i < levelCount; i++)
            {
                int levelIndex = i;
                MovingGameLevelData levelData = levelsLibrary_.GetLevelAt(levelIndex);

                LevelSelectionButtonView buttonView =
                    Instantiate(levelButtonPrefab_, contentRoot_);

                buttonView.SetTitle(BuildLevelTitle(levelIndex, levelData));
                buttonView.SetSubtitle(BuildLevelSubtitle(levelData));
                buttonView.SetOnClick(() => OpenLevel(levelIndex));
            }
        }

        private void OpenLevel(int levelIndex)
        {
            UnityNavigationLocator.Service.Navigate(
                movingGameSceneDestination_,
                new MovingGameNavigationParams(levelIndex));
        }

        private static string BuildLevelTitle(int levelIndex, MovingGameLevelData levelData)
        {
            string levelName = string.IsNullOrWhiteSpace(levelData.name)
                ? $"Level {levelIndex + 1}"
                : levelData.name;

            return $"{levelIndex + 1}. {levelName}";
        }

        private static string BuildLevelSubtitle(MovingGameLevelData levelData)
        {
            if (string.IsNullOrWhiteSpace(levelData.hint))
            {
                return $"Difficulty {levelData.difficulty}";
            }

            return $"Difficulty {levelData.difficulty} • {levelData.hint}";
        }

        private void ClearButtons()
        {
            for (int i = contentRoot_.childCount - 1; i >= 0; i--)
            {
                Destroy(contentRoot_.GetChild(i).gameObject);
            }
        }
    }
}