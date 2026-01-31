using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static CameraSingleton Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
}
