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
using SharpDX.Toolkit.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.UI.Input;
using Windows.UI.Core;
using Windows.Devices.Sensors;
using Windows.Media;

namespace Project
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class Project1Game : Game
    {
        // BEWARE: THE NUMBER OF BINARY FILES SERIALISED TO DISK = 4 TO THE POWER OF tSizeFactor-cSizeFactor. BE CAREFUL! Default difference = 5.
        public int  tSizeFactor = 11;        // Sets terrain size. Set between 7 and 13.               Default = 11.
        const int   cSizeFactor = 6,         // Sets chunk size.   Set between 4 and 8.                Default = 6.
                    loadGridSize = 5;        // Sets width of loaded chunk grid. MUST BE ODD.          Default = 3. 
        const float tRangeFactor = 1.2f,     // Sets overall landscape height.  Set between 0.1 and 2. Default = 1.
                    smoothing = 2.17f;       // Sets how much land is smoothed. Set between 1.5 and 3. Default = 2.1.

        
        private GraphicsDeviceManager graphicsDeviceManager;
        private int chunkWidth = (int)Math.Pow(2, cSizeFactor) + 1;
        private int[] lastPlayerZone = { 1, 1 };
        private Dictionary<Key, GameObject> terrainGrid = new Dictionary<Key, GameObject>();
        private Terrain currentTerrainChunk;
        public SystemMediaTransportControls bgMusic;
        public Goal goal;
        public Racer player, opponent;
        public MainPage mainPage;
        public string racer_won;
        public bool started = false, isPaused;
        public static Camera camera;
        public static Vector2 goalStart;
        public static List<Vector2> opponentPath;
        public static bool gameEnd = false;
        public static int keyBoardInputDirection = 0, accel = 0;
        public double elapsedTime = 0;
        public int opponent_difficulty = 10;

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
            // Commented this out: FractalTools.GenerateTerrainChunks(tSizeFactor, tRangeFactor, cSizeFactor, smoothing);
            this.mainPage = mainPage;
        }

        protected override void Initialize()
        {
            
            Window.Title = "Rift Valley Racer";
            isPaused = false;
            FractalTools.GenerateTerrainChunks(tSizeFactor, tRangeFactor, cSizeFactor, smoothing);
           
            

            // Set player spawn zone to be the landscape centre, maximising exploration.
            lastPlayerZone[0] = lastPlayerZone[1] = (int)Math.Pow(2, tSizeFactor - cSizeFactor) / 2;
            int xPos = lastPlayerZone[0] * chunkWidth + chunkWidth / 2,
                zPos = lastPlayerZone[1] * chunkWidth + chunkWidth / 2;

            // Sets the downsize increment for the fractal array.
            int scale = (int)Math.Pow(2, tSizeFactor / 2);       
            
            // Initialise camera and player in the correct zone.
            camera = new Camera(this);
            player = new Racer(this, new Vector3(xPos, (float)FractalTools.fractal[zPos, xPos] + 0f, zPos), mainPage.modelToLoad);
            
            // AI Racer objects
            opponent = new Racer(this, new Vector3(xPos+10, (float)FractalTools.fractal[zPos+1, xPos+10] + 0f, zPos+1), "HoverBike4");
            opponent.opponent = true;

            goalStart = (FractalTools.N-chunkWidth-chunkWidth) * Vector2.One;
            goal = new Goal(this, new Vector3(goalStart.X, (float)FractalTools.fractal[(int)goalStart.Y, (int)goalStart.X] + 2f, goalStart.Y));
            
            // Create goal.
            // The goal should be placed within (terrainWidth / scale) and (terrainHeight / scale)
            // The + 1 values are there to kind of establish the boundaries of the grid.
            // e.g. A 33x33 array has a 32x32 terrain.

            // Generate a smaller array to facilitate pathfinding based on a scale.
            double[,] smallArray = new double[(FractalTools.N / scale) + 1, (FractalTools.N / scale) + 1];

            for (int x = 0; x / scale < smallArray.GetLength(0); x += scale)
                for (int y = 0; y / scale < smallArray.GetLength(1); y += scale)
                    smallArray[(x / scale), (y / scale)] = Math.Abs(FractalTools.fractal[x, y]);

            // Pass the array into the AI class to find a path.
            Vector2 opponentPosition = new Vector2(opponent.position.X, opponent.position.Z);
            opponentPath = AI.findPath(opponentPosition / scale, goalStart / scale, smallArray);

            // Remove the initial point from the path to get it ready for the opponent, and ensure it is non-null
            if (opponentPath != null) { Project1Game.opponentPath.RemoveAt(0); }
            Project1Game.opponentPath.RemoveAt(0);


            // Scale all path points back to approximate where they actually are in the terrain
            for (int i = 0; i < opponentPath.Count; i++) {
                opponentPath[i] *= scale;
            }

            // Set the final point for the opponent to reach as the goal itself
            opponentPath.Add(goalStart);

             
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create an array of terrain chunks for the grid that the player can see.
            // This grid is centered on the player, and loads/unloads new chunks as the player moves.
            RebuildGrid(true);
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

        private void RebuildGrid(bool reset = false)
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

        protected override void Update(GameTime gameTime) {
           
            if (started && isPaused == false) {
                // Check where the player is, and replace chunks accordingly. Only replace chunks if player has changed zone.
                if ((playerZone()[0] != lastPlayerZone[0]) || (playerZone()[1] != lastPlayerZone[1])) {
                    RebuildGrid();
                    lastPlayerZone = playerZone();
                }

                //update player
                player.Update(gameTime);
                player.goalDistance =  (goal.position  - player.position ).Length();

                // Update oppponent
                opponent.Update(gameTime);
                opponent.goalDistance = (goal.position - opponent.position).Length();

                if (player.goalDistance < opponent.goalDistance)
                {
                    mainPage.first();
                } else { mainPage.second(); }
                
                // Update camera
                camera.Update(gameTime);
                goal.Update(gameTime);
                mainPage.Seek();

                if (Math.Abs(player.position.X - goal.position.X) <= 25f && Math.Abs(player.position.Z - goal.position.Z) <= 1f) {
                    Debug.WriteLine("PLAYER WON");
                    gameEnd = true;
                    racer_won = "player";
                    elapsedTime = Math.Round(this.gameTime.TotalGameTime.TotalSeconds);
                    mainPage.finish.FinishUpdate(racer_won);
                    // Ends the game by showing the finished screen (DEFEAT or VICTORY)
                    mainPage.EndGame();
                }

                if (Math.Abs(opponent.position.X - goal.position.X) <= 10f && Math.Abs(opponent.position.Z - goal.position.Z) <= 1f) {
                    Debug.WriteLine("OPPONENT WON");
                    gameEnd = true;
                    racer_won = "opponent";
                    elapsedTime = Math.Round(this.gameTime.TotalGameTime.TotalSeconds);
                    mainPage.finish.FinishUpdate(racer_won);
                    mainPage.EndGame();
                }

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

                // If the player should be able to see the goal, draw it
                if (Math.Abs(player.position.X - goalStart.X) < chunkWidth*2 &&
                    Math.Abs(player.position.Z - goalStart.Y) < chunkWidth*2) {
                    goal.Draw(gameTime);
                }

                player.Draw(gameTime);
                opponent.Draw(gameTime);
            }
            // Handle base.Draw
            base.Draw(gameTime);
        }

        public void formatTime(float secs)
        {
            TimeSpan t = TimeSpan.FromSeconds(secs/10);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                            t.Hours,
                            t.Minutes,
                            t.Seconds,
                            t.Milliseconds);
        }

        public GameObject getTerrainChunkUnderPlayer()
        {
            return currentTerrainChunk;
        }

    }


}
