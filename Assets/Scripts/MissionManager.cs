using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public bool radarDisabled;
    public bool intelDownloaded;

    void Awake()
    {
        Instance = this;
    }
}