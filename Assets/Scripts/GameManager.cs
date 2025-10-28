using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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

    [Header("Currency")] // Tạo một khu vực mới trong Inspector cho dễ nhìn
    public int coinCount = 0;
    public TMP_Text coinText; // Kéo Text (TMP) hiển thị số coin vào đây trong Inspector

    // Khởi đầu cho hiệu ứng tăng coin
    private Coroutine countingCoroutine;
    private int displayedCoinCount = 0;

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
        //UpdateCoinUI();
        UpdateKeyUI();
        // Khởi tạo hiển thị coin
        displayedCoinCount = coinCount;
        if (coinText != null) coinText.text = displayedCoinCount.ToString();
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthUI();
        if (currentHealth <= 0) OnGameOver();
    }

    public void AddCoin(int amount)
    {
        Debug.Log($"--- Nhặt được Coin! Giá trị: {amount} ---");
        coinCount += amount; // Cập nhật giá trị thật ngay lập tức

        Debug.Log($"Giá trị coinCount THỰC TẾ đã được cập nhật lên: {coinCount}");

        // Nếu coroutine chưa chạy, hãy bắt đầu nó.
        if (countingCoroutine == null)
        {
            Debug.Log("Coroutine chưa chạy. Bắt đầu một Coroutine mới.");
            countingCoroutine = StartCoroutine(CountCoinsRoutine());
        }
        else
        {
            Debug.Log("Coroutine ĐANG CHẠY. Nó sẽ tự động đuổi theo giá trị mới.");
        }
        // Nếu nó đang chạy rồi, không cần làm gì cả, nó sẽ tự động đuổi theo giá trị coinCount mới.
    }

    public bool TrySpendCoins(int amountToSpend)
    {
        if (coinCount >= amountToSpend)
        {
            coinCount -= amountToSpend;
            UpdateCoinUI(); // Cập nhật thẳng số coin, không cần hiệu ứng
            return true;
        }
        else
        {
            Debug.Log("Không đủ coin!");
            return false;
        }
    }

    private IEnumerator CountCoinsRoutine()
    {
        Debug.Log("-> Coroutine BẮT ĐẦU. Mục tiêu cần đếm đến là: " + coinCount);
        // Tốc độ đếm, số càng lớn đếm càng nhanh
        float countSpeed = 5f;

        // Vòng lặp này sẽ chạy mãi mãi cho đến khi số hiển thị bằng số thật
        while (displayedCoinCount < coinCount)
        {
            int previousDisplayedCount = displayedCoinCount; // Lưu lại giá trị cũ để so sánh
            // Lerp (nội suy) mượt mà từ số hiện tại đến số mục tiêu
            displayedCoinCount = (int)Mathf.Lerp(displayedCoinCount, coinCount, Time.deltaTime * countSpeed);

            // Thêm 1 để đảm bảo số luôn tăng lên, tránh bị kẹt ở số nguyên
            if (displayedCoinCount < coinCount)
            {
                displayedCoinCount++;
            }

            if (coinText != null) coinText.text = displayedCoinCount.ToString();

            Debug.Log($"Frame: {Time.frameCount}, Giá trị hiển thị: {previousDisplayedCount} -> {displayedCoinCount}");
            yield return null; // Chờ frame tiếp theo
        }

        // Đảm bảo con số cuối cùng luôn chính xác
        displayedCoinCount = coinCount;
        if (coinText != null) coinText.text = displayedCoinCount.ToString();

        Debug.Log($"-> Coroutine KẾT THÚC. Giá trị hiển thị cuối cùng: {displayedCoinCount}");

        // Đánh dấu coroutine đã kết thúc để có thể gọi lại lần sau
        countingCoroutine = null;
    }

    // Hàm này dùng để reset, gọi trực tiếp để tránh hiệu ứng
    void UpdateCoinUI()
    {
        displayedCoinCount = coinCount;
        if (coinText != null)
        {
            coinText.text = coinCount.ToString();
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
        // Dừng coroutine nếu đang chơi lại
        if (countingCoroutine != null) StopCoroutine(countingCoroutine);
        countingCoroutine = null;
        keyCount = 0;
        currentHealth = maxHealth;
        UpdateKeyUI();
        UpdateHealthUI();
        UpdateCoinUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }
    public void GotoMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void HealPlayer(int amount)
    {
        currentHealth += amount;
        // Đảm bảo máu không vượt quá giới hạn
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI(); // Cập nhật lại thanh máu trên UI
    }

    // Hàm để tăng máu tối đa
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Thường thì khi tăng máu tối đa, người chơi cũng được hồi máu luôn
        UpdateHealthUI();
    }
}
