using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        
        public static UIManager Instance { get; private set; }
        public DeterministicOutcomeSelector deterministicSelector;
        public StatPanel statPanel;
        public TMP_Text balanceText;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button resetButton;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // UI panel setup
            statPanel.UpdateStats();
            spinButton.onClick.AddListener(OnSpinButtonClicked);
        }

        public void OnSpinButtonClicked()
        {
            string selectedNumber = deterministicSelector.GetSelectedNumber();
            if (selectedNumber == "")
            {
                Core.GameManager.Instance.rouletteManager.Spin("");
                return;
            }

            Core.GameManager.Instance.rouletteManager.Spin(selectedNumber);
        }

        public Text resultText;

        public void ShowResult(string number)
        {
            // Kırmızı/Siyah/Yeşil kontrolü için dizinle renk bul
            string color = GetColorForNumber(number); // Kendi fonksiyonunu yaz!
            resultText.text = $"Kazanan: <color={color}>{number}</color>";
        }


        public string GetColorForNumber(string number)
        {
            if (number == "0")
                return "green";

            // Avrupa ruletinde kırmızı olan sayılar
            string[] redNumbers =
            {
                "1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36"
            };
            if (System.Array.IndexOf(redNumbers, number) >= 0)
                return "red";
            else
                return "black";
        }
        
        public void UpdateBalance(float newBalance)
        {
            balanceText.text = $"Balance: {newBalance.ToString("F2")}$";
        }

        public void OnresetButtonClicked()
        {
            
        }
    }
}