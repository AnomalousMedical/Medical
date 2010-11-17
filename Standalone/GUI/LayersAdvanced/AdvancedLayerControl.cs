using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.ObjectManagement;
using Engine;

namespace Medical.GUI
{
    class AdvancedLayerControl : Dialog
    {
        private List<LayerSection> sections = new List<LayerSection>();
        private ScrollView widgetScroll;
        private FlowLayoutContainer flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 2.0f, new Vector2(0.0f, 2.0f));
        private int lastWidth = 0;

        public AdvancedLayerControl()
            :base("Medical.GUI.LayersAdvanced.AdvancedLayerControl.layout")
        {
            widgetScroll = window.findWidget("LayerScroll") as ScrollView;
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
        }

        public void sceneUnloading()
        {
            foreach (LayerSection section in sections)
            {
                section.Dispose();
            }
            sections.Clear();
            flowLayout.clearChildren();
        }

        public void sceneLoaded(SimScene scene)
        {
            /// We must make the dialog visible so that all the new widgets
            /// added to it are properly hidden. This will check to see if the
            /// dialog is visible, if it isn't it will be made visible for the
            /// duration of this function. It will then be rehidden if it was
            /// hidden when the function was called.
            bool wasHidden = false;
            if (!Visible)
            {
                this.Visible = true;
                wasHidden = true;
            }

            lastWidth = getNewWidth();

            flowLayout.SuppressLayout = true;
            LayerSection section = null;
            foreach (TransparencyGroup group in TransparencyController.getGroupIter())
            {
                section = new LayerSection(group, widgetScroll, lastWidth);
                section.SizeChanged += section_SizeChanged;
                sections.Add(section);
                flowLayout.addChild(section.Container);
            }
            flowLayout.SuppressLayout = false;
            flowLayout.layout();

            if (section != null)
            {
                widgetScroll.CanvasSize = new Size2(lastWidth, section.Bottom);
            }

            if (wasHidden)
            {
                this.Visible = false;
            }
        }

        void section_SizeChanged(object sender, EventArgs e)
        {
            if (sections.Count > 0)
            {
                Size2 canvasSize = widgetScroll.CanvasSize;
                canvasSize.Height = sections[sections.Count - 1].Bottom;
                widgetScroll.CanvasSize = canvasSize;
            }
            adjustToWidth(getNewWidth());
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            adjustToWidth(getNewWidth());
        }

        int getNewWidth()
        {
            int newWidth = widgetScroll.ClientCoord.width;
            if (newWidth < 214)
            {
                newWidth = 214;
            }
            return newWidth;
        }

        void adjustToWidth(int newWidth)
        {
            if (newWidth != lastWidth)
            {
                flowLayout.SuppressLayout = true;
                Size2 canvasSize = widgetScroll.CanvasSize;
                canvasSize.Width = newWidth;
                widgetScroll.CanvasSize = canvasSize;

                foreach (LayerSection section in sections)
                {
                    section.changeWidth(newWidth);
                }
                lastWidth = newWidth;
                flowLayout.SuppressLayout = false;
            }
        }
    }
}
