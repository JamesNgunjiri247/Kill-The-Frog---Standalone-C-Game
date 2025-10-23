using System;
using System.Drawing;

namespace KillTheFrog
{
    public enum PowerUpType
    {
        SlowMotion,    // Slows down frog spawning and expiration
        DoublePoints,  // Double points for 10 seconds
        ExtraLife      // Gain one extra life
    }

    public class PowerUp
    {
        public Point Position { get; set; }
        public int Size { get; set; } = 30;
        public float Lifetime { get; set; } = 5.0f; // Power-ups last 5 seconds
        public bool IsAlive { get; private set; } = true;
        public DateTime SpawnTime { get; private set; }
        public PowerUpType Type { get; private set; }
        public Color Color { get; private set; }
        public string Symbol { get; private set; }

        public Rectangle Bounds => new Rectangle(Position.X - Size/2, Position.Y - Size/2, Size, Size);

        public PowerUp(Point position, PowerUpType type)
        {
            Position = position;
            Type = type;
            SpawnTime = DateTime.Now;
            SetupPowerUpType();
        }

        private void SetupPowerUpType()
        {
            switch (Type)
            {
                case PowerUpType.SlowMotion:
                    Color = Color.Cyan;
                    Symbol = "⏰";
                    break;
                    
                case PowerUpType.DoublePoints:
                    Color = Color.Orange;
                    Symbol = "2X";
                    break;
                    
                case PowerUpType.ExtraLife:
                    Color = Color.Pink;
                    Symbol = "♥";
                    break;
            }
        }

        public void Update()
        {
            if (!IsAlive) return;

            var elapsed = (DateTime.Now - SpawnTime).TotalSeconds;
            if (elapsed >= Lifetime)
            {
                Expire();
            }
        }

        public void Collect()
        {
            if (!IsAlive) return;
            IsAlive = false;
            PowerUpManager.Instance.ActivatePowerUp(Type);
        }

        public void Expire()
        {
            if (!IsAlive) return;
            IsAlive = false;
        }

        public bool Contains(Point point)
        {
            return Bounds.Contains(point);
        }
    }

    public class PowerUpManager
    {
        private static PowerUpManager? _instance;
        public static PowerUpManager Instance => _instance ??= new PowerUpManager();

        public bool SlowMotionActive { get; private set; }
        public bool DoublePointsActive { get; private set; }
        
        private DateTime _slowMotionEnd;
        private DateTime _doublePointsEnd;

        public void Update()
        {
            var now = DateTime.Now;
            
            if (SlowMotionActive && now >= _slowMotionEnd)
            {
                SlowMotionActive = false;
            }
            
            if (DoublePointsActive && now >= _doublePointsEnd)
            {
                DoublePointsActive = false;
            }
        }

        public void ActivatePowerUp(PowerUpType type)
        {
            var now = DateTime.Now;
            
            switch (type)
            {
                case PowerUpType.SlowMotion:
                    SlowMotionActive = true;
                    _slowMotionEnd = now.AddSeconds(10);
                    SoundManager.Instance.PlayKillSound(); // Reuse kill sound for now
                    break;
                    
                case PowerUpType.DoublePoints:
                    DoublePointsActive = true;
                    _doublePointsEnd = now.AddSeconds(10);
                    SoundManager.Instance.PlayKillSound();
                    break;
                    
                case PowerUpType.ExtraLife:
                    GameManager.Instance.AddLife();
                    SoundManager.Instance.PlayStartSound(); // Reuse start sound
                    break;
            }
        }

        public int ApplyPointsMultiplier(int basePoints)
        {
            return DoublePointsActive ? basePoints * 2 : basePoints;
        }

        public float ApplyTimeMultiplier(float baseTime)
        {
            return SlowMotionActive ? baseTime * 1.5f : baseTime;
        }
    }
}