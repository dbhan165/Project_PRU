using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
    // Gọi từ Button OnClick để load lại scene hiện tại
    public void OnPlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Gọi để về màn đầu (scene index 0)
    public void OnPlayAgainToStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // Gọi nếu muốn load theo tên scene (truyền tên từ Button OnClick)
    public void OnPlayAgainByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
