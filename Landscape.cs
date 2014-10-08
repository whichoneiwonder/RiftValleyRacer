using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    public class Landscape : GameObject
    {

        static float jitterModifier = 0.5f;
        public static float sidelength = 600.0f;
        int verticesPerSide;
        static Random randomGen;
        public float highestPoint, lowestPoint;
        static int detailLevel = 10;

        Vector3[][] pointMap ;
        Vector2[][] texturePoints;
        Vector3[][] normalMap;
        
        public Landscape(Project1Game game) : base(game)
        {
            randomGen = new Random( );
            verticesPerSide = (int)Math.Pow(2,detailLevel) +1;

            // Specify the file name of the texture being used
            textureName = "terraintexture.png";
            basicEffect.Texture = game.Content.Load<Texture2D>(textureName);
            basicEffect.TextureEnabled = true;
            basicEffect.VertexColorEnabled = false;
            basicEffect.SpecularPower = 5f;
            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                GenerateTerrainVPNTs());

        }
        

        public VertexPositionNormalTexture[] GenerateTerrainVPNTs()
        {
            
            float halfSide = sidelength/2.0f;

            
            //Diamond Square
            pointMap = new Vector3[verticesPerSide][];
            texturePoints = new Vector2[verticesPerSide][];
            normalMap = new Vector3[verticesPerSide][];
            for (int i = 0; i < verticesPerSide; i++)
            {
                pointMap[i] = new Vector3[verticesPerSide];
                texturePoints[i] = new Vector2[verticesPerSide];
                normalMap[i] = new Vector3[verticesPerSide];
            }

            //indices are in X,Z order
            //bottom left
            pointMap[0][0] = new Vector3(-halfSide, 0.50f, -halfSide);
            texturePoints[0][0] = new Vector2(.5f, 0.5f);
            //bottom right
            pointMap[verticesPerSide - 1][0] = new Vector3(halfSide, 0.50f, -halfSide);
            texturePoints[verticesPerSide - 1][0] = new Vector2(.5f, 0.5f);
            //top left
            pointMap[0][verticesPerSide - 1] = new Vector3(-halfSide, 0.50f, halfSide);
            texturePoints[0][verticesPerSide - 1] = new Vector2(.5f, 0.5f);
            //top right
            pointMap[verticesPerSide - 1][verticesPerSide - 1] = new Vector3(halfSide, .50f, halfSide);
            texturePoints[verticesPerSide - 1][verticesPerSide - 1] = new Vector2(.5f, 0.5f);


            //fill pointmap with diamond-square
            for (int spacing = verticesPerSide-1 ; spacing > 1; spacing /= 2)
            {
                fillSquare(pointMap, spacing);
            }

            float scaling = sidelength/verticesPerSide;

            for (int i = 0; i <= pointMap.Length - 1; i++)
            {
                for (int j = 0; j <= pointMap[0].Length - 1; j++)
                {
                    

                    pointMap[i][j].X = i * scaling - halfSide;
                    pointMap[i][j].Z = j * scaling - halfSide;
                    texturePoints[i][j] = new Vector2(randomGen.NextFloat(-0.05f, 0.05f)+ 
                        0.95f*((pointMap[i][j].Y - lowestPoint) / (highestPoint - lowestPoint)),
                        randomGen.NextFloat(0f, 0.999f));
                    pointMap[i][j].Y = (pointMap[i][j].Y - lowestPoint) * 100 / (highestPoint - lowestPoint);

                }
            }

            for (int i = 0; i <= pointMap.Length - 1; i++)
            {
                for (int j = 0; j <= pointMap[0].Length - 1; j++)
                {
                    if (i == 0 || j == 0 || i == pointMap.Length - 1 || j == pointMap[0].Length - 1)
                    {
                        normalMap[i][j] = Vector3.Up;
                    }
                    else
                    {
                        normalMap[i][j] = Vector3.Normalize(Vector3.Cross(pointMap[i][j + 1] - pointMap[i][j], pointMap[i + 1][j] - pointMap[i][j]) +
                            Vector3.Cross(pointMap[i + 1][j] - pointMap[i][j], pointMap[i][j - 1] - pointMap[i][j]) +
                            Vector3.Cross(pointMap[i][j - 1] - pointMap[i][j], pointMap[i - 1][j] - pointMap[i][j]) +
                            Vector3.Cross(pointMap[i - 1][j] - pointMap[i][j], pointMap[i][j - 1] - pointMap[i][j]));
                    }
                }
            }
            //give points a color/texture value
            List<VertexPositionNormalTexture> vertexList = new List<VertexPositionNormalTexture>();
            //and convert them to VertexPositionColour array for drawing

            for (int i = 0; i < pointMap.Length - 1; i++)
            {
                for (int j = 0; j < pointMap[0].Length -1; j++)
                {   
                    if (pointMap[i+1][j+1] != null && pointMap[i][j + 1] != null && pointMap[i + 1][j] != null)
                    {
                        vertexList.Add(new VertexPositionNormalTexture(pointMap[i][j + 1], normalMap[i][j+1], texturePoints[i][j+1]));
                        vertexList.Add(new VertexPositionNormalTexture(pointMap[i + 1][j + 1], normalMap[i+1][j+1], texturePoints[i + 1][j + 1]));
                        vertexList.Add(new VertexPositionNormalTexture(pointMap[i + 1][j], normalMap[i + 1][j], texturePoints[i + 1][j]));
                    }
                    if (pointMap[i][j]!=null && pointMap[i][j + 1] !=null && pointMap[i + 1][j]!=null)
                    {
                        vertexList.Add(new VertexPositionNormalTexture(pointMap[i][j], normalMap[i][j], texturePoints[i][j]));
                        vertexList.Add(new VertexPositionNormalTexture(pointMap[i][j + 1], normalMap[i][j+1], texturePoints[i][j + 1]));
                        vertexList.Add(new VertexPositionNormalTexture(pointMap[i + 1][j], normalMap[i+1][j], texturePoints[i + 1][j]));
                    }
                    
                }
            }
           
             
            return vertexList.ToArray();
        }

        public void fillSquare(Vector3[][] pointMap, int spacing)
        {
            if (spacing % 2 == 1)
            {
                Console.WriteLine("ERROR: odd spacing");
                throw new Exception();
                
            }
            float randomnessScaler = (float)(spacing  ) * sidelength / verticesPerSide;
            highestPoint = -100f;
            lowestPoint = 100f;
            int maxFirstIndex = pointMap.Length - 1;
            int maxSecondIndex = pointMap[0].Length - 1;
            
            //do squares
            for (int leftX = 0; leftX < maxFirstIndex; leftX += spacing)
            {
                for (int bottomZ = 0; bottomZ < maxSecondIndex; bottomZ += spacing)
                {
                    int midX = leftX + spacing/2;
                    int midZ = bottomZ + spacing/2;

                    int rightX = leftX + spacing;
                    int topZ = bottomZ + spacing;


                    //fill middle 
                    pointMap[midX][midZ].Y=                 
                         // Height
                        (pointMap[leftX][topZ].Y + pointMap[rightX][topZ].Y +
                        pointMap[leftX][bottomZ].Y + pointMap[rightX][bottomZ].Y) / 4.0f +
                        //add random jitter
                        randomGen.NextFloat(-jitterModifier, jitterModifier) * randomnessScaler;
                    
                    //check if highest/lowestpoint
                    if (pointMap[midX][midZ].Y > highestPoint)
                    {
                        highestPoint = pointMap[midX][midZ].Y;
                    }
                    if (pointMap[midX][midZ].Y < lowestPoint)
                    {
                        lowestPoint = pointMap[midX][midZ].Y;
                    }

                }
            }
            
            //do diamonds
            for (int leftX =0; leftX <= maxFirstIndex; leftX += spacing)
            {
                for (int bottomZ = 0; bottomZ <= maxSecondIndex; bottomZ += spacing)
                {
                    int midX = leftX + spacing / 2;
                    int midZ = bottomZ + spacing / 2;
                    int rightX = leftX + spacing;
                    int topZ = bottomZ + spacing;

                    //left
                    if (bottomZ < maxSecondIndex)
                    {
                        pointMap[leftX][midZ].Y =

                            pointMap[leftX][topZ].Y + pointMap[leftX][bottomZ].Y
                            ;
                        if (leftX == 0)//edge case
                        {
                            pointMap[leftX][midZ].Y += pointMap[midX][midZ].Y;
                            pointMap[leftX][midZ].Y /= 3.0f;
                        }
                        else if (leftX == maxFirstIndex)//edge case
                        {
                            pointMap[leftX][midZ].Y += pointMap[midX - spacing][midZ].Y;
                            pointMap[leftX][midZ].Y /= 3.0f;
                        }
                        else
                        {
                            pointMap[leftX][midZ].Y += pointMap[midX][midZ].Y;
                            pointMap[leftX][midZ].Y += pointMap[midX - spacing][midZ].Y;
                            pointMap[leftX][midZ].Y /= 4.0f;
                        }

                        pointMap[leftX][midZ].Y += randomGen.NextFloat(-jitterModifier, jitterModifier) * randomnessScaler;
                        //check if highest/lowestpoint
                        if (pointMap[leftX][midZ].Y > highestPoint)
                        {
                            highestPoint = pointMap[leftX][midZ].Y;
                        }
                        if (pointMap[leftX][midZ].Y < lowestPoint)
                        {
                            lowestPoint = pointMap[leftX][midZ].Y;
                        }
                    }
                    //bottom
                    if (leftX < maxFirstIndex)
                    {
                        pointMap[midX][bottomZ].Y = pointMap[leftX][bottomZ].Y + pointMap[rightX][bottomZ ].Y;
                        //edge case
                        if (bottomZ == 0)
                        {
                            pointMap[midX][bottomZ].Y += pointMap[midX][midZ].Y;
                            pointMap[midX][bottomZ].Y /= 3.0f;
                        }
                        else if (bottomZ == maxSecondIndex)
                        {
                            pointMap[midX][bottomZ].Y += pointMap[midX][midZ - spacing].Y;
                            pointMap[midX][bottomZ].Y /= 3.0f;
                        }
                        else
                        {
                            pointMap[midX][bottomZ].Y += pointMap[midX][midZ].Y;

                            pointMap[midX][bottomZ].Y += pointMap[midX][midZ - spacing].Y;
                            pointMap[midX][bottomZ].Y /= 4.0f;

                        }
                        //random component
                        pointMap[midX][bottomZ].Y += randomGen.NextFloat(-jitterModifier, jitterModifier) * randomnessScaler;

                        //check if highest/lowestpoint
                        if (pointMap[midX][bottomZ].Y > highestPoint)
                        {
                            highestPoint = pointMap[midX][bottomZ].Y;
                        }
                        if (pointMap[midX][bottomZ].Y < lowestPoint)
                        {
                            lowestPoint = pointMap[midX][bottomZ].Y;
                        }
                    }
                }
            }

        }
        public Vector3[] getPointsToBound(Vector3 point) 
        {
            /*int X = (int)((point.X + sidelength / 2) / verticesPerSide);
            int Z = (int)((point.Z + sidelength / 2) / verticesPerSide);
            */
            int X, Z, xNum=-1, zNum=-1;
            for (X = 0; X < verticesPerSide; X++)
            {
                if (pointMap[X][0].X > point.X)
                {
                    xNum = X;
                    break;
                }
            }
            for (Z = 0; Z < verticesPerSide; Z++)
            {
                if (pointMap[0][Z].Z > point.Z)
                {
                    zNum = Z;
                    break;
                }
            }
            
            if (zNum <= 0 || xNum <= 0 || xNum >= verticesPerSide || zNum >= verticesPerSide )
            {
                return new[] { new Vector3(), new Vector3(), new Vector3(), new Vector3()};
            }
            return new[] { pointMap[xNum][zNum], pointMap[xNum - 1][zNum], pointMap[xNum - 1][zNum - 1], pointMap[xNum][zNum - 1] };
        }

    
    
    }

     


}
