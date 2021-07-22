using System;
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
        public void TestAddBytes()
        {
            var testData = new byte[] {1, 2, 3, 4};
            var buffer = new RingBuffer(10);

            buffer.Push(testData);

            Assert.Equal(4, buffer.Count);
            Assert.Equal(6, buffer.Available);
        }

    }
}
