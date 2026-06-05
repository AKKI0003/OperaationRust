using UnityEngine;

public class RadarDishInteraction : MonoBehaviour
{
    bool playerInside;

    void Update()
    {
        if (playerInside &&
           Input.GetKeyDown(KeyCode.E))
        {
            MissionManager.Instance
                .radarDisabled = true;

            Debug.Log(
                "RADAR DISABLED");
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