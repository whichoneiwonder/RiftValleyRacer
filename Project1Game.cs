// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.UI.Input;
using Windows.UI.Core;
using Windows.Devices.Sensors;
namespace Project
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class Project1Game : Game
    {
        // BEWARE: THE NUMBER OF BINARY FILES SERIALISED TO DISK = 4 TO THE POWER OF tSizeFactor-cSizeFactor. BE CAREFUL! Default difference = 5.
        const int tSizeFactor = 10,    // Sets terrain size. Set between 7 and 13.               Default = 11.
                    cSizeFactor = 5,     // Sets chunk size.   Set between 4 and 8.                Default = 6.
                    loadGridSize = 5;     // Sets width of loaded chunk grid. MUST BE ODD.          Default = 3.
        int scale = (int)Math.Pow(2, tSizeFactor / 2);            // Sets the downsize increment for the fractal array.
        const float tRangeFactor = 1f,    // Sets overall landscape height.  Set between 0.1 and 2. Default = 1.
                    smoothing = 2.1f;  // Sets how much land is smoothed. Set between 1.5 and 3. Default = 2.1.

        private int chunkWidth = (int)Math.Pow(2, cSizeFactor) + 1;
        private int[] lastPlayerZone = { 1, 1 };
        private int[] goalStart; // This is the zone in which the player spawns.
        private GraphicsDeviceManager graphicsDeviceManager;
        
        private Dictionary<Key, GameObject> terrainGrid = new Dictionary<Key, GameObject>();
        private Terrain currentTerrainChunk;
        public static Camera camera;
        //public Cube player;
        public Racer player;
        public int score;
        public MainPage mainPage;
        public static float mouseX, mouseY;
        public Goal goal;
        public bool started = false;
        public Boolean isPaused;
        /// <summary>
        /// Initializes a new instance of the <see cref="Project1Game" /> class.
        /// </summary>
        public Project1Game(MainPage mainPage)
        {

            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Generate a new world and serialise it into loadable chunks.
            FractalTools.GenerateTerrainChunks(tSizeFactor, tRangeFactor, cSizeFactor, smoothing);
            this.mainPage = mainPage;
        }

        protected override void Initialize()
        {
            Window.Title = "Rift Valley Racer";
            isPaused = false;
            // Set player spawn zone to be the landscape centre, maximising exploration.
            lastPlayerZone[0] = lastPlayerZone[1] = (int)Math.Pow(2, tSizeFactor - cSizeFactor) / 2;
            int xPos = lastPlayerZone[0] * chunkWidth + chunkWidth / 2,
                zPos = lastPlayerZone[1] * chunkWidth + chunkWidth / 2;

            // Initialise camera and player in the correct zone.
            camera = new Camera(this);
            player = new Racer(this, new Vector3(xPos, 10, zPos), "HoverBike4");
            int temp = FractalTools.N - FractalTools.chunkN;
            
            
            goal = new Goal(this, new Vector3(temp, (float)FractalTools.fractal[temp, temp], temp));


            // Create goal.
            // The goal should be placed within (terrainWidth / scale) and (terrainHeight / scale)
            // The + 1 values are there to kind of establish the boundaries of the grid.
            // e.g. A 33x33 array has a 32x32 terrain.
            int terrainWidth = FractalTools.fractal.GetLength(0);
            int terrainHeight = FractalTools.fractal.GetLength(1);

            // Generate a smaller array to facilitate pathfinding based on a scale.
            double[,] smallArray = new double[(terrainWidth / scale) + 1, (terrainHeight / scale) + 1];

            for (int x = 0; x / scale < smallArray.GetLength(0); x += scale)
                for (int y = 0; y / scale < smallArray.GetLength(1); y += scale)
                    smallArray[(x / scale), (y / scale)] = Math.Abs(FractalTools.fractal[x, y]);

            // Pass the array into the AI class to find a path.
            // JUST SETTING SOME DUMMY VALUES HERE FOR TESTING.
            List<Vector2> path = AI.findPath(Vector2.Zero, new Vector2(terrainWidth / scale, terrainHeight / scale), smallArray);

            if (path != null)
                path.RemoveAt(0);

            // Then add the point between the end of the path and the goal.
            // If no path exists, then set a path from the initial position 
            // of the opponent and the position of the goal.
            // path.add(goalPosition);

            foreach (Vector2 node in path)
                Debug.WriteLine(node);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create an array of terrain chunks for the grid that the player can see.
            // This grid is centered on the player, and loads/unloads new chunks as the player moves.
            RebuildGrid(true);
            player.position.Y = (float)currentTerrainChunk.fractal[chunkWidth / 2, chunkWidth / 2] + 15;
            //player.velocity = new Vector3();
            // Create an input layout from the vertices
            base.LoadContent();
        }

        private int[] playerZone()
        {
            int[] playerZone = { 1, 1 };
            playerZone[0] = (int)player.position.X / chunkWidth;
            playerZone[1] = (int)player.position.Z / chunkWidth;
            return playerZone;
        }

        // Structure to describe terrain grid keys, and allow for overriding equals method
        struct Key
        {
            public int X, Z;
            public Key(int x, int z)
            {
                X = x;
                Z = z;
            }
        }

        private void RebuildGrid(bool reset = true)
        {
            int[] currentZone = playerZone();
            if (reset)
            {
                // Create a brand new terrain grid (hashset of terrain chunks), appropriate to where the player is.
                terrainGrid.Clear();
                for (int i = currentZone[0] - (loadGridSize / 2), gridX = 0; gridX < loadGridSize; i++, gridX++)
                {
                    for (int j = currentZone[1] - (loadGridSize / 2), gridZ = 0; gridZ < loadGridSize; j++, gridZ++)
                    {
                        Key key = new Key(i, j);
                        if (i == currentZone[0] && j == currentZone[1])
                        {
                            currentTerrainChunk = new Terrain(this, cSizeFactor, i, j);
                            terrainGrid.Add(key, currentTerrainChunk);
                        }
                        else { terrainGrid.Add(key, new Terrain(this, cSizeFactor, i, j)); }
                    }
                }

            }
            else
            {
                // Remove the strip of chunks now out of range, and add the approaching strip.

                // If the player has moved forwards or backwards a chunk in x:
                if (currentZone[0] - lastPlayerZone[0] != 0)
                {
                    for (int z = currentZone[1] - loadGridSize / 2; z <= currentZone[1] + loadGridSize / 2; z++)
                    {
                        Key key = new Key(lastPlayerZone[0] - ((currentZone[0] - lastPlayerZone[0]) * loadGridSize / 2), z);
                        terrainGrid.Remove(key);
                        Key key2 = new Key(currentZone[0] + ((currentZone[0] - lastPlayerZone[0]) * loadGridSize / 2), z);
                        terrainGrid.Add(key2, new Terrain(this, cSizeFactor, key2.X, key2.Z));
                    }
                }
                // If the player has moved forwards or backwards a chunk in z:
                else
                {
                    for (int x = currentZone[0] - loadGridSize / 2; x <= currentZone[0] + loadGridSize / 2; x++)
                    {
                        Key key = new Key(x, lastPlayerZone[1] - ((currentZone[1] - lastPlayerZone[1]) * loadGridSize / 2));
                        terrainGrid.Remove(key);
                        Key key2 = new Key(x, currentZone[1] + ((currentZone[1] - lastPlayerZone[1]) * loadGridSize / 2));
                        terrainGrid.Add(key2, new Terrain(this, cSizeFactor, key2.X, key2.Z));
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (started)
            {
                // Check where the player is, and replace chunks accordingly. Only replace chunks if player has changed zone.
                if ((playerZone()[0] != lastPlayerZone[0]) || (playerZone()[1] != lastPlayerZone[1]))
                {
                    lastPlayerZone = playerZone();
                    RebuildGrid();
                }

                // Go back to the main menu for testing purposes
                //  if (kb)




                //update player
                player.Update(gameTime);
                // Update camera
                camera.Update(gameTime);
                // Update each of the terrain chunks.
                foreach (KeyValuePair<Key, GameObject> chunk in terrainGrid) { if (chunk.Value != null) chunk.Value.Update(gameTime); };
            }
            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (started)
            {
                // Clears the screen with the background colour
                GraphicsDevice.Clear(camera.background);

                // Draw each of the terrain chunks.
                foreach (KeyValuePair<Key, GameObject> chunk in terrainGrid) { if (chunk.Value != null) chunk.Value.Draw(gameTime); };

                player.Draw(gameTime);
            }
            // Handle base.Draw
            base.Draw(gameTime);
        }

        public GameObject getTerrainChunkUnderPlayer()
        {
            return currentTerrainChunk;
        }

    }


}
