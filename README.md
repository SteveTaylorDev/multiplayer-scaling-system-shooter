# Arcade Shooter (with Scaling Systems)

One of my oldest projects, built around 2016. Based on learning materials from Unity, this project came from a desire to build much further than what the materials covered, including functionality such as:

// Health // 
Starting with a blank project, I built an arcade style shooter system with help from Unity tutorials. The first thing added beyond that was a health system, along with an indicator to the player model, which adjusts the material color to match the current health. Player attributes such as a player number and color were also added, which could be reassigned or adjusted during gameplay.

// Multiplayer // 
Drop-in/drop-out multiplayer was added, with a game manager singleton handling each, which led to the start of the scaling systems; beginning with enemy health scaling, which checks for the current difficulty level and player amount, and scales enemy max health in realtime based on an initial health value.

// Weapon // 
A dedicated script for weapon behaviour attributes was introduced, which provided functionality for customizable weapon shot speed and damage during gameplay. This was built intended to be used alongside the next scaling system that was added, EXP.

// Exp // 
Each player is assigned two exp values, passive exp and active exp, with each being spawned a different rates when hitting an enemy. The damage done determines how much of each will spawn. Exp is spawned when hitting an enemy, and attracts directly to the player. When an enemy is defeated, it spawns an equal amount of exp for each player (scaled with the enemy max health), then sets that exp to attract to its respective player, ignoring the others. It will also spawn exp that will float through space until a player gets close enough. This allows for a balanced multiplayer experience, with no player missing out on exp, but still allowing the chance to gain more depending on shots landed.

// Exp Increments // 
To keep within a reasonable limit of active rigidbodies for performance reasons, I added exp types of higher values, to be spawned when an enemy with very high health was defeated. This meant exp could be spawned with values of 5, 10, 50 etc. while keeping the same amount of physics objects active (or less).

// Object Pooling // 
One of the last things added to this project was object pooling, specifically to help with performance issues due to the large amounts of exp and bullets that can be spawned into the scene. This was handled by a game manager singleton, which instantiates a specified amount of each exp object type at runtime, sets each one's active state to false, and adds a reference to the respective pooled object list. Whenever an exp object is spawned, it will now search through the desired list for an inactive object reference, set its position and contents to the desired values, and set its active state to true.

My next intention with this project was to allow for weapons to be upgraded during gameplay based on current active exp, however I moved on to other projects, since I felt, rather than refactor issues with the current code, I could do a better job building something from the ground up, having gained a stronger understanding of C# and Unity.
