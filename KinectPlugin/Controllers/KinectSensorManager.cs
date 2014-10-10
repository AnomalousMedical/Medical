using Logging;
using Medical.Controller;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectSensorManager : IDisposable
    {
        private KinectSensor activeSensor;
        private bool connected;
        private bool useColorFeed = false;
        private byte[] colorPixels;
        private Body[] bodies;
        private BodyFrameReader bodyFrameReader;

        /// <summary>
        /// Called when a skeleton frame is ready, this event will fire on the main application thread.
        /// </summary>
        public event Action<Body[]> SkeletonFrameReady;

        /// <summary>
        /// Called when a color frame is ready, this event will fire on the main application thread.
        /// </summary>
        public event Action<byte[]> SensorColorFrameReady;

        public event Action<KinectSensorManager> StatusChanged;

        public KinectSensorManager()
        {
            //KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            ThreadPool.QueueUserWorkItem((state) =>
                {
                    findSensor();
                });
        }

        public void Dispose()
        {
            disconnectSensor();
        }

        public bool Connected
        {
            get
            {
                return connected;
            }
            private set
            {
                if (connected != value)
                {
                    connected = value;
                    if(StatusChanged != null)
                    {
                        ThreadManager.invoke(() =>
                        {
                            if (StatusChanged != null)
                            {
                                StatusChanged.Invoke(this);
                            }
                        });
                    }
                }
            }
        }

        public bool UseColorFeed
        {
            get
            {
                return useColorFeed;
            }
            set
            {
                if(useColorFeed != value)
                {
                    useColorFeed = true;
                    if(activeSensor != null)
                    {
                        ThreadPool.QueueUserWorkItem((state) =>
                            {
                                if (activeSensor != null)
                                {
                                    setColorFeedEnabled(useColorFeed, activeSensor);
                                }
                            });
                    }
                }
            }
        }

        private void findSensor()
        {
            if (activeSensor == null)
            {
                KinectSensor localSensor = KinectSensor.GetDefault();

                //If a sensor was found start it and enable its skeleton listening.
                if (localSensor != null)
                {
                    localSensor.IsAvailableChanged += localSensor_IsAvailableChanged;

                    // Turn on the body stream to receive body frames
                    bodyFrameReader = localSensor.BodyFrameSource.OpenReader();
                    bodyFrameReader.FrameArrived += bodyFrameReader_FrameArrived;

                    //TransformSmoothParameters smoothParam = new TransformSmoothParameters()
                    //{
                    //    Correction = 0.5f,
                    //    JitterRadius = 0.1f,
                    //    MaxDeviationRadius = 0.04f,
                    //    Prediction = 0.0f,
                    //    Smoothing = 0.5f,
                    //};

                    //localSensor.SkeletonStream.Enable(smoothParam);

                    //localSensor.SkeletonFrameReady += sensor_SkeletonFrameReady;

                    //if (useColorFeed)
                    //{
                    //    setColorFeedEnabled(true, localSensor);
                    //}
                    //localSensor.ColorFrameReady += sensor_ColorFrameReady;

                    // Start the sensor!
                    try
                    {
                        localSensor.Open();
                        Connected = true;
                    }
                    catch (IOException)
                    {
                        localSensor = null;
                    }

                    activeSensor = localSensor; //Make the class aware of the sensor
                }
            }
        }

        void localSensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            Connected = e.IsAvailable;
        }

        void bodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
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

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived && SkeletonFrameReady != null)
            {
                ThreadManager.invoke(() => SkeletonFrameReady.Invoke(bodies));
            }
        }

        //bool processedLastColorFrame = true;
        //void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        //{
        //    //This happens on its own thread
        //    KinectSensor sensor = sender as KinectSensor;
        //    if (processedLastColorFrame && useColorFeed && SensorColorFrameReady != null && sensor != null)
        //    {
        //        processedLastColorFrame = false;
        //        using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
        //        {
        //            if (colorFrame != null)
        //            {
        //                // Copy the pixel data from the image to a temporary array
        //                colorFrame.CopyPixelDataTo(colorPixels);
        //            }
        //        }

        //        ThreadManager.invoke(() =>
        //        {
        //            if (SensorColorFrameReady != null)
        //            {
        //                SensorColorFrameReady.Invoke(colorPixels);
        //            }
        //            processedLastColorFrame = true;
        //        });
        //    }
        //}

        private void disconnectSensor()
        {
            if(bodyFrameReader != null)
            {
                bodyFrameReader.Dispose();
                bodyFrameReader = null;
            }

            if (activeSensor != null)
            {
                KinectSensor localSensor = activeSensor;
                activeSensor = null;
                localSensor.Close();
                Connected = false;
                //localSensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                //localSensor.ColorFrameReady -= sensor_ColorFrameReady;
                //localSensor.Stop();
                //localSensor.Dispose();
            }
        }

        private void setColorFeedEnabled(bool enabled, KinectSensor sensor)
        {
            //if (enabled)
            //{
            //    sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            //    colorPixels = new byte[sensor.ColorStream.FramePixelDataLength];
            //}
            //else
            //{
            //    sensor.ColorStream.Disable();
            //    colorPixels = null;
            //}
        }
    }
}
