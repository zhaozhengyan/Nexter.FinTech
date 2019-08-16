using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FinTech.Infrastructure
{
    public class WriteSyncMemoryStream : Stream
    {
        private readonly Stream _source;
        private readonly MemoryStream _current;
        public WriteSyncMemoryStream(Stream source)
        {
            _current = new MemoryStream();
            _source = source;
        }

        public override void Flush()
        {
            _current.Flush();
            _source.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _current.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _current.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _current.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _current.Write(buffer, offset, count);
            _source.Write(buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _current.WriteAsync(buffer, offset, count, cancellationToken);
            await _source.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _current.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await _current.FlushAsync(cancellationToken);
            await _source.FlushAsync(cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _current.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _current.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var asyncResult = _current.BeginWrite(buffer, offset, count, callback, state);
            _source.WriteAsync(buffer, offset, count);
            return asyncResult;
        }

        public override void Close()
        {
            _current.Close();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return _current.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _current.EndWrite(asyncResult);
        }

        public override int ReadByte()
        {
            return _current.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            _current.WriteByte(value);
            _source.WriteByte(value);
        }

        public override bool CanTimeout => _current.CanTimeout;

        public override int ReadTimeout
        {
            get => _current.ReadTimeout;
            set => _current.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => _current.WriteTimeout;
            set => _current.WriteTimeout = value;
        }

        public override bool CanRead => _current.CanRead;
        public override bool CanSeek => _current.CanSeek;
        public override bool CanWrite => _current.CanWrite;
        public override long Length => _current.Length;
        public override long Position
        {
            get => _current.Position;
            set => _current.Position = value;
        }
    }
}
