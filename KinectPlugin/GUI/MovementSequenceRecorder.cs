using Engine.Platform;
using Engine.Saving.XMLSaver;
using Medical;
using Medical.GUI;
using Medical.Muscles;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class MovementSequenceRecorder : MDIDialog
    {
        private Int64 frequencyMicro = Clock.SecondsToMicroseconds(1.0f / 60.0f);
        private Int64 lastFrameRecordTime = 0;
        private Int64 totalTimeMicro;

        private String saveLocation = "C:/Users/AndrewPiper/Documents/Anomalous Medical/Users/AndrewPiper/Editor Projects/MovementSeqTest/Recorded.seq";

        private MovementSequence recordingSequence;

        private CheckButton record;

        private MedicalController controller;

        public MovementSequenceRecorder(MedicalController controller)
            : base("KinectPlugin.GUI.MovementSequenceRecorder.layout")
        {
            this.controller = controller;

            record = new CheckButton((Button)window.findWidget("Record"));
            record.CheckedChanged += record_CheckedChanged;
        }

        void record_CheckedChanged(Widget source, EventArgs e)
        {
            if(record.Checked)
            {
                controller.OnLoopUpdate += MedicalController_OnLoopUpdate;
                recordingSequence = new MovementSequence();
                lastFrameRecordTime = frequencyMicro;
                totalTimeMicro = 0;
            }
            else
            {
                controller.OnLoopUpdate -= MedicalController_OnLoopUpdate;
                recordingSequence.sortStates();
                using (Stream stream = File.Open(saveLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    SharedXmlSaver.Save(recordingSequence, stream);
                }
            }
        }

        void MedicalController_OnLoopUpdate(Clock time)
        {
            if(lastFrameRecordTime >= frequencyMicro)
            {
                MovementSequenceState state = new MovementSequenceState();
                state.captureState();
                state.StartTime = Clock.MicrosecondsToSeconds(totalTimeMicro);
                recordingSequence.addState(state);
                lastFrameRecordTime = 0;
            }
            else
            {
                lastFrameRecordTime += time.DeltaTimeMicro;
            }
            totalTimeMicro += time.DeltaTimeMicro;
            recordingSequence.Duration = Clock.MicrosecondsToSeconds(totalTimeMicro);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
