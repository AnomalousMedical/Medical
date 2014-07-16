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
        private KinectSensor sensor;
        private KinectStatus currentStatus;
        private bool useColorFeed = false;

        /// <summary>
        /// Called when a skeleton frame is ready, this event will fire on the main application thread.
        /// </summary>
        public event Action<Skeleton[]> SkeletonFrameReady;

        /// <summary>
        /// Called when a color frame is ready, this event will fire on the main application thread.
        /// </summary>
        public event Action<byte[]> SensorColorFrameReady;

        public event Action<KinectSensorManager> StatusChanged;

        public KinectSensorManager()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            ThreadPool.QueueUserWorkItem((state) =>
                {
                    findSensor();
                });
        }

        public void Dispose()
        {
            disconnectSensor();
        }

        public KinectStatus CurrentStatus
        {
            get
            {
                return currentStatus;
            }
            private set
            {
                if (currentStatus != value)
                {
                    currentStatus = value;
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
                    if(sensor != null)
                    {
                        ThreadPool.QueueUserWorkItem((state) =>
                            {
                                if (sensor != null)
                                {
                                    if (useColorFeed)
                                    {
                                        sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                                    }
                                    else
                                    {
                                        sensor.ColorStream.Disable();
                                    }
                                }
                            });
                    }
                }
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //This happens on its own thread

            if(SkeletonFrameReady != null)
            {
                Skeleton[] skeletons = new Skeleton[0];

                //Get the skeletons
                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    if (skeletonFrame != null)
                    {
                        skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        skeletonFrame.CopySkeletonDataTo(skeletons);
                    }
                }

                ThreadManager.invoke(() =>
                {
                    if (SkeletonFrameReady != null)
                    {
                        SkeletonFrameReady.Invoke(skeletons);
                    }
                });
            }
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (CurrentStatus != e.Status)
            {
                CurrentStatus = e.Status;
                switch (currentStatus)
                {
                    case KinectStatus.Disconnected:
                        if (e.Sensor == sensor)
                        {
                            disconnectSensor();
                        }
                        break;
                    case KinectStatus.Connected:
                        findSensor();
                        break;
                }
            }
        }

        private void findSensor()
        {
            if (sensor == null)
            {
                KinectSensor localSensor = null;
                // Look through all sensors and start the first connected one.
                foreach (var potentialSensor in KinectSensor.KinectSensors)
                {
                    if (potentialSensor.Status == KinectStatus.Connected)
                    {
                        localSensor = potentialSensor;
                        break;
                    }
                }

                //If a sensor was found start it and enable its skeleton listening.
                if (localSensor != null)
                {
                    // Turn on the skeleton stream to receive skeleton frames
                    localSensor.SkeletonStream.Enable();

                    // Add an event handler to be called whenever there is new skeleton frame data
                    localSensor.SkeletonFrameReady += sensor_SkeletonFrameReady;

                    if (useColorFeed)
                    {
                        localSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    }
                    localSensor.ColorFrameReady += sensor_ColorFrameReady;

                    // Start the sensor!
                    try
                    {
                        localSensor.Start();
                        CurrentStatus = KinectStatus.Connected;
                    }
                    catch (IOException)
                    {
                        localSensor = null;
                    }

                    sensor = localSensor; //Make the class aware of the sensor
                }
            }
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            KinectSensor sensor = sender as KinectSensor;
            //This happens on its own thread
            if (SensorColorFrameReady != null && sensor != null)
            {
                byte[] colorPixels = new byte[sensor.ColorStream.FramePixelDataLength]; //Yea this is a lot of allocation, just being lazy with the stack for now, should make a pool of these, or invoke and wait

                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        // Copy the pixel data from the image to a temporary array
                        colorFrame.CopyPixelDataTo(colorPixels);
                    }
                }

                ThreadManager.invoke(() =>
                {
                    if (SensorColorFrameReady != null)
                    {
                        SensorColorFrameReady.Invoke(colorPixels);
                    }
                });
            }
        }

        private void disconnectSensor()
        {
            if (sensor != null)
            {
                KinectSensor localSensor = sensor;
                sensor = null;
                CurrentStatus = KinectStatus.Disconnected;
                localSensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                localSensor.ColorFrameReady -= sensor_ColorFrameReady;
                localSensor.Stop();
                localSensor.Dispose();
            }
        }
    }
}
