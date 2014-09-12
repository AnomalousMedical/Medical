using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    class SelectionOperatorChooser : PopupContainer
    {
        private static ButtonEvent ToggleAddMode;
        private static ButtonEvent ToggleRemoveMode;

        static SelectionOperatorChooser()
        {
            ToggleAddMode = new ButtonEvent(EventLayers.Gui);
            ToggleAddMode.addButton(KeyboardButtonCode.KC_LCONTROL);
            DefaultEvents.registerDefaultEvent(ToggleAddMode);

            ToggleRemoveMode = new ButtonEvent(EventLayers.Gui);
            ToggleRemoveMode.addButton(KeyboardButtonCode.KC_LMENU);
            DefaultEvents.registerDefaultEvent(ToggleRemoveMode);
        }

        private AnatomyController anatomyController;
        private bool allowSelectionModeChanges = true;
        private ButtonGroup<SelectionOperator> selectionOperators;

        public SelectionOperatorChooser(AnatomyController anatomyController)
            :base("Medical.GUI.SelectionOperatorChooser.SelectionOperatorChooser.layout")
        {
            this.anatomyController = anatomyController;
            anatomyController.SelectionOperatorChanged += anatomyController_SelectionOperatorChanged;

            selectionOperators = new ButtonGroup<SelectionOperator>();
            setupSelectionButton(SelectionOperator.Select, "SelectButton");
            setupSelectionButton(SelectionOperator.Add, "AddButton");
            setupSelectionButton(SelectionOperator.Remove, "RemoveButton");
            selectionOperators.Selection = anatomyController.SelectionOperator;
            selectionOperators.SelectedButtonChanged += pickingModeGroup_SelectedButtonChanged;

            ToggleAddMode.FirstFrameDownEvent += toggleAdMode_FirstFrameDownEvent;
            ToggleAddMode.FirstFrameUpEvent += toggleAdMode_FirstFrameUpEvent;
            ToggleRemoveMode.FirstFrameDownEvent += toggleRemoveMode_FirstFrameDownEvent;
            ToggleRemoveMode.FirstFrameUpEvent += toggleRemoveMode_FirstFrameUpEvent;
        }

        public override void Dispose()
        {
            ToggleAddMode.FirstFrameDownEvent -= toggleAdMode_FirstFrameDownEvent;
            ToggleAddMode.FirstFrameUpEvent -= toggleAdMode_FirstFrameUpEvent;
            ToggleRemoveMode.FirstFrameDownEvent -= toggleRemoveMode_FirstFrameDownEvent;
            ToggleRemoveMode.FirstFrameUpEvent -= toggleRemoveMode_FirstFrameUpEvent;

            anatomyController.SelectionOperatorChanged -= anatomyController_SelectionOperatorChanged;
            base.Dispose();
        }

        void pickingModeGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            if (allowSelectionModeChanges)
            {
                allowSelectionModeChanges = false;
                anatomyController.SelectionOperator = (SelectionOperator)selectionOperators.SelectedButton.UserObject;
                hide();
                allowSelectionModeChanges = true;
            }
        }

        void anatomyController_SelectionOperatorChanged(AnatomyController source, SelectionOperator arg)
        {
            if (allowSelectionModeChanges)
            {
                allowSelectionModeChanges = false;
                selectionOperators.Selection = arg;
                allowSelectionModeChanges = true;
            }
        }

        void toggleRemoveMode_FirstFrameUpEvent(EventLayer eventLayer)
        {
            if (selectionOperators.Selection == SelectionOperator.Remove)
            {
                selectionOperators.Selection = SelectionOperator.Select;
            }
        }

        void toggleRemoveMode_FirstFrameDownEvent(EventLayer eventLayer)
        {
            selectionOperators.Selection = SelectionOperator.Remove;
        }

        void toggleAdMode_FirstFrameUpEvent(EventLayer eventLayer)
        {
            if (selectionOperators.Selection == SelectionOperator.Add)
            {
                selectionOperators.Selection = SelectionOperator.Select;
            }
        }

        void toggleAdMode_FirstFrameDownEvent(EventLayer eventLayer)
        {
            selectionOperators.Selection = SelectionOperator.Add;
        }

        private void setupSelectionButton(SelectionOperator mode, String name)
        {
            Button selectionButton = (Button)widget.findWidget(name);
            selectionOperators.addButton(mode, selectionButton);
            selectionButton.NeedToolTip = true;
            selectionButton.EventToolTip += selectionButton_EventToolTip;
        }

        void selectionButton_EventToolTip(Widget source, EventArgs e)
        {
            String text;
            switch (selectionOperators[(Button)source])
            {
                case SelectionOperator.Add:
                    text = String.Format("Add to Selection ({0})", ToggleAddMode.KeyDescription);
                    break;
                case SelectionOperator.Remove:
                    text = String.Format("Remove from Selection ({0})", ToggleRemoveMode.KeyDescription);
                    break;
                case SelectionOperator.Select:
                    text = "Select";
                    break;
                default:
                    text = "Unknown";
                    break;
            }
            TooltipManager.Instance.processTooltip(source, text, (ToolTipEventArgs)e);
        }
    }
}
