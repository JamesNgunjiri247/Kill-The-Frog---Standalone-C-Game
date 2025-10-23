using System;
using System.Threading;

namespace KillTheFrogConsole
{
    class Program
    {
        static int score = 0;
        static int lives = 3;
        static int timeLeft = 60;
        static bool gameRunning = false;
        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.Title = "Kill The Frog - Console Edition";
            Console.WriteLine("=== KILL THE FROG - CONSOLE EDITION ===");
            Console.WriteLine();
            Console.WriteLine("Instructions:");
            Console.WriteLine("- Frogs will appear as numbers (1-9)");
            Console.WriteLine("- Type the number and press Enter to kill the frog");
            Console.WriteLine("- You have 3 lives and 60 seconds");
            Console.WriteLine("- Miss too many frogs and you lose a life!");
            Console.WriteLine();
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
            
            StartGame();
        }

        static void StartGame()
        {
            score = 0;
            lives = 3;
            timeLeft = 60;
            gameRunning = true;

            Console.Clear();
            DisplayGameState();

            // Start timer thread
            Thread timerThread = new Thread(GameTimer);
            timerThread.Start();

            // Start frog spawning thread
            Thread spawnThread = new Thread(SpawnFrogs);
            spawnThread.Start();

            // Main game input loop
            while (gameRunning)
            {
                Console.Write("Enter frog number to kill: ");
                string input = Console.ReadLine();
                
                if (int.TryParse(input, out int frogNumber))
                {
                    if (frogNumber >= 1 && frogNumber <= 9)
                    {
                        KillFrog(frogNumber);
                    }
                }
            }

            // Wait for threads to finish
            timerThread.Join();
            spawnThread.Join();
            
            GameOver();
        }

        static void GameTimer()
        {
            while (gameRunning && timeLeft > 0)
            {
                Thread.Sleep(1000);
                timeLeft--;
                
                if (timeLeft <= 0)
                {
                    gameRunning = false;
                }
            }
        }

        static void SpawnFrogs()
        {
            while (gameRunning)
            {
                Thread.Sleep(random.Next(1000, 3000)); // Spawn every 1-3 seconds
                
                if (gameRunning)
                {
                    int frogNumber = random.Next(1, 10);
                    Console.WriteLine($"\\nFROG SPOTTED! Number: {frogNumber}");
                    
                    // Give player 2 seconds to respond
                    Thread.Sleep(2000);
                    
                    if (gameRunning)
                    {
                        Console.WriteLine($"Frog {frogNumber} escaped! Life lost!");
                        lives--;
                        
                        if (lives <= 0)
                        {
                            gameRunning = false;
                        }
                        
                        DisplayGameState();
                    }
                }
            }
        }

        static void KillFrog(int frogNumber)
        {
            score += 10;
            Console.WriteLine($"SPLAT! Frog {frogNumber} killed! +10 points");
            DisplayGameState();
        }

        static void DisplayGameState()
        {
            Console.WriteLine($"\\n--- SCORE: {score} | LIVES: {lives} | TIME: {timeLeft}s ---");
        }

        static void GameOver()
        {
            Console.Clear();
            Console.WriteLine("=== GAME OVER ===");
            Console.WriteLine($"Final Score: {score}");
            Console.WriteLine($"Lives Remaining: {lives}");
            Console.WriteLine();
            
            if (lives > 0)
            {
                Console.WriteLine("Time's up! Well played!");
            }
            else
            {
                Console.WriteLine("You ran out of lives! Better luck next time!");
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}