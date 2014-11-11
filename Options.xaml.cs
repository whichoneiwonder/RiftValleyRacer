using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
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
 
    public sealed partial class Options
    {
        public readonly Project1Game game;
        private MainPage parent;

        public Options(MainPage parent)
        {
            this.parent = parent;
            this.InitializeComponent();
        }

        private void pickRed(object sender, RoutedEventArgs e) 
        {
            parent.modelToLoad = "HoverBike2";
            this.cmdPurple.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdRed.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.cmdGreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdBlue.Visibility = Windows.UI.Xaml.Visibility.Visible;

            this.lblPurple.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblRed.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.lblGreen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblBlue.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void pickBlue(object sender, RoutedEventArgs e)
        {
            parent.modelToLoad = "HoverBike1";
            this.cmdPurple.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdRed.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdGreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdBlue.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            this.lblPurple.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblRed.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblGreen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblBlue.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void pickGreen(object sender, RoutedEventArgs e)
        {
            parent.modelToLoad = "HoverBike3";
            this.cmdPurple.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdRed.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdGreen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.cmdBlue.Visibility = Windows.UI.Xaml.Visibility.Visible;

            this.lblPurple.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblRed.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblGreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.lblBlue.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void pickPurple(object sender, RoutedEventArgs e)
        {
            parent.modelToLoad = "HoverBike4";
            this.cmdPurple.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.cmdRed.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdGreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cmdBlue.Visibility = Windows.UI.Xaml.Visibility.Visible;
            
            this.lblPurple.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.lblRed.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblGreen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.lblBlue.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
                    

        private void Return(object sender, RoutedEventArgs e)
        {
            parent.Children.Add(parent.mainMenu);
            parent.Children.Remove(this);
        }

        private void changeSize(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            parent.game.tSizeFactor = (int)e.NewValue;
        }

        private void rdOff_Checked(object sender, RoutedEventArgs e)
        {
            parent.music_switch("off");
        }

        private void rdOn_Checked(object sender, RoutedEventArgs e)
        {
            parent.music_switch("on");
        }

        private void cmdEasy_Click(object sender, RoutedEventArgs e)
        {
            parent.game.opponent_difficulty = 5;
        }

        private void cmdNormal_Click(object sender, RoutedEventArgs e)
        {
            parent.game.opponent_difficulty = 10;
        }

        private void cmdHard_Click(object sender, RoutedEventArgs e)
        {
            parent.game.opponent_difficulty = 15;
        }
       
    }
}
