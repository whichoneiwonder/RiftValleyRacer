using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
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
        public float accel, vel;

        public Racer(Project1Game game, Vector3 pos, String modelName)
        {
            this.position = pos;
            this.game = game;
            this.accel = 0f;
            this.vel = 0f;
            this.heading = position - new Vector3(0, 0, 0);
            this.up = Vector3.UnitY;
            this.lateral = Vector3.Cross(this.up, this.heading);

            this.model = game.Content.Load<Model>(modelName);

            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.Up);
            projection = Matrix.PerspectiveFovRH(0.9f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);
            world = Matrix.Translation(position) * Matrix.RotationZ((float)Math.PI);

            //foreach(ModelMesh mesh in model.Meshes){
            //    foreach(Effect effect in mesh.Effects){
            //        model. = game.Content.Load<Effect>("Phong");

            //    }    

            effect = game.Content.Load<Effect>("Phong");

            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (ModelMeshPart part in mesh.MeshParts)
            //    {
            //        part.Effect = effect;
            //    }
            //}


        }

        public override void Draw(GameTime gameTime)
        {
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (Effect eff in mesh.Effects)
            //    {
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["Projection"].SetValue(Project1Game.camera.Projection);
                    effect.Parameters["View"].SetValue(Project1Game.camera.View);
                    effect.Parameters["cameraPos"].SetValue(Project1Game.camera.cameraPosition);
                    effect.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);
                    effect.Parameters["lightAmbCol"].SetValue(Project1Game.camera.ambientColour);
                    effect.Parameters["lightPntPos"].SetValue(Project1Game.camera.sunPosition);
                    effect.Parameters["lightPntCol"].SetValue(Project1Game.camera.sunColour);
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
                   this.model.Draw(game.GraphicsDevice, this.world, this.view, this.projection);
        }

        public override void Update(GameTime gameTime)
        {
            float prevVel = vel;
            float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            position.Z = (float)(position.Z + vel * delta + 0.5f * accel * delta * delta);
            vel = accel * delta;


            //this.position = Project1Game.camera.cameraPosition + Vector3.Normalize(Project1Game.camera.cameraDirection);
            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.Up); ;
            projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);


            world = Matrix.Scaling(0.005f) * Matrix.RotationX((float)Math.PI) * Matrix.RotationZ((float)Math.PI) * Matrix.Translation(position);

            WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(world));
        }

    }
}