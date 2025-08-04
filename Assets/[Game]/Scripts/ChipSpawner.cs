using System;
using UnityEngine;

public class ChipSpawner : MonoBehaviour
{
    public GameObject chipPrefab; // Masa üstünde oluşacak prefab (aynı model)
    public int value;

    void OnMouseDown()
    {
        if (Chip.anyDragging) return;

        GameObject chip = Instantiate(chipPrefab, transform.position, Quaternion.identity);
        chip.GetComponent<Chip>().StartDragging();
    }
}