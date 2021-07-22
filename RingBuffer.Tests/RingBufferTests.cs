using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RingBuffer.Tests
{
    public class RingBufferTests
    {
        [Fact]
        public void TestEmptyBuffer()
        {
            var ring = new RingBuffer<byte>(10);

            Assert.Equal(0, ring.Count);
            Assert.Equal(10, ring.Available);
            Assert.Equal(10, ring.Capacity);
        }

        [Fact]
        public void TestAddBytesAtStart()
        {
            var testData = new byte[] {1, 2, 3, 4};
            var ring = new RingBuffer<byte>(10);

            ring.Push(testData);

            Assert.Equal(4, ring.Count);
            Assert.Equal(6, ring.Available);
            Assert.Equal(1, ring[0]);
            Assert.Equal(2, ring[1]);
            Assert.Equal(3, ring[2]);
            Assert.Equal(4, ring[3]);
        }

        [Fact]
        public void TestTakeBytesAtStart()
        {
            var testData = new byte[] {1, 2, 3, 4};
            var ring = new RingBuffer<byte>(10);
            ring.Push(testData);

            var result = ring.Take(4);

            Assert.Equal(0, ring.Count);
            Assert.Equal(10, ring.Available);
            Assert.Equal(1, result[0]);
            Assert.Equal(2, result[1]);
            Assert.Equal(3, result[2]);
            Assert.Equal(4, result[3]);
        }

        [Fact]
        public void TestFind()
        {
            var testData = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            var ring = new SearchableRingBuffer<byte>(10);
            ring.Push(testData);
            ring.Take(2);

            var find = new byte[] {5, 6, 7};
            var location = ring.Find(find);

            Assert.Equal(2, location);
        }

        [Fact]
        public void TestFindFull()
        {
            var testData = new byte[] {1, 2, 3};
            var ring = new SearchableRingBuffer<byte>(10);
            ring.Push(testData);

            var find = new byte[] {1, 2, 3};
            var location = ring.Find(find);

            Assert.Equal(0, location);
        }

        [Fact]
        public void TestPushOne()
        {
            var ring = new RingBuffer<byte>(10);
            ring.PushOne(1);
            ring.PushOne(2);

            Assert.Equal(2, ring.Count);
            Assert.Equal(1, ring[0]);
            Assert.Equal(2, ring[1]);
        }

        [Fact]
        public void TestTakeOne()
        {
            var testData = new byte[] {1, 2, 3};
            var ring = new RingBuffer<byte>(10);
            ring.Push(testData);

            Assert.Equal(1, ring.TakeOne());
            Assert.Equal(2, ring.TakeOne());
            Assert.Equal(1, ring.Count);
        }

        [Fact]
        public void TestFindSearchTooBig()
        {
            var testData = new byte[] {1, 2, 3};
            var ring = new SearchableRingBuffer<byte>(10);
            ring.Push(testData);
            ring.Take(2);

            var find = new byte[] {1, 2, 3};
            var location = ring.Find(find);

            Assert.True(location < 0);
        }

        [Fact]
        public void TestEnumerator()
        {
            var d0 = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            var d1 = new byte[] {11, 12, 13};
            var ring = new RingBuffer<byte>(10);
            ring.Push(d0);
            ring.Take(4);
            ring.Push(d1);

            var expected = new byte[] {5, 6, 7, 8, 9, 10, 11, 12, 13};
            var collected = new List<byte>();

            foreach (var b in ring)
            {
                collected.Add(b);
            }

            Assert.Equal(expected, collected.ToArray());
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

            var ring = new RingBuffer<byte>(50);

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
