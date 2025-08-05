using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastWinningListController : MonoBehaviour
{
    #region INSPECTOR PROPERTIES

    public RectTransform content;
    public WinNumberItem winNumberPrefab;
    public int maxHistoryCount = 20;

    #endregion

    #region PRIVATE PROPERTIES

    private List<WinNumberItem> items = new List<WinNumberItem>();

    #endregion

    #region PUBLIC METHODS

    public void AddWinNumber(string number, Color color)
    {
        WinNumberItem newItem = Instantiate(winNumberPrefab, content);
        newItem.transform.SetAsFirstSibling();
        newItem.SetNumber(number, color);

        items.Insert(0, newItem);


        if (items.Count > maxHistoryCount)
        {
            Destroy(items[items.Count - 1].gameObject);
            items.RemoveAt(items.Count - 1);
        }
    }

    #endregion
}