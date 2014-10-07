using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class SelectionMovableObject : MovableObject
    {
        private SelectionController selectionController;
        private bool showTools = false;

        public SelectionMovableObject(SelectionController selectionController)
        {
            this.selectionController = selectionController;
        }

        public Vector3 ToolTranslation
        {
            get
            {
                return selectionController.getSelectionTranslation();
            }
        }

        public void move(Vector3 offset)
        {
            //Must convert to global coords as this is what the selection controller expects.
            offset += ToolTranslation;
            selectionController.translateSelectedObject(ref offset);
        }

        public Quaternion ToolRotation
        {
            get
            {
                return selectionController.getSelectionRotation();
            }
        }

        public bool ShowTools
        {
            get
            {
                return showTools;
            }
            set
            {
                showTools = value;
            }
        }

        public void rotate(Quaternion newRot)
        {
            selectionController.rotateSelectedObject(ref newRot);
        }

        public void alertToolHighlightStatus(bool highlighted)
        {
            
        }
    }
}
