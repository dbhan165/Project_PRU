using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
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

    // ============================================================
    // >>>>>>>> CHEAT CODE <<<<<<<<
    // ============================================================
    [Header("Cheat Code Settings")]
    public string cheatCode = "GODMODE";     // mã để bật/tắt bất tử
    private string cheatBuffer = "";         // lưu tạm ký tự người chơi nhập
    public bool cheatModeOn = false;         // đang bật cheat hay chưa
    // ============================================================

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
        // Kiểm tra rơi khỏi bản đồ
        if (transform.position.y < fallDeathY)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TakeDamage(GameManager.Instance.currentHealth);
            else
                Die();
            return;
        }

        // --- Input System mới ---
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Di chuyển trái/phải
        float left = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? -1f : 0f;
        float right = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f;
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

        // Nhảy
        if (keyboard.spaceKey.wasPressedThisFrame && Mathf.Abs(rb.linearVelocity.y) < 0.001f && isGround)
        {
            Jump();
            isGround = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
            isGround = false;
            animator.SetBool("isJumping", true);
        }

        animator.SetBool("isRunning", Mathf.Abs(movement) > 0f);

        // 👇 Gọi hàm kiểm tra cheat code mỗi frame
        HandleCheatInput();
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
    if (invulnerable || cheatModeOn) return; // thêm cheatModeOn

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
        if (invulnerable || cheatModeOn) return; // nếu đang bật GODMODE thì miễn thương

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            TriggerTemporaryInvulnerability(invulnerableTime);
            animator.SetTrigger("Hit");
        }
    }

    // ============================================================
    // >>>>>>>> CHEAT CODE FUNCTIONS (Input System version) <<<<<<<<
    // ============================================================

    private void HandleCheatInput()
{
    var keyboard = Keyboard.current;
    if (keyboard == null) return; // tránh null

    foreach (KeyControl key in keyboard.allKeys)
    {
        if (key == null) continue; // tránh key null
        if (!key.wasPressedThisFrame) continue;

        string keyName = key.displayName;
        if (string.IsNullOrEmpty(keyName)) continue; // bỏ qua phím không có displayName

        keyName = keyName.ToUpper();

        // chỉ chấp nhận A–Z
        if (keyName.Length == 1 && keyName[0] >= 'A' && keyName[0] <= 'Z')
        {
            cheatBuffer += keyName;
            if (cheatBuffer.Length > 20)
                cheatBuffer = cheatBuffer.Substring(cheatBuffer.Length - 20);
        }
    }

    // Xóa ký tự
    if (keyboard.backspaceKey != null && keyboard.backspaceKey.wasPressedThisFrame && cheatBuffer.Length > 0)
        cheatBuffer = cheatBuffer.Substring(0, cheatBuffer.Length - 1);

    // Nhấn Enter
    if ((keyboard.enterKey != null && keyboard.enterKey.wasPressedThisFrame) ||
        (keyboard.numpadEnterKey != null && keyboard.numpadEnterKey.wasPressedThisFrame))
    {
        if (cheatBuffer.Equals(cheatCode, System.StringComparison.OrdinalIgnoreCase))
        {
            ToggleCheatMode();
        }
        else
        {
            Debug.Log("Sai mã cheat: " + cheatBuffer);
        }
        cheatBuffer = "";
    }
}



    private void ToggleCheatMode()
    {
        cheatModeOn = !cheatModeOn;

        if (cheatModeOn)
        {
            invulnerable = true;
            spriteRenderer.color = Color.cyan;
            Debug.Log("🛡️ GODMODE ACTIVATED — Player is now invulnerable!");
        }
        else
        {
            invulnerable = false;
            spriteRenderer.color = Color.white;
            Debug.Log("❌ GODMODE DEACTIVATED — Player can take damage again.");
        }
    }
}
