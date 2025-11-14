using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AudioManager audioManager;
    [Tooltip("Số máu mất khi chạm vào enemy")]
    public int damage = 1;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 5f;
    private Vector3 startPos;
    private bool movingRight = true;
    
    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    void Start()
    {
        startPos = transform.position;
    }
    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= startPos.x + distance)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= startPos.x - distance)
            {
                movingRight = true;
                Flip();
            }
        }
    }
    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    public void Die()
    {
        audioManager.PlayEnemyDeathSound();
        // tắt collider để tránh trigger/va chạm thêm
        var cols = GetComponents<Collider2D>();
        foreach (var c in cols) c.enabled = false;

        // tùy: play animation nếu có
        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
            Destroy(gameObject, 0.4f); // delay cho animation
        }
        else
        {
            Destroy(gameObject);
        }
    }
}