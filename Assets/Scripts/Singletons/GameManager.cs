using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Constants
    private const string MAIN_MENU_SCENE_NAME = "MainMenu";
    private const string LEVEL_SCENE_PREFIX = "lvl";
    private const string CREDITS_SCENE_NAME = "Credits";
    private const string PLAYER_NAME = "Player_Entity";
    private const string CAMERA_POSITION_TAG = "Player_Camera_Position";
    private const string CAMERA_TARGET_TAG = "Camera_target";
    private const int MAIN_MENU_LEVEL_ID = 0;
    private const int CREDITS_LEVEL_ID = 99;
    private const int MAX_GAME_LEVEL = 5;
    #endregion

    #region Serialized Fields
    [Header("UI Panels")]
    public GameObject GameOverPanel;
    public GameObject WinPanel;
    public GameObject PauseScreen;
    public GameObject MainMenuScreen;

    [Header("Camera")]
    public Camera MainCamera;
    #endregion

    #region Private Fields
    private bool pauseAvailable = false;
    private bool restartAvailable = false;
    private bool isGameFinished = false;
    private bool isLevelWon = false;
    private int currentLevel = MAIN_MENU_LEVEL_ID;
    private PlayerInput cachedPlayerInput;
    private Scene? currentGameScene = null;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        ValidateAndInitializePanels();
        ShowPanel(MainMenuScreen); // Always start in main menu
    }

    void Update()
    {
        HandleInput();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Scene Events
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only handle additively loaded game scenes
        if (mode == LoadSceneMode.Additive && scene.name != MAIN_MENU_SCENE_NAME)
        {
            currentGameScene = scene;
            SetupGameCamera();
        }
    }
    #endregion

    #region Initialization
    private void ValidateAndInitializePanels()
    {
        ValidatePanel(GameOverPanel, "GameOverPanel");
        ValidatePanel(WinPanel, "WinPanel");
        ValidatePanel(PauseScreen, "PauseScreen");
        ValidatePanel(MainMenuScreen, "MainMenuScreen");

        if (MainCamera == null)
        {
            Debug.LogError("GameManager: MainCamera is not assigned in the Inspector!");
        }

        SetPanelsActive(gameOver: false, win: false, pause: false, mainMenu: false);
    }

    private void ValidatePanel(GameObject panel, string panelName)
    {
        if (panel == null)
        {
            Debug.LogError($"GameManager: {panelName} is not assigned in the Inspector!");
        }
    }
    #endregion

    #region Input Handling
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) && isLevelWon)
        {
            LoadNext();
        }
        else if (Input.GetKeyDown(KeyCode.R) && restartAvailable)
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && pauseAvailable)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (Time.timeScale == 1f)
        {
            PauseGame();
        }
        else
        {
            UnPauseGame();
        }
    }
    #endregion

    #region Game State Management
    /// <summary>
    /// Triggers the game over state and displays the game over panel.
    /// </summary>
    public void GameOver()
    {
        if (isGameFinished) return;

        isGameFinished = true;
        pauseAvailable = false;
        restartAvailable = true;
        SetTimeScale(0f);
        SetPlayerInputEnabled(false);
        ShowExclusivePanel(GameOverPanel);
        Debug.Log("Game Over! You lost.");
    }

    /// <summary>
    /// Triggers the victory state and displays the win panel.
    /// </summary>
    public void Victory()
    {
        if (isGameFinished) return;

        isGameFinished = true;
        isLevelWon = true;
        pauseAvailable = false;
        restartAvailable = true;
        SetTimeScale(0f);
        SetPlayerInputEnabled(false);
        ShowExclusivePanel(WinPanel);
        Debug.Log("Victory! Congratulations!");
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public void RestartGame()
    {
        SetTimeScale(1f);
        isGameFinished = false;
        LoadLevel(currentLevel);
        Debug.Log("Scene reloaded.");
    }
    #endregion

    #region Pause Menu
    /// <summary>
    /// Pauses the game and displays the pause menu.
    /// </summary>
    public void PauseGame()
    {
        SetTimeScale(0f);
        restartAvailable = false;
        ShowExclusivePanel(PauseScreen);
        SetPlayerInputEnabled(false);
    }

    /// <summary>
    /// Resumes the game and hides the pause menu.
    /// </summary>
    public void UnPauseGame()
    {
        SetTimeScale(1f);
        restartAvailable = true;
        HidePanel(PauseScreen);
        SetPlayerInputEnabled(true);
    }
    #endregion

    #region Scene Management
    /// <summary>
    /// Loads the main menu.
    /// </summary>
    public void MainMenu()
    {
        LoadLevel(MAIN_MENU_LEVEL_ID);
    }

    /// <summary>
    /// Loads a specific level by ID. Level 0 is the main menu.
    /// </summary>
    /// <param name="LevelID">The level ID to load. 0 for main menu, 1+ for game levels.</param>
    public void LoadLevel(int LevelID)
    {
        currentLevel = LevelID;
        SetPanelsActive(gameOver: false, win: false, pause: false, mainMenu: false);

        if (LevelID == MAIN_MENU_LEVEL_ID)
        {
            LoadMainMenu();
        }
        else
        {
            LoadGameLevel(LevelID);
        }
    }

    /// <summary>
    /// Loads the next level in sequence.
    /// </summary>
    public void LoadNext()
    {
        // If on credits, return to main menu
        if (currentLevel == CREDITS_LEVEL_ID)
        {
            LoadLevel(MAIN_MENU_LEVEL_ID);
        }
        else
        {
            LoadLevel(currentLevel + 1);
        }
    }

    /// <summary>
    /// Quits the application. In the Unity editor, stops play mode.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadMainMenu()
    {
        pauseAvailable = false;
        restartAvailable = false;
        isLevelWon = false;
        cachedPlayerInput = null; // Clear cache since Player will be destroyed

        // Unload current game scene if one is loaded
        if (currentGameScene.HasValue)
        {
            SceneManager.UnloadSceneAsync(currentGameScene.Value);
            currentGameScene = null;
        }

        DisableGameCamera();
        SetTimeScale(0f);
        ShowExclusivePanel(MainMenuScreen);
    }

    private void LoadGameLevel(int levelID)
    {
        pauseAvailable = true;
        restartAvailable = true;
        isLevelWon = false;
        cachedPlayerInput = null; // Clear cache when loading new level

        string sceneName;

        // Level 6+ redirects to credits
        if (levelID > MAX_GAME_LEVEL)
        {
            sceneName = CREDITS_SCENE_NAME;
            currentLevel = CREDITS_LEVEL_ID;
        }
        else
        {
            sceneName = LEVEL_SCENE_PREFIX + levelID;
        }

        // Unload current game scene before loading new one
        if (currentGameScene.HasValue)
        {
            SceneManager.UnloadSceneAsync(currentGameScene.Value);
        }

        // Load new game scene additively (MainMenu stays loaded)
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SetTimeScale(1f);
        SetPlayerInputEnabled(true);
    }
    #endregion

    #region Helper Methods
    private void SetPanelsActive(bool gameOver = false, bool win = false, bool pause = false, bool mainMenu = false)
    {
        if (GameOverPanel != null) GameOverPanel.SetActive(gameOver);
        if (WinPanel != null) WinPanel.SetActive(win);
        if (PauseScreen != null) PauseScreen.SetActive(pause);
        if (MainMenuScreen != null) MainMenuScreen.SetActive(mainMenu);
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameManager: Attempted to show a null panel!");
        }
    }

    private void ShowExclusivePanel(GameObject panel)
    {
        // Hide all panels first
        SetPanelsActive(gameOver: false, win: false, pause: false, mainMenu: false);
        // Then show the specified panel
        ShowPanel(panel);
    }

    private void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    private void SetPlayerInputEnabled(bool enabled)
    {
        PlayerInput playerInput = GetPlayerInput();
        if (playerInput != null)
        {
            playerInput.enabled = enabled;
        }
    }

    private PlayerInput GetPlayerInput()
    {
        if (cachedPlayerInput != null)
        {
            return cachedPlayerInput;
        }

        GameObject playerEntity = GameObject.Find(PLAYER_NAME);
        if (playerEntity != null)
        {
            cachedPlayerInput = playerEntity.GetComponent<PlayerInput>();
        }

        return cachedPlayerInput;
    }

    private void SetupGameCamera()
    {
        if (MainCamera == null)
        {
            Debug.LogError("GameManager: MainCamera is not assigned. Cannot setup game camera.");
            return;
        }

        GameObject playerEntity = GameObject.Find(PLAYER_NAME);
        if (playerEntity == null)
        {
            Debug.LogWarning("GameManager: Player entity not found in scene. Camera setup skipped.");
            return;
        }

        // Find Camera_Position by tag
        GameObject cameraPositionObject = GameObject.FindGameObjectWithTag(CAMERA_POSITION_TAG);
        if (cameraPositionObject == null)
        {
            Debug.LogError($"GameManager: GameObject with tag '{CAMERA_POSITION_TAG}' not found.");
            return;
        }

        // Find Camera_Target by tag
        GameObject cameraTargetObject = GameObject.FindGameObjectWithTag(CAMERA_TARGET_TAG);
        if (cameraTargetObject == null)
        {
            Debug.LogError($"GameManager: GameObject with tag '{CAMERA_TARGET_TAG}' not found.");
            return;
        }

        // Get or add Camera_Follower to MainCamera
        Camera_Follower cameraFollower = MainCamera.GetComponent<Camera_Follower>();
        if (cameraFollower == null)
        {
            cameraFollower = MainCamera.gameObject.AddComponent<Camera_Follower>();
        }
        cameraFollower.Camera_target = cameraPositionObject;

        // Get Player_Camera from Camera_Position and configure it
        Player_Camera playerCamera = cameraPositionObject.GetComponent<Player_Camera>();
        if (playerCamera != null)
        {
            playerCamera.Camera_Target = cameraTargetObject;
        }
        else
        {
            Debug.LogWarning($"GameManager: Player_Camera component not found on GameObject with tag '{CAMERA_POSITION_TAG}'.");
        }

        // Get Camera_Controller from Player and configure it
        Camera_Controller cameraController = playerEntity.GetComponent<Camera_Controller>();
        if (cameraController != null)
        {
            cameraController.Camera_Target = cameraTargetObject;
            cameraController.playerRigidbody = playerEntity.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.LogWarning("GameManager: Camera_Controller component not found on player.");
        }

        Debug.Log("GameManager: Game camera setup complete.");
    }

    private void DisableGameCamera()
    {
        if (MainCamera == null) return;

        // Remove Camera_Follower when returning to main menu
        Camera_Follower cameraFollower = MainCamera.GetComponent<Camera_Follower>();
        if (cameraFollower != null)
        {
            Destroy(cameraFollower);
        }
    }
    #endregion
}