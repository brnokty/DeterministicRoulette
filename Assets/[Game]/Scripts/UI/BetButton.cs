using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class BetButton : MonoBehaviour
    {
        public Game.Roulette.BetType betType;
        public string[] numbers;
        public int amount;
        public Button button;

        private void Start()
        {
            button.onClick.AddListener(PlaceBet);
        }

        public void PlaceBet()
        {
            Game.Core.GameManager.Instance.rouletteManager.PlaceBet(betType, numbers, amount);
        }
    }
}