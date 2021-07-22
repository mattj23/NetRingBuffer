using System;
using System.Linq;
using Xunit;

namespace RingBuffer.Tests
{
    public class RingBufferTests
    {
        [Fact]
        public void TestEmptyBuffer()
        {
            RingBuffer buffer = new RingBuffer(10);

            Assert.Equal(0, buffer.Count);
            Assert.Equal(10, buffer.Available);
            Assert.Equal(10, buffer.Capacity);
        }

        [Fact]
        public void TestAddBytesAtStart()
        {
            var testData = new byte[] {1, 2, 3, 4};
            var buffer = new RingBuffer(10);

            buffer.Push(testData);

            Assert.Equal(4, buffer.Count);
            Assert.Equal(6, buffer.Available);
            Assert.Equal(1, buffer[0]);
            Assert.Equal(2, buffer[1]);
            Assert.Equal(3, buffer[2]);
            Assert.Equal(4, buffer[3]);
        }

        [Fact]
        public void TestTakeBytesAtStart()
        {
            var testData = new byte[] {1, 2, 3, 4};
            var buffer = new RingBuffer(10);
            buffer.Push(testData);

            var result = buffer.Take(4);

            Assert.Equal(0, buffer.Count);
            Assert.Equal(10, buffer.Available);
            Assert.Equal(1, result[0]);
            Assert.Equal(2, result[1]);
            Assert.Equal(3, result[2]);
            Assert.Equal(4, result[3]);
        }

        [Fact]
        public void TestStressBuffer()
        {
            var testCount = 100000;
            var sampleData = new byte[testCount];
            var readData = new byte[0];
            var random = new Random();
            random.NextBytes(sampleData);

            int readIndex = 0;
            int writeIndex = 0;

            var ring = new RingBuffer(50);

            while (readIndex < testCount)
            {
                // If the buffer is empty OR it's not full and we pass a random coin flip, we will write
                // data into it.  If the buffer is empty we have no choice but to write since a read is
                // impossible.
                if (ring.IsEmpty || (!ring.IsFull && random.NextDouble() > 0.5))
                {
                    // Write some amount  of data into it
                    var amount = random.Next(1, ring.Available);
                    amount = Math.Min(amount, testCount - writeIndex);
                    var values = sampleData.Skip(writeIndex).Take(amount).ToArray();
                    ring.Push(values);
                    writeIndex += amount;
                }

                // Otherwise we read from the buffer and check that it matches the data that was put in
                else
                {
                    var amount = random.Next(1, ring.Count);
                    amount = Math.Min(amount, testCount - readIndex);
                    var result = ring.Take(amount);
                    readData = readData.Concat(result).ToArray();
                    readIndex += amount;
                }
            }

            Assert.Equal(sampleData, readData);
        }

    }
}
