# Kill The Frog - Standalone C# Game

A simple 2D "Kill the Frog" game built with C# Windows Forms, converted from the original Unity project.

## Features

- **Visual Gameplay**: Click on frogs before they disappear to score points
- **Multiple Frog Types**: 
  - 🟢 **Normal Frogs** (10 pts) - Standard green frogs
  - 🔴 **Fast Frogs** (15 pts) - Red frogs that disappear quickly  
  - 🔵 **Slow Frogs** (5 pts) - Blue frogs that last longer
  - 🟡 **Golden Frogs** (50 pts) - Rare, high-value frogs with pulsing animation
- **Power-Up System**:
  - ⏰ **Slow Motion** - Slows down frog spawning and expiration
  - 2X **Double Points** - Double score for 10 seconds
  - ♥ **Extra Life** - Gain an additional life
- **Sound Effects**: Beep sounds for kills, misses, game over, and game start
- **High Score Persistence**: Saves your best score automatically
- **Progressive Difficulty**: Game gets harder every 15 seconds (Wave system)
- **Animated Graphics**: Death animations, pulsing golden frogs, fade effects
- **Lives System**: Start with 3 lives - miss too many frogs and the game ends
- **Timer**: 60-second game duration
- **Score System**: Points increase with difficulty waves for bonus scoring

## Requirements

- .NET 8.0 or later
- Windows operating system (uses Windows Forms)

## How to Build and Run

### Quick Start
```powershell
# Build the game
dotnet build

# Run the game
dotnet run
```

### Create Release Version
```powershell
# Build optimized release
dotnet build -c Release

# Create standalone executable (no .NET required on target machine)
dotnet publish -c Release -r win-x64 --self-contained true -o ./release
```

## How to Play

1. Click "Start Game" to begin
2. Different colored frogs will appear randomly on the screen:
   - **Green** = 10 points (normal)
   - **Red** = 15 points (fast, disappears quickly)
   - **Blue** = 5 points (slow, stays longer)  
   - **Gold** = 50 points (rare, pulses, very quick)
3. Click on frogs before they disappear to score points
4. Collect power-ups (colored squares):
   - **Cyan (⏰)** = Slow motion mode
   - **Orange (2X)** = Double points
   - **Pink (♥)** = Extra life
5. Survive increasing difficulty waves - frogs get smaller and faster!
6. You have 3 lives - missing frogs will cost you a life
7. Game lasts 60 seconds - try to get the highest score possible!

## Project Structure

```
KillTheFrog/
├── src/
│   ├── Program.cs         # Entry point
│   ├── GameForm.cs        # Main game window and UI
│   ├── GameManager.cs     # Game state and logic management
│   ├── Frog.cs           # Individual frog logic and types
│   ├── SpawnManager.cs   # Handles frog and power-up spawning
│   ├── SoundManager.cs   # Sound effects system
│   ├── HighScoreManager.cs # High score persistence
│   ├── PowerUp.cs        # Power-up system and manager
│   └── DifficultyManager.cs # Progressive difficulty system
├── bin/                   # Build output (after building)
├── KillTheFrog.csproj    # Project file
├── SimpleConsoleGame.cs  # Bonus console version
├── build.bat             # Build script
└── README_Standalone.md  # Detailed documentation
```

## Console Version

For a simple text-based version, you can compile and run `SimpleConsoleGame.cs`:

```powershell
# Compile console version
csc SimpleConsoleGame.cs /out:KillTheFrogConsole.exe

# Or copy the code to an online compiler like https://dotnetfiddle.net/
```

## Game Mechanics

### Scoring System
- **Normal Frogs**: 10 points (+ wave bonus)
- **Fast Frogs**: 15 points (+ wave bonus)  
- **Slow Frogs**: 5 points (+ wave bonus)
- **Golden Frogs**: 50 points (+ wave bonus)
- **Wave Bonus**: 10% extra points per wave (Wave 1 = 100%, Wave 2 = 110%, etc.)
- **Power-Up Bonus**: Double points when 2X power-up is active

### Lives & Timing
- **Lives**: Start with 3 lives, lose 1 for each missed frog
- **Frog Lifetimes**: 
  - Normal: 2 seconds (reduced by difficulty)
  - Fast: 1.5 seconds (reduced by difficulty)
  - Slow: 3 seconds (reduced by difficulty)  
  - Golden: 1 second (reduced by difficulty)
- **Game Duration**: 60 seconds total

### Difficulty Progression
- **Wave System**: New wave every 15 seconds
- **Spawn Rate**: Frogs spawn faster each wave (up to 3x speed)
- **Frog Size**: Frogs get smaller each wave (minimum 25 pixels)
- **Frog Speed**: Frogs disappear faster each wave (minimum 0.5 seconds)

### Power-ups
- **Spawn Rate**: Every 15 seconds
- **Duration**: Power-ups last 5 seconds on screen, effects last 10 seconds
- **Types**: Slow Motion, Double Points, Extra Life (random selection)

## Development

This project was converted from Unity to standalone C# to remove Unity dependencies. Key changes:

- Unity's MonoBehaviour → Standard C# classes
- Unity UI → Windows Forms controls
- Unity's Update loop → Timer-based updates
- Unity's Input system → Windows Forms mouse events
- Unity GameObjects → Custom game entities

## Future Enhancements

✅ **Completed Features:**
- ✅ Sound effects (using System.Media/Console.Beep)
- ✅ Animated sprites and visual effects (death animations, pulsing)
- ✅ Different frog types with varying point values (4 types)
- ✅ Power-ups and special effects (3 power-up types)
- ✅ High score persistence (saved to user AppData)
- ✅ Difficulty progression over time (wave-based system)

🚀 **Potential Future Additions:**
- Enhanced audio with WAV files using NAudio
- More sophisticated animations and particle effects
- Additional frog types and power-ups
- Multiple game modes (Endless, Time Attack, etc.)
- Online leaderboards
- Custom themes and backgrounds
- Mobile/touch support