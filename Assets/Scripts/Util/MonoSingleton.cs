using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly object Lock = new();

    // ReSharper disable once StaticMemberInGenericType
    private static bool applicationIsQuitting;

    private static T instance;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (Lock)
            {
                if (instance == null)
                {
                    instance = GetOrCreateInstance();
                    if (instance != null) instance.ApplyPersistence();
                }

                return instance;
            }
        }
    }

    private static T GetOrCreateInstance()
    {
        var instances = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (instances.Length == 0)
        {
            return null;
        }
        else if (instances.Length == 1)
        {
            return instances[0];
        }
        else
        {
            for (var i = 1; i < instances.Length; i++)
            {
                Destroy(instances[i].gameObject);
            }

            return instances[0];
        }
    }

    private void ApplyPersistence()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ApplyPersistence();
        OnAwake();
    }

    protected virtual void OnAwake() { }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}