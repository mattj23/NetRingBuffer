using System;

namespace RingBuffer
{
    public class SearchableRingBuffer<T> : RingBuffer<T> where T : IEquatable<T>
    {
        public SearchableRingBuffer(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// Search the entire buffer for an element which matches the given argument, returning either the index of
        /// the located element or a negative value if the element was not found.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int FindOne(T element)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(element))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Search the entire buffer for a sequence of elements given as the argument, returning either the index of
        /// the first element or a negative value if the sequence was not found. If the search sequence is longer than
        /// the number of elements stored in the buffer, a negative value will also be returned.
        /// </summary>
        /// <param name="sequence">The sequence to search the buffer for</param>
        /// <returns>Either the index of the found first element or a negative number if none was found</returns>
        public int Find(ReadOnlySpan<T> sequence)
        {
            if (sequence.Length > Count) return -1;

            for (int i = 0; i <= Count - sequence.Length; i++)
            {
                bool matches = true;
                for (int j = 0; j < sequence.Length; j++)
                {
                    if (!this[i + j].Equals(sequence[j]))
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches) return i;
            }

            return -1;
        }
    }
}