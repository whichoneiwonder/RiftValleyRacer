/* Modified version of the Camera Class from the solution to Lab 4
 * Using Microsoft developer network tutorial 
 * at http://msdn.microsoft.com/en-us/library/bb197901.aspx (september 2014)
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Project1
{
    public class Camera
    {
        //public BoundingSphere boundSphere;
        public Matrix View;
        public Matrix Projection;
        public Project1Game game;
        private KeyboardManager keyboardManager;
        private KeyboardState keyboard;
        private MouseManager mouseManager;
        private MouseState mouse;
        public Vector3 pos,target;
        private float yaw, pitch, roll;
        private double timePassed;
        private Boolean freeMouse;
        private Vector3 rotationReference;
        private static float mouseSensitivity = 5f; 
        private BoundingSphere bounds;
        private static float speedReduction = 70;



        // Ensures that all objects are being rendered from a consistent viewpoint
        public Camera(Project1Game game) {
            keyboardManager = new KeyboardManager(game);
            mouseManager = new MouseManager(game);

            pos = new Vector3(0, 100, 0);
            target =  Vector3.Up;
            View = Matrix.LookAtLH(pos, target, Vector3.UnitY);
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, 
                (float)game.GraphicsDevice.BackBuffer.Width / 
                game.GraphicsDevice.BackBuffer.Height, 1f, 900.0f);
            this.game = game;
            yaw = 0; 
            pitch = 0;
            roll=0;
            rotationReference = new Vector3(0, 0, -1);
            timePassed = 0;
            freeMouse = false;
            bounds = new BoundingSphere(pos, 0.5f);

        }

        public void Update(GameTime time)
        {
            
            
            while(game.Window.IsMinimized == true);
            
            double delta = time.ElapsedGameTime.TotalMilliseconds - timePassed;
            
            
            
            // press escape to exit
            if(keyboard.IsKeyDown(Keys.Escape))
            {
                game.Exit();

            }
           
            //Press Space To toggle whether inputs are captured

            keyboard = keyboardManager.GetState();
            if(keyboard.IsKeyPressed(Keys.Space))
                freeMouse  = !freeMouse;

            if (freeMouse)
                return;



            /*translation*/

            if(keyboard.IsKeyDown(Keys.W))
            {
                pos = pos + Vector3.Multiply(target - pos, (float)(delta / speedReduction));
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                pos = pos - Vector3.Multiply( target - pos, (float)(delta / speedReduction));
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                pos = pos - Vector3.Multiply(Vector3.Normalize(Vector3.Cross(Vector3.Up, target - pos)), (float)(delta / speedReduction));
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                pos = pos +  Vector3.Multiply(Vector3.Normalize(Vector3.Cross(Vector3.Up, target - pos)), (float)(delta / speedReduction));
            }
            if (keyboard.IsKeyDown(Keys.Shift))
            {
                pos = pos + Vector3.Multiply( Vector3.Up, (float)(delta / speedReduction));
            }
            if (keyboard.IsKeyDown(Keys.Control))
            {
                pos = pos - Vector3.Multiply(Vector3.Up, (float)(delta / speedReduction));
            }
            
             
            /*rotation*/
            if( time.FrameCount <= 2)
           {
               mouseManager.SetPosition(new Vector2(0.50f, .050f));

           }

            mouse = mouseManager.GetState();
            //calculate left-right rotation 
            yaw +=  (mouse.X- 0.50f)* mouseSensitivity;
            //calculate up-down rotation
            pitch += (mouse.Y - 0.50f) * mouseSensitivity;
            if (pitch >= (float)Math.PI / 2 - 0.01f)
            {
                pitch = (float)Math.PI / 2 - 0.01f;
            }
            else if (pitch <= -((float)Math.PI / 2 - 0.01f))
            {
                pitch = -((float)Math.PI / 2 - 0.01f);
            }
            

            bounds.Center = pos;

            if (delta < 1f)
            {
                pitch = 0;
            }
            target = pos + (Vector3)Vector3.Transform(rotationReference, Matrix.RotationYawPitchRoll(yaw, -pitch, roll));
            
            
            // If the screen is resized, the projection matrix will change
            View = Matrix.LookAtLH(pos, target, Vector3.Up);
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);
            
            mouseManager.SetPosition(new Vector2(0.50f,0.50f)); 
        
        }
        public void moveIfBounded(Landscape boundaryLandscape){
            ///Vector3 point1, point2, point3;
            while (pos.X > Landscape.sidelength / 2 -4)
            {
                pos.X -= 0.001f;
            }
            while (pos.X < -Landscape.sidelength / 2 +4)
            {
                pos.X += 0.001f;
            }
            while (pos.Z > Landscape.sidelength / 2 - 4)
            {
                pos.Z -= 0.001f;
            }
            while (pos.Z < -Landscape.sidelength / 2 + 4)
            {
                pos.Z += 0.001f;
            }
            Vector3[] pointsToBound = boundaryLandscape.getPointsToBound(pos);
            bounds.Center =pos ;

            if (pointsToBound != null)
            {

                while (bounds.Contains(ref pointsToBound[0], ref pointsToBound[1], ref pointsToBound[2]) != ContainmentType.Disjoint
                    || bounds.Contains(ref pointsToBound[0], ref pointsToBound[2], ref pointsToBound[3]) != ContainmentType.Disjoint)
                { 
                    bounds.Center.Y += 0.0002f;
                }
                pos = bounds.Center;
            }
            target = pos + (Vector3)Vector3.Transform(rotationReference, Matrix.RotationYawPitchRoll(yaw, -pitch, roll));
            View = Matrix.LookAtLH(pos, target, Vector3.Up);
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 900.0f);

        }

    }
}
