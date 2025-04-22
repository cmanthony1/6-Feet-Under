using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Player player;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>(); // Reference Player script
    }

    void Update()
    {
        if (player == null) return;

        float move = Mathf.Abs(Input.GetAxis("Horizontal"));
        bool isCrouching = player.IsCrouching();

        bool isWalking = move > 0.01f;

        // Handle Animator bools
        animator.SetBool("IsWalking", isWalking && !isCrouching);
        animator.SetBool("IsCrouch", isCrouching);

        // Optional: If you want only one bool active at a time
        if (isWalking && !isCrouching)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsCrouch", false);
        }
        else if (isCrouching)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsCrouch", true);
        }
        else
        {
            // Player is idle
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsCrouch", false);
        }
    }
}