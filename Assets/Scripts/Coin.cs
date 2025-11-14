using UnityEngine;
public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCoin(coinValue);
            Destroy(gameObject);
        }
    }
}