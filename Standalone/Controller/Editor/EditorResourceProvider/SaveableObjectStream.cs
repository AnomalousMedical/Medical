using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Engine.Saving;

namespace Medical
{
    /// <summary>
    /// This class wil change a saveable object into a stream. This way it can
    /// be returned through the cached resource manager.
    /// </summary>
    class SaveableObjectStream : Stream
    {
        private MemoryStream stream;
        private XmlTextWriter xmlWriter;

        public SaveableObjectStream(Saveable saveable)
        {
            stream = new MemoryStream();
            xmlWriter = new XmlTextWriter(stream, Encoding.Unicode);
            xmlWriter.Formatting = Formatting.Indented;
            EditorController.XmlSaver.saveObject(saveable, xmlWriter);
            xmlWriter.Flush();
            stream.Position = 0;
        }

        public override void Close()
        {
            xmlWriter.Close();
            base.Close();
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Length
        {
            get
            {
                return stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return stream.Position;
            }
            set
            {
                stream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }
    }
}
