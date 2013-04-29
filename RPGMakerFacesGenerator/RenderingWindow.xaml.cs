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
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace RPGMakerFacesGenerator
{
    /// <summary>
    /// Interaction logic for RenderingWindow.xaml
    /// 
    /// Since RenderTargetBitmap renders in software and thus doesn't support
    /// PS_3 (which we are using in HSV adjustements), I have to run this
    /// weird wpf/winform mix with special considerations for the DPIs.
    /// </summary>
    public partial class RenderingWindow : Window
    {
        private string suggestedFilename;

        public RenderingWindow(string suggestedFilename)
        {
            InitializeComponent();

            this.suggestedFilename = suggestedFilename;
            Loaded += new RoutedEventHandler(RenderingWindow_Loaded);
        }

        private float dpiX, dpiY;

        public void SaveScreen(string filename)
        {
            Bitmap myImage = new Bitmap(225, 225);

            Graphics gr1 = Graphics.FromImage(myImage);
            IntPtr dc1 = gr1.GetHdc();

            IntPtr dc2 = NativeMethods.GetWindowDC(NativeMethods.GetForegroundWindow());

            Dispatcher.Invoke(new MethodInvoker(() =>
            {
                Debug.Assert(Math.Abs(faceDisplay.ActualWidth * dpiX / 96.0 - 225) < 0.1);
                Debug.Assert(Math.Abs(faceDisplay.ActualHeight * dpiY / 96.0 - 225) < 0.1);

                NativeMethods.BitBlt(dc1,
                    0,
                    0,
                    225,
                    225,
                    dc2, 0, 0, 13369376);
                gr1.ReleaseHdc(dc1);
                myImage.Save(filename, ImageFormat.Png);
            }));
        }

        void RenderingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // reset size
                using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    dpiX = graphics.DpiX;
                    dpiY = graphics.DpiY;
                    var neededWidth = 225 * 96.0 / graphics.DpiX;
                    var neededHeight = 225 * 96.0 / graphics.DpiY;
                    faceDisplay.Width = Width = neededWidth;
                    faceDisplay.Height = Height = neededHeight;
                }

                faceDisplay.UpdateLayout();
                UpdateLayout();

                // ask for file name
                var sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.FileName = suggestedFilename;
                sfd.DefaultExt = ".png";
                sfd.Filter = "PNG Image (.png)|*.png";
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == true)
                {
                    SaveScreen(sfd.FileName);
                    System.Windows.MessageBox.Show("Rendering completed successfully!");
                }
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(string.Format("Error during rendering: {0}", exc));
            }
            finally
            {
                Close();
            }
        }
    }
}
