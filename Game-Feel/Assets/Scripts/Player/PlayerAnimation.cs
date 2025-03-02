using UnityEngine;
using System.Collections;


public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerControl playerControl;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerControl = GetComponent<PlayerControl>();
    }

    public void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        animator.SetFloat("veloctiyX", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("veloctiyy", Mathf.Abs(rb.linearVelocity.y));
        animator.SetBool("isGround", playerControl.isGround);
        animator.SetBool("isAttack", playerControl.isAttack);
    }
}
