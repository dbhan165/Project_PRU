using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public Sprite poweredSprite;
    public RuntimeAnimatorController poweredAnimator;
    public int damage = 2;
    public float duration = 60f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var player = other.GetComponent<PlayerController>() ?? other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            player.ApplyPowerUp(poweredSprite, poweredAnimator, damage, duration);
        }
        Destroy(gameObject);
    }
}