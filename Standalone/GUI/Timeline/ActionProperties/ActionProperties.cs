using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ActionProperties
    {
        private ScrollView actionPropertiesScroll;
        private ActionViewButton action;

        private NumericEdit startTime;
        private NumericEdit duration;

        private Dictionary<String, ActionPropertiesPanel> additionalProperties = new Dictionary<String, ActionPropertiesPanel>();
        private Vector2 additionalPropertiesPosition;
        private ActionPropertiesPanel currentPanel;

        public ActionProperties(ScrollView actionPropertiesScroll)
        {
            this.actionPropertiesScroll = actionPropertiesScroll;

            startTime = new NumericEdit(actionPropertiesScroll.findWidget("StartTime") as Edit);
            startTime.ValueChanged += new MyGUIEvent(startTime_ValueChanged);
            duration = new NumericEdit(actionPropertiesScroll.findWidget("Duration") as Edit);
            duration.ValueChanged += new MyGUIEvent(duration_ValueChanged);

            additionalPropertiesPosition = new Vector2(1, duration.Edit.Bottom + 2);

            object[] args = {actionPropertiesScroll};
            foreach (TimelineActionProperties properties in TimelineActionFactory.ActionProperties)
            {
                if (properties.GUIType != null)
                {
                    try
                    {
                        ActionPropertiesPanel propPanel = (ActionPropertiesPanel)Activator.CreateInstance(properties.GUIType, args);
                        propPanel.setPosition((int)additionalPropertiesPosition.x, (int)additionalPropertiesPosition.y);
                        additionalProperties.Add(properties.TypeName, propPanel);
                    }
                    catch (Exception)
                    {
                        throw new Exception(String.Format("Could not create the GUI for {0}. Make sure it extends ActionPropertiesPanel and has a constructor that takes a Widget."));
                    }
                }
            }
        }

        public ActionViewButton CurrentAction
        {
            get
            {
                return action;
            }
            set
            {
                Size2 canvasSize = actionPropertiesScroll.CanvasSize;
                canvasSize.Height = additionalPropertiesPosition.y;
                if (currentPanel != null)
                {
                    currentPanel.Visible = false;
                }
                action = value;
                if (action != null)
                {
                    startTime.FloatValue = action.StartTime;
                    duration.FloatValue = action.Duration;
                    additionalProperties.TryGetValue(action.Action.TypeName, out currentPanel);
                    if (currentPanel != null)
                    {
                        currentPanel.Visible = true;
                        currentPanel.CurrentAction = action.Action;
                        canvasSize.Height = currentPanel.Bottom;
                    }
                }
                else
                {
                    startTime.FloatValue = 0.0f;
                    duration.FloatValue = 0.0f;
                    currentPanel = null;
                }
                actionPropertiesScroll.CanvasSize = canvasSize;
            }
        }

        public bool Visible
        {
            get
            {
                return actionPropertiesScroll.Visible;
            }
            set
            {
                actionPropertiesScroll.Visible = value;
            }
        }

        void duration_ValueChanged(Widget source, EventArgs e)
        {
            if (action != null)
            {
                action.Duration = duration.FloatValue;
            }
        }

        void startTime_ValueChanged(Widget source, EventArgs e)
        {
            if (action != null)
            {
                action.StartTime = startTime.FloatValue;
            }
        }
    }
}
