using UnityEngine;

public class MoveToTarget2D : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private Transform target;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 direction;
        if (target == null)
            direction = Vector2.right;
        else
            direction = ((Vector2)(target.position - transform.position)).normalized;

        // ===== TODO DLC B: The outsourced code is missing a critical time parameter =====
        // causing movement speed to vary wildly depending on computer performance.
        // Find the bug and fix it!
        rb.MovePosition(rb.position + direction * speed);
    }
}
