using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TMP_Text healthText;   // kéo Text (TMP) vào đây
    public Image healthBarFill;   // kéo HealthBar_Fill (Image) vào đây

    [Header("Keys")]
    public int keyCount = 0;
    public TMP_Text keyText; // gán Text Mesh Pro hiển thị số key

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
        UpdateKeyUI();
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthUI();
        if (currentHealth <= 0) OnGameOver();
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

    public void AddKey(int amount = 1)
    {
        keyCount += amount;
        UpdateKeyUI();
    }

    void UpdateKeyUI()
    {
        if (keyText != null) keyText.text = $"Keys: {keyCount}";
    }

    public void ResetGameState()
    {
        keyCount = 0;
        currentHealth = maxHealth;
        UpdateKeyUI();
        UpdateHealthUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }
    public void GotoMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
