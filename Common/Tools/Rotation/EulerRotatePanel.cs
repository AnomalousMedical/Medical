using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Engine;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical
{
    public partial class EulerRotatePanel : DockContent
    {
        RotateController rotationController;
        private bool allowMotionUpdates = true;
        private bool allowObjectUpdate = true;
        private static float RADIAN_TO_DEG = 180.0f / (float)Math.PI;
        private static float DEG_TO_RADIAN = (float)Math.PI / 180.0f;

        public EulerRotatePanel()
        {
            InitializeComponent();
        }

        public void initialize(RotateController rotationController)
        {
            this.rotationController = rotationController;
            rotationController.OnRotationChanged += new RotationChanged(rotationChanged);
            yaw.ValueChanged += new EventHandler(rotateObject);
            pitch.ValueChanged += new EventHandler(rotateObject);
            roll.ValueChanged += new EventHandler(rotateObject);
            degrees.CheckedChanged += new EventHandler(degrees_CheckedChanged);
            yaw.Click += new EventHandler(numericUpDownClick);
            pitch.Click += new EventHandler(numericUpDownClick);
            roll.Click += new EventHandler(numericUpDownClick);
            yaw.KeyPress += new KeyPressEventHandler(yaw_KeyPress);
            pitch.KeyPress += new KeyPressEventHandler(pitch_KeyPress);
            roll.KeyPress += new KeyPressEventHandler(roll_KeyPress);
        }

        void roll_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\t')
            {
                yaw.Focus();
                yaw.Select(0, 10000);
            }
        }

        void pitch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\t')
            {
                roll.Focus();
                roll.Select(0, 10000);
            }
        }

        void yaw_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\t')
            {
                pitch.Focus();
                pitch.Select(0, 10000);
            }
        }

        void numericUpDownClick(object sender, EventArgs e)
        {
            NumericUpDown upDown = (NumericUpDown)sender;
            upDown.Select(0, 100);
        }

        void degrees_CheckedChanged(object sender, EventArgs e)
        {
            allowObjectUpdate = false;
            if (degrees.Checked)
            {
                yaw.Value = yaw.Value * (decimal)RADIAN_TO_DEG;
                pitch.Value = pitch.Value * (decimal)RADIAN_TO_DEG;
                roll.Value = roll.Value * (decimal)RADIAN_TO_DEG;
            }
            else
            {
                yaw.Value = yaw.Value * (decimal)DEG_TO_RADIAN;
                pitch.Value = pitch.Value * (decimal)DEG_TO_RADIAN;
                roll.Value = roll.Value * (decimal)DEG_TO_RADIAN;
            }
            allowObjectUpdate = true;
        }

        void rotateObject(object sender, EventArgs e)
        {
            if (allowObjectUpdate)
            {
                allowMotionUpdates = false;
                Quaternion newRot;
                if (degrees.Checked)
                {
                    newRot = new Quaternion((float)yaw.Value * DEG_TO_RADIAN, (float)pitch.Value * DEG_TO_RADIAN, (float)roll.Value * DEG_TO_RADIAN);
                }
                else
                {
                    newRot = new Quaternion((float)yaw.Value, (float)pitch.Value, (float)roll.Value);
                }
                rotationController.setRotation(ref newRot, this);
                allowMotionUpdates = true;
            }
        }

        void rotationChanged(Quaternion rotation, object sender)
        {
            if (allowMotionUpdates && sender != this)
            {
                allowObjectUpdate = false;
                Vector3 euler = rotation.getEuler();
                if (euler.isNumber())
                {
                    if (degrees.Checked)
                    {
                        yaw.Value = (decimal)(euler.x * RADIAN_TO_DEG);
                        pitch.Value = (decimal)(euler.y * RADIAN_TO_DEG);
                        roll.Value = (decimal)(euler.z * RADIAN_TO_DEG);
                    }
                    else
                    {
                        yaw.Value = (decimal)(euler.x);
                        pitch.Value = (decimal)(euler.y);
                        roll.Value = (decimal)(euler.z);
                    }
                }
                allowObjectUpdate = true;
            }
        }

        public Quaternion getCurrentRotation()
        {
            if (degrees.Checked)
            {
                return new Quaternion((float)yaw.Value * DEG_TO_RADIAN, (float)pitch.Value * DEG_TO_RADIAN, (float)roll.Value * DEG_TO_RADIAN);
            }
            else
            {
                return new Quaternion((float)yaw.Value, (float)pitch.Value, (float)roll.Value);
            }
        }
    }
}
