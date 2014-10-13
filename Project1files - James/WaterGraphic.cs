using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    
    using SharpDX.Toolkit.Graphics;
    class WaterGraphic : GameObject
    {

        float depth = 18f;
        public WaterGraphic(Project1Game game): base(game)
        {
            
            basicEffect.Texture = game.Content.Load<Texture2D>("WaterTexture.png");
            basicEffect.Alpha = 0.9f;
            basicEffect.SpecularPower = 1000.0f;
            basicEffect.SpecularColor = new Vector3(0.9f, 0.9f, 1f);
            basicEffect.PreferPerPixelLighting = true;
            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,generateWaterPoints()
                );
        }
        private VertexPositionNormalTexture[] generateWaterPoints()
        {
            float length = Landscape.sidelength/2;

            return new[]
            {
                new VertexPositionNormalTexture(new Vector3(- length, depth, -length),Vector3.Up, new Vector2(0f,0f)),
                new VertexPositionNormalTexture(new Vector3(- length, depth, length),Vector3.Up, new Vector2(0f,1f)),
                new VertexPositionNormalTexture(new Vector3( length, depth, length),Vector3.Up, new Vector2(1f,1f)),

                new VertexPositionNormalTexture(new Vector3(- length,depth, -length),Vector3.Up, new Vector2(0f,0f)),
                new VertexPositionNormalTexture(new Vector3( length, depth, length),Vector3.Up, new Vector2(1f,1f)),
                new VertexPositionNormalTexture(new Vector3( length, depth, -length),Vector3.Up, new Vector2(1f,0f)),

                new VertexPositionNormalTexture(new Vector3(- length, depth, length),Vector3.Up, new Vector2(0f,1f)),
                new VertexPositionNormalTexture(new Vector3(- length, depth, -length),Vector3.Up, new Vector2(0f,0f)),
                new VertexPositionNormalTexture(new Vector3( length, depth, length),Vector3.Up, new Vector2(1f,1f)),

                new VertexPositionNormalTexture(new Vector3( length, depth, length),Vector3.Up, new Vector2(1f,1f)),
                new VertexPositionNormalTexture(new Vector3(- length,depth, -length),Vector3.Up, new Vector2(0f,0f)),
                new VertexPositionNormalTexture(new Vector3( length, depth, -length),Vector3.Up, new Vector2(1f,0f))
               
            };


        }

         
    }
}
