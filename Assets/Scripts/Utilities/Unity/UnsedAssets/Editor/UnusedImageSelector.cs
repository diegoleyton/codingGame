using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class UnusedImageSelector
{
    [MenuItem("Tools/Unused Assets/Select Unused Sprites2")]
    private static void SelectUnusedSprites()
    {
        var candidatePaths = GetCandidatePathsForSprites();
        SelectUnusedAssetsAtPaths<Sprite>(candidatePaths, "Sprite");
    }

    [MenuItem("Tools/Unused Assets/Select Unused Non-Sprite Textures")]
    private static void SelectUnusedNonSpriteTextures()
    {
        var candidatePaths = GetCandidatePathsForNonSpriteTextures();
        SelectUnusedAssetsAtPaths<Texture2D>(candidatePaths, "Texture2D");
    }

    private static void SelectUnusedAssetsAtPaths<T>(HashSet<string> candidatePaths, string label) where T : Object
    {
        var rootPaths = GetRootAssetPaths();
        var usedPaths = new HashSet<string>();

        int totalRoots = rootPaths.Count;
        for (int i = 0; i < totalRoots; i++)
        {
            string root = rootPaths[i];
            var deps = AssetDatabase.GetDependencies(root, true);

            foreach (var dep in deps)
            {
                if (candidatePaths.Contains(dep))
                    usedPaths.Add(dep);
            }

            if (i % 50 == 0)
            {
                EditorUtility.DisplayProgressBar(
                    $"Scanning {label} dependencies",
                    $"{i}/{totalRoots} roots scanned",
                    totalRoots == 0 ? 1f : (float)i / totalRoots);
            }
        }

        EditorUtility.ClearProgressBar();

        var unusedPaths = candidatePaths
            .Where(path => !usedPaths.Contains(path))
            .OrderBy(path => path)
            .ToList();

        var unusedObjects = new List<Object>();
        foreach (var path in unusedPaths)
        {
            var obj = AssetDatabase.LoadAssetAtPath<T>(path);
            if (obj != null)
                unusedObjects.Add(obj);
        }

        Selection.objects = unusedObjects.ToArray();

        Debug.Log(
            $"Unused {label}: {unusedObjects.Count}\n" +
            string.Join("\n", unusedPaths));

        if (unusedObjects.Count > 0)
            EditorGUIUtility.PingObject(unusedObjects[0]);
    }

    private static HashSet<string> GetCandidatePathsForSprites()
    {
        var guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });
        return new HashSet<string>(
            guids.Select(AssetDatabase.GUIDToAssetPath)
                 .Where(IsRealAssetFile)
        );
    }

    private static HashSet<string> GetCandidatePathsForNonSpriteTextures()
    {
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });
        var result = new HashSet<string>();

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!IsRealAssetFile(path))
                continue;

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                continue;

            // Excluir texturas que están importadas como Sprite
            if (importer.textureType == TextureImporterType.Sprite)
                continue;

            result.Add(path);
        }

        return result;
    }

    private static List<string> GetRootAssetPaths()
    {
        var roots = new HashSet<string>();

        AddAssetsOfType("t:Scene", roots);
        AddAssetsOfType("t:Prefab", roots);
        AddAssetsOfType("t:ScriptableObject", roots);
        AddAssetsOfType("t:Material", roots);
        AddAssetsOfType("t:AnimationClip", roots);
        AddAssetsOfType("t:AnimatorController", roots);
        AddAssetsOfType("t:RuntimeAnimatorController", roots);
        AddAssetsOfType("t:SpriteAtlas", roots);

        return roots
            .Where(IsRealAssetFile)
            .OrderBy(p => p)
            .ToList();
    }

    private static void AddAssetsOfType(string filter, HashSet<string> result)
    {
        foreach (var guid in AssetDatabase.FindAssets(filter, new[] { "Assets" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (IsRealAssetFile(path))
                result.Add(path);
        }
    }

    private static bool IsRealAssetFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        if (!path.StartsWith("Assets/"))
            return false;

        if (AssetDatabase.IsValidFolder(path))
            return false;

        if (!File.Exists(path))
            return false;

        string extension = Path.GetExtension(path).ToLowerInvariant();

        if (extension == ".cs" ||
            extension == ".js" ||
            extension == ".boo" ||
            extension == ".meta" ||
            extension == ".asmdef" ||
            extension == ".asmref")
        {
            return false;
        }

        return true;
    }
}