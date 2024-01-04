using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Object m_gameScene;

    public void StartGame()
    {
        SceneManager.LoadScene(m_gameScene.name, LoadSceneMode.Additive);
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}