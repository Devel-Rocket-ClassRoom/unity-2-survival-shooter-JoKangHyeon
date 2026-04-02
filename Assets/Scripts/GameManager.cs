using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;
    #endregion

    public UiManager uiManager;

    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        instance = this;
        IsGameOver = false;
    }

    private void Start()
    {
        uiManager.UpdateScore(Score);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.TogglePause();
        }
    }

    public void AddScore(int score)
    {
        Score += score;
        uiManager.UpdateScore(Score);
    }

    public void GameOver()
    {
        IsGameOver = true;
    }
}
