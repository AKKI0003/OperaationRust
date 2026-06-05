using UnityEngine;

public class HelipadExit : MonoBehaviour
{
    void OnTriggerEnter2D(
        Collider2D other)
    {
        if (
            other.CompareTag("Player") &&
            MissionManager.Instance
            .intelDownloaded)
        {
            GameManager.Instance
                .Victory();
        }
    }
}