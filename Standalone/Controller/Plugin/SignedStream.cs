using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    class SignedStream : Stream
    {
        private Stream signedStream;
        private long realLength;

        public SignedStream(Stream signedStream, long realLength)
        {
            this.signedStream = signedStream;
            this.realLength = realLength;
        }

        public override bool CanRead
        {
            get
            {
                return signedStream.CanRead;
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
            throw new NotSupportedException();
        }

        public override long Length
        {
            get
            {
                return realLength;
            }
        }

        public override long Position
        {
            get
            {
                return signedStream.Position;
            }
            set
            {
                signedStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long bytesLeft = realLength - signedStream.Position;
            if (offset > bytesLeft)
            {
                return 0; //Refuse to read any further.
            }
            long readEndPosition = offset + count;
            if (readEndPosition > bytesLeft)
            {
                count = (int)(bytesLeft - offset);
            }

            return signedStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
