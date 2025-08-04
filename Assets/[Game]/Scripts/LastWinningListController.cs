using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastWinningListController : MonoBehaviour
{
    public RectTransform content;        // Content objesi
    public WinNumberItem winNumberPrefab; // Prefab
    public int maxHistoryCount = 20;     // Gösterilecek maksimum sayı

    private List<WinNumberItem> items = new List<WinNumberItem>();

    // Kazanan sayı eklenecek fonksiyon
    public void AddWinNumber(string number, Color color)
    {
        // Yeni prefab instantiate et
        WinNumberItem newItem = Instantiate(winNumberPrefab, content);
        newItem.transform.SetAsFirstSibling(); // En sola ekle
        newItem.SetNumber(number, color);

        items.Insert(0, newItem);

        // Max limiti geçerse en sonu sil
        if (items.Count > maxHistoryCount)
        {
            Destroy(items[items.Count - 1].gameObject);
            items.RemoveAt(items.Count - 1);
        }
    }
}
