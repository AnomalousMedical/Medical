using Engine.ObjectManagement;
using Engine.Platform;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class MusclePositionController
    {
        private SubscribingUpdateListener updateListener;
        private long blendPositionMicro;
        private long durationMicro;

        private MusclePosition start;
        private MusclePosition end;

        private MusclePosition bindPosition; //The default position of the muscles in the scene.
        private UndoRedoBuffer poseUndoRedoBuffer = new UndoRedoBuffer(20);

        /// <summary>
        /// Fired when an undo is executed.
        /// </summary>
        public event Action<MusclePositionController> OnUndo;

        /// <summary>
        /// Fired when a redo is executed.
        /// </summary>
        public event Action<MusclePositionController> OnRedo;

        /// <summary>
        /// Fired when the Undo/Redo buffer is altered, either by being cleared or a new command added.
        /// </summary>
        public event Action<MusclePositionController> OnUndoRedoChanged;

        public MusclePositionController(UpdateTimer timer, StandaloneController controller)
        {
            updateListener = new SubscribingUpdateListener(timer);
            updateListener.OnUpdate += updateListener_OnUpdate;
            controller.SceneLoaded += controller_SceneLoaded;
        }

        /// <summary>
        /// Blend from the current muscle position to the specified position over duration.
        /// </summary>
        /// <param name="endPosition">The position to blend to.</param>
        /// <param name="duration">The duration to blend over.</param>
        public void timedBlend(MusclePosition endPosition, float duration)
        {
            MusclePosition startPosition = new MusclePosition();
            startPosition.captureState();

            timedBlend(startPosition, endPosition, duration);
        }

        /// <summary>
        /// Start blending between two states. The duration is in seconds.
        /// </summary>
        /// <param name="start">The starting muscle position.</param>
        /// <param name="end">The ending muscle position.</param>
        /// <param name="duration">The duration to blend.</param>
        public void timedBlend(MusclePosition start, MusclePosition end, float duration)
        {
            this.start = start;
            this.end = end;
            updateListener.subscribeToUpdates();
            blendPositionMicro = 0;
            durationMicro = Clock.SecondsToMicroseconds(duration);
        }

        /// <summary>
        /// Undo the position buffer.
        /// </summary>
        public void undo()
        {
            poseUndoRedoBuffer.undo();
        }

        /// <summary>
        /// Redo the position buffer.
        /// </summary>
        public void redo()
        {
            poseUndoRedoBuffer.execute();
        }

        /// <summary>
        /// Push a new state onto the undo/redo buffer, will erase anything after the current undo.
        /// This will use the passed state as the undo state and the current muscle position of the scene
        /// as the redo state.
        /// </summary>
        public void pushUndoState(MusclePosition undoPosition)
        {
            pushUndoState(undoPosition, new MusclePosition(true));
        }

        /// <summary>
        /// Push a new state onto the undo/redo buffer, will erase anything after the current undo.
        /// This will use the passed states for undo and redo.
        /// </summary>
        public void pushUndoState(MusclePosition undoPosition, MusclePosition redoPosition)
        {
            poseUndoRedoBuffer.pushAndSkip(new TwoWayDelegateCommand<MusclePosition, MusclePosition>(redoPosition, undoPosition, new TwoWayDelegateCommand<MusclePosition, MusclePosition>.Funcs()
            {
                ExecuteFunc = position =>
                {
                    position.preview();
                    if(OnRedo != null)
                    {
                        OnRedo.Invoke(this);
                    }
                },
                UndoFunc = position =>
                {
                    position.preview();
                    if (OnUndo != null)
                    {
                        OnUndo.Invoke(this);
                    }
                }
            }));
            if(OnUndoRedoChanged != null)
            {
                OnUndoRedoChanged.Invoke(this);
            }
        }

        /// <summary>
        /// The default position for the muscles in the scene.
        /// </summary>
        public MusclePosition BindPosition
        {
            get
            {
                return bindPosition;
            }
        }

        public bool HasUndo
        {
            get
            {
                return poseUndoRedoBuffer.HasUndo;
            }
        }

        public bool HasRedo
        {
            get
            {
                return poseUndoRedoBuffer.HasRedo;
            }
        }

        void updateListener_OnUpdate(Clock clock)
        {
            blendPositionMicro += clock.DeltaTimeMicro;
            if (blendPositionMicro < durationMicro)
            {
                float percent = blendPositionMicro / (float)durationMicro;
                start.blend(end, percent);
            }
            else
            {
                start.blend(end, 1.0f);
                updateListener.unsubscribeFromUpdates();
            }
        }

        void controller_SceneLoaded(SimScene scene)
        {
            bindPosition = new MusclePosition();
            bindPosition.captureState();
            poseUndoRedoBuffer.clear();
            if (OnUndoRedoChanged != null)
            {
                OnUndoRedoChanged.Invoke(this);
            }
        }
    }
}
