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

        public MainPage()
        {
            InitializeComponent();
            game = new Project1Game(this);
            game.Run(this);
            mainMenu = new MainMenu(this);
            howTo = new Instructions(this);
            opt = new Options(this);

            this.Children.Add(mainMenu);
            
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            if (this.game.player.forward) { this.game.player.forward = false; }
            else { this.game.player.forward = true; }
            this.game.player.backward = false;
        }

        private void Pause(object sender, RoutedEventArgs e)
        { }
        private void Backward(object sender, RoutedEventArgs e)
        {
            if (this.game.player.backward) { this.game.player.backward = false; }
            else { this.game.player.backward = true; }
            this.game.player.forward = false;
        }

      
      public void Seek()
        {
            Vector2 heading = Vector2.Normalize(new Vector2(game.player.heading.X, game.player.heading.Z));
            Vector2 toGoal = Vector2.Normalize(new Vector2(game.goal.position.X - game.player.position.X, game.goal.position.Z - game.player.position.Z));

          this.arrow_DOWN.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
          
          this.arrow_UP.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
          
          this.arrow_LEFT.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
          
          this.arrow_RIGHT.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            float dotprod = Vector2.Dot(heading, toGoal);
            Double angle = Math.Acos((double)dotprod);
            if(dotprod > 0){
                
                if (angle >= Math.PI*(3f/4f))
                {
                    //this.arrow_LEFT.Visibility = Windows.UI.Xaml.Visibility.Visible;
                } 
                else if( angle <= Math.PI/4f){

                    this.arrow_RIGHT.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    this.arrow_UP.Visibility = Windows.UI.Xaml.Visibility.Visible;

                }

            }else 
            {
                if (angle >= Math.PI*(3f/4f))
                {
                    this.arrow_LEFT.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else if (angle <= Math.PI / 4f)
                {

                    this.arrow_RIGHT.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    this.arrow_DOWN.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
           

        }

        public void StartGame()
        {
            this.Children.Remove(mainMenu);
            this.Children.Remove(howTo);
            game.started = true;
        }
    }
}
