using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float hurtForce = 5f;

    // ground check
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    // attack
    public float attackDuration = 0.5f;

    private Rigidbody2D rb;
    public bool isGround = false;
    public bool isAttack = false;
    private bool isHurt = false;
    private float moveInput;

    public GameFeelToggle gameFeel;

    //particles
    public ParticleSystem runningParticle;   
    public ParticleSystem hurtParticle; 
    private ParticleSystem.MainModule runningMain;

    //sound
    [SerializeField] private AudioClip runningSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    private AudioSource audioSource; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (runningParticle != null)
            runningMain = runningParticle.main;
        if (runningParticle != null)
            runningParticle.Stop();
    }

    private void Update()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        moveInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && isGround)
        {
            Jump();
        }

        if (Input.GetButtonDown("Attack"))
        {
            Attack();
        }

        if (Mathf.Abs(moveInput) > 0 && isGround)
        {
            if (!runningParticle.isPlaying && gameFeel.IsParticleEnabled())
            {
                runningParticle.Play();
            }

            if (!audioSource.isPlaying && gameFeel.IsSfxEnabled())  // Check if sound effects are enabled
            {
                audioSource.clip = runningSound;  // Assign the running sound clip
                audioSource.loop = true;  // Loop the sound
                audioSource.Play();  // Play the running sound
            }
        }
        else
        {
            if (runningParticle.isPlaying)
            {
                runningParticle.Stop();
            }


            if (audioSource.isPlaying && audioSource.clip == runningSound)
            {
                audioSource.Stop(); 
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isHurt)
        {
            Move();
        }
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Attack()
    {
        if (isAttack)
            return;
        
        if (gameFeel.IsSfxEnabled() && !audioSource.isPlaying)
        {
            audioSource.clip = attackSound;  
            audioSource.loop = false;  
            audioSource.Play(); 
            Debug.Log("played attack sound");
        }

        isAttack = true;
        Debug.Log("attack");
        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttack = false;
    }

    public void GetHurt(Transform attacker)
    {
        if (isHurt)
            return;

        isHurt = true;
        rb.linearVelocity = Vector2.zero;
        Vector2 hurtDirection = (transform.position - attacker.position).normalized;
        rb.AddForce(hurtDirection * hurtForce, ForceMode2D.Impulse);

        //gameFeel stuff
        if (gameFeel.IsScreenShakeEnabled())
        {
            gameFeel.ShakeCamera();
            Debug.Log("screen shake");
        }

        if (gameFeel.IsFlashEnabled())
        {
            gameFeel.FlashScreenRed();
            Debug.Log("red flash");
        }

        if (hurtParticle != null && gameFeel.IsParticleEnabled())
        {
            hurtParticle.Play();
            Debug.Log("hurt particle");
        }

        if (gameFeel.IsSfxEnabled() && !audioSource.isPlaying)
        {
            audioSource.clip = hurtSound;
            audioSource.loop = false;  
            audioSource.Play(); 
        }

        Debug.Log("hurt");
        StartCoroutine(ResetHurt());
    }

    private IEnumerator ResetHurt()
    {
        // duration 1 second
        yield return new WaitForSeconds(1f);
        isHurt = false;
    }
}
