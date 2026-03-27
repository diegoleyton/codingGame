using System.Collections.Generic;

namespace Flowbit.GameBase.Definitions
{
    /// <summary>
    /// Defines the type of the scenes.
    /// </summary>
    public enum SceneType
    {
        MovingGameLevelSelection,
        MovingGame,
        MovingGameLevelCompletedPopup,
        MovingGameLevelSelectionPopup,
        MainScene
    }

    public static class SceneTypeExtension
    {
        private static Dictionary<SceneType, string> typeToId_ = new()
        {
            {SceneType.MovingGameLevelSelection, "LevelSelectorScene"},
            {SceneType.MovingGame, "MovingGameScene"},
            {SceneType.MainScene, "MainScene"},
            {SceneType.MovingGameLevelCompletedPopup, "MovingGameEndGamePopup"},
            {SceneType.MovingGameLevelSelectionPopup, "MovingGameLevelSelectionPopup"}
        };

        public static string GetId(this SceneType sceneType)
        {
            return typeToId_[sceneType];
        }
    }
}