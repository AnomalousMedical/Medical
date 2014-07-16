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
            CurrentStatus = e.Status;
            Log.Info("Kinect sensor {0}", currentStatus);
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

        private void findSensor()
        {
            if (sensor == null)
            {
                // Look through all sensors and start the first connected one.
                foreach (var potentialSensor in KinectSensor.KinectSensors)
                {
                    if (potentialSensor.Status == KinectStatus.Connected)
                    {
                        sensor = potentialSensor;
                        break;
                    }
                }

                //If a sensor was found start it and enable its skeleton listening.
                if (sensor != null)
                {
                    // Turn on the skeleton stream to receive skeleton frames
                    sensor.SkeletonStream.Enable();

                    // Add an event handler to be called whenever there is new skeleton frame data
                    sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;

                    sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    sensor.ColorFrameReady += sensor_ColorFrameReady;

                    // Start the sensor!
                    try
                    {
                        sensor.Start();
                        CurrentStatus = KinectStatus.Connected;
                    }
                    catch (IOException)
                    {
                        sensor = null;
                    }
                }

                if (sensor == null)
                {
                    Log.ImportantInfo("No Kinect Sensor found");
                }
            }
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            //This happens on its own thread
            if (SensorColorFrameReady != null)
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
                sensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                sensor.ColorFrameReady -= sensor_ColorFrameReady;
                sensor.Stop();
                sensor.Dispose();
                sensor = null;
                CurrentStatus = KinectStatus.Disconnected;
            }
        }
    }
}
