using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    
    using SharpDX.Toolkit.Graphics;
    class Sun : GameObject
    {

        
        public Sun(Project1Game game): base(game)
        {
            basicEffect.Texture = game.Content.Load<Texture2D>("SunTexture.png");
            basicEffect.Alpha = 0.95f;
            basicEffect.LightingEnabled = false;
            
            basicEffect.EmissiveColor = new Vector3(1f,1f,0.8f);

            vertices = Buffer.Vertex.New(game.GraphicsDevice,
                  new[]
            {
            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(0.0f, 0.0f)), // Front
            new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, -1.0f),  new Vector3(1.0f, 1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(0.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1.0f, 1.0f)),

            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), new Vector2(0.0f, 1.0f)), // BACK
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f),  new Vector3(1.0f, 1.0f, 1.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), new Vector2(0.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, 1.0f),  new Vector3(1.0f, -1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f),  new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 1.0f)),

            new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(0.0f, 1.0f)), // Top
            new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(0.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f),  new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), new Vector2(1.0f, 1.0f)),

            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(0.0f, 0.0f)), // Bottom
            new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f), new Vector2(1.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, 1.0f),  new Vector3(-1.0f, -1.0f, 1.0f), new Vector2(0.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, 1.0f),  new Vector3(1.0f, -1.0f, 1.0f), new Vector2(1.0f, 1.0f)),

            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1.0f, 1.0f)), // Left
            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), new Vector2(0.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, 1.0f),  new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, 1.0f),  new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(1.0f, 0.0f)),

            new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, -1.0f),  new Vector3(1.0f, -1.0f, -1.0f), new Vector2(0.0f, 1.0f)), // Right
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f),  new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, 1.0f),  new Vector3(1.0f, -1.0f, 1.0f), new Vector2(1.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, -1.0f),  new Vector3(1.0f, -1.0f, -1.0f), new Vector2(0.0f, 1.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
            });


        }
    }
}
