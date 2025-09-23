# Save the Cat 
A 2D top-down shooter game developed in Unity featuring infinite map generation, weapon progression, and narrative storytelling.

## 🚀 Core Features

### 🗺️ Infinite Map System
- **InfiniteMap**: Generates map chunks in a 3x3 grid around the player
- **InfiniteChildMover**: Automatically repositions objects for seamless infinite scrolling
- Dynamic object management based on player distance
- Optimized performance with configurable thresholds

### ⚔️ Weapon & Combat System
Multiple weapon types with distinct mechanics:
- **Ranged Weapons**: Assault Rifle, Shotgun, Hand Cannon
- **Throwing Weapons**: Projectiles with ballistic trajectories
- **Spin Weapons**: Rotating weapons around the player
- **Area Weapons**: Area-of-effect attacks

**WeaponUnlockManager** handles progressive weapon unlocking throughout the game.

### 📈 Upgrade & Progression System
Comprehensive character improvement system:
- **Player Upgrades**: Health, mobility, regeneration improvements
- **Weapon Upgrades**: Damage, fire rate, range, and weapon-specific enhancements
- **RuntimeUpgrade System**: Dynamic upgrade tracking during gameplay
- Level-up UI with upgrade selection

### 🎭 Player System
- Advanced movement with dash mechanics
- Input handling and animation integration
- Health system with damage and healing
- Persistent state across scenes

### 👾 Enemy System
Diverse AI-driven enemies:
- **Basic Enemies**: Zombies, Skeletons with chase behavior
- **Fast Variants**: Enhanced speed enemies
- **Boss Enemies**: Complex attack patterns and mechanics
- **Special Enemies**: Tombstone (ranged attacks)

### 🌊 Wave System
- Time-based or kill-count wave progression
- Dynamic difficulty scaling
- Enemy spawn management
- Wave completion rewards

### 🎬 Dialogue & Story System
- **DialogueManager**: Typewriter effect text display
- Scene-specific dialogue triggers
- Video integration for cutscenes
- Narrative progression tracking

### 🔄 Scene Management
Multiple game areas with smooth transitions:
- **Main Game Scenes**: GameScene, GameScene_second
- **Transition Scenes**: PlainToForest, ForestToPlain
- **Story Scenes**: House, SaveTheCatBlack
- **Ending Scenes**: Multiple ending variations

### 🎵 Audio System
- Centralized audio management
- Dynamic background music
- Spatial audio effects
- Volume controls

### 🎯 Special Systems
- **Drop System**: XP, health, magnet, and chest drops
- **Day/Night Cycle**: Dynamic lighting and atmosphere
- **Camera System**: Smooth player following
- **Pet System**: Companion mechanics

## 🎭 Narrative Design & Inspirations

### Story Structure
The game's **ending** draws inspiration from the tradition of **melancholy in French cinema**, creating a contemplative and emotionally resonant conclusion that contrasts with typical action game finales.

### Screenplay References
Screenplay References
The narrative structure draws on classical screenplay techniques, with a strong influence from the Save the Cat methodology, adapted to the language of interactive media.
Key references include:
* **La Black Vet** - Character development through environmental storytelling
* **Il Giuramento dell'Arco** - Tension building and narrative pacing
* **Salva il Gatto (Save the Cat)** - Story beats and emotional engagement.

These influences shape the game's approach to storytelling, emphasizing emotional depth and character-driven narrative alongside the action gameplay.

## 📁 Project Structure
```
Assets/
├── Scripts/
│   ├── combat/           # Combat mechanics
│   ├── core/             # Core game systems
│   ├── StoryTransition/  # Narrative transitions
│   └── enums/           # Game enumerators
├── Scenes/              # Unity scenes
├── Prefabs/             # Reusable prefabs
└── Settings/            # Project settings
```

## 🎮 Gameplay

### Controls
- **WASD**: Movement
- **Mouse**: Aim
- **Left Click**: Shoot
- **Space**: Dash
- **Click**: Advance dialogue

### Objectives
- Survive enemy waves
- Collect XP to level up
- Unlock new weapons
- Follow the storyline to save the cat
- Explore different game areas
- Experience multiple narrative endings

## 🛠️ Technical Details

### Architecture Patterns
- **Singleton**: Core managers (GameManager, AudioManager, Player)
- **Strategy**: Weapon types and upgrades
- **Observer**: Game events (UnityEvents)
- **Component-based**: Modular Unity architecture

### Performance Features
- Object pooling for projectiles and enemies
- Optimized infinite map system
- Dynamic object repositioning
- Efficient rendering management

---
**Built with Unity 2022.3 LTS**