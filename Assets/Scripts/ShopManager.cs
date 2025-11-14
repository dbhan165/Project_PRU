using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel;
    public float invincibilityCooldown = 120f;
    private float lastPurchaseTime = -999f;

    [Header("Shop Buttons")]
    public Button invincibilityButton; // Nút MUA của item Miễn thương
    public Button rageButton;          // Nút MUA của item Bạo kích

    private float invincibilityCooldownEnd = 0f;
    private float rageCooldownEnd = 0f;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleShop();
        }

        if (invincibilityButton != null)
        {
            invincibilityButton.interactable = Time.realtimeSinceStartup >= invincibilityCooldownEnd;
        }

        // Kiểm tra cooldown cho nút Bạo kích
        if (rageButton != null)
        {
            rageButton.interactable = Time.realtimeSinceStartup >= rageCooldownEnd;
        }
    }

    public void ToggleShop()
    {
        bool isShopOpen = !shopPanel.activeSelf;
        shopPanel.SetActive(isShopOpen);
        Time.timeScale = isShopOpen ? 0f : 1f;
    }

    // --- CÁC HÀM MUA BÁN SẼ THÊM VÀO ĐÂY SAU ---
    public void PurchaseHealthPotion()
    {
        if (GameManager.Instance.TrySpendCoins(10))
        {
            GameManager.Instance.HealPlayer(2);
        }
    }

    public void PurchaseMaxHealthUpgrade()
    {
        if (GameManager.Instance.TrySpendCoins(50))
        {
            GameManager.Instance.IncreaseMaxHealth(1);
        }
    }

    public void PurchaseSpeedBoost()
    {
        if (GameManager.Instance.TrySpendCoins(30))
        {
            PlayerController.Instance.IncreaseMoveSpeed(0.1f);
        }
    }

    public void PurchaseInvincibility()
    {
        // Kiểm tra xem đã đủ thời gian hồi chiêu chưa
        if (Time.realtimeSinceStartup < lastPurchaseTime + invincibilityCooldown)
        {
            Debug.Log("Vật phẩm đang trong thời gian hồi, không thể mua!");
            // Optional: Hiển thị một thông báo trên UI cho người chơi biết
            return; // Dừng lại, không cho mua
        }

        int cost = 75; // Giá tiền (nên đặt khá cao)
        if (GameManager.Instance.TrySpendCoins(cost))
        {
            // Nếu mua thành công:
            // 1. Kích hoạt hiệu ứng bất tử trên Player trong 30 giây
            PlayerController.Instance.TriggerTemporaryInvulnerability(30f);

            // Đặt thời gian kết thúc cooldown
            invincibilityCooldownEnd = Time.realtimeSinceStartup + 120f; // 2 phút

            Debug.Log("Đã mua Bất Tử trong 30 giây!");

            // 3. (Quan trọng) Tự động đóng shop để người chơi tận dụng thời gian
            ToggleShop();
        }
    }

    public void PurchaseRage()
    {
        // Kiểm tra cooldown trước
        if (Time.realtimeSinceStartup < rageCooldownEnd) return;

        int cost = 60; // Giá tiền
        if (GameManager.Instance.TrySpendCoins(cost))
        {
            PlayerController.Instance.ActivateRageMode(30f);

            // Đặt thời gian kết thúc cooldown
            rageCooldownEnd = Time.realtimeSinceStartup + 120f; // 2 phút

            ToggleShop();
        }
    }
}