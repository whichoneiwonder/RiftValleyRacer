using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    public abstract class GameObject
    {
        public BasicEffect basicEffect;
        public VertexInputLayout inputLayout;
        public Buffer<VertexPositionNormalTexture> vertices;
        public Project1Game game;
        public string textureName;
        public Texture2D texture;
        
        public GameObject(Project1Game game)
        {
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                LightingEnabled = true,
                AmbientLightColor = new Vector3(0.25f ),
                DiffuseColor = new Vector4(0.9f,0.9f,0.9f,1f),
                SpecularColor = new Vector3(),
                
                TextureEnabled = true,
                VertexColorEnabled = false,
                View = game.camera.View,
                World = Matrix.Identity
            };

            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f );
            basicEffect.DirectionalLight0.SpecularColor = new Vector3(1f, 1f, 1f);
            basicEffect.SpecularPower = 0.01f;
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {

            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            basicEffect.View = game.camera.View;
            basicEffect.Projection = game.camera.Projection;
        }

        public void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            game.GraphicsDevice.SetBlendState(game.GraphicsDevice.BlendStates.AlphaBlend) ;

            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
