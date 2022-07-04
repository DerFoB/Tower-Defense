using UnityEngine;

/// <summary>
/// This class provides the singleton functionality for a Unity component.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T Instance;
    private static bool IsApplicationQuitting = false;

    public static T GetInstance()
    {
        if (IsApplicationQuitting) { return null; }

        if (Instance == null)
        {
            Instance = FindObjectOfType<T>();
            if (Instance == null)
            {
                GameObject obj = new GameObject
                {
                    name = typeof(T).Name
                };

                Instance = obj.AddComponent<T>();
            }
        }

        return Instance;
    }

    /// <summary>
    /// IMPORTANT: Always call base.Awake() first!
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this as T)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        IsApplicationQuitting = true;
    }
}