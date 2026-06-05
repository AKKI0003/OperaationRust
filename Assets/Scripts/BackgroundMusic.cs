using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    static BackgroundMusic instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}