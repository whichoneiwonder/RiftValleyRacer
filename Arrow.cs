using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    public class Arrow : GameObject
    {
        Vector3 pos;
        Vector3 orientation;
        Vector3 origOrientation = Vector3.UnitX;
        Matrix view, world, projection;
        Model model;

        public Arrow(Project1Game game, Vector3 cameraPos)
        {
            this.pos = Project1Game.camera.cameraPosition + 0.01f * Vector3.Normalize(Project1Game.camera.cameraDirection);
            this.game = game;
            this.orientation = game.goal.position - this.pos;

            model = game.Content.Load<Model>("arrow2");

            world = Matrix.Translation(pos);
            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.UnitY);
            projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);


        }


        public override void Update(GameTime gametime)
        {
            this.pos = Project1Game.camera.cameraPosition + 0.01f * Vector3.Normalize(Project1Game.camera.cameraDirection);
            this.orientation = game.goal.position - this.pos;

            Vector3 orientationNorm = Vector3.Normalize(orientation);
            Vector3 origOrientationNorm = Vector3.Normalize(origOrientation);

            float dotProd = Vector3.Dot(origOrientationNorm,orientationNorm);
            float orientationMag = (float)Math.Sqrt(orientationNorm.X*orientation.X + orientation.Y*orientation.Y+orientation.Z*orientation.Z);
            float origOrientationMag = (float)Math.Sqrt(origOrientationNorm.X*origOrientationNorm.X + origOrientationNorm.Y*origOrientationNorm.Y+origOrientationNorm.Z*origOrientationNorm.Z);

            float rotationAngle = (float)Math.Acos(dotProd/(orientationMag*origOrientationMag));
            Vector3 rotationAxis = Vector3.Cross(origOrientationNorm, orientationNorm);

            world = Matrix.Scaling(0.5f)*Matrix.RotationX((float)Math.PI) * Matrix.RotationZ((float)Math.PI) * Matrix.RotationAxis(rotationAxis, rotationAngle) * Matrix.Translation(pos);
            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.UnitY);
            projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);


        }


        public override void Draw(GameTime gametime)
        {
            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.Up); ;
            projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);
           
            this.model.Draw(game.GraphicsDevice, this.world, this.view, this.projection);


        }


    }
}
