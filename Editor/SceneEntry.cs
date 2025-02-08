using System;
using UnityEditor;

[Serializable]
public class SceneEntry
{
    public SceneAsset asset;
    public string enumName;
    public int enumValue;

    public SceneEntry(SceneAsset asset, string enumName, int enumValue)
    {
        this.asset = asset;
        this.enumName = enumName;
        this.enumValue = enumValue;
    }
}