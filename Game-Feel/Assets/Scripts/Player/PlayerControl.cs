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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
