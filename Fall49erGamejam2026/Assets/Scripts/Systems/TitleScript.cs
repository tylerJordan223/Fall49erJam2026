using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    //script to handle title screen buttons

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayStartSFX()
    {
        AudioManager.instance.PlaySingleSFX(AudioManager.instance.startGame);
    }
}
