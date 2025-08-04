using Game.Core;
using UnityEngine;

public class BaseChip : MonoBehaviour
{
    public GameObject chipPrefab;
    public int value;
    public Renderer chipRenderer;

    private Color originalColor;
    private bool isActive = true;

    void Start()
    {
        if (chipRenderer == null) chipRenderer = GetComponent<Renderer>();
        originalColor = chipRenderer.material.color;
        UpdateChipActive(GameManager.Instance.Balance);
    }

    void OnMouseDown()
    {
        if (!isActive) return;
        if (Chip.anyDragging) return;

        GameObject chip = Instantiate(chipPrefab, transform.position, Quaternion.identity);
        chip.GetComponent<Chip>().Init(value);
        chip.GetComponent<Chip>().StartDragging();
    }

    public void UpdateChipActive(int playerBalance)
    {
        isActive = playerBalance >= value;
        chipRenderer.material.color = isActive ? originalColor : Color.gray;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = isActive;
    }
}