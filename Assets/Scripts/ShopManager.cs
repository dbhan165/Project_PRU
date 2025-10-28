using UnityEngine;
using UnityEngine.InputSystem;

public class ShopManager : MonoBehaviour
{
    [Tooltip("Kéo đối tượng ShopPanel từ Hierarchy vào đây")]
    public GameObject shopPanel;

    private bool isShopOpen = false;

    void Start()
    {
        // Luôn đảm bảo shop tắt khi bắt đầu game
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Lắng nghe phím 'B' để mở/đóng shop
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            Debug.Log("Phím 'B' đã được nhấn. Chuyển đổi trạng thái shop.");
            ToggleShop();
        }
    }

    // Hàm chính để BẬT/TẮT shop
    public void ToggleShop()
    {
        isShopOpen = !shopPanel.activeSelf;
        shopPanel.SetActive(isShopOpen);

        // Khi shop mở, dừng game. Khi shop đóng, cho game chạy lại.
        // Đây là dòng code quan trọng nhất.
        Time.timeScale = isShopOpen ? 0f : 1f;
    }

    // Hàm này sẽ được gọi bởi nút MUA của Bình Máu
    public void PurchaseHealthPotion()
    {
        int cost = 10; // Giá tiền
        if (GameManager.Instance.TrySpendCoins(cost))
        {
            // Nếu mua thành công
            GameManager.Instance.HealPlayer(2); // Hồi 2 máu
            Debug.Log("Đã mua Bình Máu!");
        }
    }

    // Hàm này sẽ được gọi bởi nút MUA của Nâng Cấp Tim
    public void PurchaseMaxHealthUpgrade()
    {
        int cost = 50; // Giá tiền
        if (GameManager.Instance.TrySpendCoins(cost))
        {
            // Nếu mua thành công
            GameManager.Instance.IncreaseMaxHealth(1); // Tăng 1 máu tối đa
            Debug.Log("Đã mua Nâng Cấp Tim!");
        }
    }

    // Hàm này sẽ được gọi bởi nút MUA của Giày Thần Tốc
    public void PurchaseSpeedBoost()
    {
        int cost = 30; // Giá tiền
        if (GameManager.Instance.TrySpendCoins(cost))
        {
            // Nếu mua thành công
            // PlayerController là Singleton, ta có thể gọi nó qua Instance
            PlayerController.Instance.IncreaseMoveSpeed(0.1f); // Tăng 10% tốc độ
            Debug.Log("Đã mua Giày Thần Tốc!");
        }
    }
}