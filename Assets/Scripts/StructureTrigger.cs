using UnityEngine;

public class StructureTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private float triggerDistance = 5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Arrow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int arrowCount = 3;
    [SerializeField] private float arrowSpawnHeight = 1.5f;
    [SerializeField] private float spreadAngle = 15f; // Angle between arrows

    [Header("Cooldown")]
    [SerializeField] private float cooldownTime = 3f;

    private Transform playerTransform;
    private bool canShoot = true;
    private float cooldownTimer = 0f;

    private void Update()
    {
        // Update cooldown timer
        if (!canShoot)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canShoot = true;
            }
        }

        // Find player if not already found
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            return;
        }

        // Check distance to player
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distance <= triggerDistance && canShoot)
        {
            ShootArrows();
        }
    }

    private void ShootArrows()
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning($"Arrow prefab not assigned on {gameObject.name}!");
            return;
        }

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + Vector3.up * arrowSpawnHeight;

        Debug.Log($"{gameObject.name} shooting {arrowCount} arrows at player!");

        // Calculate spread for multiple arrows
        for (int i = 0; i < arrowCount; i++)
        {
            // Calculate angle offset for this arrow
            float angleOffset = 0f;
            if (arrowCount > 1)
            {
                // Distribute arrows evenly across the spread angle
                float step = spreadAngle / (arrowCount - 1);
                angleOffset = -spreadAngle / 2f + (step * i);
            }

            // Rotate direction by angle offset
            Quaternion rotation = Quaternion.Euler(0f, angleOffset, 0f);
            Vector3 arrowDirection = rotation * directionToPlayer;

            // Instantiate arrow
            GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
            Arrow arrowScript = arrow.GetComponent<Arrow>();
            
            if (arrowScript != null)
            {
                arrowScript.SetDirection(arrowDirection);
                arrowScript.SetSpawner(gameObject); // Tell arrow who spawned it
            }
            else
            {
                Debug.LogWarning("Arrow prefab doesn't have Arrow script attached!");
            }
        }

        // Start cooldown
        canShoot = false;
        cooldownTimer = cooldownTime;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize trigger distance in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}