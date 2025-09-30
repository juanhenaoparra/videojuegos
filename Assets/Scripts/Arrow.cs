using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    private Vector3 direction;

    private void Start()
    {
        // Destroy arrow after lifetime expires
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move arrow in its direction
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        // Rotate arrow to face the direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if arrow hit the player
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Destroy(gameObject); // Destroy arrow on impact
        }
        // Destroy arrow if it hits something else (walls, ground, etc.)
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}