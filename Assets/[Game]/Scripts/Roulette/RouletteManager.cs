using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Game.UI;

namespace Game.Roulette
{
    public class RouletteManager : MonoBehaviour
    {
        [Header("References")] public RouletteController rouletteController;
        public BetManager betManager;
        public UIManager uiManager;
        public int PlayerChips = 1000;
        public int LastWinningNumber = 0;

        [Header("Settings")] public int minBet = 10;
        public int maxBet = 500;

        public void PlaceBet(BetType betType, int[] numbers, int amount)
        {
            if (amount < minBet || amount > maxBet || amount > PlayerChips)
                return;
            betManager.PlaceBet(betType, numbers, amount);
            PlayerChips -= amount;
        }

        public void ResetTable()
        {
            betManager.ClearBets();
        }

        public void Spin(int deterministicNumber = -1)
        {
            int result = (deterministicNumber >= 0 && deterministicNumber <= 36)
            ? deterministicNumber
            : Random.Range(0, 37);
            LastWinningNumber = result;
            rouletteController.StartSpin(deterministicNumber, () => OnSpinComplete(result));
        }

        private void OnSpinComplete(int winningNumber)
        {
            uiManager.ShowResult(winningNumber);

            // Payout ve istatistik güncelle
            int profit = 0;
            bool win = false;
            foreach (var bet in betManager.CurrentBets)
            {
                if (IsBetWin(bet, winningNumber, out int payout))
                {
                    PlayerChips += payout;
                    profit += payout;
                    win = true;
                }
            }

            GameManager.Instance.statisticsManager.RecordSpin(win, profit);

            // Bahisleri sıfırla
            betManager.ClearBets();
        }

        private bool IsBetWin(BetManager.Bet bet, int winningNumber, out int payout)
        {
            payout = 0;
            // Basit bir örnek payout hesabı (gerçek oranları ekle!)
            if (System.Array.Exists(bet.numbers, n => n == winningNumber))
            {
                switch (bet.betType)
                {
                    case BetType.Straight: payout = bet.amount * 36; break;
                    case BetType.Split: payout = bet.amount * 17; break;
                    case BetType.Street: payout = bet.amount * 11; break;
                    case BetType.Corner: payout = bet.amount * 8; break;
                    case BetType.SixLine: payout = bet.amount * 5; break;
                    // Diğer bet türleri için oranları ekle
                }

                return true;
            }

            // Outside bet logic ekle
            return false;
        }
    }
}