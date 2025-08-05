using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinNumberItem : MonoBehaviour
{
    #region INSPECTOR PROPERTIES

    [SerializeField] private Image bgImage;
    [SerializeField] private TMP_Text numberText;

    #endregion

    #region PUBLIC METHODS

    public void SetNumber(string num, Color color)
    {
        numberText.text = num;
        bgImage.color = color;
    }

    #endregion
}