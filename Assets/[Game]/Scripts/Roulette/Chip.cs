using UnityEngine;

namespace Game.Roulette
{
    public class Chip : MonoBehaviour
    {
        public int Value { get; private set; }

        public void SetValue(int value)
        {
            Value = value;
            // Görselde rakam/renk vs. update edebilirsin
        }
    }
}