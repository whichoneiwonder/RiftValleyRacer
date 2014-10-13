// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;

namespace Project1
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class Project1Game : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private List<GameObject> models;
        private Sun sun;
        public Camera camera;
        Landscape landscape;
        /// <summary>
        /// Initializes a new instance of the <see cref="Project1Game" /> class.
        /// </summary>
        public Project1Game()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Create the keyboard manager

        }

        protected override void LoadContent()
        {
            camera = new Camera(this);
            models = new List<GameObject>();
            models.Add(landscape = new Landscape(this));
            models.Add(sun = new Sun(this));
            models.Add(new WaterGraphic(this));
            // Create an input layout from the vertices
            
            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Lab 2";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {

            camera.Update(gameTime);
            camera.moveIfBounded(landscape);

            sun.basicEffect.World = Matrix.Scaling(10f) *
                Matrix.RotationYawPitchRoll((float)gameTime.TotalGameTime.TotalSeconds,
                (float)gameTime.TotalGameTime.TotalSeconds / 2,
                (float)gameTime.TotalGameTime.TotalSeconds / 5) *
                 Matrix.Translation(0f, 350f, 0f)
                * Matrix.RotationX((float)gameTime.TotalGameTime.TotalSeconds / 20 - 1)
                * Matrix.Translation(camera.pos); 
            
            
            for (int i = 0; i < models.Count; i++)
            {
                models[i].basicEffect.DirectionalLight0.Direction = (Vector3)Vector3.Transform(
                    -Vector3.Up , Matrix.RotationX((float)gameTime.TotalGameTime.TotalSeconds/20 - 1));
                models[i].Update(gameTime);
            }
            

            // Handle base.Update
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(new Color(0,0,0.2f));

            for (int i = 0; i < models.Count; i++)
            {
                models[i].Draw(gameTime);
            }
            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
