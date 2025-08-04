using Game.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [SerializeField] private DeterministicOutcomeSelector deterministicSelector;
        [SerializeField] private StatPanel statPanel;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private Image resultImage;
        [SerializeField] private LastWinningListController lastWinningListController;

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
            UpdateBalance(GameManager.Instance.Balance);
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


        public void ShowResult(string number)
        {
            // Kırmızı/Siyah/Yeşil kontrolü için dizinle renk bul
            Color color = GetColorForNumber(number); // Kendi fonksiyonunu yaz!
            resultText.text = number;
            resultImage.color = color;
            lastWinningListController.AddWinNumber(number, color);
        }


        public Color GetColorForNumber(string number)
        {
            if (number == "0")
                return Color.green;

            // Avrupa ruletinde kırmızı olan sayılar
            string[] redNumbers =
            {
                "1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36"
            };
            if (System.Array.IndexOf(redNumbers, number) >= 0)
                return Color.red;
            else
                return Color.black;
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