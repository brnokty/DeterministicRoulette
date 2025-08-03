using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class DeterministicOutcomeSelector : MonoBehaviour
    {
        public TMP_InputField inputField; // veya Dropdown ile de yapılabilir

        public string GetSelectedNumber()
        {
            // if (int.TryParse(, out int num))
            // {
            //     if (num >= 0 && num <= 36)
            //         return num;
            // }
            // return -1; // random üret

            return inputField.text;
        }
    }
}