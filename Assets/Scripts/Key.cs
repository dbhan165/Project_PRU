using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    private AudioManager audioManager;
    public int keyValue = 1;

    private void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance != null)
        {
            audioManager.PlayKeySound();
            GameManager.Instance.AddKey(keyValue);
            SceneController.instance.NextLevel();
        }

    }
}