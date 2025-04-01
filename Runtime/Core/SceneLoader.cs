using System;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Omnix.SceneManagement
{
    public static class SceneLoader
    {
        public static event Action<SceneId> OnSceneLoadingStarts;
        public static event Action<SceneId> OnSceneLoadingEnds;
        [CanBeNull] public static AsyncOperation LoadingOperation { get; private set; }
        public static SceneId CurrentlyLoading { get; private set; } = SceneId.Unknown;

        private static void OnLoadComplete(Scene scene, LoadSceneMode _)
        {
            SceneId sceneId = scene.name.SceneNameToId();
            OnSceneLoadingEnds?.Invoke(sceneId);
            
            if (CurrentlyLoading != SceneId.Unknown && sceneId == CurrentlyLoading)
            {
                LoadingOperation = null;
                CurrentlyLoading = SceneId.Unknown;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            SceneManager.sceneLoaded += OnLoadComplete;
        }

        public static void Load(SceneId id, LoadSceneMode mode = LoadSceneMode.Single, bool isAsync = false)
        {
            if (id == SceneId.Unknown)
            {
                Debug.LogError($"Cannot load scene with id {id}");
                return;
            }
            
            string sceneName = id.GetName();
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"Unable to find scene with id: {id}");
                return;
            }

            // Setup
            CurrentlyLoading = id;
            OnSceneLoadingStarts?.Invoke(id);

            // Load
            if (isAsync) LoadingOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            else SceneManager.LoadScene(sceneName, mode);
        }
    }
}