using System;

namespace KillTheFrog
{
    public class DifficultyManager
    {
        private static DifficultyManager? _instance;
        public static DifficultyManager Instance => _instance ??= new DifficultyManager();

        public float DifficultyMultiplier { get; private set; } = 1.0f;
        public int CurrentWave { get; private set; } = 1;

        private DateTime _gameStartTime;

        public void StartGame()
        {
            _gameStartTime = DateTime.Now;
            DifficultyMultiplier = 1.0f;
            CurrentWave = 1;
        }

        public void Update()
        {
            if (!GameManager.Instance.IsPlaying) return;

            var elapsed = (DateTime.Now - _gameStartTime).TotalSeconds;
            
            // Increase difficulty every 15 seconds
            var newWave = (int)(elapsed / 15) + 1;
            if (newWave != CurrentWave)
            {
                CurrentWave = newWave;
                DifficultyMultiplier = 1.0f + (CurrentWave - 1) * 0.3f; // 30% increase per wave
            }
        }

        public float ApplySpawnSpeedMultiplier(float baseInterval)
        {
            // Spawn frogs faster as difficulty increases
            return baseInterval / Math.Min(DifficultyMultiplier, 3.0f); // Cap at 3x speed
        }

        public int ApplyFrogSizeReduction(int baseSize)
        {
            // Make frogs smaller as difficulty increases
            var reduction = (CurrentWave - 1) * 3; // 3 pixels smaller per wave
            return Math.Max(baseSize - reduction, 25); // Minimum size of 25 pixels
        }

        public float ApplyFrogLifetimeReduction(float baseLifetime)
        {
            // Make frogs disappear faster
            var reduction = (CurrentWave - 1) * 0.1f; // 0.1 seconds less per wave
            return Math.Max(baseLifetime - reduction, 0.5f); // Minimum 0.5 seconds
        }

        public float GetBonusMultiplier()
        {
            // Give bonus points for higher difficulty
            return 1.0f + (CurrentWave - 1) * 0.1f; // 10% bonus per wave
        }
    }
}