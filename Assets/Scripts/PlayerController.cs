using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D rb;
    public float jumpHeight = 5f;
    public float moveSpeed = 5f;
    private float movement;
    private bool facingRight = true;
    public bool isGround = true;

    [Header("Animation")]
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Combat")]
    public int maxHealth = 10;
    public int currentHealth;
    public float knockbackForce = 5f;
    public float invulnerableTime = 1f;
    private bool invulnerable = false;

    [Header("Stomp & Death")]
    public Transform respawnPoint;
    public float stompBounce = 8f;
    public float fallDeathY = -10f;

    [Header("Power-Up")]
    public Sprite poweredSprite;
    public RuntimeAnimatorController poweredAnimator;
    public int baseDamage = 1;
    public int poweredDamage = 2;
    private Sprite defaultSprite;
    private RuntimeAnimatorController defaultAnimator;
    private int currentDamage;
    private bool isPowered = false;

    public static PlayerController Instance;

    void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        defaultSprite = spriteRenderer.sprite;
        defaultAnimator = animator.runtimeAnimatorController;
        currentDamage = baseDamage;
    }

    void Update()
    {
        if (transform.position.y < fallDeathY)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TakeDamage(GameManager.Instance.currentHealth);
            else
                Die();
            return;
        }

        float left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1f : 0f;
        float right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f;
        movement = left + right;

        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && !facingRight)
        {
            transform.eulerAngles = Vector3.zero;
            facingRight = true;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && Mathf.Abs(rb.linearVelocity.y) < 0.001f && isGround)
        {
            Jump();
            isGround = false;
            animator.SetBool("isJumping", true);
        }

        animator.SetBool("isRunning", Mathf.Abs(movement) > 0f);
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(movement * Time.fixedDeltaTime * moveSpeed, 0, 0);
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            animator.SetBool("isJumping", false);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D cp in collision.contacts)
            {
                if (cp.normal.y > 0.5f)
                {
                    StompEnemy(collision.gameObject);
                    return;
                }
            }
        }
    }

    public void PerformStompBounce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * stompBounce, ForceMode2D.Impulse);
    }

    private void StompEnemy(GameObject enemyObj)
    {
        var e = enemyObj.GetComponent<Enemy>() ?? enemyObj.GetComponentInParent<Enemy>();
        if (e != null) e.Die();

        PerformStompBounce();
        TriggerTemporaryInvulnerability(0.5f);
        animator.SetTrigger("Stomp");
    }

    public void TriggerTemporaryInvulnerability(float duration)
    {
        StartCoroutine(InvulnerableCoroutine(duration));
    }

    IEnumerator InvulnerableCoroutine(float duration)
    {
        invulnerable = true;
        float t = 0f;
        while (t < duration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            t += 0.1f;
        }
        spriteRenderer.enabled = true;
        invulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (invulnerable) return;

        if (other.CompareTag("Trap") || other.CompareTag("Enemy"))
        {
            int damage = 1;

            var trapComp = other.GetComponent<Trap>();
            if (trapComp != null) damage = trapComp.damage;
            else
            {
                var enemyComp = other.GetComponent<Enemy>() ?? other.GetComponentInParent<Enemy>();
                if (enemyComp != null) damage = enemyComp.damage;
            }

            if (GameManager.Instance != null)
                GameManager.Instance.TakeDamage(damage);
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ApplyPowerUp(Sprite newSprite, RuntimeAnimatorController newAnim, int damage, float duration)
    {
        StopCoroutine(nameof(RevertPowerUpAfter));

        if (newSprite != null) spriteRenderer.sprite = newSprite;
        if (newAnim != null && animator != null)
        {
            animator.runtimeAnimatorController = newAnim;
            animator.enabled = true;
        }

        currentDamage = damage;
        isPowered = true;
        StartCoroutine(RevertPowerUpAfter(duration));
        Debug.Log("ApplyPowerUp called with sprite: " + newSprite.name);
    }

    private IEnumerator RevertPowerUpAfter(float t)
    {
        yield return new WaitForSeconds(t);
        spriteRenderer.sprite = defaultSprite;
        animator.runtimeAnimatorController = defaultAnimator;
        currentDamage = baseDamage;
        isPowered = false;
    }

    public int GetAttackDamage()
    {
        return currentDamage;
    }
    public void TakeDamage(int damage)
{
    if (invulnerable) return;

    currentHealth -= damage;

    if (currentHealth <= 0)
    {
        Die();
    }
    else
    {
        TriggerTemporaryInvulnerability(invulnerableTime);
        animator.SetTrigger("Hit"); // nếu có animation bị đánh
    }
}

}
