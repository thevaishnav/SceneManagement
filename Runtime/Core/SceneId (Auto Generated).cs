/*********************************************/
/******** THIS CODE IS AUTO GENERATED ********/
/*********************************************/

using UnityEngine; 
using System.Collections.Generic;

public enum SceneId
{
        [Tooltip("Previous Scene in Build Index")]
        PreviousScene = -2,

        [Tooltip("Next Scene in Build Index")]
        NextScene = -1,

        [Tooltip("Invalid Scene/Unknown Scene/Null Scene")]
        Unknown = 0,
}


public static class BI
{
    public static readonly (SceneId, string)[] BUILD_INDEX = new (SceneId, string)[]
    {
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
}