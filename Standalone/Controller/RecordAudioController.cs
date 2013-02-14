using SoundPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    public class RecordAudioController : IDisposable
    {
        private CaptureDevice captureDevice;
        private Stream writeStream;
        private bool hasRecording = false;
        private String tempFilePath;

        public RecordAudioController()
        {
            tempFilePath = Path.Combine(MedicalConfig.TemporaryFilesPath, "RecordedAudio");
            if(!Directory.Exists(tempFilePath))
            {
                Directory.CreateDirectory(tempFilePath);
            }
            
            tempFilePath = Path.Combine(tempFilePath, Guid.NewGuid().ToString("D") + ".raw");
        }

        public void Dispose()
        {
            try
            {
                if (captureDevice != null)
                {
                    stopRecording();
                }
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Warning("Could not delete temporary file '{0}' for audio recording. Reason: {1}", tempFilePath, ex.Message);
            }
        }

        public unsafe void startRecording()
        {
            if (captureDevice == null)
            {
                hasRecording = true;
                captureDevice = SoundPluginInterface.Instance.SoundManager.openCaptureDevice();
                writeStream = File.Open(tempFilePath, FileMode.Create, FileAccess.Write);
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
        }

        public void stopRecording()
        {
            if (captureDevice != null)
            {
                captureDevice.Dispose();
                writeStream.Dispose();
                writeStream = null;
                captureDevice = null;
            }
        }

        public void save(Stream destStream)
        {
            if (File.Exists(tempFilePath))
            {
                using (Stream sourceStream = File.Open(tempFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (OggEncoder oggEncoder = new OggEncoder())
                    {
                        oggEncoder.encodeToStream(sourceStream, destStream);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("Cannot find temporary audio file {0}", tempFilePath);
            }
        }

        public bool HasRecording
        {
            get
            {
                return hasRecording;
            }
        }

        public bool ActivelyRecording
        {
            get
            {
                return captureDevice != null;
            }
        }
    }
}
