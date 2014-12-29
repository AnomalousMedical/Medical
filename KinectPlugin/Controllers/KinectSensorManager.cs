using Engine;
using Engine.Threads;
using Logging;
using Medical.Controller;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
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
        
        private Body[] bodies;
        private BodyFrameReader bodyFrameReader;
        private Object bodyFrameLock = new object();
        private bool processedLastBodyFrame = true;

        private ColorFrameReader colorFrameReader;
        private byte[] colorPixels;
        private Object colorPixelsLock = new object();
        private bool processedLastColorFrame = true;

        private HighDefinitionFaceFrameSource faceFrameSource;
        private HighDefinitionFaceFrameReader faceFrameReader;
        private FaceAlignment currentFaceAlignment;
        private Object faceFrameLock = new object();
        private bool processedLastFaceFrame = true;

        /// <summary>
        /// Called when a skeleton frame is ready, this event will fire on the main application thread.
        /// </summary>
        public event Action<Body> SkeletonFrameReady;

        /// <summary>
        /// Called when a color frame is ready, this event will fire on the main application thread.
        /// </summary>
        public event Action<byte[]> SensorColorFrameReady;

        /// <summary>
        /// Called when a face frame is ready. This event will be fired on the main application thread.
        /// </summary>
        public event Action<FaceAlignment> FaceFrameReady;

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

            if(faceFrameReader != null)
            {
                faceFrameReader.Dispose();
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

                    faceFrameSource = new HighDefinitionFaceFrameSource(localSensor);
                    faceFrameReader = faceFrameSource.OpenReader();
                    faceFrameReader.FrameArrived += faceFrameReader_FrameArrived;
                    currentFaceAlignment = new FaceAlignment();

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
            lock (bodyFrameLock)
            {
                if (processedLastBodyFrame)
                {
                    processedLastBodyFrame = false;
                    Body currentBody = null;
                    float closestLen = float.MaxValue;

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

                            foreach (var body in bodies)
                            {
                                if (body.IsTracked)
                                {
                                    float distance = body.Joints[JointType.SpineBase].Position.toSceneCoords().distance2(ref Vector3.Zero);
                                    if (distance < closestLen)
                                    {
                                        currentBody = body;
                                        closestLen = distance;
                                    }
                                }
                            }
                        }
                    }

                    if (currentBody != null && SkeletonFrameReady != null)
                    {
                        if (faceFrameSource != null)
                        {
                            faceFrameSource.TrackingId = currentBody.TrackingId;
                        }

                        ThreadManager.invoke(() =>
                        {
                            lock (bodyFrameLock)
                            {
                                SkeletonFrameReady.Invoke(currentBody);
                                processedLastBodyFrame = true;
                            }
                        });
                    }
                    else
                    {
                        if (faceFrameSource != null)
                        {
                            faceFrameSource.TrackingId = 0;
                        }
                        processedLastBodyFrame = true;
                    }
                }
            }
        }

        void faceFrameReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            lock (faceFrameLock)
            {
                if (processedLastFaceFrame)
                {
                    processedLastFaceFrame = false;
                    bool dataReceived = false;

                    using (HighDefinitionFaceFrame faceFrame = e.FrameReference.AcquireFrame())
                    {
                        if (faceFrame != null && faceFrame.IsFaceTracked)
                        {
                            faceFrame.GetAndRefreshFaceAlignmentResult(currentFaceAlignment);
                            dataReceived = true;
                        }
                    }

                    if (dataReceived && FaceFrameReady != null)
                    {
                        ThreadManager.invoke(() =>
                        {
                            lock (faceFrameLock)
                            {
                                FaceFrameReady.Invoke(currentFaceAlignment);
                                processedLastFaceFrame = true;
                            }
                        });
                    }
                    else
                    {
                        processedLastFaceFrame = true;
                    }
                }
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
