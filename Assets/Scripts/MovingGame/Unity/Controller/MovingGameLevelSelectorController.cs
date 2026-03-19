using System;
using UnityEngine;
using Flowbit.GameBase.Services;
using Flowbit.MovingGame.Core.Levels;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Scenes;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Builds and handles the moving game level selector UI.
    /// </summary>
    public sealed class MovingGameLevelSelectorController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MovingGameLevelsLibrary levelsLibrary_;
        [SerializeField] private Transform contentRoot_;
        [SerializeField] private LevelSelectionButtonView levelButtonPrefab_;

        private GameNavigationService navigationService_;

        private void Awake()
        {
            var serviceContainer = GlobalServiceContainer.ServiceContainer;
            navigationService_ = serviceContainer.Get<GameNavigationService>();
        }

        private void Start()
        {
            if (levelsLibrary_ == null)
            {
                throw new InvalidOperationException("Levels library is not assigned.");
            }

            if (contentRoot_ == null)
            {
                throw new InvalidOperationException("Content root is not assigned.");
            }

            if (levelButtonPrefab_ == null)
            {
                throw new InvalidOperationException("Level button prefab is not assigned.");
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
            navigationService_.Navigate(
                SceneType.MovingGame,
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