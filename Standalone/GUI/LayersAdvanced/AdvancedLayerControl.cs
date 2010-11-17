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
        private FlowLayoutContainer flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 2.0f, new Vector2(0.0f, 0.0f));

        public AdvancedLayerControl()
            :base("Medical.GUI.LayersAdvanced.AdvancedLayerControl.layout")
        {
            widgetScroll = window.findWidget("LayerScroll") as ScrollView;
        }

        public void sceneUnloading()
        {
            foreach (LayerSection section in sections)
            {
                section.Dispose();
            }
            sections.Clear();
        }

        public void sceneLoaded(SimScene scene)
        {
            flowLayout.SuppressLayout = true;
            LayerSection section = null;
            foreach (TransparencyGroup group in TransparencyController.getGroupIter())
            {
                section = new LayerSection(group, widgetScroll);
                section.SizeChanged += section_SizeChanged;
                sections.Add(section);
                flowLayout.addChild(section.Container);
            }
            flowLayout.SuppressLayout = false;
            flowLayout.layout();
            if (section != null)
            {
                Size2 canvasSize = widgetScroll.CanvasSize;
                canvasSize.Height = section.Bottom;
                widgetScroll.CanvasSize = canvasSize;
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
        }
    }
}
