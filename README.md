# Scene Management Utility
## Why Use This?
### Problems with Traditional Scene Loading
- **Scene Names**: Prone to typos, renaming breaks references.
- **Build Indices**: Reordering scenes breaks index-based loading.

### Benefits
- **Cleaner Code**: To load a scene, just call `.Load()` method directly on `SceneId`.
- **Stable References**: Easily handles renaming scenes or reordering them in build index.

## Installation
1. Open the Package Manager
2. Click on `+` button in the top-right corner
3. Select `Add Package from Git URL`
4. Paste: `https://github.com/thevaishnav/SceneManagement.git`
5. Click `Add`
6. Once installation is done, you will find a `Scene Management` appear in the tool bar, click on it and select `Rebuild Index`

## Note
It is recommended to `Rebuild Index` in all the following scenarios
- After installing the package 
- Project's build index is changed (some scene is added/removed/enabled/disabled)
- Any Scene in the build is renamed.   
In order to Rebuild the index, Select `Rebuild Index` option in the `Scene Management` menu.

## Usage
### Load a Scene
Each scene in the Build Index will have exactly 1 entry associated with it in the SceneId enum. To load a scene named "Home":
```csharp
SceneId.Home.Load();
```


### Convert Between Scene Representations
You can easily convert between Scene Name, Build Index, and Scene Id
```csharp
string sceneName = SceneId.Home.GetName();
int sceneIndex = SceneId.Home.GetBuildIndex();
SceneId sceneIdFromName = "Home".SceneNameToId();
SceneId sceneIdFromIndex = sceneIndex.SceneIndexToId();
```

## Additional Features
### Next/Previos Scene
The SceneId enum will always have placeholder entries for Next Scene and Previous Scene (in build index).
These placeholders can be used similar to all the other entries.   

Operations involving Next Scene
```csharp
SceneId.NextScene.Load();   // Load
string sceneName = SceneId.NextScene.GetName();    // Get the name of Next Scene
int sceneIndex = SceneId.NextScene.GetBuildIndex();  // Get build index of Next Scene
```

Operations involving Previous Scene:
```csharp
SceneId.PreviousScene.Load();   // Load
string sceneName = SceneId.PreviousScene.GetName();    // Get the name of Next Scene
int sceneIndex = SceneId.PreviousScene.GetBuildIndex();  // Get build index of Next Scene
```
Some interesting things you can do:
```csharp
if (SceneId.NextScene.Compare(SceneId.Home))
    // What to do if next scene is Home scene

if (SceneId.PreviousScene.Compare(SceneId.Home))
    // What to do if previous scene is Home scene
```


### SceneArgs
While you can directly seriealize `SceneId` enum, using SceneArgs class instead will allow you to do much more.
```csharp
[System.Serializable]
public class SceneArgs
{
    // Id of the scene to load
    public SceneId scene;
    
    // Mode, Single or Additive. Additive meaning keep the current scene and load the new one as well
    public LoadSceneMode mode;
    
    public bool isAsync;
}
```