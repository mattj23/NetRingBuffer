using System;

namespace RingBuffer
{
    /// <summary>
    /// A ring buffer (or circular buffer) is a fixed array stored in memory which acts like an
    /// infinitely advancing queue. 
    /// </summary>
    public class RingBuffer<T>
    {
        /// <summary>
        /// Internal storage array
        /// </summary>
        private readonly T[] _buffer;

        /// <summary>
        /// The index of the first element of data
        /// </summary>
        private int _start;

        /// <summary>
        /// This is the index of the element just past the last actual data
        /// </summary>
        private int _end;

        /// <summary>
        /// Create a ring buffer with a fixed capacity
        /// </summary>
        /// <param name="capacity"></param>
        public RingBuffer(int capacity)
        {
            Capacity = capacity;
            _buffer = new T[capacity + 1];
            _start = 0;
            _end = 0;
        }

        /// <summary>
        /// Gets the total number of items currently held by the ring buffer
        /// </summary>
        public int Count => (_end >= _start) ? _end - _start : Capacity + 1 - _start + _end;

        /// <summary>
        /// Gets the number of empty spaces left in the ring buffer
        /// </summary>
        public int Available => Capacity - Count;

        /// <summary>
        /// Gets the total capacity of the ring buffer, which was set when it was created
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Returns true if there are no elements in the ring buffer
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Returns true if there are no empty spaces in the ring buffer
        /// </summary>
        public bool IsFull => Available == 0;

        /// <summary>
        /// Gets (peek) or sets the element at an index in the ring without altering the number of elements stored. Accessing
        /// a value beyond the valid range of stored elements (as shown by Count) has undefined behavior.
        /// </summary>
        /// <param name="index">The index of the element to access, expressed as an offset from the first valid entry in the buffer</param>
        /// <returns></returns>
        public T this[int index]
        {
            get => _buffer[RawIndexOf(index)];
            set => _buffer[RawIndexOf(index)] = value;
        }

        /// <summary>
        /// Push a single element onto the end of the buffer.  Will throw an IndexOutOfRangeException if there
        /// is no empty space in the buffer.
        /// </summary>
        public T TakeOne()
        {
            if (IsEmpty)
                throw new IndexOutOfRangeException("Attempting to take an element from an empty buffer");

            var value = _buffer[_start];
            AdvanceStart();
            return value;
        }

        /// <summary>
        /// Push a single element onto the end of the buffer.  Will throw an IndexOutOfRangeException if there
        /// is no empty space in the buffer.
        /// </summary>
        /// <param name="b">The element to add to the buffer</param>
        public void PushOne(T e)
        {
            if (IsFull)
                throw new IndexOutOfRangeException("Attempting to add more elements to the buffer than there is available space");

            _buffer[_end] = e;
            AdvanceEnd();
        }

        /// <summary>
        /// Pushes a number of elements onto the end of the buffer, increasing the total count of elements and
        /// decreasing the capacity
        /// </summary>
        /// <param name="data"></param>
        public void Push(ReadOnlySpan<T> data)
        {
            if (data.Length > Available) 
                throw new IndexOutOfRangeException("Attempting to add more elements to the buffer than there is available space");

            foreach (var b in data)
            {
                _buffer[_end] = b;
                AdvanceEnd();
            }
        }

        /// <summary>
        /// Removes a number of elements from the front of the buffer and adds them to a range, decreasing the
        /// number of stored elements in the buffer. If the output span provided is larger than the number of elements
        /// currently stored in the buffer an IndexOutOfRangeException will be thrown.
        /// </summary>
        /// <param name="output">An output Span into which the leading elements will be copied</param>
        public void Take(Span<T> output)
        {
            if (output.Length > Count) 
                throw new IndexOutOfRangeException("Attempting to take more elements than are in the buffer");

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = _buffer[_start];
                AdvanceStart();
            }
        }

        /// <summary>
        /// Removes a number of elements from the front of the buffer and returns them as an array, decreasing the
        /// number of elements stored in the buffer. If the requested quantity to take is larger than the number of
        /// elements stored in the buffer an IndexOutOfRangeException will be thrown.
        /// </summary>
        /// <param name="quantity">Number of elements to take from the front of the buffer</param>
        /// <returns></returns>
        public T[] Take(int quantity)
        {
            var output = new T[quantity];
            Take(output);
            return output;
        }

        /// <summary>
        /// Calculate the _buffer index that corresponds with an index offset from _start
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int RawIndexOf(int i)
        {
            return (_start + i) % (Capacity + 1);
        }

        /// <summary>
        /// Increment the _end index by one, wrapping around to the front if necessary
        /// </summary>
        private void AdvanceEnd()
        {
            if (++_end >= _buffer.Length) _end = 0;
        }

        /// <summary>
        /// Increment the _start index by one, wrapping around to the front if necessary
        /// </summary>
        private void AdvanceStart()
        {
            if (++_start >= _buffer.Length) _start = 0;
        }


    }
}