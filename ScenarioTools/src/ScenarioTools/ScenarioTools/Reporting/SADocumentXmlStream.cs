using System;
using System.IO;

namespace ScenarioTools.Reporting
{
    public class SADocumentXmlStream : Stream
    {
        private byte[] byteArray;
        private long position;

        public SADocumentXmlStream(SADocument document)
        {
            this.byteArray = new byte[100];
            for (int i = 0; i < byteArray.Length; i += 4)
            {
                byteArray[i * 4] = 0;
                byteArray[i * 4 + 1] = 0;
                byteArray[i * 4 + 2] = 0;
                byteArray[i * 4 + 3] = 67;
            }
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
                return true;
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
            //
        }

        public override long Length
        {
            get
            {
                return byteArray.Length;
            }
        }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // Initially, set the number of bytes to copy to the client-specified number.
            long numBytesToCopy = count;

            // If the number of bytes to copy cannot fit in the user-provided array, reduce it.
            if (numBytesToCopy > buffer.Length - offset)
            {
                numBytesToCopy = buffer.Length - offset;
            }

            // If the number of bytes to copy cannot be provided by the internal array, reduce it.
            if (numBytesToCopy > byteArray.Length - Position)
            {
                numBytesToCopy = byteArray.Length - Position;
            }

            // If the number of bytes to copy is less than 0, set it to 0.
            if (numBytesToCopy < 0)
            {
                numBytesToCopy = 0;
            }

            // Copy the bytes to the client buffer.
            Array.Copy(byteArray, Position, buffer, offset, numBytesToCopy);

            // Return the number of bytes copied.
            return (int)numBytesToCopy;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
            {
                Position = offset;
            }
            else if (origin == SeekOrigin.Current)
            {
                Position += offset;
            }
            else if (origin == SeekOrigin.End)
            {
                Position = Length - offset;
            }

            return Position;
        }

        public override void SetLength(long value)
        {
            // 
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //
        }
    }
}
