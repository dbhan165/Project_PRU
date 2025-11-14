using UnityEngine;

public class BoostEffect : MonoBehaviour
{
    private AudioManager audioManager;
    private void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            audioManager.PlayBoostSound();
        }
    }
}
