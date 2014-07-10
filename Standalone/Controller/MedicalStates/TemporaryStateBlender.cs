using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    public class TemporaryStateBlender
    {
        private MedicalState currentState;
        private MedicalState targetState;
        private float currentBlend;
        private MedicalStateController stateController;
        private MedicalState undoState;
        private SubscribingUpdateListener updateListener;

        public TemporaryStateBlender(UpdateTimer mainTimer, MedicalStateController stateController)
        {
            updateListener = new SubscribingUpdateListener(mainTimer);
            updateListener.OnUpdate += updateListener_OnUpdate;
            this.stateController = stateController;
        }

        public void startTemporaryBlend(MedicalState targetState)
        {
            this.currentState = stateController.createState("TempStart");
            this.targetState = targetState;
            currentBlend = 0.0f;
            updateListener.subscribeToUpdates();
        }

        public void forceFinishBlend()
        {
            if (updateListener.IsSubscribed)
            {
                targetState.blend(0.0f, targetState);
                updateListener.unsubscribeFromUpdates();
            }
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

        void updateListener_OnUpdate(Clock clock)
        {
            if (currentBlend < 1.0f)
            {
                currentState.blend(currentBlend, targetState);
                currentBlend += clock.DeltaSeconds;
            }
            else
            {
                targetState.blend(0.0f, targetState);
                updateListener.unsubscribeFromUpdates();
            }
        }
    }
}
