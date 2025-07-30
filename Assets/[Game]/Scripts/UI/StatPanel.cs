using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class StatPanel : MonoBehaviour
    {
        public Text totalSpinsText;
        public Text totalWinsText;
        public Text totalLossesText;
        public Text totalProfitText;

        private void Start()
        {
            UpdateStats();
            Game.Core.GameManager.Instance.statisticsManager.OnStatsChanged += UpdateStats;
        }

        public void UpdateStats()
        {
            var stats = Game.Core.GameManager.Instance.statisticsManager;
            totalSpinsText.text = $"Spins: {stats.TotalSpins}";
            totalWinsText.text = $"Wins: {stats.TotalWins}";
            totalLossesText.text = $"Losses: {stats.TotalLosses}";
            totalProfitText.text = $"Profit: {stats.TotalProfit}";
        }
    }
}