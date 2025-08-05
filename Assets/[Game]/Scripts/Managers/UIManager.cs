using Game.Core;
using Game.Roulette;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Singleton

        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        #endregion

        #region INSPECTOR PROPERTIES

        [SerializeField] private DeterministicOutcomeSelector deterministicSelector;
        [SerializeField] private StatPanel statPanel;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private Image resultImage;
        [SerializeField] private LastWinningListController lastWinningListController;
        public YouWonPopUp youWonPopUp;

        #endregion

        #region UNITY METHODS

        private void Start()
        {
            // UI panel setup
            statPanel.UpdateStats();
            UpdateBalance(GameManager.Instance.Balance);
            spinButton.onClick.AddListener(OnSpinButtonClicked);
            resetButton.onClick.AddListener(OnResetButtonClicked);
        }

        #endregion

        #region PUBLIC METHODS

        public void OnSpinButtonClicked()
        {
            string selectedNumber = deterministicSelector.GetSelectedNumber();
            RouletteManager.Instance.Spin(selectedNumber);
        }


        public void ShowResult(string number)
        {
            Color color = GetColorForNumber(number);
            resultText.text = number;
            resultImage.color = color;
            lastWinningListController.AddWinNumber(number, color);
        }


        public Color GetColorForNumber(string number)
        {
            if (number == "0" || number == "00")
                return Color.green;

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

        public void OnResetButtonClicked()
        {
            RouletteManager.Instance.ResetTable();
            // UpdateBalance(GameManager.Instance.Balance);
            resultText.text = "...";
            // resultImage.color = Color.clear;
            deterministicSelector.inputField.text = "";
            youWonPopUp.Disappear();
        }

        #endregion
    }
}