using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class AnatomyLayerManager : PinableMDIDialog
    {
        private LayerController layerController;
        private Button undoButton;
        private Button redoButton;

        public AnatomyLayerManager(LayerController layerController)
            : base("Medical.GUI.AnatomyLayerManager.AnatomyLayerManager.layout")
        {
            this.layerController = layerController;
            layerController.OnRedo += updateUndoRedo;
            layerController.OnUndo += updateUndoRedo;
            layerController.OnUndoRedoChanged += updateUndoRedo;
            layerController.OnActiveTransparencyStateChanged += updateUndoRedo;

            Button unhideAll = window.findWidget("UnhideAll") as Button;
            unhideAll.MouseButtonClick += (s, e) =>
            {
                LayerState undo = LayerState.CreateAndCapture();
                this.layerController.unhideAll();
                this.layerController.pushUndoState(undo);
            };

            undoButton = window.findWidget("Undo") as Button;
            undoButton.MouseButtonClick += (s, e) => layerController.undo();

            redoButton = window.findWidget("Redo") as Button;
            redoButton.MouseButtonClick += (s, e) => layerController.redo();
        }

        void updateUndoRedo(LayerController obj)
        {
            undoButton.Enabled = layerController.HasUndo;
            redoButton.Enabled = layerController.HasRedo;
        }
    }
}
