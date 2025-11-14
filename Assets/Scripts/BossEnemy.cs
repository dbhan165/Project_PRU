using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthFillImage;
    public RectTransform healthBarUI;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;

    [Header("Detection Zone")]
    public Transform zoneStart;
    public Transform zoneEnd;
    private bool playerInZone = false;

    [Header("Attack Settings")]
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    private float lastAttackTime = -999f;

    [Header("Key Drop")]
    public GameObject keyPrefab;
    public Transform dropPoint;

    private Transform player;
    private bool isDead = false;

    public GameObject coinPrefab;
    public int coinsToDrop = 20;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthFillImage != null)
            healthFillImage.fillAmount = 1f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        CheckPlayerInZone();
        if (playerInZone)
        {
            MoveHorizontallyTowardPlayer();
            CheckAttackPlayer();
        }

        UpdateHealthBarPosition();
    }

    void CheckPlayerInZone()
    {
        float minX = Mathf.Min(zoneStart.position.x, zoneEnd.position.x);
        float maxX = Mathf.Max(zoneStart.position.x, zoneEnd.position.x);
        float playerX = player.position.x;

        playerInZone = playerX >= minX && playerX <= maxX;
    }

    void MoveHorizontallyTowardPlayer()
    {
        float distance = Mathf.Abs(player.position.x - transform.position.x);

        if (distance > stopDistance)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            Vector3 move = new Vector3(direction * moveSpeed * Time.deltaTime, 0f, 0f);
            transform.position += move;

            // Flip sprite
            transform.eulerAngles = direction > 0 ? Vector3.zero : new Vector3(0f, 180f, 0f);
        }
        Debug.Log("Boss moving toward player");

    }

    void CheckAttackPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
                pc.TakeDamage(1);
        }
    }

    void UpdateHealthBarPosition()
    {
        if (healthBarUI == null) return;
        Vector3 worldPos = transform.position + healthBarOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        healthBarUI.position = screenPos;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (healthFillImage != null)
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        if (keyPrefab != null && dropPoint != null)
            Instantiate(keyPrefab, dropPoint.position, Quaternion.identity);

        Destroy(gameObject);
        if (healthBarUI != null)
            Destroy(healthBarUI.gameObject);

        if (coinPrefab != null)
        {
            for (int i = 0; i < coinsToDrop; i++)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
