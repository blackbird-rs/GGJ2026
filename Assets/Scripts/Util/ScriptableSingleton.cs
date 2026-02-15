using System;
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
                throw new Exception("ScriptableSingleton<" + typeof(T) + "> - No asset found in Resources folder.");
            }

            if (results.Length > 1)
            {
                Debug.LogError("ScriptableSingleton<" + typeof(T) + "> - Multiple assets found in Resources folder.");
            }

            return instance = results[0];
        }
    }
}