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
            return inputField.text;
        }
    }
}