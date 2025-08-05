using Game.Roulette;
using UnityEngine;

namespace Game.Core
{
    [System.Serializable]
    public class SaveData
    {
        public int totalSpins, totalWins, totalLosses, totalProfit;
        public int chips;
        public string lastWinningNumber;
    }

    public class SaveManager : MonoBehaviour
    {
        #region Singleton

        public static SaveManager Instance { get; private set; }

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

        #endregion

        #region PRIVATE PROPERTIES

        private const string SAVE_KEY = "RouletteSaveData";

        #endregion

        #region PUBLIC METHODS

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

        #endregion
    }
}