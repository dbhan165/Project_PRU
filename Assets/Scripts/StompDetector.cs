using UnityEngine;

public class StompDetector : MonoBehaviour
{
    public float bounceForce = 8f;
    private PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        if (player == null)
            Debug.LogWarning("StompDetector: PlayerController not found in parent.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        // Kiểm tra cả Enemy thường và Boss
        var enemy = other.GetComponent<Enemy>() ?? other.GetComponentInParent<Enemy>();
        var boss = other.GetComponent<BossEnemy>() ?? other.GetComponentInParent<BossEnemy>();

        if (enemy != null)
        {
            enemy.Die(); // enemy thường chết ngay
        }
        else if (boss != null)
        {
            boss.TakeDamage(1); // boss chỉ mất máu, không chết ngay
        }
        else
        {
            Debug.Log("StompDetector: hit object tagged Enemy but no valid Enemy or BossEnemy component found.");
        }

        // Bounce player và miễn sát thương tạm thời
        if (player != null)
        {
            player.PerformStompBounce();
            player.TriggerTemporaryInvulnerability(0.5f);
        }
    }
}
