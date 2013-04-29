using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace RPGMakerFacesGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CommandBinding_Exit(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private string _filename = null;

        private string filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
                WindowTitle = "RPG Faces Generator";
                if (_filename != null)
                {
                    WindowTitle += " - " + System.IO.Path.GetFileName(_filename);
                    SuggestedExportFilename = System.IO.Path.GetFileNameWithoutExtension(_filename) + ".png";
                }
                else
                {
                    SuggestedExportFilename = null;
                }
            }
        }


        public string WindowTitle
        {
            get { return (string)GetValue(WindowTitleProperty); }
            set { SetValue(WindowTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowTitleProperty = 
            DependencyProperty.Register("WindowTitle", typeof(string), typeof(MainWindow), new UIPropertyMetadata("RPG Faces Generator"));


        public string SuggestedExportFilename
        {
            get { return (string)GetValue(SuggestedExportFilenameProperty); }
            set { SetValue(SuggestedExportFilenameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SuggestedExportFilename.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuggestedExportFilenameProperty =
            DependencyProperty.Register("SuggestedExportFilename", typeof(string), typeof(MainWindow), new UIPropertyMetadata(null));


        private string save(string suggestedFilename, ActiveFaceParts activeFaceParts)
        {
            if (suggestedFilename == null)
            {
                var sfd = new SaveFileDialog();
                sfd.FileName = "facesset";
                sfd.DefaultExt = ".rfg";
                sfd.Filter = "RPG Faces Generator (.rfg)|*.rfg";
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == true)
                    suggestedFilename = sfd.FileName;
                else
                    return null;
            }

            using (var fileStream = new FileStream(suggestedFilename, FileMode.Create))
            using (var outputStream = new StreamWriter(fileStream))
                activeFaceParts.SaveTo(outputStream);
            MessageBox.Show("File saved successfully!");

            return suggestedFilename;
        }
        
        private void CommandBinding_Save(object sender, ExecutedRoutedEventArgs e)
        {
            string newFilename = save(filename, (ActiveFaceParts)e.Parameter);
            if (newFilename != null)
                filename = newFilename;
        }

        private void CommandBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            string newFilename = save(null, (ActiveFaceParts)e.Parameter);
            if (newFilename != null)
                filename = newFilename;
        }

        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.DefaultExt = ".rfg";
            ofd.Filter = "RPG Faces Generator (.rfg)|*.rfg";
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(ofd.FileName, FileMode.Open))
                using (var inputStream = new StreamReader(fileStream))
                {
                    ((ActiveFaceParts)e.Parameter).LoadFrom(inputStream);
                    filename = ofd.FileName;
                }
            }
        }

        private void CommandBinding_About(object sender, ExecutedRoutedEventArgs e)
        {
            var ab = new AboutBox(this);
            ab.ShowDialog();
        }

        private void CommandBinding_New(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to start with a new face?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ((ActiveFaceParts)e.Parameter).Reset();
                filename = null;
            }
        }
    }
}
