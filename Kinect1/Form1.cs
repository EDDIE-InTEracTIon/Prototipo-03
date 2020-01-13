using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
namespace Kinect1
{
    public partial class Form1 : Form
    {
        KinectSensor kinectSensor = null;
        BodyFrameReader bodyFrameReader = null;
        MultiSourceFrameReader myReader = null;
        Body[] bodies = null;

        public Form1()
        {
            InitializeComponent();
            initialiseKinect();//kinect
        }

        public void initialiseKinect()
        {
            kinectSensor = KinectSensor.GetDefault(); 

            if(kinectSensor != null)
            {
                kinectSensor.Open();
            }

            bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
            myReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color);
            myReader.MultiSourceFrameArrived += myReader_MultiSourceFrameArrived;

            void myReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
            {
                var reference = e.FrameReference.AcquireFrame();

                using (var frame = reference.ColorFrameReference.AcquireFrame())
                {
                    if (frame != null)
                    {


                        var width = frame.FrameDescription.Width;
                        var height = frame.FrameDescription.Height;
                        var data = new byte[width * height * 32 / 8];
                        frame.CopyConvertedFrameDataToArray(data, ColorImageFormat.Bgra);

                        var bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                        var bitmapData = bitmap.LockBits(new Rectangle(0,0,bitmap.Width,bitmap.Height),ImageLockMode.WriteOnly,bitmap.PixelFormat);
                        Marshal.Copy(data, 0,bitmapData.Scan0,data.Length);
                        bitmap.UnlockBits(bitmapData);
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
                        pictureBox1.Image = bitmap;
                    }

                }


            }

            if (bodyFrameReader != null)
            {
                bodyFrameReader.FrameArrived += Reader_FrameArrived;
            }

             void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
            {
                bool dataReceived = false;

                using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
                {
                    if (bodyFrame != null)
                    {
                            if (bodies == null)
                            {
                                bodies = new Body[bodyFrame.BodyCount];
                            }
                    }
                   

                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }

                if (dataReceived)
                {
                    foreach(Body body in bodies)
                    {
                        if ( body.IsTracked)
                        {
                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            Joint handR = joints[JointType.HandRight];

                            float hr_distance_x = handR.Position.X;
                            float hr_distance_y = handR.Position.Y;
                            float hr_distance_z = handR.Position.Z;

                            textBox1.Text = hr_distance_x.ToString("#.##");
                            textBox2.Text = hr_distance_y.ToString("#.##");
                            textBox3.Text = hr_distance_z.ToString("#.##");

                            Joint handL = joints[JointType.HandLeft];

                            float hl_distance_x = handL.Position.X;
                            float hl_distance_y = handL.Position.Y;
                            float hl_distance_z = handL.Position.Z;

                            textBox6.Text = hl_distance_x.ToString("#.##");
                            textBox5.Text = hl_distance_y.ToString("#.##");
                            textBox4.Text = hl_distance_z.ToString("#.##");


                        }
                    }
                }
            }
        }

 
    }
}
 