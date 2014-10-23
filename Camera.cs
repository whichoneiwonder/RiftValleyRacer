using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;


// The following code is modified from the Lab solution for week 4.

namespace Project
{
    public class Camera
    {
        public Matrix View;
        public Matrix Projection;
        public Matrix World;
        public Project1Game game;
        public Vector3 cameraDirection, cameraPosition, cameraLook, sunPosition, sunColour, ambientColour;
        public Color background;


        // Ensures that all objects are being rendered from a consistent viewpoint
        public Camera(Project1Game game)
        {
            // Set the camera to default position and direction
            cameraPosition = new Vector3();//startPosition;
            cameraDirection = new Vector3();//startDirection;

            // Set the sun to default direction and colour
            sunPosition = Vector3.Normalize(new Vector3(0f, -1f, 0f));
            sunColour = new Vector3(1f, 1f, 0.9f);

            // Set ambient light to default colour
            ambientColour = new Vector3(0.2f, 0.2f, 0.3f);

            // Set background to default colour
            background = new Color(0.4f, 0.4f, 0.8f, 1);

            // Set up matrices
            View = Matrix.LookAtRH(cameraPosition, cameraLook, Vector3.UnitY);
            Projection = Matrix.PerspectiveFovRH((float)Math.PI / 2.3f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);
            World = Matrix.RotationX(0) * Matrix.RotationY(0) * Matrix.RotationZ(0);
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {

            // Set cameraPosition and cameraLook relative to player
            cameraPosition = game.player.position - game.player.vel / 100f;
            cameraPosition -= 0.05f*game.player.heading;
            cameraPosition.Y += 0.02f;
            cameraLook = game.player.position+game.player.heading+ game.player.vel/20f;
            //cameraLook.Y += 0.01f;

            // Update sun position and background colour
            sunPosition.X = (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds / 10.0));
            sunPosition.Y = (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds / 10.0));
            background = new Color(0.25f * (-sunPosition.Y + 1f), 0.35f * (-sunPosition.Y + 1f), 0.8f * (-sunPosition.Y + 1f), 1);

            // Update matrices
            View = Matrix.LookAtRH(cameraPosition, cameraLook, Vector3.UnitY);
            Projection = Matrix.PerspectiveFovRH((float)Math.PI / 2f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.0001f, 1000.0f);

        }
    }
}
