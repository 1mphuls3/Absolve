using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private TransitionFadeScript fade;
    public void StartGame()
    {
        fade.FadeToScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Tutorial()
    {
        fade.FadeToScene(1);
    }
}
