using System.Media;

namespace KillTheFrog
{
    public class SoundManager
    {
        private static SoundManager? _instance;
        public static SoundManager Instance => _instance ??= new SoundManager();

        private SoundPlayer? _killSound;
        private SoundPlayer? _missSound;
        private SoundPlayer? _gameOverSound;

        private SoundManager()
        {
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            try
            {
                // Create simple beep sounds programmatically
                // We'll use system sounds for now, but you can replace with WAV files later
                _killSound = new SoundPlayer();
                _missSound = new SoundPlayer();
                _gameOverSound = new SoundPlayer();
            }
            catch
            {
                // If sound initialization fails, continue without sound
            }
        }

        public void PlayKillSound()
        {
            try
            {
                // Play a high-pitched beep for successful kill
                System.Console.Beep(800, 100);
            }
            catch
            {
                // Ignore if sound fails
            }
        }

        public void PlayMissSound()
        {
            try
            {
                // Play a low-pitched beep for miss
                System.Console.Beep(300, 200);
            }
            catch
            {
                // Ignore if sound fails
            }
        }

        public void PlayGameOverSound()
        {
            try
            {
                // Play a descending sound for game over
                System.Console.Beep(600, 150);
                System.Threading.Thread.Sleep(50);
                System.Console.Beep(400, 150);
                System.Threading.Thread.Sleep(50);
                System.Console.Beep(200, 300);
            }
            catch
            {
                // Ignore if sound fails
            }
        }

        public void PlayStartSound()
        {
            try
            {
                // Play an ascending sound for game start
                System.Console.Beep(400, 100);
                System.Threading.Thread.Sleep(30);
                System.Console.Beep(600, 100);
                System.Threading.Thread.Sleep(30);
                System.Console.Beep(800, 150);
            }
            catch
            {
                // Ignore if sound fails
            }
        }
    }
}