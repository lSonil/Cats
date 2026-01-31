using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject Pause_Screen;
    public GameObject Player;
    public bool Pause_enabled = true;

     void Start()
    {
        Player = GameObject.Find("Player_Entity");
        Pause_Screen = GameObject.Find("PauseScreen");
        Pause_Screen.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Pause_Screen.SetActive(true);
        Player.GetComponent<PlayerInput>().enabled=false;
    }
    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        Pause_Screen.SetActive(false);
        Player.GetComponent<PlayerInput>().enabled = true;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Main_Menu_Scene", LoadSceneMode.Single);
    }

    public void Load_Level(int LevelID)
    {
        switch (LevelID)
        {

            case 1  : { SceneManager.LoadScene("lvl1" , LoadSceneMode.Single); } break;
            case 2  : { SceneManager.LoadScene("lvl2" , LoadSceneMode.Single); } break;
            case 3  : { SceneManager.LoadScene("lvl3" , LoadSceneMode.Single); } break;
            case 4  : { SceneManager.LoadScene("lvl4" , LoadSceneMode.Single); } break;
            case 5  : { SceneManager.LoadScene("lvl5" , LoadSceneMode.Single); } break;
            case 6  : { SceneManager.LoadScene("lvl6" , LoadSceneMode.Single); } break;
            case 7  : { SceneManager.LoadScene("lvl7" , LoadSceneMode.Single); } break;
            case 8  : { SceneManager.LoadScene("lvl8" , LoadSceneMode.Single); } break;
            case 9  : { SceneManager.LoadScene("lvl9" , LoadSceneMode.Single); } break;
            case 10 : { SceneManager.LoadScene("lvl10", LoadSceneMode.Single); } break;

        }
        Time.timeScale = 1f;

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Pause_enabled == true)
        {
            Debug.Log("Escape key pressed with timescale="+ Time.timeScale);
            if (Time.timeScale == 1f)
            {
                PauseGame();
            }
            else
            {
                UnPauseGame();
            }

        }
    }

}
