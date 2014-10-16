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

using System;

using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class Project1Game : Game
    {
        // BEWARE: THE NUMBER OF BINARY FILES SERIALISED TO DISK = 4 TO THE POWER OF tSizeFactor-cSizeFactor. BE CAREFUL! Default difference = 5.
        const int   tSizeFactor  = 11,    // Sets terrain size. Set between 7 and 13. Default = 11.
                    cSizeFactor  = 6,     // Sets chunk size.   Set between 4 and 8.  Default = 6.
                    loadGridSize = 3;
        const float tRangeFactor = 1f,    // Sets overall landscape height.  Set between 0.1 and 2. Default = 1.
                    smoothing    = 2.1f;  // Sets how much land is smoothed. Set between 1.5 and 3. Default = 2.1.

        private int chunkWidth = (int)Math.Pow(2, cSizeFactor)+1;
        private int[] lastPlayerZone = {1, 1}; // This is the zone in which the player spawns.
        private GraphicsDeviceManager graphicsDeviceManager;
        private GameObject[,] terrainGrid;
        public static Camera camera;
        public Cube player;
        
        public static float mouseX, mouseY;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project1Game" /> class.
        /// </summary>
        public Project1Game()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Generate a new world and serialise it into loadable chunks.
            FractalTools.GenerateTerrainChunks(tSizeFactor, tRangeFactor, cSizeFactor, smoothing);
        }

        protected override void Initialize()
        {
            Window.Title = "Project 1";

            // Set Player spawn zone to be the landscape centre, maximising exploration.
            lastPlayerZone[0] = lastPlayerZone[1] = (int)Math.Pow(2, tSizeFactor-cSizeFactor)/2;
            player = new Cube(this, gameTime);
            camera = new Camera(this);
            // Set camera to begin in the zone of the player. When Player class is implemented, this should be changed.
            int xPos = lastPlayerZone[0]*chunkWidth+chunkWidth/2,
                zPos = lastPlayerZone[1]*chunkWidth+chunkWidth/2;

            base.Initialize();
        }

        protected override void LoadContent() {

            // Create an array of terrain chunks for the grid that the player can see.
            // This grid is centered on the player, and loads/unloads new chunks as the player moves.
            RebuildGrid();
            Terrain t = (Terrain)terrainGrid[1, 1];
            player.position = new Vector3((float)lastPlayerZone[0]*chunkWidth+chunkWidth/2,
                (float)t.fractal[chunkWidth/2, chunkWidth/2]+15, 
                (float)lastPlayerZone[1]*chunkWidth+chunkWidth/2);
            player.velocity = new Vector3();
            // Create an input layout from the vertices
            base.LoadContent();
        }

        private int[] playerZone() {
            int[] playerZone = {1, 1};
            //  Until Player class is implemented, just return the zone of the camera.
            playerZone[0] = (int)player.position.X/chunkWidth;
            playerZone[1] = (int)player.position.Z/chunkWidth;
            return playerZone;
        }

        private void RebuildGrid() {
            // Create a new terrain grid of chunks, appropriate to where the player is.
            terrainGrid = new GameObject[loadGridSize, loadGridSize];
            for (int i = lastPlayerZone[0]-1, gridX = 0; gridX < loadGridSize; i++, gridX++) {
                for (int j = lastPlayerZone[1]-1, gridZ = 0; gridZ < loadGridSize; j++, gridZ++) {
                    terrainGrid[gridX, gridZ] = new Terrain(this, cSizeFactor, i, j);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            
            
            // Check where the player is, and replace chunks accordingly. Only replace chunks if player has changed zone.
            if ((playerZone()[0] != lastPlayerZone[0]) || (playerZone()[1] != lastPlayerZone[1])) {
                lastPlayerZone = playerZone();
                RebuildGrid();
            }
            //update player
            player.Update(gameTime);

            // Update camera
            camera.Update(gameTime);
            

            // Update each of the terrain chunks.
            foreach (GameObject chunk in terrainGrid) { if (chunk != null) chunk.Update(gameTime); };



            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the background colour
            GraphicsDevice.Clear(camera.background);

            // Draw each of the terrain chunks.
            foreach (GameObject chunk in terrainGrid) { if (chunk != null) chunk.Draw(gameTime); };
            player.Draw(gameTime);
            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
