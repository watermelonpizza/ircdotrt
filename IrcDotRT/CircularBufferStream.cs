﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IrcDotRT
{
    // Allows reading and writing to circular buffer as stream.
    // Note: Stream is non-blocking and non-thread-safe.
    internal class CircularBufferStream : Stream
    {
        // Buffer of data.
        private byte[] buffer;

        // Current index within buffer for writing and reading.
        private long writePosition;
        private long readPosition;

        public CircularBufferStream(int length)
            : this(new byte[length])
        {
        }

        public CircularBufferStream(byte[] buffer)
        {
            this.buffer = buffer;
            writePosition = 0;
            readPosition = 0;
        }

        public byte[] Buffer
        {
            get { return buffer; }
        }

        public long WritePosition
        {
            get { return writePosition; }
            set { writePosition = value % buffer.Length; }
        }

        public override long Position
        {
            get { return readPosition; }
            set { readPosition = value % buffer.Length; }
        }

        public override long Length
        {
            get { return writePosition - readPosition; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; ; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override void Flush()
        {
            //
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    readPosition = offset % buffer.Length;
                    break;
                case SeekOrigin.End:
                    readPosition = (buffer.Length - offset) % buffer.Length;
                    break;
                case SeekOrigin.Current:
                    readPosition = (readPosition + offset) % buffer.Length;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return readPosition;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Write block of bytes from given buffer into circular buffer, wrapping around when necessary.
            int writeCount;
            while ((writeCount = Math.Min(count, (int)(buffer.Length - writePosition))) > 0)
            {
                System.Buffer.BlockCopy(buffer, offset, buffer, (int)writePosition, writeCount);
                writePosition = (writePosition + writeCount) % buffer.Length;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // Read block of bytes from circular buffer, wrapping around when necessary.
            int totalReadCount = 0;
            int readCount;
            count = Math.Min(buffer.Length - offset, count);
            while ((readCount = Math.Min(count, (int)(writePosition - readPosition))) > 0)
            {
                System.Buffer.BlockCopy(buffer, (int)readPosition, buffer, offset, readCount);
                readPosition = (readPosition + readCount) % buffer.Length;
                offset += readCount;
                count = Math.Min(buffer.Length - offset, count);
                totalReadCount += readCount;
            }
            return totalReadCount;
        }
    }
}
