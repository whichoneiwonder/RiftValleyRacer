using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace Project
{
    public class Racer : GameObject
    {
        public Vector3 position;
        public Model model;
        public Matrix world, view, projection, WorldInverseTranspose;
        public Vector3 heading, lateral, up;
        public Vector3 accel, vel;
        public float gravity = 0.9f;
        public Racer(Project1Game game, Vector3 pos, String modelName)
        {
            this.position = pos;
            this.game = game;
            this.accel = new Vector3();
            this.vel = new Vector3();
            this.heading = Vector3.UnitZ;
            this.up = Vector3.UnitY;
            this.lateral = Vector3.Cross(this.up, this.heading);

            this.model = game.Content.Load<Model>(modelName );

            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.Up);
            projection = Matrix.PerspectiveFovRH(0.9f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);
            world = Matrix.Translation(position) * Matrix.RotationZ((float)Math.PI);

            //foreach(ModelMesh mesh in model.Meshes){
            //    foreach(Effect effect in mesh.Effects){
            //        model. = game.Content.Load<Effect>("Phong");

            //    }    

            //effect = game.Content.Load<Effect>("Phong");
            //basicEffect = new BasicEffect(game.GraphicsDevice)
            //{
            //    LightingEnabled = true,
            //    View = view,
            //    Projection = projection,
            //    World = world,
            //    //FogEnabled = true,
            //    //FogColor = Color.Gray.ToVector3(),
            //    //FogStart = 2.75f,
            //    //FogEnd = 5.25f

            //};
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (ModelMeshPart part in mesh.MeshParts)
            //    {
            //        part.Effect = basicEffect;
            //    }
            //}


        }
       

        public override void Draw(GameTime gameTime)
        {
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (Effect eff in mesh.Effects)
            //    {
                    //effect.Parameters["World"].SetValue(world);
                    //effect.Parameters["Projection"].SetValue(Project1Game.camera.Projection);
                    //effect.Parameters["View"].SetValue(Project1Game.camera.View);
                    //effect.Parameters["cameraPos"].SetValue(Project1Game.camera.cameraPosition);
                    //effect.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);
                    //effect.Parameters["lightAmbCol"].SetValue(Project1Game.camera.ambientColour);
                    //effect.Parameters["lightPntPos"].SetValue(Project1Game.camera.sunPosition);
                    //effect.Parameters["lightPntCol"].SetValue(Project1Game.camera.sunColour);
            //    }
            //}
            List<Matrix> bones = new List<Matrix>();
                    int i = 0;
                    while (i < model.Bones.Count)
                    {
                        i++;
                        bones.Add(Matrix.Identity);
                    }

            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (Effect meshEffect in mesh.Effects)
            //    {
           
            //        meshEffect.Parameters["World"].SetValue(world);
            //        meshEffect.Parameters["Projection"].SetValue(projection);
            //        meshEffect.Parameters["View"].SetValue(view);
            //        meshEffect.Parameters["cameraPos"].SetValue(Project1Game.camera.cameraPosition);
            //        meshEffect.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);
            //        meshEffect.Parameters["lightAmbCol"].SetValue(Project1Game.camera.ambientColour);
            //        meshEffect.Parameters["lightPntPos"].SetValue(Project1Game.camera.sunPosition);
            //        meshEffect.Parameters["lightPntCol"].SetValue(Project1Game.camera.sunColour);

                   

            //        mesh.Draw(game.GraphicsDevice,bones.ToArray(), meshEffect);
            //    }
            //}

                   // effect.CurrentTechnique.Passes[0].Apply();
            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.Up); ;
            projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);


            world = Matrix.Scaling(0.005f) * Matrix.RotationX((float)Math.PI) * Matrix.RotationZ((float)Math.PI) * Matrix.Translation(position);

            WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(world));
            this.model.Draw(game.GraphicsDevice, this.world, this.view, this.projection);
        }

        public override void Update(GameTime gameTime)
        {
            Vector3[] pointsToBound = ((Terrain)game.getTerrainChunkUnderPlayer()).getTerrainUnderPoint(position);
            BoundingSphere instanceBound = new BoundingSphere(position, 1f);
            if (instanceBound.Intersects(ref pointsToBound[1], ref pointsToBound[2], ref pointsToBound[3]))
            {
                vel.Y = 1f;
                //bounceyness 
                accel+=  Vector3.Normalize(Vector3.Cross(pointsToBound[2] - pointsToBound[1], pointsToBound[3] - pointsToBound[1]));
            }

            else if (instanceBound.Intersects(ref pointsToBound[0], ref pointsToBound[1], ref pointsToBound[2]))
            {
                vel.Y=1f ;
                //      /*bounceyness *
                accel += Vector3.Normalize(Vector3.Cross(pointsToBound[2] - pointsToBound[0], pointsToBound[1] - pointsToBound[0]));

            }

            Vector3 prevVel = vel;
            float delta =(float)gameTime.ElapsedGameTime.TotalMilliseconds / 200f;
            Debug.WriteLine("delta is: " + delta);


            vel.Y -= delta*gravity;



            position = (position + vel * delta  + 0.5f * accel * delta * delta);
            vel += accel * delta;
            accel = new Vector3();

            //this.position = Project1Game.camera.cameraPosition + Vector3.Normalize(Project1Game.camera.cameraDirection);
            
        }

        public void accelerate(float factor)
        {
            this.accel += factor * Vector3.Normalize(heading);
        }

    }
}