using UnityEngine;
using System.Collections.Generic;

public class ChipBaseManager : MonoBehaviour
{
    [System.Serializable]
    public class ChipEntry
    {
        public int value;
        public GameObject prefab;
        public Transform baseSpot; // Masadaki konumu (BaseArea üstündeki pozisyonlar)
    }

    public List<ChipEntry> chipEntries;
    public int playerMoney = 900;

    void Start()
    {
        UpdateBaseChips();
    }

    public void UpdateBaseChips()
    {
        foreach(var entry in chipEntries)
        {
            bool canShow = playerMoney >= entry.value;
            entry.prefab.SetActive(canShow);
            // İstersen baseSpot konumuna prefabı yerleştir:
            if (canShow && entry.baseSpot != null)
            {
                entry.prefab.transform.position = entry.baseSpot.position;
            }
        }
    }

    public void SetMoney(int money)
    {
        playerMoney = money;
        UpdateBaseChips();
    }
}