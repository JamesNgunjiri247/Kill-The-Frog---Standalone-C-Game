using System;
using System.Drawing;
using System.Windows.Forms;

namespace KillTheFrog
{
    public partial class GameForm : Form
    {
        private SpawnManager? _spawnManager;
        private System.Windows.Forms.Timer _gameTimer;
        private Label _scoreLabel;
        private Label _livesLabel;
        private Label _timeLabel;
        private Label _highScoreLabel;
        private Label _waveLabel;
        private Button _startButton;
        private Panel _gameOverPanel;
        private Label _gameOverLabel;
        private Button _restartButton;
        private DateTime _lastFrameTime;

        public GameForm()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 600);
            Text = "Kill The Frog";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.LightGreen;

            // Score Label
            _scoreLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(100, 23),
                Text = "Score: 0",
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            Controls.Add(_scoreLabel);

            // Lives Label
            _livesLabel = new Label
            {
                Location = new Point(120, 10),
                Size = new Size(100, 23),
                Text = "Lives: 3",
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            Controls.Add(_livesLabel);

            // Time Label
            _timeLabel = new Label
            {
                Location = new Point(230, 10),
                Size = new Size(100, 23),
                Text = "Time: 60",
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            Controls.Add(_timeLabel);

            // High Score Label
            _highScoreLabel = new Label
            {
                Location = new Point(340, 10),
                Size = new Size(150, 23),
                Text = $"High Score: {HighScoreManager.Instance.HighScore}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Purple
            };
            Controls.Add(_highScoreLabel);

            // Wave Label
            _waveLabel = new Label
            {
                Location = new Point(500, 10),
                Size = new Size(100, 23),
                Text = "Wave: 1",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Red
            };
            Controls.Add(_waveLabel);

            // Start Button
            _startButton = new Button
            {
                Location = new Point(350, 250),
                Size = new Size(100, 50),
                Text = "Start Game",
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.Yellow
            };
            _startButton.Click += StartButton_Click;
            Controls.Add(_startButton);

            // Game Over Panel
            _gameOverPanel = new Panel
            {
                Location = new Point(250, 200),
                Size = new Size(300, 150),
                BackColor = Color.Red,
                Visible = false
            };

            _gameOverLabel = new Label
            {
                Location = new Point(10, 20),
                Size = new Size(280, 50),
                Text = "Game Over!\nFinal Score: 0",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _gameOverPanel.Controls.Add(_gameOverLabel);

            _restartButton = new Button
            {
                Location = new Point(100, 80),
                Size = new Size(100, 40),
                Text = "Restart",
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            _restartButton.Click += RestartButton_Click;
            _gameOverPanel.Controls.Add(_restartButton);

            Controls.Add(_gameOverPanel);

            // Game Timer
            _gameTimer = new System.Windows.Forms.Timer
            {
                Interval = 16 // ~60 FPS
            };
            _gameTimer.Tick += GameTimer_Tick;

            ResumeLayout(false);
        }

        private void InitializeGame()
        {
            _spawnManager = new SpawnManager(ClientSize);
            _lastFrameTime = DateTime.Now;

            // Subscribe to game events
            GameManager.Instance.ScoreChanged += OnScoreChanged;
            GameManager.Instance.LivesChanged += OnLivesChanged;
            GameManager.Instance.TimeChanged += OnTimeChanged;
            GameManager.Instance.GameOver += OnGameOver;

            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            _startButton.Visible = false;
            _gameOverPanel.Visible = false;
            _spawnManager?.Clear();
            GameManager.Instance.StartGame();
            _gameTimer.Start();
            _lastFrameTime = DateTime.Now;
            Focus(); // Ensure form has focus for mouse clicks
        }

        private void RestartButton_Click(object? sender, EventArgs e)
        {
            StartButton_Click(sender, e);
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            var currentTime = DateTime.Now;
            var deltaTime = (float)(currentTime - _lastFrameTime).TotalSeconds;
            _lastFrameTime = currentTime;

            GameManager.Instance.Update(deltaTime);
            _spawnManager?.Update();
            
            // Update wave display
            _waveLabel.Text = $"Wave: {DifficultyManager.Instance.CurrentWave}";
            
            Invalidate(); // Trigger repaint
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            if (!GameManager.Instance.IsPlaying) return;

            // Check for power-up clicks first
            var powerUp = _spawnManager?.GetPowerUpAt(e.Location);
            if (powerUp != null)
            {
                powerUp.Collect();
                return;
            }

            // Check for frog clicks
            var frog = _spawnManager?.GetFrogAt(e.Location);
            if (frog != null)
            {
                frog.Kill();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (_spawnManager == null) return;

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw frogs
            foreach (var frog in _spawnManager.Frogs)
            {
                if (frog.IsAlive)
                {
                    DrawFrog(g, frog);
                }
            }

            // Draw power-ups
            foreach (var powerUp in _spawnManager.PowerUps)
            {
                if (powerUp.IsAlive)
                {
                    DrawPowerUp(g, powerUp);
                }
            }

            // Draw power-up status indicators
            DrawPowerUpStatus(g);
        }

        private void DrawFrog(Graphics g, Frog frog)
        {
            var bounds = frog.Bounds;
            
            // Apply death animation scaling
            if (frog.IsBeingKilled)
            {
                var scale = 1.0f - frog.AnimationProgress;
                var newSize = (int)(frog.Size * scale);
                bounds = new Rectangle(
                    frog.Position.X - newSize/2, 
                    frog.Position.Y - newSize/2, 
                    newSize, 
                    newSize
                );
            }
            
            // Apply pulse animation for golden frogs
            if (frog.Type == FrogType.Golden && !frog.IsBeingKilled)
            {
                var pulse = frog.PulseAnimation;
                var pulseSize = (int)(frog.Size * (0.9f + 0.1f * pulse));
                bounds = new Rectangle(
                    frog.Position.X - pulseSize/2,
                    frog.Position.Y - pulseSize/2,
                    pulseSize,
                    pulseSize
                );
            }
            
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            
            // Draw frog body with type-specific color
            var color = frog.Color;
            if (frog.IsBeingKilled)
            {
                // Fade out effect
                var alpha = (int)(255 * (1.0f - frog.AnimationProgress));
                color = Color.FromArgb(alpha, color);
            }
            
            using var brush = new SolidBrush(color);
            g.FillEllipse(brush, bounds);
            
            // Draw frog border (thicker for golden frogs)
            var borderWidth = frog.Type == FrogType.Golden ? 3 : 2;
            var borderColor = frog.IsBeingKilled ? 
                Color.FromArgb((int)(255 * (1.0f - frog.AnimationProgress)), Color.Black) : 
                Color.Black;
            using var pen = new Pen(borderColor, borderWidth);
            g.DrawEllipse(pen, bounds);
            
            if (frog.IsBeingKilled) return; // Skip details during death animation
            
            // Draw simple eyes
            var eyeSize = Math.Max(1, bounds.Width / 6);
            var leftEye = new Rectangle(bounds.X + eyeSize, bounds.Y + eyeSize, eyeSize, eyeSize);
            var rightEye = new Rectangle(bounds.Right - eyeSize * 2, bounds.Y + eyeSize, eyeSize, eyeSize);
            
            using var eyeBrush = new SolidBrush(Color.White);
            g.FillEllipse(eyeBrush, leftEye);
            g.FillEllipse(eyeBrush, rightEye);
            
            using var pupilBrush = new SolidBrush(Color.Black);
            var pupilSize = Math.Max(1, eyeSize / 2);
            g.FillEllipse(pupilBrush, leftEye.X + pupilSize/2, leftEye.Y + pupilSize/2, pupilSize, pupilSize);
            g.FillEllipse(pupilBrush, rightEye.X + pupilSize/2, rightEye.Y + pupilSize/2, pupilSize, pupilSize);
            
            // Draw point value for special frogs
            if (frog.Type != FrogType.Normal && bounds.Width > 20)
            {
                using var font = new Font("Arial", Math.Max(6, bounds.Width / 6), FontStyle.Bold);
                using var textBrush = new SolidBrush(Color.White);
                var text = frog.ScoreValue.ToString();
                var textSize = g.MeasureString(text, font);
                var textPos = new PointF(
                    bounds.X + (bounds.Width - textSize.Width) / 2,
                    bounds.Y + bounds.Height - textSize.Height - 2
                );
                g.DrawString(text, font, textBrush, textPos);
            }
        }

        private void DrawPowerUp(Graphics g, PowerUp powerUp)
        {
            var bounds = powerUp.Bounds;
            
            // Draw power-up background
            using var brush = new SolidBrush(powerUp.Color);
            g.FillRectangle(brush, bounds);
            
            // Draw border
            using var pen = new Pen(Color.White, 2);
            g.DrawRectangle(pen, bounds);
            
            // Draw symbol
            using var font = new Font("Arial", 12, FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.White);
            var textSize = g.MeasureString(powerUp.Symbol, font);
            var textPos = new PointF(
                bounds.X + (bounds.Width - textSize.Width) / 2,
                bounds.Y + (bounds.Height - textSize.Height) / 2
            );
            g.DrawString(powerUp.Symbol, font, textBrush, textPos);
        }

        private void DrawPowerUpStatus(Graphics g)
        {
            var y = 40;
            using var font = new Font("Arial", 10, FontStyle.Bold);
            
            if (PowerUpManager.Instance.SlowMotionActive)
            {
                using var brush = new SolidBrush(Color.Cyan);
                g.DrawString("⏰ SLOW MOTION", font, brush, new Point(10, y));
                y += 20;
            }
            
            if (PowerUpManager.Instance.DoublePointsActive)
            {
                using var brush = new SolidBrush(Color.Orange);
                g.DrawString("2X DOUBLE POINTS", font, brush, new Point(10, y));
                y += 20;
            }
        }

        private void OnScoreChanged(int score)
        {
            if (InvokeRequired)
            {
                Invoke(() => _scoreLabel.Text = $"Score: {score}");
            }
            else
            {
                _scoreLabel.Text = $"Score: {score}";
            }
        }

        private void OnLivesChanged(int lives)
        {
            if (InvokeRequired)
            {
                Invoke(() => _livesLabel.Text = $"Lives: {lives}");
            }
            else
            {
                _livesLabel.Text = $"Lives: {lives}";
            }
        }

        private void OnTimeChanged(float time)
        {
            if (InvokeRequired)
            {
                Invoke(() => _timeLabel.Text = $"Time: {Math.Ceiling(time)}");
            }
            else
            {
                _timeLabel.Text = $"Time: {Math.Ceiling(time)}";
            }
        }

        private void OnGameOver(int finalScore)
        {
            if (InvokeRequired)
            {
                Invoke(() => ShowGameOver(finalScore));
            }
            else
            {
                ShowGameOver(finalScore);
            }
        }

        private void ShowGameOver(int finalScore)
        {
            _gameTimer.Stop();
            
            bool isNewHighScore = HighScoreManager.Instance.TrySetNewHighScore(finalScore);
            
            string gameOverText = $"Game Over!\nFinal Score: {finalScore}";
            if (isNewHighScore)
            {
                gameOverText += "\n🎉 NEW HIGH SCORE! 🎉";
                _gameOverLabel.ForeColor = Color.Gold;
            }
            else
            {
                gameOverText += $"\nHigh Score: {HighScoreManager.Instance.HighScore}";
                _gameOverLabel.ForeColor = Color.White;
            }
            
            _gameOverLabel.Text = gameOverText;
            _highScoreLabel.Text = $"High Score: {HighScoreManager.Instance.HighScore}";
            _gameOverPanel.Visible = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _gameTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}