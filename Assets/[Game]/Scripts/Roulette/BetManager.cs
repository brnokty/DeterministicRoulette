using System.Collections.Generic;
using UnityEngine;

namespace Game.Roulette
{
    public class BetManager : MonoBehaviour
    {
        // Basit bahis objesi
        public class Bet
        {
            public BetAreaType areaType;
            public int amount;
        }

        public List<Bet> CurrentBets { get; private set; } = new List<Bet>();

        // Bahis ekleme
        public void PlaceBet(BetAreaType areaType, int amount)
        {
            CurrentBets.Add(new Bet
            {
                areaType = areaType,
                amount = amount
            });
        }

        public void ClearBets()
        {
            CurrentBets.Clear();
        }

        // Kazanç hesaplama
        public int CalculateWinnings(BetAreaType winningArea, int landedNumber, bool isRed, bool isEven)
        {
            int totalWinnings = 0;

            foreach (var bet in CurrentBets)
            {
                int payout = GetPayoutRatio(bet.areaType, landedNumber, isRed, isEven);
                if (payout > 0)
                {
                    totalWinnings += bet.amount * payout;
                }
            }

            return totalWinnings;
        }

        // Kazanç oranı hesaplayan fonksiyon
        public int GetPayoutRatio(BetAreaType areaType, int landedNumber, bool isRed, bool isEven)
        {
            // Tekli bahis
            if (areaType == BetAreaType.Single_0 && landedNumber == 0) return 36;
            for (int i = 1; i <= 36; i++)
                if (areaType == (BetAreaType)System.Enum.Parse(typeof(BetAreaType), $"Single_{i}") && landedNumber == i)
                    return 36;

            // Split bahisler
            if (areaType.ToString().StartsWith("Split_"))
            {
                string[] nums = areaType.ToString().Replace("Split_", "").Split('_');
                foreach (string n in nums)
                    if (int.Parse(n) == landedNumber)
                        return 18;
            }

            // Corner bahisler
            if (areaType.ToString().StartsWith("Corner_"))
            {
                string[] nums = areaType.ToString().Replace("Corner_", "").Split('_');
                foreach (string n in nums)
                    if (int.Parse(n) == landedNumber)
                        return 9;
            }

            // Street
            if (areaType.ToString().StartsWith("Street_"))
            {
                string[] nums = areaType.ToString().Replace("Street_", "").Split('_');
                foreach (string n in nums)
                    if (int.Parse(n) == landedNumber)
                        return 12;
            }

            // Line
            if (areaType.ToString().StartsWith("Line_"))
            {
                string[] nums = areaType.ToString().Replace("Line_", "").Split('_');
                foreach (string n in nums)
                    if (int.Parse(n) == landedNumber)
                        return 6;
            }

            // Column (Sütun)
            if (areaType == BetAreaType.Column_1 && IsInColumn(1, landedNumber)) return 3;
            if (areaType == BetAreaType.Column_2 && IsInColumn(2, landedNumber)) return 3;
            if (areaType == BetAreaType.Column_3 && IsInColumn(3, landedNumber)) return 3;

            // Dozen
            if (areaType == BetAreaType.Dozen_1st && landedNumber >= 1 && landedNumber <= 12) return 3;
            if (areaType == BetAreaType.Dozen_2nd && landedNumber >= 13 && landedNumber <= 24) return 3;
            if (areaType == BetAreaType.Dozen_3rd && landedNumber >= 25 && landedNumber <= 36) return 3;

            // Red / Black
            if (areaType == BetAreaType.Red && isRed) return 2;
            if (areaType == BetAreaType.Black && !isRed && landedNumber != 0) return 2;

            // Odd / Even
            if (areaType == BetAreaType.Odd && landedNumber % 2 == 1) return 2;
            if (areaType == BetAreaType.Even && landedNumber != 0 && landedNumber % 2 == 0) return 2;

            // Low / High
            if (areaType == BetAreaType.Low && landedNumber >= 1 && landedNumber <= 18) return 2;
            if (areaType == BetAreaType.High && landedNumber >= 19 && landedNumber <= 36) return 2;

            // Zero
            if (areaType == BetAreaType.Zero && landedNumber == 0) return 36;

            return 0;
        }

        private bool IsInColumn(int columnIndex, int number)
        {
            if (number < 1 || number > 36) return false;
            return (number - 1) % 3 == (columnIndex - 1);
        }
    }
}
