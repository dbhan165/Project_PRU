using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private AudioManager audioManager;

    [Header("Movement")]
    public Rigidbody2D rb;
    public float jumpHeight = 5f;
    public float moveSpeed = 5f;
    private float movement;
    private bool facingRight = true;
    [HideInInspector] public bool isGround = true;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

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

    [Header("Cheat Code Settings")]
    public string cheatCode = "GODMODE";
    private string cheatBuffer = "";
    public bool cheatModeOn = false;

    [Header("Fly Hack Settings")]
    public bool flyModeOn = false;
    public float flySpeed = 7f;
    private float defaultGravity;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        audioManager = FindAnyObjectByType<AudioManager>();

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        defaultSprite = spriteRenderer ? spriteRenderer.sprite : null;
        defaultAnimator = animator ? animator.runtimeAnimatorController : null;
    }

    void OnEnable()
    {
        ResetStateOnEnable();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentDamage = baseDamage;
    }

    void Update()
    {
        if (rb == null || animator == null) return;

        // Ground check
        isGround = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // Rơi khỏi map
        if (transform.position.y < fallDeathY)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TakeDamage(GameManager.Instance.currentHealth);
            else
                Respawn();
            return;
        }
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Movement input
        float left = (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) ? -1f : 0f;
        float right = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) ? 1f : 0f;
        movement = left + right;

        // Jump
        if (!flyModeOn && keyboard.spaceKey.wasPressedThisFrame && isGround)
        {
            Jump();
            if (audioManager != null) audioManager.PlayJumpSound();
            animator.SetBool("isJumping", true);
        }

        // Flip
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

        animator.SetBool("isRunning", Mathf.Abs(movement) > 0f);

        // ===== Fly Mode Movement =====
        if (flyModeOn)
        {
            float flyX = 0f;
            float flyY = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) flyX = -1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) flyX = 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) flyY = 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) flyY = -1f;

            Vector2 flyMove = new Vector2(flyX, flyY).normalized * flySpeed * Time.deltaTime;
            transform.position += new Vector3(flyMove.x, flyMove.y, 0f);
        }

        HandleCheatInput();
    }

    void FixedUpdate()
    {
        if (!flyModeOn)
            transform.position += new Vector3(movement * Time.fixedDeltaTime * moveSpeed, 0, 0);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
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
        StopCoroutine(nameof(InvulnerableCoroutine));
        StartCoroutine(InvulnerableCoroutine(duration));
    }

    IEnumerator InvulnerableCoroutine(float duration)
    {
        invulnerable = true;
        float t = 0f;
        while (t < duration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            t += 0.1f;
        }
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        invulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (invulnerable || cheatModeOn) return;

        if (other.CompareTag("Trap") || other.CompareTag("Enemy"))
        {
            int damage = 1;
            if (audioManager != null) audioManager.PlayGetTrapSound();

            var trapComp = other.GetComponent<Trap>();
            if (trapComp != null) damage = trapComp.damage;
            else
            {
                var enemyComp = other.GetComponent<Enemy>() ?? other.GetComponentInParent<Enemy>();
                if (enemyComp != null) damage = enemyComp.damage;
            }

            if (GameManager.Instance != null)
                GameManager.Instance.TakeDamage(damage);
            else
                TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable || cheatModeOn) return;

        currentHealth -= damage;

        if (currentHealth <= 0) Die();
        else
        {
            TriggerTemporaryInvulnerability(invulnerableTime);
            animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
        if (audioManager != null)
        {
            audioManager.StopBackgroundMusic();
            audioManager.PlayGameOverSound();
        }

        StartCoroutine(DelayedReload());
    }

    IEnumerator DelayedReload()
    {
        yield return new WaitForSeconds(0.8f);
        Time.timeScale = 1f;
        Respawn();
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Respawn()
    {
        if (respawnPoint == null)
        {
            ReloadScene();
            return;
        }

        transform.position = respawnPoint.position;
        rb.linearVelocity = Vector2.zero;
        movement = 0f;

        currentHealth = maxHealth;
        invulnerable = false;
        cheatModeOn = false;
        flyModeOn = false;
        cheatBuffer = "";

        animator.ResetTrigger("Die");
        animator.SetBool("isJumping", false);
        animator.SetBool("isRunning", false);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.white;
        }
        if (defaultAnimator != null && animator != null)
            animator.runtimeAnimatorController = defaultAnimator;

        StopAllCoroutines();
    }

    public void ApplyPowerUp(Sprite newSprite, RuntimeAnimatorController newAnim, int damage, float duration)
    {
        if (audioManager != null) audioManager.PlayLevelUpSound();
        StopCoroutine(nameof(RevertPowerUpAfter));

        if (newSprite != null && spriteRenderer != null) spriteRenderer.sprite = newSprite;
        if (newAnim != null && animator != null)
            animator.runtimeAnimatorController = newAnim;

        currentDamage = damage;
        isPowered = true;
        StartCoroutine(RevertPowerUpAfter(duration));
    }

    private IEnumerator RevertPowerUpAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (spriteRenderer != null && defaultSprite != null) spriteRenderer.sprite = defaultSprite;
        if (animator != null && defaultAnimator != null) animator.runtimeAnimatorController = defaultAnimator;
        currentDamage = baseDamage;
        isPowered = false;
    }

    public int GetAttackDamage() => currentDamage;

    private void HandleCheatInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        foreach (KeyControl key in keyboard.allKeys)
        {
            if (key == null || !key.wasPressedThisFrame) continue;

            string keyName = key.displayName;
            if (string.IsNullOrEmpty(keyName)) continue;

            keyName = keyName.ToUpper();
            if (keyName.Length == 1 && keyName[0] >= 'A' && keyName[0] <= 'Z')
            {
                cheatBuffer += keyName;
                if (cheatBuffer.Length > 20) cheatBuffer = cheatBuffer.Substring(cheatBuffer.Length - 20);
            }
        }

        if (keyboard.backspaceKey != null && keyboard.backspaceKey.wasPressedThisFrame && cheatBuffer.Length > 0)
            cheatBuffer = cheatBuffer.Substring(0, cheatBuffer.Length - 1);

        if ((keyboard.enterKey != null && keyboard.enterKey.wasPressedThisFrame) ||
            (keyboard.numpadEnterKey != null && keyboard.numpadEnterKey.wasPressedThisFrame))
        {
            if (cheatBuffer.Equals(cheatCode, System.StringComparison.OrdinalIgnoreCase))
                ToggleCheatMode();

            cheatBuffer = "";
        }

        // Toggle Fly Mode bằng phím F
        if (keyboard.fKey != null && keyboard.fKey.wasPressedThisFrame)
            ToggleFlyMode();
    }

    private void ToggleCheatMode()
    {
        cheatModeOn = !cheatModeOn;

        if (cheatModeOn)
        {
            invulnerable = true;
            if (spriteRenderer != null) spriteRenderer.color = Color.cyan;
            Debug.Log("God Mode ON!");
        }
        else
        {
            invulnerable = false;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            Debug.Log("God Mode OFF!");
        }
    }

    private void ToggleFlyMode()
    {
        flyModeOn = !flyModeOn;

        if (flyModeOn)
        {
            defaultGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            if (spriteRenderer != null) spriteRenderer.color = Color.yellow;
            Debug.Log("Fly Mode ON!");
        }
        else
        {
            rb.gravityScale = defaultGravity;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            Debug.Log("Fly Mode OFF!");
        }
    }


    void ResetStateOnEnable()
    {
        currentDamage = baseDamage;
        currentHealth = Mathf.Max(1, currentHealth);
        invulnerable = false;
        cheatModeOn = false;
        flyModeOn = false;
        cheatBuffer = "";
        if (animator != null)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
        }
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}