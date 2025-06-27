using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    public HostGameManager _hostGameManager;
    public static HostSingleton Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindAnyObjectByType<HostSingleton>();

            if (instance == null)
            {
                Debug.LogError("No HostSingleton in the scene");
                return null;
            }

            return instance;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        _hostGameManager = new();
    }
    private void OnDestroy()
    {
        _hostGameManager?.Dispose();
    }
}
