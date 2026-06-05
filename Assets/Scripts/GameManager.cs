using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    bool gameEnded;

    void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;

        gameOverPanel.SetActive(true);

        Time.timeScale = 0;
    }

    public void Victory()
    {
        if (gameEnded) return;

        gameEnded = true;

        victoryPanel.SetActive(true);

        Time.timeScale = 0;
    }
}