using UnityEngine;

public class Key : MonoBehaviour
{
    public int keyValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance != null) GameManager.Instance.AddKey(keyValue);
        Destroy(gameObject);
    }
}