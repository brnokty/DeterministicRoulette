using UnityEngine;

namespace Game.Core
{
    public class SaveManager : MonoBehaviour
    {
        private const string SAVE_KEY = "RouletteSaveData";

        [System.Serializable]
        public class SaveData
        {
            public int totalSpins, totalWins, totalLosses, totalProfit;
            public int chips; // Oyuncu bakiyesi
            public string lastWinningNumber;
            // Gerekirse aktif bahisler, tercih vs. eklenebilir
        }

        public void SaveGame()
        {
            var stats = GameManager.Instance.statisticsManager;
            var roulette = GameManager.Instance.rouletteManager;

            SaveData data = new SaveData()
            {
                totalSpins = stats.TotalSpins,
                totalWins = stats.TotalWins,
                totalLosses = stats.TotalLosses,
                totalProfit = stats.TotalProfit,
                chips = roulette.PlayerChips,
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

            var stats = GameManager.Instance.statisticsManager;
            var roulette = GameManager.Instance.rouletteManager;

            // stats.ResetStats();
            stats.SetStats(data.totalSpins, data.totalWins, data.totalLosses, data.totalProfit);

            roulette.PlayerChips = data.chips;
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
