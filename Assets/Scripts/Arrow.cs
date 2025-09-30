using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Arrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody rb;
    private bool hasHit = false;
    private GameObject spawner; // Structure that spawned this arrow

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        // Destroy arrow after lifetime expires
        Destroy(gameObject, lifetime);
        Debug.Log("Arrow spawned!");
    }

    public void SetDirection(Vector3 dir)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        
        Vector3 direction = dir.normalized;
        rb.linearVelocity = direction * speed;
        
        // Rotate arrow to face the direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void SetSpawner(GameObject spawnerObject)
    {
        spawner = spawnerObject;
        // Ignore collision with spawner
        if (spawner != null)
        {
            Collider spawnerCollider = spawner.GetComponent<Collider>();
            if (spawnerCollider != null)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), spawnerCollider);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        // Ignore other arrows and spawner
        if (other.CompareTag("Arrow") || other.gameObject == spawner) return;
        
        Debug.Log($"Arrow triggered with: {other.gameObject.name}");
        HandleCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        
        // Ignore other arrows and spawner
        if (collision.gameObject.CompareTag("Arrow") || collision.gameObject == spawner) return;
        
        Debug.Log($"Arrow collided with: {collision.gameObject.name}");
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject hitObject)
    {
        hasHit = true;
        
        // Check if arrow hit the player (check the object and its parent)
        PlayerHealth playerHealth = hitObject.GetComponent<PlayerHealth>();
        
        // If not found, check parent objects
        if (playerHealth == null)
        {
            playerHealth = hitObject.GetComponentInParent<PlayerHealth>();
        }
        
        // If still not found, check children
        if (playerHealth == null)
        {
            playerHealth = hitObject.GetComponentInChildren<PlayerHealth>();
        }
        
        if (playerHealth != null)
        {
            Debug.Log($"Arrow hit player! Dealing {damage} damage.");
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Destroy arrow if it hits something else
        Debug.Log($"Arrow hit something else: {hitObject.name}, destroying...");
        Destroy(gameObject);
    }
}