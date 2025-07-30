using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        public DeterministicOutcomeSelector deterministicSelector;
        public StatPanel statPanel;
        [SerializeField] private Button spinButton;


        private void Start()
        {
            // UI panel setup
            statPanel.UpdateStats();
            spinButton.onClick.AddListener(OnSpinButtonClicked);
        }

        public void OnSpinButtonClicked()
        {
            int selectedNumber = deterministicSelector.GetSelectedNumber();
            Game.Core.GameManager.Instance.rouletteManager.Spin(selectedNumber);
        }
        
        public Text resultText;

        public void ShowResult(int number)
        {
            // Kırmızı/Siyah/Yeşil kontrolü için dizinle renk bul
            string color = GetColorForNumber(number); // Kendi fonksiyonunu yaz!
            resultText.text = $"Kazanan: <color={color}>{number}</color>";
        }

        
        public string GetColorForNumber(int number)
        {
            if (number == 0)
                return "green";

            // Avrupa ruletinde kırmızı olan sayılar
            int[] redNumbers = {1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36};
            if (System.Array.IndexOf(redNumbers, number) >= 0)
                return "red";
            else
                return "black";
        }

    }
}