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
        public string LastWinningNumber;

        [Header("Settings")] public GameType gameType = GameType.EuropeanRoulette;
        public int minBet = 10;
        public int maxBet = 500;

        public void PlaceBet(BetType betType, string[] numbers, int amount)
        {
            if (amount < minBet || amount > maxBet || amount > PlayerChips)
                return;
            // betManager.PlaceBet(betType, numbers, amount);
            PlayerChips -= amount;
        }

        public void ResetTable()
        {
            betManager.ClearBets();
        }

        public void Spin(string deterministicNumber)
        {
            string result = "";
            if (gameType == GameType.EuropeanRoulette)
            {
                 result = (deterministicNumber != "" && !string.IsNullOrEmpty(deterministicNumber))
                    ? deterministicNumber
                    : rouletteController.europeanWheelOrder[Random.Range(0, 37)];
            }
            else
            {
                result = (deterministicNumber != "" && !string.IsNullOrEmpty(deterministicNumber))
                    ? deterministicNumber
                    : rouletteController.americanWheelOrder[Random.Range(0, 38)];
            }

            LastWinningNumber = result;
            rouletteController.StartSpin(result, () => OnSpinComplete(result));
        }

        private void OnSpinComplete(string winningNumber)
        {
            uiManager.ShowResult(winningNumber);


            bool isRed = IsRed(winningNumber); // Yardımcı fonksiyonun olsun
            bool isEven = int.Parse(winningNumber) != 0 && (int.Parse(winningNumber) % 2 == 0);

            int winnings = BetManager.Instance.CalculateWinnings(int.Parse(winningNumber), isRed, isEven);
            if (winnings > 0)
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Win);
                GameManager.Instance.AddMoney(winnings);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Lose);
            }

            print("winnings = " + winnings + " for number " + winningNumber + "Balance = " +
                  GameManager.Instance.Balance);


            // Bahisleri sıfırla veya yeni turu başlat
            BetManager.Instance.ClearBets();
        }

        private bool IsRed(string number)
        {
            string[] redNumbers =
            {
                "1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36"
            };
            return System.Array.Exists(redNumbers, n => n == number);
        }
    }
}