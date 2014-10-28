using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Windows.Devices.Sensors;

namespace Project
{
    public class Racer : GameObject
    {
        public Vector3 position;
        public Model model;
        public Matrix world, view, projection;
        public Vector3 heading, lateral, up;
        public Vector3 accel, vel;
        public float gravity = 15f, thrustPower = 5f, opponentStepSize = 0.25f, maxPlayerSpeed = 15f;
        public bool forward = false, backward = false, opponent = false;
 
        public Accelerometer accelerometer;
        public AccelerometerReading accelerometerReading;
        private float yaw;

        public Racer(Project1Game game, Vector3 pos, String modelName)
        {
            this.position = pos;
            this.game = game;
            this.accel = new Vector3();
            this.vel = new Vector3();
            this.heading = Vector3.UnitZ;
            this.up = Vector3.UnitY;
            this.lateral = Vector3.Cross(this.up, this.heading);
            yaw = 0;

            this.model = game.Content.Load<Model>(modelName );
            accelerometer = Accelerometer.GetDefault();
            
            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.Up);
            projection = Matrix.PerspectiveFovRH(0.9f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);
            world = Matrix.Translation(position) * Matrix.RotationZ((float)Math.PI) * Matrix.RotationY(yaw);
     }
       

        public override void Draw(GameTime gameTime)
        {
            
            List<Matrix> bones = new List<Matrix>();
                    int i = 0;
                    while (i < model.Bones.Count)
                    {
                        i++;
                        bones.Add(Matrix.Identity);
                    }
            
            view =Project1Game.camera.View;
            projection = Project1Game.camera.Projection;

            world = Matrix.Scaling(0.01f) *
                Matrix.RotationX((float)Math.PI) *
                Matrix.RotationZ((float)Math.PI)*
                Matrix.RotationY(yaw) *
                Matrix.Translation(position);

            //WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(world));
            this.model.Draw(game.GraphicsDevice, this.world, this.view, this.projection);
        }

        public override void Update(GameTime gameTime)
        {

            // If this is an opponent racer, update the object with the following block
            if (opponent)
            {
                // If there are no points left, or the game has been won, remain stationery
                if (Project1Game.opponentPath.Count == 0) { return; }

                // If the first element has been reached near enough, remove it
                if (Math.Abs(position.X - (float)Project1Game.opponentPath[0].X) < 1 &&
                    Math.Abs(position.Z - (float)Project1Game.opponentPath[0].Y) < 1) {
                    Project1Game.opponentPath.RemoveAt(0);
                    if (Project1Game.opponentPath.Count == 0) {
                        return;
                    }
                }

                // Set the current goal according to the next point in the path,
                // and set the velocity to the vector pointing there from the current position
                Vector3 currentGoalPosition = new Vector3(Project1Game.opponentPath[0].X, 
                                                         (float)FractalTools.fractal[(int)position.Z, (int)position.X]+1.5f, 
                                                          Project1Game.opponentPath[0].Y),
                        velocity = Vector3.Subtract(currentGoalPosition, position);

                // Ensure opponent stays on terrain at a set height
                if (position.Y < (float)FractalTools.fractal[(int)position.Z, (int)position.X]+1.5f) {
                    position.Y = (float)FractalTools.fractal[(int)position.Z, (int)position.X]+1.5f;
                }

                // Move, given the velocity
                position += Vector3.Normalize(velocity)*opponentStepSize;
                return;
            }

            // If this is the player racer, update the object with the following block
            BoundingSphere instanceBound = new BoundingSphere(position, 1.3f);
            Vector3 normalForce = new Vector3();
            Vector3[] pointsToBound = ((Terrain)game.getTerrainChunkUnderPlayer()).getTerrainUnderPoint(position);
            float avgheight = 0f;
            int numPoints = 0;
            foreach(Vector3 vec in pointsToBound){
                avgheight+= vec.Y;
                numPoints++;
            }
            avgheight/=(float)numPoints;
            Boolean touchingTerrain = false;
            while (true)
            {
                Boolean belowTerrain = true;
                
                foreach(Vector3 point in pointsToBound){
                    if(position.Y > point.Y){
                        belowTerrain = false;
                    }    
                }

                instanceBound.Center = position;
                if (belowTerrain) { position.Y = avgheight; }

                else if (instanceBound.Contains(ref pointsToBound[2], ref pointsToBound[1], ref pointsToBound[3]) != ContainmentType.Disjoint)
                {
                    vel.Y+= 0.01f;

                    //bounceyness 
                    normalForce = Vector3.Normalize(Vector3.Cross(pointsToBound[2] - pointsToBound[1], pointsToBound[3] - pointsToBound[1])) / 2f;
                    position += normalForce / 200f;
                    accel += normalForce*0.01f;
                    touchingTerrain = true;
                }

                else if (instanceBound.Contains(ref pointsToBound[1], ref pointsToBound[0], ref pointsToBound[2]) != ContainmentType.Disjoint)
                {
                    vel.Y += 0.01f;
                    // bounceyness
                    normalForce = Vector3.Normalize(Vector3.Cross(pointsToBound[2] - pointsToBound[0], pointsToBound[1] - pointsToBound[0])) / 2f;
                    position += normalForce/200f;

                    accel += normalForce*0.01f ;
                    touchingTerrain = true;

                } else  {
                    vel = (vel*3 + Vector3.Reflect(vel, normalForce))/4f;                    
                    break;
                }
            }

            Vector3 prevVel = vel;
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!touchingTerrain) { accel.Y -= gravity/2f; }
            accel -= 0.05f * vel;

            float factor = 0;

            // If there is an accelerometer present, utilise it as a controller
            if (accelerometer != null) {
                accelerometerReading = accelerometer.GetCurrentReading();
                yaw -= (float)(0.01 * accelerometerReading.AccelerationX);
                if (forward) { factor = 1f * thrustPower; } else if (backward) { factor = -1f * thrustPower; }
            } else {
                yaw -= (float)(0.01 * Project1Game.direction);
                factor = Project1Game.accel * thrustPower;
            }

            this.accel += factor * Vector3.Normalize(heading);

            if (vel.Length() > maxPlayerSpeed) { vel *= maxPlayerSpeed / vel.Length() ; }

            // Ensure racer stays within boundaries at all times
            if (position.X > FractalTools.N - FractalTools.chunkN) {
                position.X = FractalTools.N - FractalTools.chunkN;
            }
            if (position.Z > FractalTools.N - FractalTools.chunkN) {
                position.Z = FractalTools.N - FractalTools.chunkN;
            }

            position = (position + vel * delta + 0.5f * accel * delta * delta);
            vel += accel * delta;

            heading = Vector3.Transform(Vector3.UnitZ, Matrix3x3.RotationY(yaw));

            accel = new Vector3();
            
        }

    }
}