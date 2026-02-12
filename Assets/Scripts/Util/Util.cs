using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Util
{
#if UNITY_EDITOR
    public static IEnumerable<T> LoadAllAssetsOfType<T>() where T : UnityEngine.Object
    {
        return AssetDatabase
            .FindAssets($"t:{typeof(T).Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>);
    }
#endif
}