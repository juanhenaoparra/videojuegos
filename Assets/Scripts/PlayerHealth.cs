using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int currentHP;

    [Header("Visual Feedback")]
    [SerializeField] private HitFlashEffect hitFlashEffect;

    private void Start()
    {
        currentHP = maxHP;
        Debug.Log($"Player Health initialized: {currentHP}/{maxHP} HP");

        // Auto-find HitFlashEffect if not assigned
        if (hitFlashEffect == null)
        {
            hitFlashEffect = GetComponent<HitFlashEffect>();
            if (hitFlashEffect == null)
            {
                Debug.LogWarning("PlayerHealth: No HitFlashEffect found. Visual feedback will not work.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP); // Ensure HP doesn't go below 0

        Debug.Log($"Player took {damage} damage! Current HP: {currentHP}/{maxHP}");

        // Trigger visual feedback
        if (hitFlashEffect != null)
        {
            hitFlashEffect.Flash();
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Add death logic here (restart level, game over, etc.)
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP); // Cap at max HP
        Debug.Log($"Player healed {amount} HP! Current HP: {currentHP}/{maxHP}");
    }
}