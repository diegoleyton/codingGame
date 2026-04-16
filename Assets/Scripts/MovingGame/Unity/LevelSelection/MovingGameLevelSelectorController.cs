using System;
using UnityEngine;
using Flowbit.GameBase.Services;
using Flowbit.GameBase.Progress;
using Flowbit.MovingGame.Core.Levels;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Scenes;
using Flowbit.Utilities.Navigation;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Builds and handles the moving game level selector UI.
    /// </summary>
    public sealed class MovingGameLevelSelectorController : SceneBase
    {
        [Header("References")]
        [SerializeField] private MovingGameLevelsLibrary levelsLibrary_;
        [SerializeField] private Transform contentRoot_;
        [SerializeField] private LevelSelectionButtonView levelButtonPrefab_;

        private IGameNavigationService navigationService_;
        private ILevelProgressService levelProgressService_;

        protected override bool HasBackButton => true;

        protected override void Start()
        {
            base.Start();
            var serviceContainer = GlobalServiceContainer.ServiceContainer;
            navigationService_ = serviceContainer.Get<IGameNavigationService>();
            levelProgressService_ = serviceContainer.Get<ILevelProgressService>();

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

                buttonView.SetTitle(levelData.name, levelIndex);
                buttonView.SetDificulty(levelData.GetDificulty());
                buttonView.SetStars(
                    levelProgressService_.GetBestStarCount(levelIndex),
                    levelsLibrary_.GetRankingMetadata()?.maxStars ?? 4);
                bool isUnlocked = levelProgressService_.IsLevelUnlocked(levelIndex);
                buttonView.SetLocked(!isUnlocked);
                Action onClick = isUnlocked
                    ? () => OpenLevelDetails(levelData, levelIndex)
                    : null;
                buttonView.SetOnClick(onClick);
            }
        }

        private void OpenLevelDetails(MovingGameLevelData levelData, int levelIndex)
        {
            navigationService_.Navigate(
                SceneType.MovingGameLevelSelectionPopup,
                new MovingGameLevelSelectionPopupParams(levelData, levelIndex, () => OpenLevel(levelIndex)));
        }

        private void OpenLevel(int levelIndex)
        {
            navigationService_.Navigate(
                SceneType.MovingGame,
                new MovingGameNavigationParams(levelIndex));
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
