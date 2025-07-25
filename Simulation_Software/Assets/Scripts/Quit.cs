using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviour
{

    public void OnStartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

