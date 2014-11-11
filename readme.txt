RIFT VALLEY RACER
=================

Authors
-------

Adhy Karina
James McMahon
Aston Howindt
Steven Spratley
Viet Nguyen

Basic Overview
--------------

Rift Valley Racer is a single player game, where the aim of the game is to beat the other AI players to a randomly  
generated goal. The goal is designated with a spinning pyramid, and if the player gets within a certain distance of the  
goal before the AI racer, he/she has won.

One of the key aspects of Rift Valley racer is that the terrain for this game is generated procedurally for each iteration  
of the game. This means that each experience will be completely different, with a different look and feel for the terrain.  
The goal will still be the same, but the obstacles and difficulties will change.

The software was implemented using SharpDX on Windows 8 Desktop and mobile devices.


Terrain
-------

The terrain is initially created at the point when the game is loaded using the diamond square algorithm. This produces a  
height map, which represents the height of each point on the terrain.
This terrain, once initialised, is then split into several chunks, which are loaded and unloaded throughout the game  
depending on where the player is. For example, the player may only be surrounded by 9 chunks at any given moment, when  
there are many more than that for the entire terrain.

This was done so that the player would have a large area to explore and race through, while also maintaining a reasonable  
frame rate. The fog effect, discussed later, allowed the terrain chunks to be loaded seamlessly with little visual cue that  
the loading was taking place.



User Interface
--------------

For mobile devices (in particular the Samsung slate, which was the platform this software was developed on), the use of  
touch and accelerometers was use:
	- Two XAML buttons for "Thrust" and "Reverse": these controlled the acceleration parameter of the player’s racer
	- Accelerometer readings were used for the steering to turn left or right.

For the desktop, the traditional use of keyboard and mouse input was used:
	- 'Up' to move forward
	- 'Down' to move backwards
	- 'Left' to turn left
	- 'Right' to turn right


Menu
----

Both mobile and desktop utilised a XAML menu system, in keeping with Windows 8 styles. 
The XAML section of this game features a menu system which has an Instructions page, an Options page, and a Main Menu page,  
where the previous pages can be accessed or the game can be started straight away.

The instructions page details the controls for both the Desktop and the mobile platforms, with more emphasis placed on  
mobile devices since this software was created for the Samsung Slate tablet.
The options page allows players to choose their difficulty, which changes the performance of the AI racer, the colour of  
HoverBike they wish to use, and whether they wish to have the background music playing.

When the game is started, there is also a button which can be pressed that will bring up the pause page, allowing players  
to exit. Unfortunately, the options are not able to be changed from this page.

Models
------

The models used (i.e. HoverBikes) were created from scratch by the team using Blender from drawings produced by the team as  
well. None of the game objects implemented in this software was taken from external sources: they were all produced by the  
team specifically for the Rift Valley Racer.


Shading and Lighting
--------------------

For the terrain, a custom Gouraud shader was used to achieve a smooth lighting effect while also minimising the  
computational cost. It was decided that the Phong shading would be too costly for such a large object within the game. 
Ideally, the Phong effect would have been used on the HoverBikes, but due to difficulties encountered with the SharpDX  
documentation, as well as time pressures, normally model rendering was used for these.

In addition the custom Gouraud shader, the fog effect was implemented in this effect file (TerrainShader.fx). This  
meant that the terrain, at a specified distance away from the camera, would fade into the background colour, obscuring the  
loading of terrain chunks mentioned above.


Artificial Intelligence
-----------------------

For the artificial intelligence, an A* pathfinding algorithm was used. This would find an optimal path from a start position 
to an end position with a reasonable amount of computation required by the tablet.

Due to concerns that the pathfinding algorithm would produce results that were too good (i.e. making it impossible for the 
user to actually win), the values passed into the pathfinding algorithm were of a significantly condensed form of the array 
used to construct the terrain, in order to ensure that the AI will not always choose the most optimal path to the goal based 
on the terrain's effects on speed.

Only one AI racer was implemented in the final version of the game, but ideally there would be possibly three different AI  
racers, which could have varying degrees of performance. This could prove more challenging for the player than just playing  
against one AI opponent.


Physics
-------
Uses surface normals to alter an acceleration constant, and uses simple motion equations to work out position and velocity.

The surface normals create a 'bounce' for the racer, so the bounce of the surace instead of flying just above it constantly.
This provides the oportunity to have large jumps, where significant amounts for terrain can be traveresed, while also making 
if difficult to go up steep hills.

The basic physics equations included the use of a reduced gravity to simulate the big jumps and reduce the bounce when falling down.


Thanks
------

Firstly, thanks go to Chris Ewin. Without his help and guidance, we would not have been able to do as much as we did, let  
alone anything at all. In addition, the shader files that were used in this software were adapted from files provided by  
Chris.
Secondly, thanks go to the entire team. Although it was probably not the results we were hoping for at the start of this  
process, the hard work by everyone was really appreciated.
