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


        public Racer(Project1Game game, Vector3 pos, String modelName)
        {
            this.position = pos;
            this.game = game;

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




        }

        public override void Draw(GameTime gameTime)
        {
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (Effect eff in mesh.Effects)
            //    {
            //        eff.Parameters["World"].SetValue(world);
            //        eff.Parameters["Projection"].SetValue(game.camera.Projection);
            //        eff.Parameters["View"].SetValue(game.camera.View);
            //        eff.Parameters["cameraPos"].SetValue(game.camera.cameraPos);
            //        eff.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);
            //        eff.Parameters["lightAmbCol"].SetValue(Project1Game.camera.ambientColour);
            //        eff.Parameters["lightPntPos"].SetValue(Project1Game.camera.sunPosition);
            //        eff.Parameters["lightPntCol"].SetValue(Project1Game.camera.sunColour);
            //    }
            //}

            effect.Parameters["World"].SetValue(world);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["cameraPos"].SetValue(Project1Game.camera.cameraPosition);
            effect.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);
            effect.Parameters["lightAmbCol"].SetValue(Project1Game.camera.ambientColour);
            effect.Parameters["lightPntPos"].SetValue(Project1Game.camera.sunPosition);
            effect.Parameters["lightPntCol"].SetValue(Project1Game.camera.sunColour);

            effect.CurrentTechnique.Passes[0].Apply();

            this.model.Draw(game.GraphicsDevice, this.world, this.view, this.projection);
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //this.position = Project1Game.camera.cameraPosition + Vector3.Normalize(Project1Game.camera.cameraDirection);
            view = Matrix.LookAtRH(Project1Game.camera.cameraPosition, Project1Game.camera.cameraLook, Vector3.Up); ;
            projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);


            world = Matrix.Scaling(0.01f)* Matrix.RotationX((float)Math.PI/2f * delta/100f) *Matrix.RotationZ((float)Math.PI * delta/100f) * Matrix.Translation(position);

            WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(world));
        }

    }
}