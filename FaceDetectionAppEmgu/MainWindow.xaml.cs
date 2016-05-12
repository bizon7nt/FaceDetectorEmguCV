using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.CvEnum;


namespace FaceDetectionAppEmgu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string SelectedImage { get; set; }

        private CascadeClassifier classifier { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpeg";
            dlg.Filter = "All Files|*.*|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value)
            {
                // Open document 
                string filename = dlg.FileName;
                OpenImage(filename);
            }
        }

        private void OpenImage(string file)
        {
            var imageSource = new BitmapImage(new Uri(file, UriKind.Absolute));
            image.Source = imageSource;
            SelectedImage = file;
        }

        private void faceDetect_Click(object sender, RoutedEventArgs e)
        {
            Image<Bgr, byte> My_Image = new Image<Bgr, byte>(SelectedImage);

            // there's only one channel (greyscale), hence the zero index
            //var faces = nextFrame.DetectHaarCascade(haar)[0];
            Image<Gray, byte> grayImage = My_Image.Convert<Gray, byte>();
            double scaleNumber = 1.1;
            int minNeighbours = 4;
            System.Drawing.Size minSize = new System.Drawing.Size(grayImage.Width / 8, grayImage.Height / 8);
            System.Drawing.Size maxSize = new System.Drawing.Size(grayImage.Width / 2, grayImage.Height / 2);
            classifier = new CascadeClassifier("haarcascades/haarcascade_frontalface_alt2.xml");
            var faces = classifier.DetectMultiScale(grayImage, scaleNumber, minNeighbours, minSize, maxSize);

            foreach (var face in faces)
            {
                My_Image.Draw(face, new Bgr(System.Drawing.Color.Red), 5);
            }
            image.Source = ToBitmapSource(My_Image);
        }

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap
                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                //DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }
    }
}
