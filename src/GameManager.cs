using System;

namespace KillTheFrog
{
    public class GameManager
    {
        private static GameManager? _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        public int Score { get; private set; } = 0;
        public int Lives { get; private set; } = 3;
        public float GameDuration { get; set; } = 60f; // seconds
        public bool IsPlaying { get; private set; } = false;

        public float TimeRemaining { get; private set; }

        public event Action<int>? ScoreChanged;
        public event Action<int>? LivesChanged;
        public event Action<float>? TimeChanged;
        public event Action<int>? GameOver;

        public void StartGame()
        {
            Score = 0;
            TimeRemaining = GameDuration;
            Lives = 3;
            IsPlaying = true;

            DifficultyManager.Instance.StartGame();
            
            ScoreChanged?.Invoke(Score);
            LivesChanged?.Invoke(Lives);
            TimeChanged?.Invoke(TimeRemaining);
            
            SoundManager.Instance.PlayStartSound();
        }

        public void Update(float deltaTime)
        {
            if (!IsPlaying) return;
            
            PowerUpManager.Instance.Update();
            DifficultyManager.Instance.Update();
            
            TimeRemaining -= deltaTime;
            TimeChanged?.Invoke(TimeRemaining);
            
            if (TimeRemaining <= 0f) 
                EndGame();
        }

        public void AddScore(int amount)
        {
            if (!IsPlaying) return;
            var powerUpAmount = PowerUpManager.Instance.ApplyPointsMultiplier(amount);
            var difficultyAmount = (int)(powerUpAmount * DifficultyManager.Instance.GetBonusMultiplier());
            Score += difficultyAmount;
            ScoreChanged?.Invoke(Score);
        }

        public void AddLife()
        {
            if (!IsPlaying) return;
            Lives += 1;
            LivesChanged?.Invoke(Lives);
        }

        public void Miss()
        {
            if (!IsPlaying) return;
            Lives -= 1;
            LivesChanged?.Invoke(Lives);
            if (Lives <= 0) 
                EndGame();
        }

        public void EndGame()
        {
            IsPlaying = false;
            SoundManager.Instance.PlayGameOverSound();
            GameOver?.Invoke(Score);
        }
    }
}