using Game.UI;
using UnityEngine;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        public Roulette.RouletteManager rouletteManager;
        public StatisticsManager statisticsManager;
        public SaveManager saveManager;
        public SoundManager soundManager;

        [Header("Player Data")]
        [SerializeField] private int balance = 1000;
        private int preBetBalance;

        public int Balance
        {
            get => balance;
            private set
            {
                balance = value;
                UIManager.Instance?.UpdateBalance(balance);
            }
        }
        public int PreBetBalance => preBetBalance;

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

            if (!rouletteManager) rouletteManager = FindObjectOfType<Roulette.RouletteManager>();
            if (!statisticsManager) statisticsManager = FindObjectOfType<StatisticsManager>();
            if (!saveManager) saveManager = FindObjectOfType<SaveManager>();
            if (!soundManager) soundManager = FindObjectOfType<SoundManager>();
        }

        private void Start()
        {
            saveManager?.LoadGame();
        }

        public void NewGame()
        {
            statisticsManager.ResetStats();
            rouletteManager.ResetTable();
            SetBalance(1000);
        }

        public void OnApplicationQuit()
        {
            saveManager?.SaveGame();
        }

        public void SetBalance(int amount)
        {
            Balance = amount;
            ChipBaseManager.Instance?.UpdateAllChips(balance);
            UIManager.Instance?.UpdateBalance(balance);
        }

        public void AddMoney(int amount)
        {
            Balance += amount;
            ChipBaseManager.Instance?.UpdateAllChips(balance);
            UIManager.Instance?.UpdateBalance(balance);
        }

        public bool SpendMoney(int amount)
        {
            if (balance >= amount)
            {
                Balance -= amount;
                ChipBaseManager.Instance?.UpdateAllChips(balance);
                return true;
            }
            return false;
        }

        public void StartBetting()
        {
            preBetBalance = balance;
        }

        public void ResetBalanceToPreBet()
        {
            SetBalance(preBetBalance);
        }
    }
}
