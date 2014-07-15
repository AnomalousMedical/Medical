using Logging;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectSensorManager : IDisposable
    {
        private KinectSensor sensor;
        private KinectStatus currentStatus;

        public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;

        public KinectSensorManager()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            findSensor();
        }

        public void Dispose()
        {
            disconnectSensor();
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if(SkeletonFrameReady != null)
            {
                SkeletonFrameReady.Invoke(sender, e);
            }
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            currentStatus = e.Status;
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

                    // Start the sensor!
                    try
                    {
                        sensor.Start();
                        currentStatus = KinectStatus.Connected;
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

        private void disconnectSensor()
        {
            if (sensor != null)
            {
                sensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                sensor.Stop();
                sensor = null;
                currentStatus = KinectStatus.Disconnected;
            }
        }
    }
}
