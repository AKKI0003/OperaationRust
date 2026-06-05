using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        if (movement != Vector2.zero)
        {
            lastDirection = movement;
        }

        animator.SetFloat("MoveX", lastDirection.x);
        animator.SetFloat("MoveY", lastDirection.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            movement *
            moveSpeed *
            Time.fixedDeltaTime
        );
    }
}