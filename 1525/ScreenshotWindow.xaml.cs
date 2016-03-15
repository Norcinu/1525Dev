using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Diagnostics;

/*  TODO:
 *  Uses a massive newRefloatValue of memory. Perhaps this should load an image on the fly
 *  when a button is pressed rather than loading them all into memory. 
 */

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for ScreenshotWindow.xaml
    /// </summary>
    public partial class ScreenshotWindow : Window
    {
        string[] files = new string[] { "" };
        string Filename { get; set; }
        string FileDate { get; set; }
        string FileTime { get; set; }
        
        List<System.Drawing.Image> images = new List<System.Drawing.Image>();
        int currentImage = 0;
        readonly int maxImages = 0;
        

        public ScreenshotWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            Filename = "";
            FileDate = "";
            FileTime = "";

            try
            {
                files = Directory.GetFiles(@"D:\GameHistory", "*.png", SearchOption.TopDirectoryOnly);
                Array.Sort(files, (str1, str2) => File.GetCreationTime(str1).CompareTo(File.GetCreationTime(str2)));
                maxImages = files.Length - 1;// images.Count - 1;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            
            SetImageSource();
            UpdateLabel();
        }
        
        private void SetImageSource()
        {
            try
            {
                var bi = new BitmapImage();
                var ms = new MemoryStream();
                bi.BeginInit();
                var image = System.Drawing.Image.FromFile(files[currentImage]);
                image.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();
                Image1.Source = bi;

                Filename = files[currentImage];
                LblFilename.Content = "Filename: " + files[currentImage];
                var str = File.GetCreationTime(files[currentImage]);
                LblDate.Content = "Game Date: " + str.Date.ToString(@"dd/MM/yyyy");
                LblTime.Content = "Game Time: " + str.Hour + ":" + str.Minute + ":" + str.Second;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage < maxImages)
            {
                ++currentImage;
                SetImageSource();
                UpdateLabel();
            }
        }
        
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage > 0)
            {
                --currentImage;
                SetImageSource();
                UpdateLabel();
            }
        }
        
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (maxImages > 10 && currentImage > 10)
                currentImage -= 10;
            else if (maxImages > 10 && currentImage < 10)
                currentImage = 0;
            else
                currentImage = 0;
        
            SetImageSource();
            UpdateLabel();
        }
        
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (maxImages > 10 && currentImage < (maxImages - 10))
                currentImage += 10;
            else if (maxImages > 10 && (currentImage > (maxImages - 10) && currentImage < maxImages))
                currentImage = maxImages;
            else
                currentImage = maxImages;
            
            SetImageSource();
            UpdateLabel();
        }
        
        private void UpdateLabel()
        {
            LblCount.Content = (currentImage + 1) + "/" + (maxImages + 1);
        }
        
        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var img in images)
            {
                img.Dispose();
            }
            Close();
        }
    }
}
