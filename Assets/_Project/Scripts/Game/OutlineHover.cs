using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OutlineHover : MonoBehaviour
{
    [Header("Outline Settings")]
    public Material outlineMaterial;
    private Material originalMaterial;

    public SpriteRenderer spriteRenderer;

    void Start()
    {
        originalMaterial = spriteRenderer.material;
    }

    void OnMouseEnter()
    {
        if (outlineMaterial != null)
            spriteRenderer.material = outlineMaterial;
    }

    void OnMouseExit()
    {
        spriteRenderer.material = originalMaterial;
    }
}
