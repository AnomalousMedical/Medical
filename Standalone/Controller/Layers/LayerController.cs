using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class LayerController
    {
        private Dictionary<String, UndoRedoBuffer> undoRedoBuffers = new Dictionary<string, UndoRedoBuffer>();
        private String activeStateName = null;

        /// <summary>
        /// Fired when an undo is executed.
        /// </summary>
        public event Action<LayerController> OnUndo;

        /// <summary>
        /// Fired when a redo is executed.
        /// </summary>
        public event Action<LayerController> OnRedo;

        /// <summary>
        /// Fired when the Undo/Redo buffer is altered, either by being cleared or a new command added.
        /// </summary>
        public event Action<LayerController> OnUndoRedoChanged;

        /// <summary>
        /// Fired when the active transparency state changes.
        /// </summary>
        public event Action<LayerController> OnActiveTransparencyStateChanged;

        public LayerController()
        {
            TransparencyController.ActiveTransparencyStateChanged += TransparencyController_ActiveTransparencyStateChanged;
            TransparencyController.TransparencyStateAdded += TransparencyController_TransparencyStateAdded;
            TransparencyController.TransparencyStateRemoved += TransparencyController_TransparencyStateRemoved;
            activeStateName = TransparencyController.ActiveTransparencyState;
            foreach(String name in TransparencyController.TransparencyStateNames)
            {
                addBuffer(name);
            }
        }

        /// <summary>
        /// Unhide everything
        /// </summary>
        public void unhideAll()
        {
            TransparencyController.smoothSetAllAlphas(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
        }

        /// <summary>
        /// Undo the layer buffer for the active state.
        /// </summary>
        public void undo()
        {
            undoRedoBuffers[activeStateName].undo();
        }

        /// <summary>
        /// Redo the layer buffer for the active state..
        /// </summary>
        public void redo()
        {
            undoRedoBuffers[activeStateName].execute();
        }

        /// <summary>
        /// Push a new state onto the undo/redo buffer, will erase anything after the current undo.
        /// This will use the passed state as the undo state and the current muscle position of the scene
        /// as the redo state.
        /// </summary>
        public void pushUndoState(LayerState undoLayers)
        {
            pushUndoState(undoLayers, LayerState.CreateAndCapture());
        }

        /// <summary>
        /// Push a new state onto the undo/redo buffer, will erase anything after the current undo.
        /// This will use the passed states for undo and redo.
        /// </summary>
        public void pushUndoState(LayerState undoState, LayerState redoState)
        {
            if (!undoState.isTheSameAs(redoState)) //This uses the slightly unreliable isTheSameAs function, but worse case scenerio we end up with a duplicate undo.
            {
                undoRedoBuffers[activeStateName].pushAndSkip(new TwoWayDelegateCommand<LayerState, LayerState>(redoState, undoState, new TwoWayDelegateCommand<LayerState, LayerState>.Funcs()
                {
                    ExecuteFunc = state =>
                    {
                        state.apply();
                        if (OnRedo != null)
                        {
                            OnRedo.Invoke(this);
                        }
                    },
                    UndoFunc = state =>
                    {
                        state.apply();
                        if (OnUndo != null)
                        {
                            OnUndo.Invoke(this);
                        }
                    }
                }));
                if (OnUndoRedoChanged != null)
                {
                    OnUndoRedoChanged.Invoke(this);
                }
            }
        }

        public bool HasUndo
        {
            get
            {
                return undoRedoBuffers[activeStateName].HasUndo;
            }
        }

        public bool HasRedo
        {
            get
            {
                return undoRedoBuffers[activeStateName].HasRedo;
            }
        }

        void TransparencyController_ActiveTransparencyStateChanged(String name)
        {
            activeStateName = name;
            if(OnActiveTransparencyStateChanged != null)
            {
                OnActiveTransparencyStateChanged.Invoke(this);
            }
        }

        void TransparencyController_TransparencyStateRemoved(String name)
        {
            undoRedoBuffers.Remove(name);
        }

        void TransparencyController_TransparencyStateAdded(String name)
        {
            addBuffer(name);
        }

        private void addBuffer(String name)
        {
            UndoRedoBuffer buffer = new UndoRedoBuffer(20);
            undoRedoBuffers.Add(name, buffer);
        }
    }
}
