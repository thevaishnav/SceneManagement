using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Omnix.SceneManagement
{
    public static class SceneIdExtension
    {
        /// <returns> True if the given build index is valid </returns>
        public static bool IsBuildIndexValid(this int index)
        {
            return index >= 0 && index < BI.BUILD_INDEX.Length;
        }

        /// <returns>
        /// Name of the scene with <see cref="SceneId"/>.
        /// Empty string if scene id is <see cref="SceneId.Unknown"/> or invalid (in case of <see cref="SceneId.PreviousScene"/> / <see cref="SceneId.NextScene"/>)
        /// </returns>
        public static string GetName(this SceneId sceneId)
        {
            switch (sceneId)
            {
                case SceneId.PreviousScene:
                {
                    int buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
                    return IsBuildIndexValid(buildIndex) ? BI.BUILD_INDEX[buildIndex].Item2 : string.Empty;
                }
                case SceneId.NextScene:
                {
                    int buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
                    return IsBuildIndexValid(buildIndex) ? BI.BUILD_INDEX[buildIndex].Item2 : string.Empty;
                }
                case SceneId.Unknown:
                    return string.Empty;
                default:
                    return BI.ID_TO_NAME[sceneId];
            }
        }
        
        /// <returns>
        /// Returns build index of the scene
        /// -1 if scene id is <see cref="SceneId.Unknown"/> or invalid (in case of <see cref="SceneId.PreviousScene"/> / <see cref="SceneId.NextScene"/>) </returns>
        public static int GetBuildIndex(this SceneId sceneId)
        {
            switch (sceneId)
            {
                case SceneId.PreviousScene:
                {
                    int buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
                    return IsBuildIndexValid(buildIndex) ? buildIndex : -1;
                }
                case SceneId.NextScene:
                {
                    int buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
                    return IsBuildIndexValid(buildIndex) ? buildIndex : -1;
                }
                case SceneId.Unknown:
                    return -1;
                default:
                    return BI.ID_TO_INDEX[sceneId];
            }
        }

        /// <returns> Returns <see cref="SceneId"/> of scene with given name, <see cref="SceneId.Unknown"/> if name is invalid. </returns>
        public static SceneId SceneNameToId(this string name)
        {
            return BI.NAME_TO_ID.GetValueOrDefault(name, SceneId.Unknown);
        }

        /// <returns> Returns <see cref="SceneId"/> of scene with given build index, <see cref="SceneId.Unknown"/> if build-index is invalid </returns>
        public static SceneId SceneIndexToId(this int buildIndex)
        {
            return IsBuildIndexValid(buildIndex) ? BI.BUILD_INDEX[buildIndex].Item1 : SceneId.Unknown;
        }

        public static void Load(this SceneId id, LoadSceneMode mode = LoadSceneMode.Single, bool isAsync = true)
        {
            SceneLoader.Load(id, mode, isAsync);
        }
    }
}