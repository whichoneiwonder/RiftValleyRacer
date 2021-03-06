﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Direct3D11;
using System.Runtime.Serialization.Formatters.Binary;

namespace Project1 {
    using SharpDX.Toolkit.Graphics;
    class Terrain : ColoredGameObject {

        // Variables for the fractal's dimensions and vertical range
        int N, vertexN;
        public double[,] fractal;

        // Vertex array to store details of every vertex of every triangle in the terrain
        VertexPositionNormalColor[] land;

        bool outOfBounds = false;

        public Terrain(Game game, int sizeFactor, int zoneX, int zoneZ) {
            // Initialise fractal heightmap and vertex array
            N = (int)Math.Pow(2, sizeFactor)+1;
            double[,] fractal = new double[N, N];
            vertexN = (int)(6*Math.Pow((N-1), 2));
            land = new VertexPositionNormalColor[vertexN];

            // Deserialise the appropriate binary file to populate this landscape's fractal, and create vertices.
            DeserialiseFractal(zoneX, zoneZ);
            VerticesFromArray((float)N, (float)zoneX, (float)zoneZ);

            // Setup a basic effect with default parameters
            basicEffect = new BasicEffect(game.GraphicsDevice) {
                VertexColorEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
                View = Project1Game.camera.View,
                Projection = Project1Game.camera.Projection,
                World = Project1Game.camera.World
            };

            // Set ambient light to global ambient light
            basicEffect.AmbientLightColor = Project1Game.camera.ambientColour;

            // Set DirectionalLight0 to global directional light
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = Project1Game.camera.sunPosition;
            basicEffect.DirectionalLight0.DiffuseColor = Project1Game.camera.sunColour;

            // Set DirectionalLight1 to being always overhead
            basicEffect.DirectionalLight1.Enabled = true;
            basicEffect.DirectionalLight1.Direction = Vector3.Down;
            basicEffect.DirectionalLight1.DiffuseColor = new Vector3(0.1f, 0.15f, 0.2f);

            vertices = Buffer.Vertex.New(game.GraphicsDevice, land);
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        // Deserialise appropriate binary file to populate this landscape's fractal array.
        private void DeserialiseFractal(int zoneX, int zoneZ) {
            String fileName = "fractal"+(zoneX*(N-1))+"-"+(zoneZ*(N-1))+".dat";
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs;
            try {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                fractal = (double[,])bf.Deserialize(fs);
                fs.Close();
            } catch (FileNotFoundException) {
                // If there is no file, this means the game is trying to load land that is out of bounds.
                outOfBounds = true;
                fractal = new double[N, N];
                for (int i = 0; i < N; i++) {
                    for (int j = 0; j < N; j++) {
                        fractal[i, j] = 0;
                    }
                }
            }
        }

        // Iterate through fractal and set the land vertices accordingly.
        private void VerticesFromArray(float dimensions, float X, float Z) {
            float offset = dimensions-1;
            int currentVertex = 0;
            Vector3 a, b, c, d, na, nb, nc, nd;

            for (int row = 0; row < N - 1; row++) {
                for (int col = 0; col < N - 1; col++) {
                    // Create four unique vectors for two triangles (two vectors are shared)
                    a = new Vector3((float)col+offset*X, (float)fractal[row, col], (float)row+offset*Z);
                    b = new Vector3((float)col+offset*X, (float)fractal[row + 1, col], (float)row+offset*Z+1);
                    c = new Vector3((float)col+offset*X+1, (float)fractal[row, col + 1], (float)row+offset*Z);
                    d = new Vector3((float)col+offset*X+1, (float)fractal[row + 1, col + 1], (float)row+offset*Z+1);

                    // Create a normal vector for each vertex
                    if (outOfBounds) {
                        na = nb = nc = nd = Vector3.Up;
                    } else {
                        // Get normals from FractalTools, as they have been calculated across all chunks
                        na = FractalTools.vertexNormalIndex[(int)(col+offset*X), (int)(row+offset*Z)];
                        nb = FractalTools.vertexNormalIndex[(int)(col+offset*X), (int)(row+offset*Z+1)];
                        nc = FractalTools.vertexNormalIndex[(int)(col+offset*X+1), (int)(row+offset*Z)];
                        nd = FractalTools.vertexNormalIndex[(int)(col+offset*X+1), (int)(row+offset*Z+1)];
                    }

                    // Initialise six vertices (two triangles) for every square in the fractal array
                    InitVertex(a, na, ref land[currentVertex]);     // top left
                    InitVertex(b, nb, ref land[currentVertex + 1]); // bottom left
                    InitVertex(c, nc, ref land[currentVertex + 2]); // top right
                    InitVertex(c, nc, ref land[currentVertex + 3]); // top right
                    InitVertex(b, nb, ref land[currentVertex + 4]); // bottom left
                    InitVertex(d, nd, ref land[currentVertex + 5]); // bottom right
                    currentVertex += 6;
                }
            }
        }
        // Initialise a VertexPositionNormalColor instance in an array.
        private void InitVertex(Vector3 position, Vector3 normal, ref VertexPositionNormalColor vertex) {
            vertex.Position = position;
            vertex.Normal = normal;
            if (position.Y > 0) {
                vertex.Color = Color.SmoothStep(Color.SaddleBrown, Color.Snow, (float)(position.Y/(FractalTools.initialRange/2)));
            } else {
                vertex.Color = Color.SmoothStep(Color.SaddleBrown, Color.Green, (float)(-position.Y/(FractalTools.initialRange/2)));
            }
        }

        public override void Update(GameTime gameTime) {
            basicEffect.View = Project1Game.camera.View;
            basicEffect.World = Project1Game.camera.World;
            basicEffect.AmbientLightColor = Project1Game.camera.ambientColour;
            basicEffect.DirectionalLight0.Direction    = Project1Game.camera.sunPosition;
            basicEffect.DirectionalLight0.DiffuseColor = Project1Game.camera.sunColour;
        }

        public override void Draw(GameTime gameTime) {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

    }

}
