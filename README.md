# TowerOfHanoi

The Tower of Hanoi is a mathematical game or puzzle. It consists of three rods and a number of disks of different sizes, which can slide onto any rod. The puzzle starts with the disks in a neat stack in ascending order of size on one rod, the smallest at the top, thus making a conical shape.

The objective of the puzzle is to move the entire stack to another rod, obeying the following simple rules:

Only one disk can be moved at a time.
Each move consists of taking the upper disk from one of the stacks and placing it on top of another stack or on an empty rod.
No larger disk may be placed on top of a smaller disk.

- Check samplescene.unity
- Game scene is used for testing only

How to play
------------
1. Press 0 to reduce the number of disks/peg - minimum peg count is 2
2. Press 1 to increase the number of disks/peg - maximum peg count is 10
3. Press Enter/Return key to start the game.
4. Press wasd to move the player and explore the environment

Check the game at -->
https://krhariragavan.github.io/TowerOfHanoi/

Script Details
--------------
Game.cs - Contains core game logic
Move.cs - Used to move the Disks/Peg. It uses Command pattern to implement Undo
IMove.cs - Interface for Move.cs - Implements command patter
Disk.cs - Attached to all the disks/peg
Tower.cs - Attached to all towers
PlayerMovement.cs - Used for player movement
GameManager.cs - Not used at the moment - Used for testing purpose only

WebGl Build Folder Details
---------------
Build 
Template Data
index.html

P.S.
Lighting can be better. :)