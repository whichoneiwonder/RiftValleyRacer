using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    public class Cube : ColoredGameObject
    {

        public Vector3 position, velocity;
        protected KeyboardManager keyboardManager;
        protected KeyboardState keyboard;
        double lastFrame;
        Camera camera;

        float gravity = 0.09f;
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

            position = new Vector3();
            velocity = new Vector3();
            camera = Project1Game.camera;

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
            lastFrame = gameTime.ElapsedGameTime.TotalMilliseconds;
            keyboardManager = new KeyboardManager(game);

        }

        public override void Update(GameTime gameTime)
        {
            // move the cube.
            velocity.Y -= gravity;
            camera = Project1Game.camera;
            keyboard = keyboardManager.GetState();

            if (keyboard.IsKeyDown(Keys.Up))
            {
                velocity.Z += 1;
            }

            checkForCollisions();

            position = position + (velocity * (float)((gameTime.ElapsedGameTime.TotalMilliseconds - lastFrame) / 10.0));
            basicEffect.World = Matrix.Translation(position);


            this.lastFrame = gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;
            basicEffect.World = Matrix.Translation(position);
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        public void checkForCollisions()
        {
            Vector3[] pointsToBound = ((Terrain)game.getTerrainChunkUnderPlayer()).getTerrainUnderPoint(position);
            BoundingSphere instanceBound = new BoundingSphere(position, 1f);
            // = new BoundingBox(boundsBox.Minimum + position, position + boundsBox.Maximum);

            if (instanceBound.Intersects(ref pointsToBound[1], ref pointsToBound[2], ref pointsToBound[3]))
            {
                velocity.Y += 3 * gravity;
                //bounceyness 
                //-  Vector3.Normalize(Vector3.Cross(pointsToBound[2] - pointsToBound[1], pointsToBound[3] - pointsToBound[1]));
            }

            else if (instanceBound.Intersects(ref pointsToBound[0], ref pointsToBound[1], ref pointsToBound[2]))
            {
                velocity.Y += 3 * gravity;
                //      /*bounceyness *
                //    - Vector3.Normalize(Vector3.Cross(pointsToBound[2] - pointsToBound[0], pointsToBound[1] - pointsToBound[0]));

            }

        }
    }

}
