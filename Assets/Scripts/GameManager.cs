using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem; // Input System mới

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("UI")]
    public GameObject gameOverPanel;
    public GameObject pauseMenuUI; // ⚡ thêm menu pause ở đây
    public TMP_Text healthText;
    public Image healthBarFill;

    [Header("Keys")]
    public int keyCount = 0;
    public TMP_Text keyText;

    private bool isPaused = false;

    [Header("Currency")]
    public int coinCount = 0;
    public TMP_Text coinText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        UpdateHealthUI();
        UpdateKeyUI();
    }

    void Update()
    {
        // Nhận input kể cả khi Time.timeScale = 0
        InputSystem.Update();

        if (Input.GetKeyDown(KeyCode.Escape)) // fallback cho chắc
        {
            Debug.Log("✅ ESC detected (InputSystem or Legacy)!");
            if (isPaused) ResumeGame();
            else PauseGame();
        }

    }

    // =================== Health ===================

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
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 0f;
    }

    // =================== Pause ===================

    void PauseGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // =================== Scene & Keys ===================

    public void RestartToStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
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
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GotoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }
    void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = coinCount.ToString();
    }
    public bool TrySpendCoins(int amountToSpend)
    {
        if (coinCount >= amountToSpend)
        {
            coinCount -= amountToSpend;
            UpdateCoinUI();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HealPlayer(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthUI();
    }
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        UpdateHealthUI();
    }
}
