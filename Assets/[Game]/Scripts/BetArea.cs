using UnityEngine;
using System.Collections.Generic;
using Game.Roulette;


public enum BetAreaType
{
    // Tekli sayılar
    Single_0,
    Single_1,
    Single_2,
    Single_3,
    Single_4,
    Single_5,
    Single_6,
    Single_7,
    Single_8,
    Single_9,
    Single_10,
    Single_11,
    Single_12,
    Single_13,
    Single_14,
    Single_15,
    Single_16,
    Single_17,
    Single_18,
    Single_19,
    Single_20,
    Single_21,
    Single_22,
    Single_23,
    Single_24,
    Single_25,
    Single_26,
    Single_27,
    Single_28,
    Single_29,
    Single_30,
    Single_31,
    Single_32,
    Single_33,
    Single_34,
    Single_35,
    Single_36,

    // Split (yan yana ve alt alta her kombinasyon)
    Split_0_1,
    Split_0_2, // Sıfırdan splits (Avrupa rulet, ister eklersin)
    Split_1_2,
    Split_2_3,
    Split_4_5,
    Split_5_6,
    Split_7_8,
    Split_8_9,
    Split_10_11,
    Split_11_12,
    Split_13_14,
    Split_14_15,
    Split_16_17,
    Split_17_18,
    Split_19_20,
    Split_20_21,
    Split_22_23,
    Split_23_24,
    Split_25_26,
    Split_26_27,
    Split_28_29,
    Split_29_30,
    Split_31_32,
    Split_32_33,
    Split_34_35,
    Split_35_36,

    // Dikey splits
    Split_1_4,
    Split_2_5,
    Split_3_6,
    Split_4_7,
    Split_5_8,
    Split_6_9,
    Split_7_10,
    Split_8_11,
    Split_9_12,
    Split_10_13,
    Split_11_14,
    Split_12_15,
    Split_13_16,
    Split_14_17,
    Split_15_18,
    Split_16_19,
    Split_17_20,
    Split_18_21,
    Split_19_22,
    Split_20_23,
    Split_21_24,
    Split_22_25,
    Split_23_26,
    Split_24_27,
    Split_25_28,
    Split_26_29,
    Split_27_30,
    Split_28_31,
    Split_29_32,
    Split_30_33,
    Split_31_34,
    Split_32_35,
    Split_33_36,

    // Corner (4'lü köşe)
    Corner_1_2_4_5,
    Corner_2_3_5_6,
    Corner_4_5_7_8,
    Corner_5_6_8_9,
    Corner_7_8_10_11,
    Corner_8_9_11_12,
    Corner_10_11_13_14,
    Corner_11_12_14_15,
    Corner_13_14_16_17,
    Corner_14_15_17_18,
    Corner_16_17_19_20,
    Corner_17_18_20_21,
    Corner_19_20_22_23,
    Corner_20_21_23_24,
    Corner_22_23_25_26,
    Corner_23_24_26_27,
    Corner_25_26_28_29,
    Corner_26_27_29_30,
    Corner_28_29_31_32,
    Corner_29_30_32_33,
    Corner_31_32_34_35,
    Corner_32_33_35_36,

    // Street (her satırdaki üçlüler)
    Street_1_2_3,
    Street_4_5_6,
    Street_7_8_9,
    Street_10_11_12,
    Street_13_14_15,
    Street_16_17_18,
    Street_19_20_21,
    Street_22_23_24,
    Street_25_26_27,
    Street_28_29_30,
    Street_31_32_33,
    Street_34_35_36,

    // Line (6'lı iki satır)
    Line_1_2_3_4_5_6,
    Line_4_5_6_7_8_9,
    Line_7_8_9_10_11_12,
    Line_10_11_12_13_14_15,
    Line_13_14_15_16_17_18,
    Line_16_17_18_19_20_21,
    Line_19_20_21_22_23_24,
    Line_22_23_24_25_26_27,
    Line_25_26_27_28_29_30,
    Line_28_29_30_31_32_33,
    Line_31_32_33_34_35_36,

    // Column (sütunlar - 2 to 1)
    Column_1,
    Column_2,
    Column_3,

    // Dozen
    Dozen_1st,
    Dozen_2nd,
    Dozen_3rd,

    // Klasik dış bahisler
    Red,
    Black,
    Odd,
    Even,
    Low,
    High,

    Zero,
    DoubleZero // Amerikan rulet için, Avrupa ruletinde yok
    // Amerikan için DoubleZero da ekleyebilirim!
}


public class BetArea : MonoBehaviour
{
    public BetAreaType areaType;
    public List<Chip> placedChips = new List<Chip>();

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

        BetManager.Instance.PlaceBet(areaType, chip.value);
    }

    public void RemoveChip(Chip chip)
    {
        placedChips.Remove(chip);
    }
    
    public void ClearAllChips()
    {
        foreach (var chip in placedChips)
        {
            if (chip != null)
                Destroy(chip.gameObject);
        }
        placedChips.Clear();
    }
}