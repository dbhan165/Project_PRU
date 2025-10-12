using UnityEngine;

public class StompDetector : MonoBehaviour
{
    public float bounceForce = 8f;
    private PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        if (player == null) Debug.LogWarning("StompDetector: PlayerController not found in parent.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        // hỗ trợ trường hợp Enemy component ở parent hoặc child
        var enemy = other.GetComponent<Enemy>() ?? other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.Die();
        }
        else
        {
            Debug.Log("StompDetector: hit object tagged Enemy but no Enemy component found on it.");
        }

        // bounce player
        if (player != null && player.rb != null)
        {
            // reset vertical velocity rồi add impulse để bounce chính xác
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, 0f);
            player.rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
    }
}