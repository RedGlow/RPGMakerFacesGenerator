using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RPGMakerFacesGenerator
{
    static class CustomCommands
    {
        static CustomCommands()
        {
            Exit = new RoutedCommand("Exit", typeof(CustomCommands));
            About = new RoutedCommand("About", typeof(CustomCommands));
        }

        public static RoutedCommand Exit { get; private set; }

        public static RoutedCommand About { get; private set; }
    }
}
