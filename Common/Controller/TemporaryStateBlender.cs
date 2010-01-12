﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    public class TemporaryStateBlender : UpdateListener
    {
        private UpdateTimer mainTimer;
        MedicalState currentState;
        MedicalState targetState;
        private float currentBlend;
        private bool blending = false;
        MedicalStateController stateController;
        private MedicalState undoState;

        public TemporaryStateBlender(UpdateTimer mainTimer, MedicalStateController stateController)
        {
            this.mainTimer = mainTimer;
            this.stateController = stateController;
            mainTimer.addFixedUpdateListener(this);
        }

        public void startTemporaryBlend(MedicalState targetState)
        {
            this.currentState = stateController.createState("TempStart");
            this.targetState = targetState;
            currentBlend = 0.0f;
            blending = true;
        }

        public void stopBlend()
        {
            blending = false;
        }

        public void recordUndoState()
        {
            undoState = stateController.createState("Undo");
        }

        public void blendToUndo()
        {
            startTemporaryBlend(undoState);
        }

        public MedicalState createBaselineState()
        {
            return stateController.createState("Baseline");
        }

        public MedicalState UndoState
        {
            get
            {
                return undoState;
            }
        }

        #region UpdateListener Members

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            if (blending)
            {
                if (currentBlend < 1.0f)
                {
                    currentState.blend(currentBlend, targetState);
                    currentBlend += (float)clock.Seconds;
                }
                else
                {
                    targetState.blend(0.0f, targetState);
                    blending = false;
                }
            }
        }

        #endregion
    }
}
