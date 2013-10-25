using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Logging;
using Engine.Editing;

namespace Medical
{
    public class LayerChangeAction : TimelineAction
    {
        private bool finished = false;

        public LayerChangeAction()
            : this(null, new LayerState(""))
        {
            LayerState.captureState();
        }

        public LayerChangeAction(String transparencyState, LayerState layerState)
        {
            this.TransparencyState = transparencyState;
            this.LayerState = layerState;
            this.Duration = 1.0f;
            UseSystemLayerTransitionTime = false;
        }

        public LayerChangeAction(String transparencyState, LayerState layerState, float startTime)
            :this(transparencyState, layerState)
        {
            this.StartTime = startTime;
        }

        public override void started(float timelineTime, Clock clock)
        {
            if (TransparencyController.hasTransparencyState(TransparencyState))
            {
                String currentTransparencyState = TransparencyController.ActiveTransparencyState;
                TransparencyController.ActiveTransparencyState = TransparencyState;
                LayerState.timedApply(Duration);
                TransparencyController.ActiveTransparencyState = currentTransparencyState;
                finished = false;
            }
            else
            {
                //Could not find the specified window, so just apply to the active window.
                LayerState.timedApply(Duration);
                finished = false;
            }
        }

        public override void skipTo(float timelineTime)
        {
            if (TransparencyController.hasTransparencyState(TransparencyState))
            {
                if (timelineTime <= EndTime)
                {
                    float currentPosition = timelineTime - StartTime;
                    String currentTransparencyState = TransparencyController.ActiveTransparencyState;
                    TransparencyController.ActiveTransparencyState = TransparencyState;
                    float percent = 1.0f;
                    if (Duration != 0.0f)
                    {
                        percent = currentPosition / Duration;
                    }
                    LayerState.instantlyApplyBlendPercent(percent);
                    LayerState.timedApply(Duration - currentPosition);
                    TransparencyController.ActiveTransparencyState = currentTransparencyState;
                }
                else
                {
                    LayerState.instantlyApply();
                    finished = true;
                }
            }
            else
            {
                //Could not find the specified window, so just apply to the active window.
                if (timelineTime <= EndTime)
                {
                    float currentPosition = timelineTime - StartTime;
                    TransparencyController.ActiveTransparencyState = TransparencyState;
                    float percent = 1.0f;
                    if (Duration != 0.0f)
                    {
                        percent = currentPosition / Duration;
                    }
                    LayerState.instantlyApplyBlendPercent(percent);
                    LayerState.timedApply(Duration - currentPosition);
                }
                else
                {
                    LayerState.instantlyApply();
                    finished = true;
                }
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = timelineTime > StartTime + Duration;
        }

        public override void capture()
        {
            TransparencyState = TransparencyController.ActiveTransparencyState;
            LayerState.captureState();
        }

        public override void editing()
        {
            String currentTransparencyState = TransparencyController.ActiveTransparencyState;
            TransparencyController.ActiveTransparencyState = TransparencyState;
            LayerState.timedApply(MedicalConfig.CameraTransitionTime);
            TransparencyController.ActiveTransparencyState = currentTransparencyState;
        }

        public override void findFileReference(TimelineStaticInfo info)
        {

        }

        public override void cleanup(CleanupInfo cleanupInfo)
        {

        }

        public override bool Finished
        {
            get { return finished; }
        }

        public LayerState LayerState { get; set; }

        [Editable(Advanced = true)]
        public String TransparencyState { get; set; }

        [Editable(Advanced = true)]
        public bool UseSystemLayerTransitionTime { get; set; }

        public override string TypeName
        {
            get
            {
                return "Change Layers";
            }
        }

        public override float Duration
        {
            get
            {
                return UseSystemLayerTransitionTime ? MedicalConfig.CameraTransitionTime : base.Duration;
            }
            set
            {
                base.Duration = value;
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addCommand(new EditInterfaceCommand("Capture", (callback, caller) =>
            {
                capture();
            }));
        }

        #region Saveable

        private static readonly String LAYER_STATE = "LayerState";
        private static readonly String TRANSPARENCY_STATE = "TransparencyState";
        private static readonly String USE_SYSTEM_LAYER_TRANSITION_TIME = "UseSystemLayerTransitionTime";

        protected LayerChangeAction(LoadInfo info)
            : base(info)
        {
            LayerState = info.GetValue<LayerState>(LAYER_STATE);
            TransparencyState = info.GetString(TRANSPARENCY_STATE);
            UseSystemLayerTransitionTime = info.GetBoolean(USE_SYSTEM_LAYER_TRANSITION_TIME, false);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(LAYER_STATE, LayerState);
            info.AddValue(TRANSPARENCY_STATE, TransparencyState);
            info.AddValue(USE_SYSTEM_LAYER_TRANSITION_TIME, UseSystemLayerTransitionTime);
        }

        #endregion
    }
}
