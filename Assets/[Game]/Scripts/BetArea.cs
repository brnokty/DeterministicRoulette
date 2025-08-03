using UnityEngine;
using System.Collections.Generic;

public class BetArea : MonoBehaviour
{
    public float snapRange = 0.8f; // Chip bu kadar yakınsa snaplesin
    private List<Chip> placedChips = new List<Chip>();

    public Vector3 GetSnapPosition()
    {
        // Kendi merkezine biraz yukarıda stack olacak şekilde
        return transform.position + Vector3.up * (0.2f + 0.1f * placedChips.Count);
    }

    public void AddChip(Chip chip)
    {
        placedChips.Add(chip);
        // Chip'in parent'ı bu BetArea olur (istersen)
        chip.transform.parent = transform;
    }
}