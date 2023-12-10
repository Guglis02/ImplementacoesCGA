using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Object gameScene;

    public void Awake()
    {
        try {SceneManager.UnloadSceneAsync(gameScene.name);}
        catch (System.ArgumentException) {}
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameScene.name);
    }

    public void Quit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}