using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using Engine;
using Medical;
using Engine.Performance;
using Engine.Platform;
using SoundPlugin;
using System.IO;

namespace Medical.GUI
{
    public class QuickSoundRecorder : Dialog
    {
        public event Action<String> SoundUpdated;
        private CheckButton record;
        private Button save;
        private TextBox status;
        private TextBox etLabel;
        private RecordAudioController recordAudioController = new RecordAudioController();
        private Func<String, Stream> streamProvider;
        private MedicalController medicalController;
        private Int64 startTime;

        public static void ShowDialog(MedicalController medicalController, String soundFile, Func<String, Stream> streamProvider, Action<String> soundUpdatedCallback)
        {
            QuickSoundRecorder recorder = new QuickSoundRecorder(medicalController, soundFile, streamProvider);

            recorder.SoundUpdated += soundUpdatedCallback;
            recorder.Closed += (sender, e) =>
            {
                recorder.Dispose();
            };

            recorder.center();
            recorder.ensureVisible();
            recorder.open(true);
        }

        private QuickSoundRecorder(MedicalController medicalController, String file, Func<String, Stream> streamProvider)
            : base("Medical.GUI.Editor.QuickSoundRecorder.QuickSoundRecorder.layout")
        {
            this.OutputFile = file;
            this.streamProvider = streamProvider;
            this.medicalController = medicalController;

            record = new CheckButton((Button)window.findWidget("Record"));
            record.CheckedChanged += new MyGUIEvent(enabled_CheckedChanged);
            record.Checked = PerformanceMonitor.Enabled;

            save = (Button)window.findWidget("Save");
            save.MouseButtonClick += new MyGUIEvent(save_MouseButtonClick);
            save.Enabled = false;

            Button cancel = (Button)window.findWidget("Cancel");
            cancel.MouseButtonClick += cancel_MouseButtonClick;

            status = (TextBox)window.findWidget("StatusLabel");
            etLabel = (TextBox)window.findWidget("EtLabel");
        }

        public override void Dispose()
        {
            recordAudioController.Dispose();
            base.Dispose();
        }

        void enabled_CheckedChanged(Widget source, EventArgs e)
        {
            save.Enabled = !record.Checked;
            if (record.Checked)
            {
                recordAudioController.startRecording();
                status.Caption = "Recording";
                medicalController.FixedLoopUpdate += MedicalController_FixedLoopUpdate;
                startTime = medicalController.MainTimer.ElapsedTime;
            }
            else
            {
                stopRecording();
            }
        }

        private void stopRecording()
        {
            medicalController.FixedLoopUpdate -= MedicalController_FixedLoopUpdate;
            recordAudioController.stopRecording();
            status.Caption = "Stopped";
        }

        protected override void onClosing(DialogCancelEventArgs args)
        {
            base.onClosing(args);
            if (recordAudioController.ActivelyRecording)
            {
                stopRecording();
            }
        }

        void MedicalController_FixedLoopUpdate(Clock time)
        {
            Int64 timeDelta = medicalController.MainTimer.ElapsedTime - startTime;
            DateTime dateTime = new DateTime(timeDelta * 10);
            etLabel.Caption = dateTime.ToString("HH:mm:ss");
        }

        void cancel_MouseButtonClick(Widget source, EventArgs e)
        {
            this.Visible = false;
        }

        public String OutputFile { get; private set; }

        unsafe void save_MouseButtonClick(Widget source, EventArgs e)
        {
            bool saved = false;
            try
            {
                using (Stream destFile = streamProvider.Invoke(OutputFile))
                {
                    recordAudioController.save(destFile);
                }
                status.Caption = "Saved";
                saved = true;
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error saving audio file.\nReason: {0}", ex.Message), "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Logging.Log.Error(ex.Message);
            }
            if (saved)
            {
                if (SoundUpdated != null)
                {
                    SoundUpdated.Invoke(OutputFile);
                }
                this.Visible = false;
            }
        }
    }
}
