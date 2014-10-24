using System;
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
        { }

        private void pickBlue(object sender, RoutedEventArgs e)
        { }
        private void pickGreen(object sender, RoutedEventArgs e)
        {}
            
        private void pickPurple(object sender, RoutedEventArgs e)
        {}
        
        private void Return(object sender, RoutedEventArgs e)
        {
            parent.Children.Add(parent.mainMenu);
            parent.Children.Remove(this);
        }

        private void changeSize(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            parent.game.tSizeFactor = (int)e.NewValue; 
        }
       
    }
}
