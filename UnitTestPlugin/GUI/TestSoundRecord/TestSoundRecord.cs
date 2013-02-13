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
        private TextBox text;
        private CheckButton enabled;
        private StandaloneController standaloneController;
        private CaptureDevice captureDevice;
        private Stream writeStream;

        public TestSoundRecord(StandaloneController standaloneController)
            : base("UnitTestPlugin.GUI.TestSoundRecord.TestSoundRecord.layout")
        {
            this.standaloneController = standaloneController;

            text = (TextBox)window.findWidget("Text");
            enabled = new CheckButton((Button)window.findWidget("Enabled"));
            enabled.CheckedChanged += new MyGUIEvent(enabled_CheckedChanged);
            enabled.Checked = PerformanceMonitor.Enabled;

            Button reset = (Button)window.findWidget("ResetButton");
            reset.MouseButtonClick += new MyGUIEvent(reset_MouseButtonClick);
        }

        void enabled_CheckedChanged(Widget source, EventArgs e)
        {
            PerformanceMonitor.Enabled = enabled.Checked;
            if (enabled.Checked)
            {
                captureDevice = SoundPluginInterface.Instance.SoundManager.openCaptureDevice();
                writeStream = new MemoryStream();// File.Open(Path.Combine(MedicalConfig.UserDocRoot, "TestOutput.wav"), FileMode.Create, FileAccess.Write);
                unsafe
                {
                    captureDevice.start((byte* buffer, int length) =>
                    {
                        byte[] byteBuffer = new byte[length];
                        for (int i = 0; i < length; ++i)
                        {
                            byteBuffer[i] = buffer[i];
                        }
                        writeStream.Write(byteBuffer, 0, length);
                    });
                }
            }
            else
            {
                captureDevice.Dispose();
                if (writeStream != null)
                {
                    writeStream.Seek(0, SeekOrigin.Begin);
                    using (Stream fileWriter = File.Open(Path.Combine(MedicalConfig.UserDocRoot, "Test.raw"), FileMode.Create, FileAccess.Write))
                    {
                        writeStream.CopyTo(fileWriter);
                    }
                    writeStream.Close();
                    writeStream = null;
                }
                captureDevice = null;
            }
        }

        unsafe void reset_MouseButtonClick(Widget source, EventArgs e)
        {
            try
            {
                if (File.Exists(Path.Combine(MedicalConfig.UserDocRoot, "Test.raw")))
                {
                    using (Stream sourceFile = File.Open(Path.Combine(MedicalConfig.UserDocRoot, "Test.raw"), FileMode.Open, FileAccess.Read))
                    {
                        using (Stream destFile = File.Open(Path.Combine(MedicalConfig.UserDocRoot, "Test.ogg"), FileMode.Create, FileAccess.Write))
                        {
                            OggEncoder.encodeToStream(sourceFile, destFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error(ex.Message);
            }
        }
    }
}
