using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    
    void Awake()
    {
        animator = GetComponent<Animator>();    
    }

    public void SetMove(float value)
    {
        animator.SetFloat("MovementX", value);
    }

    public void IsCrouching(bool state)
    {
        animator.SetBool("IsCrouching", state);
    }

    public void IsJumping(bool state)
    {
        animator.SetBool("IsJumping", state);
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void Hurt()
    {
        animator.SetTrigger("Hurt");
    }
}
