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

namespace UnitTestPlugin.GUI
{
    class TestSoundRecord : MDIDialog
    {
        private CheckButton enabled;
        private StandaloneController standaloneController;
        private RecordAudioController recordAudioController = new RecordAudioController();

        public TestSoundRecord(StandaloneController standaloneController)
            : base("UnitTestPlugin.GUI.TestSoundRecord.TestSoundRecord.layout")
        {
            this.standaloneController = standaloneController;

            enabled = new CheckButton((Button)window.findWidget("Enabled"));
            enabled.CheckedChanged += new MyGUIEvent(enabled_CheckedChanged);
            enabled.Checked = PerformanceMonitor.Enabled;

            Button reset = (Button)window.findWidget("ResetButton");
            reset.MouseButtonClick += new MyGUIEvent(reset_MouseButtonClick);
        }

        public override void Dispose()
        {
            recordAudioController.Dispose();
            base.Dispose();
        }

        void enabled_CheckedChanged(Widget source, EventArgs e)
        {
            PerformanceMonitor.Enabled = enabled.Checked;
            if (enabled.Checked)
            {
                recordAudioController.startRecording();
            }
            else
            {
                recordAudioController.stopRecording();
            }
        }

        unsafe void reset_MouseButtonClick(Widget source, EventArgs e)
        {
            FileSaveDialog fileSave = new FileSaveDialog(MainWindow.Instance, "Save your audio file", wildcard: "Ogg Vorbis (*.ogg)|*.ogg");
            fileSave.showModal((result, file) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    try
                    {
                        using (Stream destFile = File.Open(file, FileMode.Create, FileAccess.Write))
                        {
                            recordAudioController.save(destFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.Error(ex.Message);
                    }
                }
            });
        }
    }
}
