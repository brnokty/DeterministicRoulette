using Game.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class StatPanel : MonoBehaviour
    {
        #region INSPECTOR PROPERTIES

        public TMP_Text totalSpinsText;
        public TMP_Text totalWinsText;
        public TMP_Text totalLossesText;
        public TMP_Text totalProfitText;

        #endregion

        #region UNITY METHODS

        private void Start()
        {
            UpdateStats();
            StatisticsManager.Instance.OnStatsChanged += UpdateStats;
        }

        #endregion

        #region PUBLIC METHODS

        public void UpdateStats()
        {
            var stats = StatisticsManager.Instance;
            totalSpinsText.text = $"Spins: {stats.TotalSpins}";
            totalWinsText.text = $"Wins: {stats.TotalWins}";
            totalLossesText.text = $"Losses: {stats.TotalLosses}";
            totalProfitText.text = $"Profit: {stats.TotalProfit}";
        }

        #endregion
    }
}