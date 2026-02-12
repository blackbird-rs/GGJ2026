#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#else
using System;
#endif
using UnityEngine;

public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            T[] results = Resources.LoadAll<T>("");
            if (results.Length == 0)
            {
#if UNITY_EDITOR
                const string savePath = "Assets/Generated/Resources/";
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                instance = CreateInstance<T>();
                var path = Path.Combine(savePath, $"{typeof(T).Name}.asset");
                AssetDatabase.CreateAsset(instance, path);
                return instance;
#else
                throw new Exception("ScriptableSingleton<" + typeof(T) + "> - No asset found in Resources folder.");
#endif
            }

            if (results.Length > 1)
            {
                Debug.LogError("ScriptableSingleton<" + typeof(T) + "> - Multiple assets found in Resources folder.");
            }

            return instance = results[0];
        }
    }
}