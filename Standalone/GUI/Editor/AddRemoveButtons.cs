using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class AddRemoveButtons
    {
        private Button addButton;
        private Button removeButton;
        private Widget parentWidget;

        public event EventDelegate<AddRemoveButtons, bool> VisibilityChanged;

        public AddRemoveButtons(Button addButton, Button removeButton, Widget parentWidget)
        {
            this.addButton = addButton;
            this.removeButton = removeButton;
            this.parentWidget = parentWidget;
        }

        public event MyGUIEvent AddButtonClicked
        {
            add
            {
                addButton.MouseButtonClick += value;
            }
            remove
            {
                addButton.MouseButtonClick -= value;
            }
        }

        public event MyGUIEvent RemoveButtonClicked
        {
            add
            {
                removeButton.MouseButtonClick += value;
            }
            remove
            {
                removeButton.MouseButtonClick -= value;
            }
        }

        public bool Visible
        {
            get
            {
                return parentWidget.Visible;
            }
            set
            {
                if (parentWidget.Visible != value)
                {
                    parentWidget.Visible = value;
                    if (VisibilityChanged != null)
                    {
                        VisibilityChanged.Invoke(this, value);
                    }
                }
            }
        }

        public int Height
        {
            get
            {
                return parentWidget.Height;
            }
        }
    }
}
