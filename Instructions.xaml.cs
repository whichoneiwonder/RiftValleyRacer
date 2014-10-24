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

using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using SharpDX;

namespace Project
{
    public sealed partial class Instructions
    {
        // Checks for current page number
        private static int page_num = 0;

        private string[] text_l = {"1. To steer, rotate the tablet towards the direction you want to move your vehicle.",  
                                   "2. The aim of the game is to reach the goal first before the opponent does.", 
                                   "3. Follow the red arrow above, which points towards the goal location.",
                                   "The more you play, the better you'll be! What're you waiting for? Start playing and HAVE FUN! :D"}; 

        private MainPage parent;

        public Instructions(MainPage parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.txtInstructions.Text = text_l[page_num];
        }

        private void StartGame(object sender, RoutedEventArgs e) 
        {
            parent.StartGame();
            parent.Children.Remove(this);
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            page_num++;

            if (page_num > 0 && page_num < 3) 
            { 
                this.cmdPrev.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.cmdNext.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.cmdBack.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.cmdStart.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            } else if (page_num == 3)
            { 
                this.cmdNext.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.cmdBack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.cmdStart.Visibility = Windows.UI.Xaml.Visibility.Visible; 
            } else if (page_num == 0) 
            { 
                this.cmdPrev.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            this.txtInstructions.Text = text_l[page_num];
        }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            page_num--;

            if (page_num > 0 && page_num < 3)
            {
                this.cmdPrev.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.cmdNext.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.cmdBack.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.cmdStart.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else if (page_num == 3)
            {
                this.cmdNext.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.cmdBack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.cmdStart.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else if (page_num == 0) { this.cmdPrev.Visibility = Windows.UI.Xaml.Visibility.Collapsed; }
            this.txtInstructions.Text = text_l[page_num];
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            page_num = 0;
            parent.Children.Add(parent.mainMenu);
            parent.Children.Remove(this);
        }
    }
}
