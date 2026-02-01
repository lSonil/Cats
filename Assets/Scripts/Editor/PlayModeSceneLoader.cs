#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PlayModeSceneLoader
{
    [MenuItem("Tools/Setup MainMenu as Play Mode Scene")]
    public static void SetupMainMenuStartup()
    {
        string mainMenuPath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(mainMenuPath);

        if (EditorSceneManager.playModeStartScene != null)
        {
            UnityEngine.Debug.Log("MainMenu set as Play Mode startup scene!");
        }
    }
}
#endif