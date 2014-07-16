using Engine.Platform;
using Engine.Saving.XMLSaver;
using Medical;
using Medical.Controller;
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
        private Int64 frequencyMicro;
        private Int64 lastFrameRecordTime = 0;
        private Int64 totalTimeMicro;

        private MovementSequence recordingSequence;

        private CheckButton record;
        private CheckButton play;
        private Button save;
        private Button clear;
        private SingleNumericEdit captureRate;

        private MedicalController medicalController;
        private MovementSequenceController movementSequenceController;

        public MovementSequenceRecorder(MedicalController medicalController, MovementSequenceController movementSequenceController)
            : base("KinectPlugin.GUI.MovementSequenceRecorder.layout")
        {
            this.medicalController = medicalController;
            this.movementSequenceController = movementSequenceController;

            record = new CheckButton((Button)window.findWidget("Record"));
            record.CheckedChanged += record_CheckedChanged;

            play = new CheckButton((Button)window.findWidget("Play"));
            play.CheckedChanged += play_CheckedChanged;
            play.Enabled = false;

            save = (Button)window.findWidget("Save");
            save.MouseButtonClick += save_MouseButtonClick;
            save.Enabled = false;

            clear = (Button)window.findWidget("Clear");
            clear.MouseButtonClick += clear_MouseButtonClick;
            clear.Enabled = false;

            captureRate = new SingleNumericEdit((EditBox)window.findWidget("CaptureRate"));
            captureRate.MinValue = 1.0f;
            captureRate.MaxValue = 60.0f;
            captureRate.Value = 30.0f;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        void record_CheckedChanged(Widget source, EventArgs e)
        {
            if(record.Checked)
            {
                medicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
                recordingSequence = new MovementSequence();
                lastFrameRecordTime = frequencyMicro;
                totalTimeMicro = 0;
                frequencyMicro = Clock.SecondsToMicroseconds(1.0f / captureRate.Value);

                play.Enabled = false;
                save.Enabled = false;
                clear.Enabled = false;
            }
            else
            {
                medicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
                recordingSequence.sortStates();

                play.Enabled = true;
                save.Enabled = true;
                clear.Enabled = true;
            }
        }

        void MedicalController_OnLoopUpdate(Clock time)
        {
            if (lastFrameRecordTime >= frequencyMicro)
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

        void play_CheckedChanged(Widget source, EventArgs e)
        {
            if(recordingSequence != null)
            {
                if(play.Checked)
                {
                    movementSequenceController.CurrentSequence = recordingSequence;
                    movementSequenceController.playCurrentSequence();
                    play.Button.Caption = "Pause";
                }
                else
                {
                    movementSequenceController.stopPlayback();
                    movementSequenceController.CurrentSequence = null;
                    play.Button.Caption = "Play";
                }
            }
        }

        void save_MouseButtonClick(Widget source, EventArgs e)
        {
            if (recordingSequence != null)
            {
                FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, message: "Save Recorded Sequence", wildcard: "Movement Sequence (*.seq)|*.seq");
                saveDialog.showModal((result, path) =>
                {
                    if (result == NativeDialogResult.OK)
                    {
                        using (Stream stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            SharedXmlSaver.Save(recordingSequence, stream);
                        }
                    }
                });
            }
        }

        void clear_MouseButtonClick(Widget source, EventArgs e)
        {
            recordingSequence = null;
            play.Enabled = false;
            save.Enabled = false;
            clear.Enabled = false;
        }
    }
}
