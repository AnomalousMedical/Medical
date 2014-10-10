using Engine;
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
        private Object colorPixelsLock = new object();
        private Body[] bodies;
        private BodyFrameReader bodyFrameReader;
        private ColorFrameReader colorFrameReader;
        bool processedLastColorFrame = true;

        /// <summary>
        /// Called when a skeleton frame is ready, this event will fire on the main application thread.
        /// </summary>
        public event Action<Body[]> SkeletonFrameReady;

        /// <summary>
        /// Called when a color frame is ready, this event will fire on the main application thread.
        /// </summary>
        public event Action<byte[]> SensorColorFrameReady;

        public event Action<KinectSensorManager> StatusChanged;

        public event Action<KinectSensorManager> ColorFeedChanged;

        public KinectSensorManager()
        {
            ThreadPool.QueueUserWorkItem((state) =>
                {
                    findSensor();
                });
        }

        public void Dispose()
        {
            if (bodyFrameReader != null)
            {
                bodyFrameReader.Dispose();
                bodyFrameReader = null;
            }

            setColorFeedEnabled(false, activeSensor);

            if (activeSensor != null)
            {
                activeSensor.Close();
                activeSensor = null;
                Connected = false;
            }
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
                    useColorFeed = value;
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

        public bool HasColorFeed
        {
            get
            {
                return colorFrameReader != null;
            }
        }

        public IntSize2 ColorFrameSize { get; private set; }

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

                    if(useColorFeed)
                    {
                        setColorFeedEnabled(true, localSensor);
                    }

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

        private void setColorFeedEnabled(bool enabled, KinectSensor sensor)
        {
            lock (colorPixelsLock)
            {
                if (enabled)
                {
                    if (colorFrameReader == null)
                    {
                        colorFrameReader = sensor.ColorFrameSource.OpenReader();
                        colorFrameReader.FrameArrived += colorFrameReader_FrameArrived;
                        FrameDescription colorFrameDescription = sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
                        ColorFrameSize = new IntSize2(colorFrameDescription.Width, colorFrameDescription.Height);
                        colorPixels = new byte[colorFrameDescription.LengthInPixels * 4];
                    }
                }
                else
                {
                    if (colorFrameReader != null)
                    {
                        colorFrameReader.Dispose();
                        colorFrameReader = null;
                        colorPixels = null;
                    }
                }

                if (ColorFeedChanged != null)
                {
                    ThreadManager.invoke(() => ColorFeedChanged.Invoke(this));
                }
            }
        }

        void colorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            lock (colorPixelsLock)
            {
                if (processedLastColorFrame && useColorFeed)
                {
                    processedLastColorFrame = false;
                    using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
                    {
                        if (colorFrame != null)
                        {
                            FrameDescription colorFrameDescription = colorFrame.FrameDescription;
                            //If the frame parameters don't match, don't process this frame
                            if (colorFrameDescription.Width == ColorFrameSize.Width && colorFrameDescription.Height == ColorFrameSize.Height)
                            {
                                using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                                {
                                    colorFrame.CopyConvertedFrameDataToArray(colorPixels, ColorImageFormat.Rgba);
                                }

                                ThreadManager.invoke(() =>
                                {
                                    lock (colorPixelsLock)
                                    {
                                        if (SensorColorFrameReady != null)
                                        {
                                            if (colorPixels != null)
                                            {
                                                SensorColorFrameReady.Invoke(colorPixels);
                                            }
                                        }
                                        processedLastColorFrame = true;
                                    }
                                });
                            }
                            else
                            {
                                processedLastColorFrame = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
