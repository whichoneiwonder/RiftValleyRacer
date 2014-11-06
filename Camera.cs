using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using System.Diagnostics;


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
            sunPosition = Vector3.Normalize(new Vector3(0f, -200f, 0f));
            sunColour = new Vector3(1f, 1f, 0.9f);

            // Set ambient light to default colour
            ambientColour = new Vector3(0.2f, 0.2f, 0.3f);

            // Set background to default colour
            background = new Color(0.4f, 0.4f, 0.8f, 1);

            // Set up matrices
            View = Matrix.LookAtRH(cameraPosition, cameraLook, Vector3.UnitY);
            Projection = Matrix.PerspectiveFovRH((float)Math.PI / 1.6f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);
            World = Matrix.Identity;
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            
            // Set cameraPosition and cameraLook relative to player
            cameraPosition = game.player.position - game.player.vel / 50f;
            cameraPosition -= 2f * game.player.heading;
            cameraPosition.Y += 1.2f;
            cameraLook = game.player.position + game.player.heading * 2f ;

            // Update sun position and background colour
            sunPosition = new Vector3(0f, -1f, 0f);
            Matrix3x3 transform = Matrix3x3.RotationZ((float)gameTime.ElapsedGameTime.TotalSeconds/100f);
            Vector3.Transform( sunPosition, transform);

            background = new Color(0.25f * (-sunPosition.Y + 1f), 0.35f * (-sunPosition.Y + 1f), 0.8f * ( 1f), 1);
            Vector3.Transform( sunPosition,Matrix.Scaling(100)*Matrix.Translation( game.player.position));//



            // Update matrices
            View = Matrix.LookAtRH(cameraPosition, cameraLook, Vector3.UnitY);
            Projection = Matrix.PerspectiveFovRH((float)Math.PI / 
                (2f - (game.player.vel.Length() + game.player.prevVel.Length()) / ( 2.5f*game.player.maxPlayerSpeed)),
                (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);

        }
    }
}
