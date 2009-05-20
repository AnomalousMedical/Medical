using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class CameraSection
    {
        #region Static

        private const String CAMERA_HEADER = "Cameras";
        private const String FRONT_CAMERA_POSITION_ENTRY = "FrontPosition";
        private static readonly Vector3 FRONT_CAMERA_POSITION_DEFAULT = new Vector3(0.0f, 0.0f, 150.0f);
        private const String FRONT_CAMERA_LOOKAT_ENTRY = "FrontLookAt";
        private static readonly Vector3 FRONT_CAMERA_LOOKAT_DEFAULT = Vector3.Zero;

        private const String LEFT_CAMERA_POSITION_ENTRY = "LeftPosition";
        private static readonly Vector3 LEFT_CAMERA_POSITION_DEFAULT = new Vector3(150.0f, 0.0f, 0.0f);
        private const String LEFT_CAMERA_LOOKAT_ENTRY = "LeftLookAt";
        private static readonly Vector3 LEFT_CAMERA_LOOKAT_DEFAULT = Vector3.Zero;

        private const String RIGHT_CAMERA_POSITION_ENTRY = "RightPosition";
        private static readonly Vector3 RIGHT_CAMERA_POSITION_DEFAULT = new Vector3(-150.0f, 0.0f, 0.0f);
        private const String RIGHT_CAMERA_LOOKAT_ENTRY = "RightLookAt";
        private static readonly Vector3 RIGHT_CAMERA_LOOKAT_DEFAULT = Vector3.Zero;

        private const String BACK_CAMERA_POSITION_ENTRY = "BackPosition";
        private static readonly Vector3 BACK_CAMERA_POSITION_DEFAULT = new Vector3(0.0f, 0.0f, -150.0f);
        private const String BACK_CAMERA_LOOKAT_ENTRY = "BackLookAt";
        private static readonly Vector3 BACK_CAMERA_LOOKAT_DEFAULT = Vector3.Zero;

        #endregion Static

        private ConfigSection configSection;

        public CameraSection(ConfigFile configFile)
        {
            configSection = configFile.createOrRetrieveConfigSection(CAMERA_HEADER);
        }

        public Vector3 FrontCameraPosition
        {
            get
            {
                return configSection.getValue(FRONT_CAMERA_POSITION_ENTRY, FRONT_CAMERA_POSITION_DEFAULT);
            }
            set
            {
                configSection.setValue(FRONT_CAMERA_POSITION_ENTRY, value);
            }
        }

        public Vector3 FrontCameraLookAt
        {
            get
            {
                return configSection.getValue(FRONT_CAMERA_LOOKAT_ENTRY, FRONT_CAMERA_LOOKAT_DEFAULT);
            }
            set
            {
                configSection.setValue(FRONT_CAMERA_LOOKAT_ENTRY, value);
            }
        }

        public Vector3 BackCameraPosition
        {
            get
            {
                return configSection.getValue(BACK_CAMERA_POSITION_ENTRY, BACK_CAMERA_POSITION_DEFAULT);
            }
            set
            {
                configSection.setValue(BACK_CAMERA_POSITION_ENTRY, value);
            }
        }

        public Vector3 BackCameraLookAt
        {
            get
            {
                return configSection.getValue(BACK_CAMERA_LOOKAT_ENTRY, BACK_CAMERA_LOOKAT_DEFAULT);
            }
            set
            {
                configSection.setValue(BACK_CAMERA_LOOKAT_ENTRY, value);
            }
        }

        public Vector3 LeftCameraPosition
        {
            get
            {
                return configSection.getValue(LEFT_CAMERA_POSITION_ENTRY, LEFT_CAMERA_POSITION_DEFAULT);
            }
            set
            {
                configSection.setValue(LEFT_CAMERA_POSITION_ENTRY, value);
            }
        }

        public Vector3 LeftCameraLookAt
        {
            get
            {
                return configSection.getValue(LEFT_CAMERA_LOOKAT_ENTRY, LEFT_CAMERA_LOOKAT_DEFAULT);
            }
            set
            {
                configSection.setValue(LEFT_CAMERA_LOOKAT_ENTRY, value);
            }
        }

        public Vector3 RightCameraPosition
        {
            get
            {
                return configSection.getValue(RIGHT_CAMERA_POSITION_ENTRY, RIGHT_CAMERA_POSITION_DEFAULT);
            }
            set
            {
                configSection.setValue(RIGHT_CAMERA_POSITION_ENTRY, value);
            }
        }

        public Vector3 RightCameraLookAt
        {
            get
            {
                return configSection.getValue(RIGHT_CAMERA_LOOKAT_ENTRY, RIGHT_CAMERA_LOOKAT_DEFAULT);
            }
            set
            {
                configSection.setValue(RIGHT_CAMERA_LOOKAT_ENTRY, value);
            }
        }
    }
}
