using UnityEngine;

public class Coin : MonoBehaviour
{
    [Tooltip("Số tiền nhận được khi nhặt đồng coin này")]
    public int coinValue = 1;

    // Đây là hàm đặc biệt của Unity, nó sẽ tự động chạy KHI có một vật thể khác
    // đi vào vùng "Trigger" của coin.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // // 'other' chính là đối tượng đã va chạm với coin.
        // Chúng ta kiểm tra 'tag' của nó. Nếu nó không phải là "Player",
        // thì chúng ta không làm gì cả và kết thúc hàm ngay lập tức.
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Nếu đi được đến đây, có nghĩa là người chơi đã chạm vào coin.
        // Bước 1: Gọi đến GameManager (đối tượng quản lý game duy nhất)
        // và yêu cầu nó thực hiện hàm AddCoin().
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin(coinValue);
        }

        // Bước 2: Tự hủy gameObject (chính là đồng coin này)
        // để nó biến mất khỏi màn chơi.
        Destroy(gameObject);
    }
}
