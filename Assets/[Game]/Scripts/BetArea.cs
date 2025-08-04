using UnityEngine;
using System.Collections.Generic;

public class BetArea : MonoBehaviour
{
    // [HideInInspector] public float snapRange = 0.000005f;
    private List<Chip> placedChips = new List<Chip>();

    public int GetChipCount()
    {
        // Masanın üstündeki kaç chip burada?
        return placedChips.Count;
    }

    public Vector3 GetSnapPosition()
    {
        // Sadece "chip stack" için kullanılabilir, chip eklenmeden önce stack yükseklik hesaplanacaksa
        return transform.position + Vector3.up * (0.04f + 0.008f * placedChips.Count);
    }

    public void AddChip(Chip chip)
    {
        placedChips.Add(chip);
        chip.transform.parent = transform;
    }

    public void RemoveChip(Chip chip)
    {
        placedChips.Remove(chip);
    }
}