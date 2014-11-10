﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Finished : Page
    {
        public readonly Project1Game game;
        private MainPage parent;

        public Finished(MainPage parent)
        {
            this.parent = parent;
          // Need this fixed.
            


            this.InitializeComponent();
        }

        private void cmdMenu_Click(object sender, RoutedEventArgs e)
        {
            parent.Children.Remove(this);
            parent.game.Exit();
            parent.Children.Add(parent.mainMenu);
        }

        private void cmdExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }


        public void FinishUpdate(String winner)
        {
            if (parent.game.racer_won == "opponent")
            {
                this.VictoryDefeat.Text = "DEFEAT";
                this.VictoryDefeat.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.txtFirst.Text = "OPPONENT";
                this.txtSecond.Text = "PLAYER";
            }
            else if (parent.game.racer_won == "player")
            {
                this.VictoryDefeat.Text = "VICTORY";
                this.VictoryDefeat.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                this.txtFirst.Text = "PLAYER";
                this.txtSecond.Text = "OPPONENT";
            }

            this.txtDuration.Text = "" + this.parent.game.elapsedTime.ToString();
        }
    }
}
