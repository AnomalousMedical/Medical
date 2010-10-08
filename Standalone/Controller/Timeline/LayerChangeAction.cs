using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Change Layers", 155 / 255f, 187 / 255f, 89 / 255f, GUIType=typeof(Medical.GUI.LayerChangeProperties))]
    class LayerChangeAction : TimelineAction
    {
        public LayerChangeAction()
            : this(null, null)
        {

        }

        public LayerChangeAction(String transparencyState, LayerState layerState)
        {
            this.TransparencyState = transparencyState;
            this.LayerState = layerState;
            ChangeMultiplier = 1.0f;
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
            LayerState.apply(ChangeMultiplier);
            TransparencyController.ActiveTransparencyState = currentTransparencyState;
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {

        }

        public void capture()
        {
            TransparencyState = TransparencyController.ActiveTransparencyState;
            LayerState.captureState();
        }

        public void preview()
        {
            String currentTransparencyState = TransparencyController.ActiveTransparencyState;
            TransparencyController.ActiveTransparencyState = TransparencyState;
            LayerState.apply(ChangeMultiplier);
            TransparencyController.ActiveTransparencyState = currentTransparencyState;
        }

        public override bool Finished
        {
            get { return true; }
        }

        public LayerState LayerState { get; set; }

        public String TransparencyState { get; set; }

        public float ChangeMultiplier { get; set; }

        #region Saveable

        private static readonly String LAYER_STATE = "LayerState";
        private static readonly String TRANSPARENCY_STATE = "TransparencyState";
        private static readonly String CHANGE_MULTIPLIER = "ChangeMultiplier";

        protected LayerChangeAction(LoadInfo info)
            : base(info)
        {
            LayerState = info.GetValue<LayerState>(LAYER_STATE);
            TransparencyState = info.GetString(TRANSPARENCY_STATE);
            ChangeMultiplier = info.GetSingle(CHANGE_MULTIPLIER, 1.0f);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(LAYER_STATE, LayerState);
            info.AddValue(TRANSPARENCY_STATE, TransparencyState);
            info.AddValue(CHANGE_MULTIPLIER, ChangeMultiplier);
            base.getInfo(info);
        }

        #endregion
    }
}
