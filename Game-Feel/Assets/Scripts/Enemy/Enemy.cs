using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("platform")]
    public Collider2D platformCollider;

    private float leftBoundary;
    private float rightBoundary;

    [Header("patrol")]
    public float patrolSpeed = 2f;

    [Header("attack")]
    public float attackJumpForce = 5f;
    public float attackDiveForce = 10f;
    public float attackDelay = 0.5f;
    public float detectionRange = 10f;
    public LayerMask detectionLayer;

    [Header("death")]
    public int requiredHitCount = 3;

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool playerInSight = false;
    private Transform player;
    private int hitCount = 0;

    private float currentPatrolSpeed;
    private int facingDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPatrolSpeed = Mathf.Abs(patrolSpeed);
        facingDirection = 1;
        if (platformCollider != null)
        {
            leftBoundary = platformCollider.bounds.min.x;
            rightBoundary = platformCollider.bounds.max.x;
        }
        else
        {
            Debug.LogWarning("Platform Collider unassigned");
        }
    }

    void Update()
    {
        if (rb == null)
            return;

        DetectPlayer();

        if (!isAttacking && playerInSight)
            StartCoroutine(AttackPlayer());
        else if (!isAttacking && !playerInSight)
            Patrol();
    }

    void Patrol()
    {
        if (transform.position.x <= leftBoundary)
        {
            currentPatrolSpeed = Mathf.Abs(patrolSpeed);
            facingDirection = 1;
        }
        else if (transform.position.x >= rightBoundary)
        {
            currentPatrolSpeed = -Mathf.Abs(patrolSpeed);
            facingDirection = -1;
        }
        rb.linearVelocity = new Vector2(currentPatrolSpeed, rb.linearVelocity.y);
    }

    void DetectPlayer()
    {
        Vector2 origin = transform.position;
        Vector2 direction = new Vector2(facingDirection, 0);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, detectionRange, detectionLayer);
        Debug.DrawRay(origin, direction * detectionRange, Color.red);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            playerInSight = true;
            player = hit.collider.transform;
        }
        else
        {
            playerInSight = false;
            player = null;
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * attackJumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(attackDelay);
        if (playerInSight && player != null)
        {
            Vector2 attackDir = (player.position - transform.position).normalized;
            if (attackDir.y > 0)
                attackDir.y = -Mathf.Abs(attackDir.y);
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(attackDir * attackDiveForce, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    public void TakeHit()
    {
        if (hitCount >= requiredHitCount)
            return;
        hitCount++;
        if (hitCount >= requiredHitCount)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

}
