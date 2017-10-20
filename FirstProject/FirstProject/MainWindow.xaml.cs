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

using Microsoft.Kinect;

namespace FirstProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private KinectSensor kinectSensor = KinectSensor.GetDefault();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStram_Click(object sender, RoutedEventArgs e)
        {
            if (kinectSensor.IsOpen)
            {
                kinectSensor.Close();
                labelStatus.Content = "Kinect Desconnected";
            }
            else
            {
                kinectSensor.Open();
                labelStatus.Content = "Kinect Connected";

                ColorFrameReader frameReader = kinectSensor.ColorFrameSource.OpenReader();
                TypedReference reference = new TypedReference
                //frameReader.FrameArrived += 

                //MultiSourceFrameReader frame = kinectSensor.OpenMultiSourceFrameReader(
                //FrameSourceTypes.Color);
                // | FrameSourceTypes.Depth | FrameSourceTypes.Infrared

                
            }
        }

        //void MultiReader(object sender, MultiSourceFrameArrivedEventArgs eventArgs)
        //{
        //    var reference = eventArgs.FrameReference.AcquireFrame();

        //    //color frame
        //    using (var frame = reference.ColorFrameReference.AcquireFrame())
        //    {
        //        if (frame != null)
        //        {

        //        }
        //    }

        //    //depth frame
        //    using (var frame = reference.DepthFrameReference.AcquireFrame())
        //    {
        //        if (frame != null)
        //        {

        //        }
        //    }

        //    //infrared frame
        //    using (var frame = reference.InfraredFrameReference.AcquireFrame())
        //    {
        //        if (frame != null)
        //        {

        //        }
        //    }
        //}

        //private ImageSource ToBitmap(ColorFrame colorFrame)
        //{
        //    int width = colorFrame.FrameDescription.Width;
        //    int height = colorFrame.FrameDescription.Height;

        //    byte[] pixels = new byte[width*height*((PixelFormats.Bgr32.BitsPerPixel +7)/8)];

        //    if (colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
        //    {
        //        colorFrame.CopyRawFrameDataToArray(pixels);
        //    }
        //    else
        //    {
        //        colorFrame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
        //    }

        //    int stride = width * PixelFormats.Bgr32.BitsPerPixel / 8;

        //    return BitmapSource.Create(width,height,StreamScreen.Width,
        //        StreamScreen.Height,PixelFormats.Bgr32,null,pixels,stride);
        //}
    }
}
