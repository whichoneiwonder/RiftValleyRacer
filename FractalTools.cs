using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Direct3D11;
using System.Runtime.Serialization.Formatters.Binary; 

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class FractalTools {

        // Generates a large fractal before subdividing it and serialising those divisions to binary files.
        public static void GenerateTerrainChunks(int terrainSizeFactor, float terrainRangeFactor, int chunkSizeFactor){
            int numberOfChunks = (int)Math.Pow(4, terrainSizeFactor-chunkSizeFactor),
                chunkWidth     = (int)Math.Pow(2, chunkSizeFactor)+1,
                terrainWidth   = (int)Math.Pow(2, terrainSizeFactor)+1, 
                terrainRange   = (int)(terrainRangeFactor*terrainWidth/2.0);

            // Generate fractal using the diamond-square algorithm
            double[,] fractal = DiamondSquare(terrainWidth, terrainRange);

            // Iterate over the fractal, serialising it off as square chunks that can be loaded later
            // Begin by iterating chunk by chunk
            for (int yStart=0; yStart<terrainWidth-1; yStart+=chunkWidth-1) {
                for (int xStart=0; xStart<terrainWidth-1; xStart+=chunkWidth-1) {

                    // Create a sub-fractal that will represent a chunk, and copy across every cell 
                    double[,] chunk = new double[chunkWidth, chunkWidth];
                    for (int y = yStart, i = 0; i < chunkWidth; y++, i++) {
                        for (int x = xStart, j = 0; j < chunkWidth; x++, j++) {
                            chunk[i, j] = fractal[y, x];
                        }
                    }

                    // Chunk is copied, ready to be serialised to disk
                    SerialiseFractal(chunk, xStart, yStart);
                }
            }
        }

        // Declare random number generator
        static Random random, seed = new Random();

        // The diamond-square algorithm, used to create a fractal array.
        private static double[,] DiamondSquare(int N, double initialRange) {
            // Seed random generator and set range
            random = new Random(seed.Next());
            double range = initialRange, average;

            // Initialise fractal array
            double[,] fractal = new double[N, N];

            // Set corner values to create the first square
            fractal[0, 0] = fractal[0, N-1] = fractal[N-1, 0] = fractal[N-1, N-1] = 0.0;

            // Step through each diamond/square in the fractal and set centres
            // When all diamonds/squares have been looked at, halve step size and range
            for (int step = N-1; step > 1; step /= 2, range /= 2) {
                // Look at squares and set centres (creating diamonds)
                for (int row = 0; row < N-1; row += step) {
                    for (int col = 0; col < N-1; col += step) {
                        // Average corner values
                        average = (fractal[row, col] +                   // top left
                                   fractal[row, col + step] +            // top right
                                   fractal[row + step, col] +            // bottom left
                                   fractal[row + step, col + step])/4.0; // bottom right

                        // Set centre value of the square to its new height.
                        fractal[row + step/2, col + step/2] = newHeight(range, average);
                    }
                }
                // Look at diamonds and set centres (creating squares)
                for (int row = 0; row < N-1; row += step) {
                    for (int col = 0; col < N-1; col += step) {
                        // Variables for corners and centre, to make things faster
                        double  TL = fractal[row, col],
                                TR = fractal[row + step, col],
                                BL = fractal[row, col + step],
                                BR = fractal[row + step, col + step],
                                C  = fractal[row + step/2, col + step/2];

                        // Set the centre values of the four surrounding diamonds to their new heights.
                        // Regarding averages, average the surrounding three values if the fourth doesn't exist (i.e. if coordinate lies on an edge).

                        // Left
                        average = row <= 0 ? (TL+BL+C)/3.0 : (TL+BL+C+fractal[row - step/2, col + step/2])/4.0;
                        fractal[row, col + step/2] = newHeight(range, average);
                        // Right
                        average = row >= N-1-step ? (TR+BR+C)/3.0 : (TR+BR+C+fractal[row + step + step/2, col + step/2])/4.0;
                        fractal[row + step, col + step/2] = newHeight(range, average);
                        // Top
                        average = col <= 0 ? (TL+TR+C)/3.0 : (TL+TR+C+fractal[row + step/2, col - step/2])/4.0;
                        fractal[row + step/2, col] = newHeight(range, average);
                        // Base
                        average = col >= N-1-step ? (BL+BR+C)/3.0 : (BL+BR+C+fractal[row + step/2, col + step + step/2])/4.0;
                        fractal[row + step/2, col + step] = newHeight(range, average);
                    }
                }
            }
            return fractal;
        }

        // Calculate new height value based on the current range and average.
        private static double newHeight(double range, double average) {
            return average - range/2.0 + random.NextDouble()*range;
        }

        // Serialise fractal array to binary file.
        private static void SerialiseFractal(double[,] fractal, int zoneX, int zoneY) {
            String fileName = "fractal"+zoneX+"-"+zoneY+".dat";
            BinaryFormatter bf = new BinaryFormatter();   
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);   
            bf.Serialize(fs, fractal);
            fs.Close();
        }

    }

}
