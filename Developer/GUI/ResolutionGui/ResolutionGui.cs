using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;
using Engine;
using Engine.Platform;
using Medical;
using Medical.GUI;
using MyGUIPlugin;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Developer.GUI
{
    class ResolutionGui : MDIDialog
    {
        private SceneViewController sceneViewController;
        private NumericEdit xRes;
        private NumericEdit yRes;
        private MainWindow mainWindow;

        public ResolutionGui(MainWindow mainWindow)
            : base("Developer.GUI.ResolutionGui.ResolutionGui.layout")
        {
            this.mainWindow = mainWindow;

            xRes = new NumericEdit(window.findWidget("XRes") as EditBox);
            xRes.MinValue = 1;
            xRes.MaxValue = 99999;
            xRes.Increment = 100;
            xRes.IntValue = mainWindow.WindowWidth;

            yRes = new NumericEdit(window.findWidget("YRes") as EditBox);
            yRes.MinValue = 1;
            yRes.MaxValue = 99999;
            yRes.Increment = 100;
            yRes.IntValue = mainWindow.WindowHeight;

            Button setSize = window.findWidget("SetSize") as Button;
            setSize.MouseButtonClick += SetSize_MouseButtonClick;

            Button getSize = window.findWidget("GetSize") as Button;
            getSize.MouseButtonClick += GetSize_MouseButtonClick;
        }

        private void GetSize_MouseButtonClick(Widget source, EventArgs e)
        {
            xRes.IntValue = mainWindow.WindowWidth;
            yRes.IntValue = mainWindow.WindowHeight;
        }

        private void SetSize_MouseButtonClick(Widget source, EventArgs e)
        {
            mainWindow.Maximized = false;
            mainWindow.setSize(xRes.IntValue, yRes.IntValue);
            IntSize2 sizeOffset = new IntSize2();
            sizeOffset.Width = xRes.IntValue - mainWindow.WindowWidth;
            sizeOffset.Height = yRes.IntValue - mainWindow.WindowHeight;
            if(sizeOffset.Width > 0 || sizeOffset.Height > 0)
            {
                mainWindow.setSize(xRes.IntValue + sizeOffset.Width, yRes.IntValue + sizeOffset.Height);
            }
        }
    }
}
