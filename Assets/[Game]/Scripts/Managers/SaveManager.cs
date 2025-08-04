using Game.Roulette;
using UnityEngine;

namespace Game.Core
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        private const string SAVE_KEY = "RouletteSaveData";

        [System.Serializable]
        public class SaveData
        {
            public int totalSpins, totalWins, totalLosses, totalProfit;
            public int chips; // Oyuncu bakiyesi
            public string lastWinningNumber;
        }

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
            }
        }

        public void SaveGame()
        {
            var stats = StatisticsManager.Instance;
            var roulette = RouletteManager.Instance;

            SaveData data = new SaveData()
            {
                totalSpins = stats.TotalSpins,
                totalWins = stats.TotalWins,
                totalLosses = stats.TotalLosses,
                totalProfit = stats.TotalProfit,
                chips = GameManager.Instance.Balance,
                lastWinningNumber = roulette.LastWinningNumber
            };

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        public void LoadGame()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY)) return;
            string json = PlayerPrefs.GetString(SAVE_KEY);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            var stats = StatisticsManager.Instance;
            var roulette = RouletteManager.Instance;

            // stats.ResetStats();
            stats.SetStats(data.totalSpins, data.totalWins, data.totalLosses, data.totalProfit);

            GameManager.Instance.Balance = data.chips;
            roulette.LastWinningNumber = data.lastWinningNumber;
        }

        public void SaveBalance(int newBalance)
        {
            // Eski veriyi çek
            string json = PlayerPrefs.GetString(SAVE_KEY, "");
            SaveData data = string.IsNullOrEmpty(json) ? new SaveData() : JsonUtility.FromJson<SaveData>(json);

            // Yeni bakiyeyi setle
            data.chips = newBalance;

            // Tekrar kaydet
            string updatedJson = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, updatedJson);
            PlayerPrefs.Save();
        }

        public int LoadBalance()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY)) return 1000; // default
            string json = PlayerPrefs.GetString(SAVE_KEY);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.chips;
        }
    }
}