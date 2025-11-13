using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Start()
    {
        // Khi bắt đầu game, đảm bảo đang chạy bình thường
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Update()
    {
        // Nếu chưa có bàn phím thì bỏ qua
        if (Keyboard.current == null)
            return;

        // Cập nhật Input System để nhận phím cả khi Time.timeScale = 0
        InputSystem.Update();

        // Bấm ESC để bật/tắt menu
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("ESC pressed!"); // để kiểm tra xem có bắt được phím không

            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Debug.Log("Game resumed");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Debug.Log("Game paused");
    }
}
