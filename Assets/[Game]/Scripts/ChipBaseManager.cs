using UnityEngine;
using System.Collections.Generic;

public class ChipBaseManager : MonoBehaviour
{
    public static ChipBaseManager Instance { get; private set; }
    public List<BaseChip> baseChips;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateAllChips(int playerBalance)
    {
        foreach (var chip in baseChips)
            chip.UpdateChipActive(playerBalance);
    }
}