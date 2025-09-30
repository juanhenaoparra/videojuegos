using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitFlashEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private AnimationCurve flashCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine flashCoroutine;
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    // Shader property IDs for URP (works with Universal Render Pipeline)
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        // Get all renderers on this object and children
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"HitFlashEffect on {gameObject.name}: No renderers found!");
        }

        // Store original colors
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && renderer.sharedMaterial != null)
            {
                // Try to get base color (URP uses _BaseColor, Built-in uses _Color)
                Color originalColor = renderer.sharedMaterial.HasProperty(BaseColorID)
                    ? renderer.sharedMaterial.GetColor(BaseColorID)
                    : renderer.sharedMaterial.GetColor(ColorID);
                originalColors[renderer] = originalColor;
            }
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
                if (renderer != null && originalColors.ContainsKey(renderer))
                {
                    // Blend between original color and flash color
                    Color blendedColor = Color.Lerp(originalColors[renderer], flashColor, flashAmount);

                    renderer.GetPropertyBlock(propertyBlock);

                    // Set color for both URP and Built-in render pipeline
                    if (renderer.sharedMaterial.HasProperty(BaseColorID))
                    {
                        propertyBlock.SetColor(BaseColorID, blendedColor);
                    }
                    if (renderer.sharedMaterial.HasProperty(ColorID))
                    {
                        propertyBlock.SetColor(ColorID, blendedColor);
                    }

                    renderer.SetPropertyBlock(propertyBlock);
                }
            }

            yield return null;
        }

        // Restore original colors
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && originalColors.ContainsKey(renderer))
            {
                renderer.GetPropertyBlock(propertyBlock);

                if (renderer.sharedMaterial.HasProperty(BaseColorID))
                {
                    propertyBlock.SetColor(BaseColorID, originalColors[renderer]);
                }
                if (renderer.sharedMaterial.HasProperty(ColorID))
                {
                    propertyBlock.SetColor(ColorID, originalColors[renderer]);
                }

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