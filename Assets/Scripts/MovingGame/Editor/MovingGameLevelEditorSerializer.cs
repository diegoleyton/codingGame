using System;
using System.IO;

using Flowbit.MovingGame.Core.Levels;

using UnityEditor;
using UnityEngine;

namespace Flowbit.MovingGame.Editor
{
    internal static class MovingGameLevelEditorSerializer
    {
        public const string DefaultLevelsAssetPath =
            "Assets/Content/Levels/MovingGame/moving_game_levels.json";

        public static MovingGameLevelsFileData Load(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                throw new ArgumentException("Asset path cannot be null or empty.", nameof(assetPath));
            }

            string fullPath = Path.GetFullPath(assetPath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Levels file was not found at '{assetPath}'.", fullPath);
            }

            string json = File.ReadAllText(fullPath);
            return MovingGameLevelsParser.Parse(json);
        }

        public static void Save(string assetPath, MovingGameLevelsFileData fileData)
        {
            if (fileData == null)
            {
                throw new ArgumentNullException(nameof(fileData));
            }

            string json = JsonUtility.ToJson(fileData, true);
            string fullPath = Path.GetFullPath(assetPath);

            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();
        }
    }
}
