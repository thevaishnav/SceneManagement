using UnityEngine.SceneManagement;

namespace Omnix.SceneManagement
{
    [System.Serializable]
    public class SceneArgs
    {
        public SceneId scene;
        public LoadSceneMode mode;
        public bool isAsync;

        public SceneArgs(string name, LoadSceneMode mode = LoadSceneMode.Single, bool isAsync = true)
        {
            this.scene = name.SceneNameToId();
            this.mode = mode;
            this.isAsync = isAsync;
        }
        
        public SceneArgs(SceneId scene, LoadSceneMode mode = LoadSceneMode.Single, bool isAsync = true)
        {
            this.scene = scene;
            this.mode = mode;
            this.isAsync = isAsync;
        }

        public void Load() => SceneLoader.Load(scene, mode, isAsync);
    }
}