using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float rollForce = 6f;
    public float hurtForce = 5f;

    [Header("Ground Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    private bool wasGroundedLastFrame = false;

    [Header("Attack Settings")]
    public float attackDuration = 0.5f;

    [Header("Game Feel")]
    public GameFeelToggle gameFeel;

    [Header("Particles")]
    public ParticleSystem runningParticle;
    public ParticleSystem hurtParticle;
    public ParticleSystem landingParticle;  


    [Header("Sound")]
    public AudioClip runningSound;
    public AudioClip attackSound;
    public AudioClip hurtSound;

    [Header("HeroKnight Extras")]
    [SerializeField] bool noBlood = false;
    [SerializeField] GameObject slideDust;

    [Header("Health")]
    public int health;


    private Animator animator;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private Sensor_HeroKnight groundSensor;
    private Sensor_HeroKnight wallSensorR1;
    private Sensor_HeroKnight wallSensorR2;
    private Sensor_HeroKnight wallSensorL1;
    private Sensor_HeroKnight wallSensorL2;

    private bool isGrounded = false;
    private bool isHurt = false;
    private bool isRolling = false;
    private bool isWallSliding = false;
    private bool isDead = false;

    private float moveInput;
    private float timeSinceAttack = 0f;
    private float delayToIdle = 0f;
    private float rollDuration = 8f / 14f;
    private float rollCurrentTime = 0f;

    private int facingDirection = 1;
    private int currentAttack = 0;

    private ParticleSystem.MainModule runningMain;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (runningParticle != null)
        {
            runningMain = runningParticle.main;
            runningParticle.Stop();
        }

        Transform sensorParent = transform;
        groundSensor = sensorParent.Find("GroundSensor")?.GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = sensorParent.Find("WallSensor_R1")?.GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = sensorParent.Find("WallSensor_R2")?.GetComponent<Sensor_HeroKnight>();
        wallSensorL1 = sensorParent.Find("WallSensor_L1")?.GetComponent<Sensor_HeroKnight>();
        wallSensorL2 = sensorParent.Find("WallSensor_L2")?.GetComponent<Sensor_HeroKnight>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        timeSinceAttack += Time.deltaTime;
        if (isRolling)
            rollCurrentTime += Time.deltaTime;
        if (rollCurrentTime > rollDuration)
            isRolling = false;

        if (groundSensor != null)
        {
            if (!isGrounded && groundSensor.State())
            {
                isGrounded = true;
                animator.SetBool("Grounded", isGrounded);
                if (!wasGroundedLastFrame && landingParticle != null && gameFeel.IsParticleEnabled())
                {
                    landingParticle.Play();
                }
            }
            if (isGrounded && !groundSensor.State())
            {
                isGrounded = false;
                animator.SetBool("Grounded", isGrounded);
            }
        }
        else
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        wasGroundedLastFrame = isGrounded;

        moveInput = Input.GetAxis("Horizontal");

        if (moveInput > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            facingDirection = 1;
        }
        else if (moveInput < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            facingDirection = -1;
        }

        if (!isRolling && !isHurt)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        animator.SetFloat("AirSpeedY", rb.linearVelocity.y);

        if (wallSensorR1 != null && wallSensorR2 != null && wallSensorL1 != null && wallSensorL2 != null)
        {
            isWallSliding = (wallSensorR1.State() && wallSensorR2.State()) || (wallSensorL1.State() && wallSensorL2.State());
            animator.SetBool("WallSlide", isWallSliding);
        }

        if (Mathf.Abs(moveInput) > Mathf.Epsilon && isGrounded)
        {
            if (runningParticle != null && !runningParticle.isPlaying && gameFeel.IsParticleEnabled())
                runningParticle.Play();
            if (!audioSource.isPlaying && gameFeel.IsSfxEnabled())
            {
                audioSource.clip = runningSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (runningParticle != null && runningParticle.isPlaying)
                runningParticle.Stop();
            if (audioSource.isPlaying && audioSource.clip == runningSound)
                audioSource.Stop();
        }


        // Dead
        if (health <= 0 && !isRolling && !isDead)
        {
            isDead = true;
            StartCoroutine(PlayerDeath());
            return;
        }
        // hurt
        else if (!isRolling && isHurt)
        {
            animator.SetTrigger("Hurt");
            GetHurt(null);
        }
        else if (Input.GetKeyDown("f") && timeSinceAttack > 0.25f && !isRolling)
        {
            currentAttack++;
            if (currentAttack > 3)
                currentAttack = 1;
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;
            animator.SetTrigger("Attack" + currentAttack);
            timeSinceAttack = 0f;
            if (gameFeel.IsSfxEnabled() && !audioSource.isPlaying)
            {
                audioSource.clip = attackSound;
                audioSource.loop = false;
                audioSource.Play();
            }
            
        }
        else if (Input.GetMouseButtonDown(1) && !isRolling)
        {
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("IdleBlock", false);
        }
        else if (Input.GetKeyDown("left shift") && !isRolling && !isWallSliding)
        {
            isRolling = true;
            animator.SetTrigger("Roll");
            rb.linearVelocity = new Vector2(facingDirection * rollForce, rb.linearVelocity.y);
        }
        else if (Input.GetKeyDown("space") && isGrounded && !isRolling)
        {
            animator.SetTrigger("Jump");
            isGrounded = false;
            animator.SetBool("Grounded", isGrounded);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (groundSensor != null)
                groundSensor.Disable(0.2f);
        }
        else if (Mathf.Abs(moveInput) > Mathf.Epsilon)
        {
            delayToIdle = 0.05f;
            animator.SetInteger("AnimState", 1);
        }
        else
        {
            delayToIdle -= Time.deltaTime;
            if (delayToIdle < 0)
                animator.SetInteger("AnimState", 0);
        }
    }

    void FixedUpdate() { }

    public void GetHurt(Transform attacker)
    {
        // if (isHurt)
        //     return;
        // isHurt = true;
        if (gameFeel.IsHitstopEnabled())
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 hurtDirection = (attacker != null) ? (transform.position - attacker.position).normalized : new Vector2(-facingDirection, 1).normalized;
            rb.AddForce(hurtDirection * hurtForce, ForceMode2D.Impulse);
        }
        if (gameFeel.IsScreenShakeEnabled())
            gameFeel.ShakeCamera();
        if (gameFeel.IsFlashEnabled())
            gameFeel.FlashScreenRed();
        if (hurtParticle != null && gameFeel.IsParticleEnabled())
            hurtParticle.Play();
        if (gameFeel.IsSfxEnabled() && !audioSource.isPlaying)
        {
            audioSource.clip = hurtSound;
            audioSource.loop = false;
            audioSource.Play();
            // Debug.Log("hurt sound");
        }
        StartCoroutine(ResetHurt());
    }

    private IEnumerator ResetHurt()
    {
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    void AE_SlideDust()
    {
        Vector3 spawnPosition = Vector3.zero;
        if (facingDirection == 1 && wallSensorR2 != null)
            spawnPosition = wallSensorR2.transform.position;
        else if (facingDirection == -1 && wallSensorL2 != null)
            spawnPosition = wallSensorL2.transform.position;
        if (slideDust != null)
        {
            GameObject dust = Instantiate(slideDust, spawnPosition, transform.rotation);
            dust.transform.localScale = new Vector3(facingDirection, 1, 1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            health -= 1;
            Debug.Log("Player hit by enemy. Health: " + health);

            isHurt = true;
        }
        }
    private IEnumerator PlayerDeath()
    {
        if (gameFeel.IsScreenShakeEnabled())
            gameFeel.ShakeCamera();

        if (gameFeel.IsFlashEnabled())
            gameFeel.FlashScreenRed();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
                enemyScript.enabled = false;
        }

        animator.SetBool("noBlood", noBlood);
        animator.SetTrigger("Death");

        yield return new WaitForSeconds(2f); 
        Destroy(gameObject);  
    }

}
