using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    class EditInterfaceProperties : TimelineDataPanel
    {
        private EditInterface editInterface;
        private ResizingTable table;
        private PropertiesTable propTable;

        public EditInterfaceProperties(Widget parent)
            :base(parent, "Medical.GUI.PropTimeline.SubActionProperties.EditInterfaceProperties.layout")
        {
            table = new ResizingTable((ScrollView)parentWidget.findWidget("TableScroll"));
            propTable = new PropertiesTable(table);
        }

        public override void Dispose()
        {
            propTable.Dispose();
            table.Dispose();
            base.Dispose();
        }

        public override void setCurrentData(TimelineData data)
        {
            PropTimelineData propData = (PropTimelineData)data;
            editInterface = ((EditableShowPropSubAction)propData.Action).EditInterface;
            propTable.EditInterface = editInterface;
        }
    }
}
