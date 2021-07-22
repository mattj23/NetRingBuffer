using System;

namespace RingBuffer
{
    public class RingBuffer
    {
        private readonly byte[] _buffer;

        /// <summary>
        /// The index of the first element of data
        /// </summary>
        private int _start;

        /// <summary>
        /// This is the index of the element just past the last actual data
        /// </summary>
        private int _end;

        public RingBuffer(int capacity)
        {
            _buffer = new byte[capacity];
            _start = 0;
            _end = 0;
        }

        public int Count => _end;

        public int Available => _buffer.Length - Count;

        public int Capacity => _buffer.Length;

        public void Push(byte[] data)
        {
            foreach (var b in data)
            {
                _buffer[_end] = b;
                _end++;
            }
        }

        public byte[] Take()
        {
            throw new NotImplementedException();
        }



    }
}