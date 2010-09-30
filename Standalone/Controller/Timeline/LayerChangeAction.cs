﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class LayerChangeAction : TimelineAction
    {
        private LayerState layerState;
        private String transparencyState;

        public LayerChangeAction(String transparencyState, LayerState layerState)
        {
            this.transparencyState = transparencyState;
            this.layerState = layerState;
            ChangeMultiplier = 1.0f;
        }

        public LayerChangeAction(String transparencyState, LayerState layerState, float startTime)
            :this(transparencyState, layerState)
        {
            this.StartTime = startTime;
        }

        public void started(float timelineTime, Clock clock)
        {
            String currentTransparencyState = TransparencyController.ActiveTransparencyState;
            TransparencyController.ActiveTransparencyState = transparencyState;
            layerState.apply(ChangeMultiplier);
            TransparencyController.ActiveTransparencyState = currentTransparencyState;
        }

        public void stopped(float timelineTime, Clock clock)
        {
            
        }

        public void update(float timelineTime, Clock clock)
        {
            
        }

        public float StartTime
        {
            get;
            set;
        }

        public bool Finished
        {
            get { return true; }
        }

        public float ChangeMultiplier { get; set; }
    }
}
