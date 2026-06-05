using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public GameObject prompt;

    void OnTriggerEnter2D(
        Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            prompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(
        Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            prompt.SetActive(false);
        }
    }
}