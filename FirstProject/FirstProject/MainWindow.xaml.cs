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
        //Kinect SDK
        private KinectSensor kinectSensor = KinectSensor.GetDefault();
        //private MultiSourceFrameReader reader = null;
        ColorFrameReader colorFrameReader;
        FrameDescription colorFrameDescription;
        ColorImageFormat colorImageFormat = ColorImageFormat.Bgra;

        DepthFrameReader depthFrameReader;
        FrameDescription depthFrameDescription;

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
                    //Imagem de Profundidade
                    depthFrameDescription = kinectSensor.DepthFrameSource.FrameDescription;

                    depthImage = new WriteableBitmap(depthFrameDescription.Height, depthFrameDescription.Width, 96, 96, PixelFormats.Gray8, null);
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
                //UpdateDepthValue();

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

        //private void UpdateDepthValue()
        //{
        //    try
        //    {
        //        CanvasPoint.Children.Clear();

        //        var ellipse = new Ellipse()
        //        {
        //            Width = R,
        //            Height = R,
        //            StrokeThickness = R / 4,
        //            Stroke = Brushes.Red,
        //        };
        //        Canvas.SetLeft(ellipse, depthPoint.X - (R / 2));
        //        Canvas.SetTop(ellipse, depthPoint.Y - (R / 2));
        //        CanvasPoint.Children.Add(ellipse);

        //        int depthindex = (int)((depthPoint.Y * depthFrameDesc.Width) + depthPoint.X);

        //        var text = new TextBlock()
        //        {
        //            Text = string.Format("{0}mm", depthBuffer[depthindex]),
        //            FontSize = 20,
        //            Foreground = Brushes.Green,
        //        };
        //        Canvas.SetLeft(text, depthPoint.X);
        //        Canvas.SetTop(text, depthPoint.Y - R);
        //        CanvasPoint.Children.Add(text)
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message);
        //        Close();
        //    }
        //}

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
        
    }
}
