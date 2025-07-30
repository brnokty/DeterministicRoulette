using UnityEngine;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        public Roulette.RouletteManager rouletteManager;
        public StatisticsManager statisticsManager;
        public SaveManager saveManager;
        public SoundManager soundManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Managerlar için null kontrolü yapılabilir
            if (!rouletteManager) rouletteManager = FindObjectOfType<Roulette.RouletteManager>();
            if (!statisticsManager) statisticsManager = FindObjectOfType<StatisticsManager>();
            if (!saveManager) saveManager = FindObjectOfType<SaveManager>();
            if (!soundManager) soundManager = FindObjectOfType<SoundManager>();
        }

        private void Start()
        {
            saveManager?.LoadGame(); // Oyunu açarken otomatik load
        }

        public void NewGame()
        {
            statisticsManager.ResetStats();
            rouletteManager.ResetTable();
        }

        public void OnApplicationQuit()
        {
            saveManager?.SaveGame(); // Çıkarken otomatik kaydet
        }
    }
}
