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

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using SharpDX;
using System;
using System.Diagnostics;

namespace Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public readonly Project1Game game;
        public MainMenu mainMenu;
        public Instructions howTo;
        public Options opt;
        public Pause pause;
        public Finished finish;
        public string modelToLoad = "HoverBike1";
        // Timer variables 
        public int time = 0;

        public MainPage()
        {
            InitializeComponent();
            game = new Project1Game(this);
            game.Run(this);
            mainMenu = new MainMenu(this);
            howTo = new Instructions(this);
            opt = new Options(this);
            pause = new Pause(this);
            finish = new Finished(this);
            this.Children.Add(mainMenu);
        }

        private void ForwardPress(object sender, RoutedEventArgs e)
        {
            this.cmdThrust1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.cmdThrust2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.game.player.forward = true;
        }

        private void ForwardRelease(object sender, RoutedEventArgs e)
        {
            this.cmdThrust2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.cmdThrust1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.game.player.forward = false;
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            this.game.isPaused = true;
            this.Children.Add(pause);
        }

        private void BackwardPress(object sender, RoutedEventArgs e)
        {
            this.cmdReverse1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.cmdReverse2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.game.player.backward = true;
        }

        private void BackwardRelease(object sender, RoutedEventArgs e)
        {
            this.cmdReverse2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.cmdReverse1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.game.player.backward = false;
        }

        public Double Seek()
        {
            Vector3 heading = (game.player.heading);
            Vector3 toGoal = (game.goal.position - game.player.position);
            heading.Y = 0;
            toGoal.Y = 0;
            heading.Normalize();
            toGoal.Normalize();
            this.arrow_UP.Visibility = Windows.UI.Xaml.Visibility.Visible;

            float dotprod = Vector3.Dot(heading, toGoal);
            Double angle = 180 * Math.Acos((double)dotprod) / Math.PI;

            if (Vector3.Cross(heading, toGoal).Y > 0f)
            {
                angle = -angle;
            }
            //account for the fact that the image points left
            angle += 90;
            while (angle > 359)
            {
                angle -= 360;
            }
            while (angle < 0)
            {
                angle += 360;
            }
            try
            {
                this.Arrow_Up_rotation_transform.Angle = angle;
            }
            catch { };

            return angle;
        }

        public void EndGame()
        {
            this.game.Exit();
            this.Children.Add(this.finish);
        }

        public void updateSpeed(float speed)
        {
            this.txtSpeed.Text = ""+ speed + " m/s"; 
        }

        public void first()
        {
            this.txtPosition.Text = "1ST";
        }

        public void second()
        {
            this.txtPosition.Text = "2ND";
        }

        public void music_switch(String toggle)
        { 
            if (toggle == "off")
            {
                this.media.Pause();
            }
            else if (toggle == "on")
            {
                this.media.Play();
            }   
        }

        public void StartGame()
        {
            game.player.loadMod(modelToLoad);
            this.Children.Remove(mainMenu);
            this.Children.Remove(howTo);
            game.started = true;
        }
    }
}
