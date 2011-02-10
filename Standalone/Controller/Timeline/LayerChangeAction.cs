using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Change Layers")]
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
        }

        public LayerChangeAction(String transparencyState, LayerState layerState, float startTime)
            :this(transparencyState, layerState)
        {
            this.StartTime = startTime;
        }

        public override void started(float timelineTime, Clock clock)
        {
            String currentTransparencyState = TransparencyController.ActiveTransparencyState;
            TransparencyController.ActiveTransparencyState = TransparencyState;
            LayerState.timedApply(Duration);
            TransparencyController.ActiveTransparencyState = currentTransparencyState;
            finished = false;
        }

        public override void skipTo(float timelineTime)
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
            LayerState.apply(3.0f);
            TransparencyController.ActiveTransparencyState = currentTransparencyState;
        }

        public override bool Finished
        {
            get { return finished; }
        }

        public LayerState LayerState { get; set; }

        public String TransparencyState { get; set; }

        #region Saveable

        private static readonly String LAYER_STATE = "LayerState";
        private static readonly String TRANSPARENCY_STATE = "TransparencyState";

        protected LayerChangeAction(LoadInfo info)
            : base(info)
        {
            LayerState = info.GetValue<LayerState>(LAYER_STATE);
            TransparencyState = info.GetString(TRANSPARENCY_STATE);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(LAYER_STATE, LayerState);
            info.AddValue(TRANSPARENCY_STATE, TransparencyState);
            base.getInfo(info);
        }

        #endregion
    }
}
