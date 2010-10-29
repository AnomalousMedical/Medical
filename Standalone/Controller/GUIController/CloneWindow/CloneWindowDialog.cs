using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using System.Text.RegularExpressions;
using Engine.Renderer;

namespace Medical.GUI
{
    class CloneWindowDialog : Dialog
    {
        public event EventHandler CreateCloneWindow;

        private ComboBox monitorCombo;
        private ComboBox resolutionCombo;

        private static readonly char[] seps = { 'x' };
        private const String resolutionRegex = "[1-9][0-9]* x [1-9][0-9]*";

        public CloneWindowDialog()
            :base("Medical.Controller.GUIController.CloneWindow.CloneWindowDialog.layout")
        {
            this.Modal = true;
            this.SmoothShow = true;

            RenderSystem rs = Root.getSingleton().getRenderSystem();
            monitorCombo = window.findWidget("MonitorCombo") as ComboBox;
            resolutionCombo = window.findWidget("ResolutionCombo") as ComboBox;

            uint numMonitors = (uint)wx.Display.Count;//Root.getSingleton().getDisplayMonitorCount();
            for(uint i = 0; i < numMonitors; ++i)
            {
                monitorCombo.addItem("Monitor " + i);
            }

            if (rs.hasConfigOption("Video Mode"))
            {
                resolutionCombo.Enabled = true;
                ConfigOption configOption = rs.getConfigOption("Video Mode");
                foreach (String value in configOption.PossibleValues)
                {
                    Match match = Regex.Match(value, resolutionRegex);
                    String resString = value.Substring(match.Index, match.Length);
                    if (resolutionCombo.findItemIndexWith(resString) == uint.MaxValue)
                    {
                        resolutionCombo.addItem(resString);
                    }
                }
            }
            else
            {
                resolutionCombo.Enabled = false;
            }

            Button acceptButton = window.findWidget("AcceptButton") as Button;
            acceptButton.MouseButtonClick += new MyGUIEvent(acceptButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        public WindowInfo createWindowInfo()
        {
            String[] res = resolutionCombo.getItemNameAt(resolutionCombo.SelectedIndex).Split(seps, StringSplitOptions.RemoveEmptyEntries);
            WindowInfo info = new WindowInfo("Clone", int.Parse(res[0]), int.Parse(res[1]));
            info.MonitorIndex = (int)monitorCombo.SelectedIndex;
            return info;
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            if (monitorCombo.ItemCount > 1)
            {
                monitorCombo.SelectedIndex = 1;
            }
            else
            {
                monitorCombo.SelectedIndex = 0;
            }
            if (resolutionCombo.Enabled)
            {
                resolutionCombo.SelectedIndex = 0;
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void acceptButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
            if (CreateCloneWindow != null)
            {
                CreateCloneWindow.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
