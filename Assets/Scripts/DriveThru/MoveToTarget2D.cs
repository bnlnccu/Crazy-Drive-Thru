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

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }
}
