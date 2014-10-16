using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;


// The following code is modified from the Lab solution for week 4.

namespace Project1
{
    public class Camera
    {
        public Matrix View;
        public Matrix Projection;
        public Matrix World;
        public Game game;
        public Vector3 cameraDirection, cameraPosition, cameraLook, sunPosition, sunColour, ambientColour;
        public Color background;
        public float forwardMovement, pan, tilt, oldTilt;

        // Ensures that all objects are being rendered from a consistent viewpoint
        public Camera(Game game, Vector3 startPosition, Vector3 startDirection) {
            // Set the camera to default position and direction
            cameraPosition  = startPosition;
            cameraDirection = startDirection;

            // Set the sun to default direction and colour
            sunPosition = Vector3.Normalize(new Vector3(0f, -1f, 0f));
            sunColour   = new Vector3(1f, 1f, 0.9f);

            // Set ambient light to default colour
            ambientColour = new Vector3(0.2f, 0.2f, 0.3f);

            // Set background to default colour
            background = new Color(0.4f, 0.4f, 0.8f, 1);

            // Set up matrices
            View = Matrix.LookAtLH(cameraPosition, cameraLook, Vector3.UnitY);
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 2.3f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);
            World = Matrix.RotationX(0) * Matrix.RotationY(0) * Matrix.RotationZ(0);
            this.game = game;
        }

        public void Update(GameTime gameTime) {

            // Check mouse and WASD keys, and set variables
            forwardMovement = (Project1Game.upKey-Project1Game.downKey)-(Project1Game.mouseR-Project1Game.mouseL);
            pan  = -(0.5f - Project1Game.mouseX)-(Project1Game.leftKey-Project1Game.rightKey)*0.5f;
            tilt = -(0.5f - Project1Game.mouseY);

            // Rotate cameraDirection accordingly
            cameraDirection = Vector3.Normalize((Vector3)Vector3.Transform(cameraDirection, Matrix.RotationY(pan/30f)));
            cameraDirection = Vector3.Normalize((Vector3)Vector3.Transform(cameraDirection, Matrix.RotationX(cameraDirection.Z * tilt/40f)));
            cameraDirection = Vector3.Normalize((Vector3)Vector3.Transform(cameraDirection, Matrix.RotationZ(-cameraDirection.X * tilt/40f)));

            // Translate cameraPosition relative to cameraDirection
            cameraPosition += cameraDirection * forwardMovement/2f;

            // Set cameraLook to equal cameraPosition, and translate it in direction of cameraDirection
            cameraLook = cameraPosition + cameraDirection;

            // Update sun position and background colour
            sunPosition.X = (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds/10.0));
            sunPosition.Y = (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds/10.0));
            background = new Color(0.25f*(-sunPosition.Y+1f), 0.35f*(-sunPosition.Y+1f), 0.8f*(-sunPosition.Y+1f), 1);

            // Update matrices
            View = Matrix.LookAtLH(cameraPosition, cameraLook, Vector3.UnitY);

        }
    }
}
