using Game.Roulette;
using Game.UI;
using UnityEngine;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance { get; private set; }
        
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
        [Header("References")]

        [Header("Player Data")] [SerializeField]
        private int balance = 1000;

        private int preBetBalance;

        public int Balance
        {
            get => balance;
            set
            {
                balance = value;
                UIManager.Instance?.UpdateBalance(balance);
            }
        }

        public int PreBetBalance => preBetBalance;

        

        private void Start()
        {
            SaveManager.Instance?.LoadGame();
            Balance = SaveManager.Instance.LoadBalance();
        }

        public void NewGame()
        {
            StatisticsManager.Instance.ResetStats();
            RouletteManager.Instance.ResetTable();
            // SetBalance(1000);
        }

        public void OnApplicationQuit()
        {
            SaveManager.Instance?.SaveGame();
        }

        public void SetBalance(int amount)
        {
            Balance = amount;
            SaveManager.Instance.SaveBalance(balance);
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