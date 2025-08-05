using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class DeterministicOutcomeSelector : MonoBehaviour
    {
        #region INSPECTOR PROPERTIES

        public TMP_InputField inputField;

        #endregion

        #region PUBLIC METHODS

        public string GetSelectedNumber()
        {
            return inputField.text;
        }

        #endregion
    }
}