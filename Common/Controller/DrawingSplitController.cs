using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.GUI.View;
using System.Windows.Forms;

namespace Medical.Controller
{
    public class DrawingSplitController
    {
        DrawingWindow frontView = new DrawingWindow();
        DrawingWindow backView = new DrawingWindow();
        DrawingWindow leftView = new DrawingWindow();
        DrawingWindow rightView = new DrawingWindow();
        DrawingSplitHost splitHost;
        DrawingSplitView currentView;
        DrawingWindow activeWindow = null;
        bool maximized = false;

        public DrawingSplitController()
        {

        }

        public void initialize(DrawingSplitHost splitHost)
        {
            this.splitHost = splitHost;

            //CameraSection cameras = AnomalyConfig.CameraSection;
            frontView.initialize("UpperLeft", this);
            frontView.Dock = DockStyle.Fill;

            backView.initialize("UpperRight", this);
            backView.Dock = DockStyle.Fill;

            leftView.initialize("BottomLeft", this);
            leftView.Dock = DockStyle.Fill;

            rightView.initialize("BottomRight", this);
            rightView.Dock = DockStyle.Fill;
        }

        public void createFourWaySplit()
        {
            changeSplit(new FourWaySplit());
        }

        public void createThreeWayUpperSplit()
        {
            changeSplit(new ThreeWayUpperSplit());
        }

        public void createTwoWaySplit()
        {
            changeSplit(new TwoWaySplit());
        }

        public void createOneWaySplit()
        {
            changeSplit(new OneWaySplit());
        }

        private void changeSplit(DrawingSplitView splitView)
        {
            splitView.Dock = DockStyle.Fill;
            currentView = splitView;
            splitHost.Controls.Clear();
            splitHost.Controls.Add(splitView);
            activeWindow = null;
            configureWindows();
        }

        public void setActiveWindow(DrawingWindow window)
        {
            if (activeWindow != null)
            {
                activeWindow.BorderStyle = BorderStyle.None;
            }
            activeWindow = window;
            window.BorderStyle = BorderStyle.Fixed3D;
        }

        public void toggleMaximize()
        {
            if (maximized)
            {
                splitHost.Controls.Clear();
                splitHost.Controls.Add(currentView);
                configureWindows();
            }
            else
            {
                maximized = true;
                frontView.Enabled = false;
                backView.Enabled = false;
                leftView.Enabled = false;
                rightView.Enabled = false;
                activeWindow.Enabled = true;
                splitHost.Controls.Clear();
                splitHost.Controls.Add(activeWindow);
            }
        }

        /// <summary>
        /// Helper function to configure the windows for a given split view.
        /// </summary>
        private void configureWindows()
        {
            maximized = false;
            if (currentView.FrontView != null)
            {
                currentView.FrontView.Controls.Add(frontView);
                frontView.Enabled = true;
                if (activeWindow == null)
                {
                    setActiveWindow(frontView);
                }
            }
            else
            {
                frontView.Enabled = false;
            }
            if (currentView.BackView != null)
            {
                currentView.BackView.Controls.Add(backView);
                backView.Enabled = true;
                if (activeWindow == null)
                {
                    setActiveWindow(backView);
                }
            }
            else
            {
                backView.Enabled = false;
            }
            if (currentView.LeftView != null)
            {
                currentView.LeftView.Controls.Add(leftView);
                leftView.Enabled = true;
                if (activeWindow == null)
                {
                    setActiveWindow(leftView);
                }
            }
            else
            {
                leftView.Enabled = false;
            }
            if (currentView.RightView != null)
            {
                currentView.RightView.Controls.Add(rightView);
                rightView.Enabled = true;
                if (activeWindow == null)
                {
                    setActiveWindow(rightView);
                }
            }
            else
            {
                rightView.Enabled = false;
            }
        }
    }
}
