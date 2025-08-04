using UnityEngine;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("References")]
        public Roulette.RouletteManager rouletteManager;
        public StatisticsManager statisticsManager;
        public SaveManager saveManager;
        public SoundManager soundManager;

        [Header("Player Data")]
        [SerializeField] private int balance = 1000; // Başlangıç parası

        public int Balance
        {
            get => balance;
            private set
            {
                balance = value;
                // Eğer UI otomatik güncellenecekse burada event/observer veya UI fonksiyonu çağırabilirsin:
                // OnBalanceChanged?.Invoke(balance);
            }
        }

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
            SetBalance(1000); // Yeni oyunda para sıfırlansın mı?
        }

        public void OnApplicationQuit()
        {
            saveManager?.SaveGame();
        }

        // -------- PARA FONKSİYONLARI --------

        /// <summary>
        /// Para harcatır. Yetmezse false döner.
        /// </summary>
        public bool SpendMoney(int amount)
        {
            if (balance >= amount)
            {
                Balance -= amount;
                // UI güncelleme veya ses/feedback tetikle
                return true;
            }
            return false;
        }

        /// <summary>
        /// Para ekler.
        /// </summary>
        public void AddMoney(int amount)
        {
            Balance += amount;
            // UI güncelleme veya ses/feedback tetikle
        }

        /// <summary>
        /// Bakiyeyi direkt olarak ayarlar.
        /// </summary>
        public void SetBalance(int amount)
        {
            Balance = amount;
            // UI güncelleme veya ses/feedback tetikle
        }

        // ------- SAVE/LOAD için (SaveManager ile entegre) --------
        public void SavePlayerData()
        {
            saveManager?.SaveBalance(balance);
            // Diğer data ile birlikte...
        }

        public void LoadPlayerData()
        {
            Balance = saveManager != null ? saveManager.LoadBalance() : 1000;
        }
    }
}
