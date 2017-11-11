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
using Microsoft.Kinect.Face;

namespace FirstProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Kinect SDK
        KinectSensor kinectSensor = KinectSensor.GetDefault();
        
        ColorFrameReader colorFrameReader;
        FrameDescription colorFrameDescription;
        ColorImageFormat colorImageFormat = ColorImageFormat.Bgra;

        DepthFrameReader depthFrameReader;
        FrameDescription depthFrameDescription;

        BodyFrameReader bodyFrameReader;
        IList<Body> bodies;
        int bodyCount;

        FaceFrameSource[] faceFrameSource;
        FaceFrameReader[] faceFrameReader;
        FaceFrameResult[] faceFrameResults;


        //WPF
        WriteableBitmap colorBitmap;
        byte[] colorBuffer;
        int colorStride;
        Int32Rect colorRect;

        WriteableBitmap depthImage;
        ushort[] depthBuffer;
        byte[] depthBitmapBuffer;
        Int32Rect depthRect;
        int depthStride;
        Point depthPoint;
        const int R = 20;


        public MainWindow()
        {
            InitializeComponent();
        }

        //Imagem a cores
        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            if (kinectSensor.IsOpen)
            {
                try
                {
                    kinectSensor.Close();
                    labelStatus.Content = "Kinect Desconnected";
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    Close();
                }
            }
            else
            {
                labelStatus.Content = "Kinect Collecting Color Image";
                kinectSensor.Open();

                bodies = new Body[kinectSensor.BodyFrameSource.BodyCount];

                try
                {
                    colorFrameDescription = kinectSensor.ColorFrameSource.CreateFrameDescription(colorImageFormat);

                    colorFrameReader = kinectSensor.ColorFrameSource.OpenReader();
                    colorFrameReader.FrameArrived += colorFrameReader_colorArrived;

                    colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96, 96, PixelFormats.Bgra32, null);
                    colorStride = colorFrameDescription.Width * (int)colorFrameDescription.BytesPerPixel;
                    colorRect = new Int32Rect(0, 0, colorFrameDescription.Width, colorFrameDescription.Height);
                    colorBuffer = new byte[colorStride * colorFrameDescription.Height];
                    Screen.Source = colorBitmap;
                    

                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    Close();
                }
                
            }
        }

        //Imagem de Profundidade
        private void btnDepth_Click(object sender, RoutedEventArgs e)
        {
            if (kinectSensor.IsOpen)
            {
                try
                {
                    kinectSensor.Close();
                    labelStatus.Content = "Kinect Desconnected";
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    Close();
                }
            }
            else
            {
                labelStatus.Content = "Kinect Collecting Depth Image";
                kinectSensor.Open();
                try
                {
                    depthFrameDescription = kinectSensor.DepthFrameSource.FrameDescription;

                    depthImage = new WriteableBitmap(depthFrameDescription.Width, depthFrameDescription.Height, 96, 96, PixelFormats.Gray8, null);
                    depthBuffer = new ushort[depthFrameDescription.LengthInPixels];
                    depthBitmapBuffer = new byte[depthFrameDescription.LengthInPixels];
                    depthRect = new Int32Rect(0, 0, depthFrameDescription.Width, depthFrameDescription.Height);
                    depthStride = (int)depthFrameDescription.Width;

                    Screen.Source = depthImage;

                    depthPoint = new Point(depthFrameDescription.Width / 2, depthFrameDescription.Height / 2);

                    depthFrameReader = kinectSensor.DepthFrameSource.OpenReader();
                    depthFrameReader.FrameArrived += depthFrameReader_FrameArrived;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    Close();
                }

            }
        }

        private void depthFrameReader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            try
            {
                UpdateEllipsePosition();
                UpdateDepth(e);
                DrawDepthFrame();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Close();
            }
        }

        private void DrawDepthFrame()
        {
            try
            {
                //0 a 8000
                for (int i=0;i < depthBuffer.Length;i++)
                {
                    depthBitmapBuffer[i] = (byte)(depthBuffer[i] % 255);
                }
                depthImage.WritePixels(depthRect, depthBitmapBuffer, depthStride, 0);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Close();
            }
        }
        
        private void UpdateDepth(DepthFrameArrivedEventArgs e)
        {
            try
            {
                using (var depthFrame = e.FrameReference.AcquireFrame())
                {
                    if (depthFrame == null)
                    {
                        return;
                    }
                    depthFrame.CopyFrameDataToArray(depthBuffer);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Close();
            }
        }

        void colorFrameReader_colorArrived(object sender, ColorFrameArrivedEventArgs args)
        {
            try
            {
                UpdateEllipsePosition();
                UpdateColorFrame(args);
                DrawColorFrame();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Close();
            }
        }

        private void DrawColorFrame()
        {
            try
            {
                colorBitmap.WritePixels(colorRect,colorBuffer,colorStride,0);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Close();
            }
        }

        private void UpdateColorFrame(ColorFrameArrivedEventArgs args)
        {
            try
            {
                using (var colorFrame = args.FrameReference.AcquireFrame())
                {
                    if (colorFrame == null)
                    {
                        return;
                    }
                    colorFrame.CopyConvertedFrameDataToArray(colorBuffer, colorImageFormat);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Close();
            }
        }

        private void UpdateEllipsePosition()
        { 
            Canvas.SetLeft(Target, Canvas.ActualWidth/2 - Target.ActualWidth/2);
            Canvas.SetTop(Target, Canvas.ActualHeight/2 - Target.ActualHeight/2);
            labelStatus.Content = "X "+(Canvas.ActualWidth / 2 - Target.ActualWidth / 2) +
                " - Y "+(Canvas.ActualHeight / 2 - Target.ActualHeight / 2);
        }
        
    }
}
