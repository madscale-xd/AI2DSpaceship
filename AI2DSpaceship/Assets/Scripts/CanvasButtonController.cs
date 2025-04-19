using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButtonController : MonoBehaviour
{
    public Button buttonOne;
    public Button buttonTwo;

    void Start()
    {
        buttonOne.onClick.AddListener(LoadSpaceshipScene);
        buttonTwo.onClick.AddListener(QuitApp);
    }

    void LoadSpaceshipScene()
    {
        Debug.Log("Loading SpaceshipScene...");
        SceneManager.LoadScene("SpaceshipScene");
    }

    void QuitApp()
    {
        Debug.Log("Quitting application...");
        Application.Quit();

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
    #endif
    }
}
