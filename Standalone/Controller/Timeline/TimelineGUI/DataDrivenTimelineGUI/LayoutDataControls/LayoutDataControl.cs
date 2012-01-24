using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using OgreWrapper;
using System.IO;

namespace Medical
{
    class LayoutDataControl : DataControl
    {
        private static readonly String LAYOUT_DATA_CONTROL_MEMORY = "LAYOUT_DATA_CONTROL_MEMORY";
        private static readonly String LAYOUT_MEMORY_FILE_NAME = "LAYOUT_MEMORY_FILE_NAME.layout";

        private List<DataControl> subControls = new List<DataControl>();

        private Layout myGUILayout;
        private Widget parentWidget;

        public LayoutDataControl(Stream layoutStream, Widget attachTo)
        {
            OgreResourceGroupManager.getInstance().addResourceLocation(LAYOUT_DATA_CONTROL_MEMORY, "Memory", "MyGUI", true);
            MemoryArchive memoryArchive = MemoryArchiveFactory.Instance.getArchive(LAYOUT_DATA_CONTROL_MEMORY);

            MemoryStream memoryStream = new MemoryStream();
            layoutStream.CopyTo(memoryStream);
            memoryArchive.addMemoryStreamResource(LAYOUT_MEMORY_FILE_NAME, memoryStream);

            myGUILayout = LayoutManager.Instance.loadLayout(LAYOUT_DATA_CONTROL_MEMORY + LAYOUT_MEMORY_FILE_NAME);
            parentWidget = myGUILayout.getWidget(0);

            parentWidget.attachToWidget(attachTo);

            memoryArchive.destroyMemoryStreamResource(LAYOUT_MEMORY_FILE_NAME);
            OgreResourceGroupManager.getInstance().removeResourceLocation(LAYOUT_DATA_CONTROL_MEMORY, "MyGUI");
        }

        public override void Dispose()
        {
            LayoutManager.Instance.unloadLayout(myGUILayout);
        }

        public Widget findWidget(String name)
        {
            return parentWidget.findWidget(name);
        }

        public void addControl(DataControl control)
        {
            subControls.Add(control);
        }

        public void removeControl(DataControl control)
        {
            subControls.Remove(control);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            foreach (DataControl control in subControls)
            {
                control.captureData(examSection);
            }
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            foreach (DataControl control in subControls)
            {
                control.displayData(examSection);
            }
        }

        public override void bringToFront()
        {
            LayerManager.Instance.upLayerItem(parentWidget);
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public override void layout()
        {
            parentWidget.setPosition((int)Location.x, (int)Location.y);
            Height = parentWidget.Height;
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return new Size2(parentWidget.Width, parentWidget.Height);
            }
        }

        public override bool Visible
        {
            get
            {
                return parentWidget.Visible;
            }
            set
            {
                parentWidget.Visible = value;
            }
        }
    }
}
