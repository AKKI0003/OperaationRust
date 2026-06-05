using UnityEngine;

public class VIPTerminal : MonoBehaviour
{
    bool playerInside;

    void Update()
    {
        if (playerInside &&
           Input.GetKeyDown(KeyCode.E))
        {
            if (
                MissionManager.Instance
                .radarDisabled)
            {
                MissionManager.Instance
                    .intelDownloaded = true;

                Debug.Log(
                    "INTEL DOWNLOADED");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}