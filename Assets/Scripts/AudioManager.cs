using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource effectMusicSource;

    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip getTrapClip;
    [SerializeField] private AudioClip boostClip;
    [SerializeField] private AudioClip keyClip;
    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip levelUpClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBackgroundMusic();
    }
    void PlayBackgroundMusic()
    {
        backgroundAudioSource.clip = backgroundMusicClip;
        backgroundAudioSource.Play();
    }
    public void StopBackgroundMusic()
    {
        backgroundAudioSource.Stop();
    }
    public void PlayJumpSound()
    {
        effectMusicSource.PlayOneShot(jumpClip);
    }
    public void PlayGameOverSound()
    {
        effectMusicSource.PlayOneShot(gameOverClip);
    }
    public void PlayGetTrapSound()
    {
        effectMusicSource.PlayOneShot(getTrapClip);
    }
    public void PlayBoostSound()
    {
        effectMusicSource.PlayOneShot(boostClip);
    }
    public void PlayKeySound()
    {
        effectMusicSource.PlayOneShot(keyClip);
    }
    public void PlayEnemyDeathSound()
    {
        effectMusicSource.PlayOneShot(enemyDeathClip);
    }
    public void PlayLevelUpSound()
    {
        effectMusicSource.PlayOneShot(levelUpClip);
    }
}
