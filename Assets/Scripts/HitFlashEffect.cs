using UnityEngine;
using System.Collections;

public class HitFlashEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private AnimationCurve flashCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine flashCoroutine;

    // Shader property IDs for better performance
    private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");

    private void Awake()
    {
        // Get all renderers on this object and children
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"HitFlashEffect on {gameObject.name}: No renderers found!");
        }
    }

    /// <summary>
    /// Trigger the flash effect
    /// </summary>
    public void Flash()
    {
        // Stop any ongoing flash
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    /// <summary>
    /// Trigger flash with custom color
    /// </summary>
    public void Flash(Color customColor)
    {
        Color originalColor = flashColor;
        flashColor = customColor;
        Flash();
        flashColor = originalColor;
    }

    private IEnumerator FlashCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / flashDuration;
            float flashAmount = flashCurve.Evaluate(normalizedTime);

            // Apply to all renderers
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    renderer.GetPropertyBlock(propertyBlock);
                    propertyBlock.SetColor(FlashColorID, flashColor);
                    propertyBlock.SetFloat(FlashAmountID, flashAmount);
                    renderer.SetPropertyBlock(propertyBlock);
                }
            }

            yield return null;
        }

        // Ensure flash is completely off
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(FlashAmountID, 0f);
                renderer.SetPropertyBlock(propertyBlock);
            }
        }

        flashCoroutine = null;
    }

    private void OnDestroy()
    {
        // Clean up - reset all materials
        if (renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    renderer.SetPropertyBlock(null);
                }
            }
        }
    }
}