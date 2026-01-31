using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Input Settings")]
    public InputActionReference restartAction;

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;

    private bool isGameOver = false;

    void Awake()
    {
        // Singleton pattern - ensure only one GameManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // Hide game over panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (restartAction != null)
        {
            restartAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (restartAction != null)
        {
            restartAction.action.Disable();
        }
    }

    void Update()
    {
        // Allow restart at any time
        if (restartAction != null && restartAction.action.WasPressedThisFrame())
        {
            RestartGame();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Pause the game

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("Panel activated! Active state: " + gameOverPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Game Over Panel is not assigned!");
        }

        Debug.Log("Game Over! Press Restart to restart.");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
