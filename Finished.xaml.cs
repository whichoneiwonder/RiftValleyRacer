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
    public sealed partial class Finished : Page
    {
        public readonly Project1Game game;
        private MainPage parent;

        public Finished(MainPage parent)
        {
            this.InitializeComponent();
            this.parent = parent;
            if (parent.game.racer_won == "player")
            {

            }
        }

        private void cmdMenu_Click(object sender, RoutedEventArgs e)
        {
            parent.Children.Remove(this);
            parent.game.Exit();
            parent.Children.Add(parent.mainMenu);
        }
    }
}
