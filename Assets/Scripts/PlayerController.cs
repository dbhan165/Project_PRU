using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpHeight = 5f;
    private float movement;
    public float moveSpeed = 5f;
    private bool facingRight = true;
    public bool isGround = true;
    public Animator animator;

    public float knockbackForce = 5f;
    private bool invulnerable = false;
    public float invulnerableTime = 1f;

    public int maxHealth = 10;
    public int currentHealth;
    public Transform respawnPoint;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
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

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement * Time.fixedDeltaTime * 5f, 0, 0) * moveSpeed;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // giữ để detect ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            animator.SetBool("isJumping", false);
        }
    }

    // DÙNG TRIGGER CHO TRAP
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (invulnerable) return;

        if (other.CompareTag("Trap") || other.CompareTag("Enemy"))
        {
            int damage = 1;

            // nếu là Trap đọc Trap.damage
            var trapComp = other.GetComponent<Trap>();
            if (trapComp != null)
            {
                damage = trapComp.damage;
            }
            else
            {
                // nếu là Enemy đọc Enemy.damage
                var enemyComp = other.GetComponent<Enemy>();
                if (enemyComp != null) damage = enemyComp.damage;
            }

            // trừ máu qua GameManager
            if (GameManager.Instance != null)
                GameManager.Instance.TakeDamage(damage);

            // knockback ra xa đối tượng
            Vector2 dir = (Vector2)(transform.position - other.transform.position);
            dir = dir.normalized;
            rb.AddForce(new Vector2(dir.x * knockbackForce, Mathf.Abs(dir.y) * knockbackForce), ForceMode2D.Impulse);

            StartCoroutine(InvulnerableCoroutine());
        }
    }

    IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float t = 0f;
            while (t < invulnerableTime)
            {
                sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(0.1f);
                t += 0.1f;
            }
            sr.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invulnerableTime);
        }
        invulnerable = false;
    }

    void Die()
    {
        animator.SetTrigger("Die");
        // quay lại màn đầu luôn
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}