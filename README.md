# Project Overview

Here you can find the game builds and gameplay video with the link below:

https://drive.google.com/drive/folders/1opHJ28ABxIeEPJNKlehDai_gTe8fErcB?usp=sharing
---
## Project Details

### Canvas Target Resolution  
- **Resolution:** 1080x2400  
- **Platform:** Mobile  

### Unity Target Version  
- **Version:** 2022.3.53f1 (LTS)

---

## Features Used in the Project

### Core Features  
- **Object Pooling:** Efficient management of bullets and characters to optimize performance.  
- **Interfaces:**  
  - `IPoolable`: Used for defining reusable, modular behaviors in the pooling system.
  - `ISpecialPower`: Defines the structure for special powers and their behavior when added to the game.  
- **Scriptable Objects:** Game configuration data such as `GameData` and special powers are handled via scriptable objects for modularity.  
- **Zenject Dependency Injection:** Used for decoupling and better management of dependencies between managers and objects.  
- **Git LFS (Large File Storage):** Enables efficient version control for large assets like audio files or textures.  
- **DOTween:** Used for smooth animations such as UI transitions and character movement.  
- **UniTask:** For performance-oriented, lightweight asynchronous programming instead of traditional coroutines.  
- **Event-Driven Architecture:** Implemented using `Action` delegates to facilitate communication between managers and UI.  
- **CancellationToken:** Ensures proper management of async operations, avoiding potential memory leaks or unintended behavior.

---

## Gameplay Mechanics

### Game Start  
- The game begins when the "Start" button on the main menu is pressed.

### Core Gameplay  
- A static character is present on the game screen.  
- The character automatically fires bullets forward every 2 seconds.  

### Special Powers  
- **There are 5 special powers available in the game.**  

#### Special Power Effects  
1. **Extra Bullets:** Adds two additional bullets to each shot, fired at a 45-degree angle.  
2. **Quickfire:** Fires two bullets in quick succession.  
3. **Rapid Fire:** Reduces firing interval to 1 second.  
4. **Bullet Speed Boost:** Increases bullet speed by 50%.  
5. **Character Duplication:** Spawns a duplicate character with the same firing abilities at a random position.  

#### Activation  
- Players can select and activate powers by clicking the respective buttons.  
- A maximum of 3 powers can be selected in one game session.  
- Once 3 powers are selected, the remaining ones are disabled.  

### End Game Button  
- Players can end the game and return to the main menu by pressing the "End Game" button during gameplay.
