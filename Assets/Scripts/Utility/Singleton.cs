using UnityEngine;

// Generic Singleton class
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    // Optional: Prevent the object from being destroyed when loading new scenes
    //void Awake()
    //{
    //    if (_instance != null && _instance != this)
    //    {
    //        Destroy(this.gameObject);
    //        return;
    //    }

    //    _instance = this as T;

    //    DontDestroyOnLoad(this.gameObject);
    //}

    // Your other MonoBehaviour code goes here
}
