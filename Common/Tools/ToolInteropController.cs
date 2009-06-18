using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// This class provides a simple way to make the tools communicate with each
    /// other. This is optional and the intertool communication can be handled
    /// without this class if desired.
    /// </summary>
    public class ToolInteropController
    {
        private MoveController moveController;
        private SelectionController selectionController;
        private RotateController rotateController;
        private ToolManager toolManager;
        private bool allowMovement = true;

        public ToolInteropController()
        {

        }

        public void setMoveController(MoveController moveController)
        {
            this.moveController = moveController;
            moveController.OnTranslationChanged += onTranslationChanged;
        }

        void onTranslationChanged(Vector3 newTranslation, object sender)
        {
            if (selectionController != null && allowMovement)
            {
                selectionController.translateSelectedObject(ref newTranslation);
            }
        }

        public void setRotateController(RotateController rotateController)
        {
            this.rotateController = rotateController;
            rotateController.OnRotationChanged += onRotationChanged;
        }

        void onRotationChanged(Quaternion newRotation, object sender)
        {
            if (selectionController != null && allowMovement)
            {
                selectionController.rotateSelectedObject(ref newRotation);
            }
        }

        public void setSelectionController(SelectionController selectionController)
        {
            this.selectionController = selectionController;
            selectionController.OnSelectionChanged += onSelectionChanged;
            selectionController.OnSelectionRotated += onSelectionRotated;
            selectionController.OnSelectionTranslated += onSelectionTranslated;
        }

        void onSelectionTranslated(Vector3 newPosition)
        {
            if (moveController != null)
            {
                allowMovement = false;
                moveController.setTranslation(ref newPosition, selectionController);
                allowMovement = true;
            }
            if (toolManager != null)
            {
                toolManager.setToolTranslation(newPosition);
            }
        }

        void onSelectionRotated(Quaternion newRotation)
        {
            if (rotateController != null)
            {
                allowMovement = false;
                rotateController.setRotation(ref newRotation, selectionController);
                allowMovement = true;
            }
        }

        void onSelectionChanged(SelectionChangedArgs args)
        {
            allowMovement = false;
            if (moveController != null)
            {
                Vector3 translation = args.Owner.getSelectionTranslation();
                moveController.setTranslation(ref translation, selectionController);
            }
            if (rotateController != null)
            {
                Quaternion rotation = args.Owner.getSelectionRotation();
                rotateController.setRotation(ref rotation, selectionController);
            }
            if (toolManager != null)
            {
                toolManager.setEnabled(args.Owner.hasSelection());
                toolManager.setToolTranslation(args.Owner.getSelectionTranslation());
            }
            allowMovement = true;
        }

        public void setToolManager(ToolManager toolManager)
        {
            this.toolManager = toolManager;
        }
    }
}
