using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    class TimelineObjectProperties : MDIDialog
    {
        private ResizingTable table;
        private PropertiesTable propertiesTable;
        private AddRemoveButtons addRemoveButtons;
        private ScrollView tableScrollView;

        public TimelineObjectProperties(EditUICallback uiCallback)
            : base("Medical.GUI.Timeline.TimelineObjectProperties.layout")
        {
            Widget addRemoveButtonParent = window.findWidget("AddRemovePanel");
            addRemoveButtons = new AddRemoveButtons((Button)addRemoveButtonParent.findWidget("AddButton"), (Button)addRemoveButtonParent.findWidget("RemoveButton"), addRemoveButtonParent);
            addRemoveButtons.VisibilityChanged += new Engine.EventDelegate<AddRemoveButtons, bool>(addRemoveButtons_VisibilityChanged);

            tableScrollView = (ScrollView)window.findWidget("ScrollView");
            table = new ResizingTable(tableScrollView);
            propertiesTable = new PropertiesTable(table, uiCallback, addRemoveButtons);

            this.Resized += new EventHandler(TimelineObjectProperties_Resized);
        }

        public override void Dispose()
        {
            propertiesTable.Dispose();
            table.Dispose();
            base.Dispose();
        }

        public PropertiesTable PropertiesTable
        {
            get
            {
                return propertiesTable;
            }
        }

        public bool Enabled
        {
            get
            {
                return table.Enabled;
            }
            set
            {
                table.Enabled = value;
            }
        }

        void TimelineObjectProperties_Resized(object sender, EventArgs e)
        {
            table.layout();
        }

        void addRemoveButtons_VisibilityChanged(AddRemoveButtons source, bool visible)
        {
            if (visible)
            {
                tableScrollView.setSize(tableScrollView.Width, tableScrollView.Height - addRemoveButtons.Height);
            }
            else
            {
                tableScrollView.setSize(tableScrollView.Width, tableScrollView.Height + addRemoveButtons.Height);
            }
        }
    }
}
