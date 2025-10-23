using System;
using System.Drawing;

namespace KillTheFrog
{
    public enum FrogType
    {
        Normal,     // Green, 10 points, 2.0s lifetime
        Fast,       // Red, 15 points, 1.5s lifetime  
        Slow,       // Blue, 5 points, 3.0s lifetime
        Golden      // Gold, 50 points, 1.0s lifetime, rare
    }

    public class Frog
    {
        public Point Position { get; set; }
        public int Size { get; set; } = 50;
        public float Lifetime { get; set; }
        public int ScoreValue { get; set; }
        public bool IsAlive { get; private set; } = true;
        public bool IsBeingKilled { get; private set; } = false;
        public DateTime SpawnTime { get; private set; }
        public DateTime? DeathTime { get; private set; }
        public FrogType Type { get; private set; }
        public Color Color { get; private set; }

        public Rectangle Bounds => new Rectangle(Position.X - Size/2, Position.Y - Size/2, Size, Size);

        public float AnimationProgress
        {
            get
            {
                if (IsBeingKilled && DeathTime.HasValue)
                {
                    var elapsed = (DateTime.Now - DeathTime.Value).TotalSeconds;
                    return Math.Min((float)(elapsed / 0.3), 1.0f); // 0.3 second death animation
                }
                return 0f;
            }
        }

        public float PulseAnimation
        {
            get
            {
                var elapsed = (DateTime.Now - SpawnTime).TotalSeconds;
                return (float)(0.5 + 0.5 * Math.Sin(elapsed * 8)); // Pulse effect
            }
        }

        public Frog(Point position, FrogType type = FrogType.Normal)
        {
            Position = position;
            Type = type;
            SpawnTime = DateTime.Now;
            SetupFrogType();
        }

        private void SetupFrogType()
        {
            switch (Type)
            {
                case FrogType.Normal:
                    Lifetime = 2.0f;
                    ScoreValue = 10;
                    Color = Color.DarkGreen;
                    Size = 50;
                    break;
                    
                case FrogType.Fast:
                    Lifetime = 1.5f;
                    ScoreValue = 15;
                    Color = Color.Red;
                    Size = 45;
                    break;
                    
                case FrogType.Slow:
                    Lifetime = 3.0f;
                    ScoreValue = 5;
                    Color = Color.Blue;
                    Size = 55;
                    break;
                    
                case FrogType.Golden:
                    Lifetime = 1.0f;
                    ScoreValue = 50;
                    Color = Color.Gold;
                    Size = 40;
                    break;
            }
            
            // Apply power-up effects
            Lifetime = PowerUpManager.Instance.ApplyTimeMultiplier(Lifetime);
            
            // Apply difficulty effects
            Size = DifficultyManager.Instance.ApplyFrogSizeReduction(Size);
            Lifetime = DifficultyManager.Instance.ApplyFrogLifetimeReduction(Lifetime);
        }

        public void Update()
        {
            if (!IsAlive) return;

            // Check if death animation is complete
            if (IsBeingKilled && DeathTime.HasValue)
            {
                var elapsed = (DateTime.Now - DeathTime.Value).TotalSeconds;
                if (elapsed >= 0.3) // Death animation duration
                {
                    IsAlive = false;
                    return;
                }
            }

            // Check normal expiration
            if (!IsBeingKilled)
            {
                var elapsed = (DateTime.Now - SpawnTime).TotalSeconds;
                if (elapsed >= Lifetime)
                {
                    Expire();
                }
            }
        }

        public void Kill()
        {
            if (!IsAlive || IsBeingKilled) return;
            IsBeingKilled = true;
            DeathTime = DateTime.Now;
            GameManager.Instance.AddScore(ScoreValue);
            SoundManager.Instance.PlayKillSound();
        }

        public void Expire()
        {
            if (!IsAlive || IsBeingKilled) return;
            IsAlive = false;
            GameManager.Instance.Miss();
            SoundManager.Instance.PlayMissSound();
        }

        public bool Contains(Point point)
        {
            return Bounds.Contains(point);
        }
    }
}