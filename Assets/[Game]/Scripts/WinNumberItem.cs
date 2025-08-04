using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinNumberItem : MonoBehaviour
{
    public Image bgImage;
    public TMP_Text numberText;

    public void SetNumber(string num, Color color)
    {
        numberText.text = num;
        bgImage.color = color;
    }
}