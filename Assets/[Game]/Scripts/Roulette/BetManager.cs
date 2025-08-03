using System.Collections.Generic;
using UnityEngine;

namespace Game.Roulette
{
    public class BetManager : MonoBehaviour
    {
        public class Bet
        {
            public BetType betType;
            public string[] numbers; // Bahis hangi sayıları kapsıyor
            public int amount;
        }

        public List<Bet> CurrentBets { get; private set; } = new List<Bet>();

        public void PlaceBet(BetType betType, string[] numbers, int amount)
        {
            CurrentBets.Add(new Bet
            {
                betType = betType,
                numbers = numbers,
                amount = amount
            });
        }

        public void ClearBets()
        {
            CurrentBets.Clear();
        }
    }
}