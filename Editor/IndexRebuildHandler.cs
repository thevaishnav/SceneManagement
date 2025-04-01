using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


public static class IndexRebuildHandler
{
    private const string SCRIPT_TEMPLATE = @"/*********************************************/
/******** THIS CODE IS AUTO GENERATED ********/
/*********************************************/

using UnityEngine; 
using System.Collections.Generic;

public enum SceneId
{
        [Tooltip(""Previous Scene in Build Index"")]
        PreviousScene = -2,

        [Tooltip(""Next Scene in Build Index"")]
        NextScene = -1,

        [Tooltip(""Invalid Scene/Unknown Scene/Null Scene"")]
        Unknown = 0,

SCENE_ID_ENTRIES
}


public static class BI
{
    public static readonly (SceneId, string)[] BUILD_INDEX = new (SceneId, string)[]
    {
BUILD_INDEX_ENTRIES
    };

    public static readonly Dictionary<SceneId, string> ID_TO_NAME;
    public static readonly Dictionary<string, SceneId> NAME_TO_ID;
    public static readonly Dictionary<SceneId, int> ID_TO_INDEX;

    static BI()
    {
        ID_TO_NAME = new Dictionary<SceneId, string>();
        ID_TO_INDEX = new Dictionary<SceneId, int>();
        NAME_TO_ID = new Dictionary<string, SceneId>();

        int index = -3;
        foreach ((SceneId id, string name) in BUILD_INDEX)
        {
            ID_TO_NAME.Add(id, name);
            NAME_TO_ID.Add(name, id);
            ID_TO_INDEX.Add(id, index);
            index++;
        }
    }
}";

    private const string INDEX_DATA_RESOURCE_NAME = "SceneIndexData";
    private const string INDEX_DATA_RESOURCE_PATH = "Assets/Editor/Resources";
    private const string TEMP_SCENE_ID_GUID = "9768c0ca177aa244b94d1da3eb61b469";
    private const string SCRIPT_FOLDER = "Assets/Scripts";
    private const string SCRIPT_PATH = SCRIPT_FOLDER + "/SceneId (Auto Generated).cs";

    private static SceneIndexData LoadOrCreateSceneIndex()
    {
        SceneIndexData indexData = Resources.Load<SceneIndexData>(INDEX_DATA_RESOURCE_NAME);
        if (indexData == null)
        {
            indexData = ScriptableObject.CreateInstance<SceneIndexData>();
            if (!Directory.Exists(INDEX_DATA_RESOURCE_PATH))
            {
                Directory.CreateDirectory(INDEX_DATA_RESOURCE_PATH);
                AssetDatabase.ImportAsset(INDEX_DATA_RESOURCE_PATH);
            }

            AssetDatabase.CreateAsset(indexData, INDEX_DATA_RESOURCE_PATH + "/" + INDEX_DATA_RESOURCE_NAME + ".asset");
            AssetDatabase.SaveAssets();
        }

        return indexData;
    }

    public static string ConvertToPascalCase(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            throw new ArgumentException("Scene name cannot be null or empty", nameof(sceneName));

        // Remove anything in parentheses
        sceneName = Regex.Replace(sceneName, @"\s*\(.*?\)", "");

        // Remove non-alphanumeric characters except spaces
        sceneName = Regex.Replace(sceneName, @"[^a-zA-Z0-9 ]", "");

        // Normalize spaces and convert to PascalCase
        StringBuilder enumName = new StringBuilder();
        bool capitalizeNext = true;

        foreach (char c in sceneName)
        {
            if (char.IsWhiteSpace(c))
            {
                capitalizeNext = true;
            }
            else
            {
                enumName.Append(capitalizeNext ? char.ToUpper(c) : c);
                capitalizeNext = false;
            }
        }

        return enumName.ToString();
    }

    [MenuItem("Scene Management/Rebuild Index")]
    public static void RebuildSceneIndex()
    {
        var indexData = LoadOrCreateSceneIndex();
        Dictionary<SceneAsset, SceneEntry> sceneDictionary =
            indexData.sceneEntries.ToDictionary(entry => entry.asset, entry => entry);

        var scenesInBuild = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path))
            .ToList();

        HashSet<SceneAsset> completeList = scenesInBuild.Concat(sceneDictionary.Keys).ToHashSet();
        List<SceneEntry> sceneEntries = new List<SceneEntry>();
        HashSet<string> sceneIdEnumEntries = new HashSet<string>();
        int sceneId = sceneDictionary.Values.Select(sc => sc.enumValue).DefaultIfEmpty(0).Max() + 1;

        foreach (SceneAsset scene in completeList)
        {
            if (sceneDictionary.TryGetValue(scene, out var entry) == false)
            {
                var enumEntry =
                    ConvertToPascalCase(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(scene)));
                entry = new SceneEntry(scene, enumEntry, sceneId);
                sceneDictionary.Add(scene, entry);
                sceneId++;
            }

            if (sceneIdEnumEntries.Contains(entry.enumName))
                throw new Exception(
                    "Build index contains multiple scenes with the same name, Scene Manager does not know hot to rectify this.");

            sceneIdEnumEntries.Add(entry.enumName);
            sceneEntries.Add(entry);
        }

        // Delete Temp Script (Included in the project to sure it compliles)
        string scriptPath = AssetDatabase.GUIDToAssetPath(TEMP_SCENE_ID_GUID);
        if (!string.IsNullOrEmpty(scriptPath) && File.Exists(scriptPath))
            File.Delete(scriptPath);

        sceneEntries.Sort((s1, s2) => s1.enumValue.CompareTo(s2.enumValue));

        string sceneIdEntries = string.Join("\n", sceneEntries.Select(SelectSceneIdEntry));
        string buildIndexEntries = string.Join(",\n", scenesInBuild.Select(SelectBuildIndexEntry));
        string generatedCode = SCRIPT_TEMPLATE.Replace("SCENE_ID_ENTRIES", sceneIdEntries)
            .Replace("BUILD_INDEX_ENTRIES", buildIndexEntries);
        
        if (!Directory.Exists(SCRIPT_FOLDER))
            Directory.CreateDirectory(SCRIPT_FOLDER);
        
        File.WriteAllText(SCRIPT_PATH, generatedCode);
        AssetDatabase.Refresh();

        // Persist updated list
        indexData.sceneEntries = sceneEntries;
        EditorUtility.SetDirty(indexData);
        AssetDatabase.SaveAssets();
        Debug.Log("Scene Index Updated.");
        return;

        string SelectSceneIdEntry(SceneEntry scene)
        {
            var sceneName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(scene.asset));
            var enumName = ConvertToPascalCase(sceneName);
            if (string.Equals(enumName, scene.enumName) == false)
            {
                bool pickNewName = EditorUtility.DisplayDialog("Scene Name Changed",
                    $"Name of the scene \"{sceneName}\" was changed, now you have to decide which name to use in the SceneId enum.\nPick New Name if either of the following is True:\n1. You are certain that there are no references to this scene's id in the code (Dont worry about serialized references)\n2. You've refactored the related enum value to match the new Scene Name before rebuilding the scene index.",
                    "New Name", "Old Name");
                if (pickNewName)
                    scene.enumName = enumName;
            }

            return $"        {scene.enumName} = {scene.enumValue},";
        }

        string SelectBuildIndexEntry(SceneAsset asset)
        {
            var info = sceneDictionary[asset];
            var sceneName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(asset));
            return $"        (SceneId.{info.enumName}, \"{sceneName}\")";
        }
    }
}