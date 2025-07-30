using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class DeterministicOutcomeSelector : MonoBehaviour
    {
        public InputField inputField; // veya Dropdown ile de yapılabilir

        public int GetSelectedNumber()
        {
            if (int.TryParse(inputField.text, out int num))
            {
                if (num >= 0 && num <= 36)
                    return num;
            }
            return -1; // random üret
        }
    }
}