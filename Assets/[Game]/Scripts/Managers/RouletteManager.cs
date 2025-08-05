using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Game.UI;
using Random = UnityEngine.Random;

namespace Game.Roulette
{
    public class RouletteManager : MonoBehaviour
    {
        #region Singleton

        public static RouletteManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        #endregion

        #region INSPECTOR PROPERTIES

        [Header("References")] public RouletteController rouletteController;

        [Header("Settings")] public GameType gameType = GameType.EuropeanRoulette;

        #endregion

        #region PUBLIC PROPERTIES

        [HideInInspector] public int PlayerChips = 950;
        [HideInInspector] public string LastWinningNumber;

        #endregion

        #region UNITY METHODS

        private void Start()
        {
            PlayerChips = GameManager.Instance.Balance;
            rouletteController.SetGameType(gameType);
        }

        #endregion

        #region PUBLIC METHODS

        public void ResetTable()
        {
            BetManager.Instance.ResetCurrentBet();
            PlayerChips = GameManager.Instance.Balance;
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

        #endregion

        #region PRIVATE METHODS

        private void OnSpinComplete(string winningNumber)
        {
            UIManager.Instance.ShowResult(winningNumber);


            bool isRed = IsRed(winningNumber); // Yardımcı fonksiyonun olsun
            bool isEven = int.Parse(winningNumber) != 0 && (int.Parse(winningNumber) % 2 == 0);

            int winnings = BetManager.Instance.CalculateWinnings(int.Parse(winningNumber), isRed, isEven);
            if (winnings > 0)
            {
                SoundManager.Instance.PlaySound(SoundType.Win);
                GameManager.Instance.AddMoney(winnings);
                StatisticsManager.Instance.RecordSpin(true, winnings);
                rouletteController.PlayConfetti();
                UIManager.Instance.youWonPopUp.Show(winnings);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundType.Lose);
                StatisticsManager.Instance.RecordSpin(false, winnings);
            }

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

        #endregion
    }
}