using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stopDistance = 1f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    public void MoveToPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        isMoving = true;

        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
    }

    private void MoveTowardsTarget()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > stopDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            // Поворот персонажа в сторону движения
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            StopMoving();
        }
    }

    public void StopMoving()
    {
        isMoving = false;
        rb.velocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public Vector3 GetCurrentPosition()
    {
        return transform.position;
    }
}
