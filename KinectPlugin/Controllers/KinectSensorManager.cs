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

        public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;

        public KinectSensorManager()
        {
            //Setup kinect
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (this.sensor != null)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new skeleton frame data
                this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (sensor == null)
            {
                Logging.Log.ImportantInfo("No Kinect Sensor found");
            }
        }

        public void Dispose()
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if(SkeletonFrameReady != null)
            {
                SkeletonFrameReady.Invoke(sender, e);
            }
        }
    }
}
