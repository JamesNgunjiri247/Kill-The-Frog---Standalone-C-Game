using System;
using System.IO;

namespace KillTheFrog
{
    public class HighScoreManager
    {
        private static HighScoreManager? _instance;
        public static HighScoreManager Instance => _instance ??= new HighScoreManager();

        private readonly string _scoreFilePath;
        public int HighScore { get; private set; }

        private HighScoreManager()
        {
            _scoreFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KillTheFrog", "highscore.txt");
            LoadHighScore();
        }

        private void LoadHighScore()
        {
            try
            {
                if (File.Exists(_scoreFilePath))
                {
                    var scoreText = File.ReadAllText(_scoreFilePath);
                    if (int.TryParse(scoreText, out int score))
                    {
                        HighScore = score;
                        return;
                    }
                }
            }
            catch
            {
                // If loading fails, start with 0
            }

            HighScore = 0;
        }

        public bool TrySetNewHighScore(int score)
        {
            if (score > HighScore)
            {
                HighScore = score;
                SaveHighScore();
                return true; // New high score!
            }
            return false; // Not a new high score
        }

        private void SaveHighScore()
        {
            try
            {
                var directory = Path.GetDirectoryName(_scoreFilePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_scoreFilePath, HighScore.ToString());
            }
            catch
            {
                // If saving fails, continue without saving
            }
        }

        public void ResetHighScore()
        {
            HighScore = 0;
            SaveHighScore();
        }
    }
}