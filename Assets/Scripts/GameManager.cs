using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Input Settings")]
    public PlayerInput playerInput;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    private InputAction restartAction;
    private bool isGameFinished = false;

    void Awake()
    {
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
        if (gameOverPanel == null)
        {
            var panelObject = GameObject.Find("GameOverPanel");
            if (panelObject != null)
            {
                gameOverPanel = panelObject;
            }
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("GameManager: GameOverPanel is not assigned and was not found in the scene.");
        }

        EnsureInput();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
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

        if (uiMap != null)
        {
            restartAction = uiMap.FindAction("Restart");
            if (restartAction != null)
            {
                Debug.Log("GameManager: Restart action found and ready.");
            }
            else
            {
                Debug.LogError("GameManager: Restart action not found in UI action map!");
            }
        }
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
        if (isGameFinished) return;

        isGameFinished = true;
        EndGame(gameOverPanel);
        Debug.Log("Game Over! Ai pierdut.");
    }


    public void Victory()
    {
        if (isGameFinished) return;

        isGameFinished = true;
        EndGame(winPanel);
        Debug.Log("Victorie! Felicit?ri!");
    }

    private void EndGame(GameObject panelToActivate)
    {
        Time.timeScale = 0f;

        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);
        }
        else
        {
            Debug.LogError("Panel-ul de UI nu a fost asignat �n Inspector!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}