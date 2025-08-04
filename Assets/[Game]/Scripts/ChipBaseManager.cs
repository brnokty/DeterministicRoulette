using System;
using UnityEngine;
using System.Collections.Generic;
using Game.Core;

public class ChipBaseManager : MonoBehaviour
{
    public List<BaseChip> baseChips;

    private void Start()
    {
        UpdateAllChips(GameManager.Instance.Balance);
    }

    public void UpdateAllChips(int playerBalance)
    {
        foreach (var chip in baseChips)
        {
            chip.UpdateChipActive(playerBalance);
        }
    }
}
