using UnityEngine;

namespace Game.Core
{
    public class StatisticsManager : MonoBehaviour
    {
        #region Singleton

        public static StatisticsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        #endregion

        #region PUBLIC PROPERTIES

        public int TotalSpins { get; private set; }
        public int TotalWins { get; private set; }
        public int TotalLosses { get; private set; }
        public int TotalProfit { get; private set; }

        public delegate void OnStatsChangedDelegate();

        public event OnStatsChangedDelegate OnStatsChanged;

        #endregion

        #region PUBLIC METHODS

        public void RecordSpin(bool win, int profit)
        {
            TotalSpins++;
            if (win) TotalWins++;
            else TotalLosses++;
            TotalProfit += profit;

            OnStatsChanged?.Invoke();
        }

        public void ResetStats()
        {
            TotalSpins = 0;
            TotalWins = 0;
            TotalLosses = 0;
            TotalProfit = 0;
            OnStatsChanged?.Invoke();
        }


        public void SetStats(int spins, int wins, int losses, int profit)
        {
            TotalSpins = spins;
            TotalWins = wins;
            TotalLosses = losses;
            TotalProfit = profit;
            OnStatsChanged?.Invoke();
        }

        #endregion
    }
}