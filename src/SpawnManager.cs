using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KillTheFrog
{
    public class SpawnManager
    {
        private readonly Random _random = new();
        private readonly Size _gameArea;
        private DateTime _lastSpawn;
        private DateTime _lastPowerUpSpawn;
        private readonly float _initialDelay = 1.0f;
        private readonly float _minSpawnInterval = 0.8f;
        private readonly float _maxSpawnInterval = 2.0f;
        private readonly float _powerUpSpawnInterval = 15.0f; // Power-ups every 15 seconds
        private readonly int _spawnPadding = 50;

        public List<Frog> Frogs { get; private set; } = new();
        public List<PowerUp> PowerUps { get; private set; } = new();

        public SpawnManager(Size gameArea)
        {
            _gameArea = gameArea;
            _lastSpawn = DateTime.Now.AddSeconds(_initialDelay);
            _lastPowerUpSpawn = DateTime.Now.AddSeconds(_powerUpSpawnInterval);
        }

        public void Update()
        {
            // Update existing frogs
            for (int i = Frogs.Count - 1; i >= 0; i--)
            {
                Frogs[i].Update();
                if (!Frogs[i].IsAlive)
                {
                    Frogs.RemoveAt(i);
                }
            }

            // Update existing power-ups
            for (int i = PowerUps.Count - 1; i >= 0; i--)
            {
                PowerUps[i].Update();
                if (!PowerUps[i].IsAlive)
                {
                    PowerUps.RemoveAt(i);
                }
            }

            // Spawn new frogs if game is playing
            if (GameManager.Instance.IsPlaying && ShouldSpawnFrog())
            {
                SpawnFrog();
                ScheduleNextSpawn();
            }

            // Spawn power-ups occasionally
            if (GameManager.Instance.IsPlaying && ShouldSpawnPowerUp())
            {
                SpawnPowerUp();
                ScheduleNextPowerUpSpawn();
            }
        }

        private bool ShouldSpawnFrog()
        {
            return DateTime.Now >= _lastSpawn;
        }

        private bool ShouldSpawnPowerUp()
        {
            return DateTime.Now >= _lastPowerUpSpawn;
        }

        private void SpawnFrog()
        {
            var x = _random.Next(_spawnPadding, _gameArea.Width - _spawnPadding);
            var y = _random.Next(_spawnPadding, _gameArea.Height - _spawnPadding);
            
            // Determine frog type based on probability
            FrogType frogType = DetermineFrogType();
            
            var frog = new Frog(new Point(x, y), frogType);
            Frogs.Add(frog);
        }

        private FrogType DetermineFrogType()
        {
            var roll = _random.NextDouble();
            
            if (roll < 0.05)        // 5% chance
                return FrogType.Golden;
            else if (roll < 0.25)   // 20% chance  
                return FrogType.Fast;
            else if (roll < 0.45)   // 20% chance
                return FrogType.Slow;
            else                    // 55% chance
                return FrogType.Normal;
        }

        private void ScheduleNextSpawn()
        {
            var baseInterval = _random.NextSingle() * (_maxSpawnInterval - _minSpawnInterval) + _minSpawnInterval;
            var powerUpInterval = PowerUpManager.Instance.ApplyTimeMultiplier(baseInterval);
            var finalInterval = DifficultyManager.Instance.ApplySpawnSpeedMultiplier(powerUpInterval);
            _lastSpawn = DateTime.Now.AddSeconds(finalInterval);
        }

        private void SpawnPowerUp()
        {
            var x = _random.Next(_spawnPadding, _gameArea.Width - _spawnPadding);
            var y = _random.Next(_spawnPadding, _gameArea.Height - _spawnPadding);
            
            // Random power-up type
            var powerUpTypes = Enum.GetValues<PowerUpType>();
            var randomType = powerUpTypes[_random.Next(powerUpTypes.Length)];
            
            var powerUp = new PowerUp(new Point(x, y), randomType);
            PowerUps.Add(powerUp);
        }

        private void ScheduleNextPowerUpSpawn()
        {
            _lastPowerUpSpawn = DateTime.Now.AddSeconds(_powerUpSpawnInterval);
        }

        public Frog? GetFrogAt(Point position)
        {
            return Frogs.FirstOrDefault(f => f.IsAlive && f.Contains(position));
        }

        public PowerUp? GetPowerUpAt(Point position)
        {
            return PowerUps.FirstOrDefault(p => p.IsAlive && p.Contains(position));
        }

        public void Clear()
        {
            Frogs.Clear();
            PowerUps.Clear();
        }
    }
}