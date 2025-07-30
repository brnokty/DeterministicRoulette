using UnityEngine;

namespace Game.Core
{
    public class StatisticsManager : MonoBehaviour
    {
        public int TotalSpins { get; private set; }
        public int TotalWins { get; private set; }
        public int TotalLosses { get; private set; }
        public int TotalProfit { get; private set; } // +kazanç/-kayıp

        public delegate void OnStatsChangedDelegate();
        public event OnStatsChangedDelegate OnStatsChanged;

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

        // YENİ EKLE: Save/Load için topluca ayar
        public void SetStats(int spins, int wins, int losses, int profit)
        {
            TotalSpins = spins;
            TotalWins = wins;
            TotalLosses = losses;
            TotalProfit = profit;
            OnStatsChanged?.Invoke();
        }
    }
}