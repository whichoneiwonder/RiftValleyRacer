using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    public class Cube : ColoredGameObject
    {

        BoundingBox boundsBox;
        public Vector3 position, velocity;
        protected KeyboardManager keyboardManager;
        protected KeyboardState keyboard;
        GameTime lastFrame;
        Camera camera;
        public Cube(Project1Game game, GameTime gameTime)
        {
            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                new[]
                    {
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange), // Front
                        new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange), // BACK
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.Orange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), Color.OrangeRed), // Top
                        new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), Color.OrangeRed), // Bottom
                        new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), Color.DarkOrange), // Left
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), Color.DarkOrange), // Right
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.DarkOrange),
                    });

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false,
                View = Matrix.LookAtLH(new Vector3(0, 0, -10), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            boundsBox = new BoundingBox(new Vector3(1f, 1f, 1f), new Vector3(-1f, -1f, -1f));
            position = new Vector3();
            velocity = new Vector3();
            camera = Project1Game.camera;

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
            lastFrame = gameTime;

        }

        public override void Update(GameTime gameTime)
        {
            // move the cube.
           // velocity.Y -= 0.9f;//Gravity
            camera = Project1Game.camera;



            position += velocity*(gameTime.ElapsedGameTime.Milliseconds - lastFrame.ElapsedGameTime.Milliseconds);
            basicEffect.World = Matrix.Translation(position);

    

        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
