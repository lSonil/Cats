using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Input Settings")]
    public PlayerInput playerInput;

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;

    private InputAction restartAction;

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

        EnsureInput();
    }

    void OnEnable()
    {
        EnsureInput();
        if (restartAction != null)
        {
            restartAction.performed += OnRestartPerformed;
            restartAction.Enable();
        }
    }

    void OnDisable()
    {
        if (restartAction != null)
        {
            restartAction.performed -= OnRestartPerformed;
        }
    }

    private void EnsureInput()
    {
        // Prefer the UI action map on the player's PlayerInput
        if (playerInput == null)
        {
            var playerObject = GameObject.Find("Player");
            if (playerObject != null)
            {
                playerInput = playerObject.GetComponent<PlayerInput>();
            }
        }

        if (playerInput == null)
        {
            Debug.LogWarning("GameManager: PlayerInput reference is not assigned and Player object was not found.");
            return;
        }

        var uiMap = playerInput.actions.FindActionMap("UI");
        if (uiMap != null && !uiMap.enabled)
        {
            uiMap.Enable();
        }

        restartAction = uiMap?.FindAction("Restart", true);
    }

    private void OnRestartPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        RestartGame();
    }

    public void GameOver()
    {
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