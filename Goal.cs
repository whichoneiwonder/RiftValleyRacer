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
    public class Goal : GameObject
    {
        public Vector3 position;
        public Model model;
        public Matrix world, view, projection;

        public Goal(Project1Game game, Vector3 position)
        {
            this.game = game;
            this.position = position;
            this.model = game.Content.Load<Model>("goalTriangle");
        }

        public override void Update(GameTime gameTime)
        { 
            world = Matrix.Scaling(3f)*Matrix.RotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.Translation(position);
            view = Project1Game.camera.View;
            projection = Project1Game.camera.Projection;
        }

        public override void Draw(GameTime gameTime)
        {
            this.model.Draw(game.GraphicsDevice, this.world, this.view, this.projection);
        }
    }
}
