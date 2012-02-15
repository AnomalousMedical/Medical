using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical
{
    /// <summary>
    /// This factory will build MyGUI controls on a form automatically based on
    /// the data given.
    /// </summary>
    public class MyGUIDataControlFactory : DataControlFactory
    {
        private Widget parentWidget;
        private DataDrivenTimelineGUI gui;
        private ColumnLayoutDataControl currentLayoutDataControl;
        private Stack<ColumnLayoutDataControl> layoutDataControlStack = new Stack<ColumnLayoutDataControl>();

        public MyGUIDataControlFactory(Widget parentWidget, DataDrivenTimelineGUI gui)
        {
            this.parentWidget = parentWidget;
            this.gui = gui;
        }

        public void pushColumnLayout()
        {
            if (currentLayoutDataControl != null)
            {
                layoutDataControlStack.Push(currentLayoutDataControl);
            }
            currentLayoutDataControl = new ColumnLayoutDataControl(5);
            currentLayoutDataControl.SuppressLayout = true;

            //If there is a parent layout container, add the new one as a child.
            if (layoutDataControlStack.Count > 0)
            {
                layoutDataControlStack.Peek().addControl(currentLayoutDataControl);
            }
        }

        public void popColumnLayout()
        {
            currentLayoutDataControl.SuppressLayout = false;
            //Pop the top level control, skip this if we are on the last one.
            if (layoutDataControlStack.Count > 1)
            {
                layoutDataControlStack.Pop();
                currentLayoutDataControl = layoutDataControlStack.Peek();
            }
        }

        public void addField(BooleanDataField field)
        {
            currentLayoutDataControl.addControl(new CheckBoxDataControl(parentWidget, field));
        }

        public void addField(MenuItemField field)
        {
            currentLayoutDataControl.addControl(new MenuButtonDataControl(parentWidget, gui, field));
        }

        public void addField(MultipleChoiceField field)
        {
            currentLayoutDataControl.addControl(new RadioButtonDataControl(parentWidget, field));
        }

        public void addField(NotesDataField field)
        {
            currentLayoutDataControl.addControl(new WordWrapDataControl(parentWidget, field));
        }

        public void addField(NumericDataField field)
        {
            currentLayoutDataControl.addControl(new NumericEditDataControl(parentWidget, field));
        }

        public void addField(PlayExampleDataField field)
        {
            currentLayoutDataControl.addControl(new PlayExampleButton(parentWidget, gui, field));
        }

        public void addField(StaticTextDataField field)
        {
            currentLayoutDataControl.addControl(new TextDisplayDataControl(parentWidget, field));
        }

        public void addField(MoveCameraDataField field)
        {
            currentLayoutDataControl.addControl(new MoveCameraDataControl(parentWidget, gui, field));
        }

        public void addField(ChangeLayersDataField field)
        {
            currentLayoutDataControl.addControl(new ChangeLayersDataControl(parentWidget, gui, field));
        }

        public void addField(MoveCameraChangeLayersDataField field)
        {
            currentLayoutDataControl.addControl(new MoveCameraChangeLayersDataControl(parentWidget, gui, field));
        }

        public void addField(CloseGUIPlayTimelineField field)
        {
            currentLayoutDataControl.addControl(new CloseGUIPlayTimelineDataControl(parentWidget, gui, field));
        }

        public void addField(ChangeMedicalStateDataField field)
        {
            currentLayoutDataControl.addControl(new ChangeMedicalStateDataControl(parentWidget, gui, field));
        }

        public void addField(DoActionsDataField field)
        {
            currentLayoutDataControl.addControl(new DoActionsDataControl(parentWidget, gui, field));
        }

        public DataControl TopLevelControl
        {
            get
            {
                return currentLayoutDataControl;
            }
        }
    }
}
