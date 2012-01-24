using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Logging;

namespace Medical.LayoutDataControls
{
    class LayoutDataControlFactory : DataControlFactory
    {
        private LayoutDataControl layoutControl;
        private DataDrivenTimelineGUI gui;

        public LayoutDataControlFactory(LayoutDataControl layoutControl, DataDrivenTimelineGUI gui)
        {
            this.layoutControl = layoutControl;
            this.gui = gui;
        }

        public void pushColumnLayout()
        {
            
        }

        public void popColumnLayout()
        {
            
        }

        public void addField(BooleanDataField field)
        {
            Button button = layoutControl.findWidget(field.Name) as Button;
            if (button != null)
            {
                LayoutCheckBoxDataControl control = new LayoutCheckBoxDataControl(button, field);
                layoutControl.addControl(control);
            }
            else
            {
                Log.Error("Could not find a Button control named '{0}' on the layout to make a Check Box. Skipping this button.", field.Name);
            }
        }

        public void addField(MenuItemField field)
        {
            Button button = layoutControl.findWidget(field.Name) as Button;
            if (button != null)
            {
                LayoutMenuButtonDataControl control = new LayoutMenuButtonDataControl(button, gui, field);
                layoutControl.addControl(control);
            }
            else
            {
                Log.Error("Could not find a Button control named '{0}' on the layout to make a Menu Item. Skipping this button.", field.Name);
            }
        }

        public void addField(MultipleChoiceField field)
        {
            throw new NotImplementedException("Multiple choice not supported by layout data controls.");
        }

        public void addField(NotesDataField field)
        {
            Edit edit = layoutControl.findWidget(field.Name) as Edit;
            if (edit != null)
            {
                LayoutWordWrapDataControl control = new LayoutWordWrapDataControl(edit, field);
                layoutControl.addControl(control);
            }
            else
            {
                Log.Error("Could not find an Edit control named '{0}' on the layout to make a Notes Field. Skipping this button.", field.Name);
            }
        }

        public void addField(NumericDataField field)
        {
            Edit edit = layoutControl.findWidget(field.Name) as Edit;
            if (edit != null)
            {
                NumericEditDataControl control = new NumericEditDataControl(edit, field);
                layoutControl.addControl(control);
            }
            else
            {
                Log.Error("Could not find an Edit control named '{0}' on the layout to make a Numeric Edit Field. Skipping this button.", field.Name);
            }
        }

        public void addField(PlayExampleDataField field)
        {
            Button button = layoutControl.findWidget(field.Name) as Button;
            if (button != null)
            {
                LayoutPlayExampleButton control = new LayoutPlayExampleButton(button, gui, field);
                layoutControl.addControl(control);
            }
            else
            {
                Log.Error("Could not find a Button control named '{0}' on the layout to make a Play Example Item. Skipping this button.", field.Name);
            }
        }
    }
}
