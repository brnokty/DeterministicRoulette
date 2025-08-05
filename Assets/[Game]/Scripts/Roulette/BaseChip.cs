using Game.Core;
using UnityEngine;

public class BaseChip : MonoBehaviour
{
    #region INSPECTOR PROPERTIES

    public GameObject chipPrefab;
    public int value;
    public Renderer chipRenderer;
    public int colorMaterialIndex = 1;

    #endregion

    #region PRIVATE PROPERTIES

    private Color originalColor;
    private bool isActive = true;

    #endregion

    #region UNITY METHODS

    void Start()
    {
        if (chipRenderer == null) chipRenderer = GetComponent<Renderer>();
        originalColor = chipRenderer.materials[colorMaterialIndex].color;
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

    #endregion

    #region PUBLIC METHODS

    public void UpdateChipActive(int playerBalance)
    {
        isActive = playerBalance >= value;
        chipRenderer.materials[colorMaterialIndex].color = isActive ? originalColor : Color.gray;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = isActive;
    }

    #endregion
}