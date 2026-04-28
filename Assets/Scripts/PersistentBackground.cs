using UnityEngine;

public class PersistentBackground : MonoBehaviour
{
    private static PersistentBackground instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}