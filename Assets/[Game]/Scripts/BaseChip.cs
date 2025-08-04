using System;
using Game.Core;
using UnityEngine;

public class BaseChip : MonoBehaviour
{
    public GameObject chipPrefab; // Masa üstünde oluşacak prefab (aynı model)
    public int value;

    [Header("Chip Visuals")]
    public Renderer chipRenderer; // Chip’in Renderer’ı (inspector’dan atayabilirsin)
    private Color originalColor;

    void Start()
    {
        if (chipRenderer == null)
            chipRenderer = GetComponent<Renderer>();
        originalColor = chipRenderer.material.color;
    }

    void OnMouseDown()
    {
        if (!isActive) return; // Pasifse tıklama!
        if (Chip.anyDragging) return;

        GameObject chip = Instantiate(chipPrefab, transform.position, Quaternion.identity);
        chip.GetComponent<Chip>().StartDragging();
    }

    private bool isActive = true;

    // Para güncellendiğinde bu fonksiyonu çağır
    public void UpdateChipActive(int playerBalance)
    {
        isActive = playerBalance >= value;
        // Renk ayarı
        chipRenderer.material.color = isActive ? originalColor : Color.gray; // Veya Color.Lerp(originalColor, Color.gray, 0.6f);
        // Collider kapama/açma
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = isActive;
    }
}