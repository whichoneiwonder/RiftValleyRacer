﻿// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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

            this.game.player.accelerate(3f);
        }

        private void Backward(object sender, RoutedEventArgs e)
        {
            
            this.game.player.accelerate(-3f);
        }

        // TASK 1: Update the game's score
        public void UpdateScore(int score)
        {
           
        }

        public void StartGame()
        {
            this.Children.Remove(mainMenu);
            this.Children.Remove(howTo);
            game.started = true;
        }
    }
}
