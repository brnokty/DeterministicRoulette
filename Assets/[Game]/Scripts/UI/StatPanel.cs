using Game.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class StatPanel : MonoBehaviour
    {
        public TMP_Text totalSpinsText;
        public TMP_Text totalWinsText;
        public TMP_Text totalLossesText;
        public TMP_Text totalProfitText;

        private void Start()
        {
            UpdateStats();
            StatisticsManager.Instance.OnStatsChanged += UpdateStats;
        }

        public void UpdateStats()
        {
            var stats = StatisticsManager.Instance;
            totalSpinsText.text = $"Spins: {stats.TotalSpins}";
            totalWinsText.text = $"Wins: {stats.TotalWins}";
            totalLossesText.text = $"Losses: {stats.TotalLosses}";
            totalProfitText.text = $"Profit: {stats.TotalProfit}";
        }
    }
}