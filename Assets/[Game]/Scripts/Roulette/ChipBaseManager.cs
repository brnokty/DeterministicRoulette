using UnityEngine;
using System.Collections.Generic;

public class ChipBaseManager : MonoBehaviour
{
    #region Singleton

    public static ChipBaseManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    #region INSPECTOR PROPERTIES

    public List<BaseChip> baseChips;

    #endregion

    #region UNITY METHODS

    public void UpdateAllChips(int playerBalance)
    {
        foreach (var chip in baseChips)
            chip.UpdateChipActive(playerBalance);
    }

    #endregion
}