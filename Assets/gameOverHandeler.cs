using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOverHandeler : MonoBehaviour
{
    public GameObject restartButton;

    public void showEndScreen()
    {
        restartButton.SetActive(true);
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
