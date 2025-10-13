using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth; // đã bỏ [ReadOnly]

    [Header("UI")]
    public GameObject gameOverPanel; // Canvas/GameOverUI (set inactive in inspector)
    public TMP_Text healthText;      // Canvas/HUD/Text (TMP)
    public Image healthBarFill;      // Canvas/HUD/HealthBar_BG/HealthBar_Fill (Image type = Filled)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            OnGameOver();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {Mathf.Max(0, currentHealth)}/{maxHealth}";
        if (healthBarFill != null)
            healthBarFill.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
    }

    void OnGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // gọi từ nút Play Again (OnClick)
    public void RestartToStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void RestartCurrent()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
