﻿using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public float gravity = 5f;
        public bool forward = false, backward = false;

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
           
             
            view =Project1Game.camera.View;
            projection = Project1Game.camera.Projection;

            world = Matrix.Scaling(0.0001f) *
                Matrix.RotationX((float)Math.PI) *
                Matrix.RotationZ((float)Math.PI)*
                Matrix.RotationY(yaw) *
                Matrix.Translation(position);

            //WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(world));
            this.model.Draw(game.GraphicsDevice, this.world, this.view, this.projection);
        }

        public override void Update(GameTime gameTime)
        {
            if (accelerometer != null)
            {
                accelerometerReading = accelerometer.GetCurrentReading();
                yaw -= (float)(0.01 * accelerometerReading.AccelerationX);
            }
            BoundingSphere instanceBound = new BoundingSphere(position, 0.1f);
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
                if (belowTerrain)
                {
                    position.Y = avgheight;
                }


                else if (instanceBound.Contains(ref pointsToBound[2], ref pointsToBound[1], ref pointsToBound[3]) != ContainmentType.Disjoint)
                {
                    vel.Y+= 0.0001f;

                    //bounceyness 
                    normalForce = Vector3.Normalize(Vector3.Cross(pointsToBound[2] - pointsToBound[1], pointsToBound[3] - pointsToBound[1])) / 2f;
                     position += normalForce / 2000f;
                    //position -= vel * 0.01f;
                     accel += normalForce*0.001f;
                     touchingTerrain = true;

                }

                else if (instanceBound.Contains(ref pointsToBound[1], ref pointsToBound[0], ref pointsToBound[2]) != ContainmentType.Disjoint)
                {
                    vel.Y += 0.0001f;
                    //      /*bounceyness *
                    normalForce = Vector3.Normalize(Vector3.Cross(pointsToBound[2] - pointsToBound[0], pointsToBound[1] - pointsToBound[0])) / 2f;
                    position += normalForce/2000f;
                    //position-=vel*0.01f;
                    accel += normalForce*0.001f ;
                    touchingTerrain = true;

                }
                 
                else
                {
                    
                    vel = (vel +vel+vel+ Vector3.Reflect(vel, normalForce))/4f;
                    Debug.WriteLine(accel);

                    
                    break;
                }

                
            }

            Vector3 prevVel = vel;
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Debug.WriteLine("delta is: " + delta);


            if (!touchingTerrain) 
            { 
                accel.Y -= gravity/2f;
                
            }
            accel -= 0.05f * vel;

            float factor = 0;
            if (forward) { factor = 3f; }
            else if (backward) { factor = -3f; }
            this.accel += factor * Vector3.Normalize(heading);
            float maxVel = 3f;
            if (vel.Length() > maxVel) { vel *= maxVel / vel.Length() ; }

            position = (position + vel * delta + 0.5f * accel * delta * delta);
            vel += accel * delta;

            heading = Vector3.Transform(Vector3.UnitZ, Matrix3x3.RotationY(yaw));


            accel = new Vector3();
            //this.position = Project1Game.camera.cameraPosition + Vector3.Normalize(Project1Game.camera.cameraDirection);
            
        }

    }
}